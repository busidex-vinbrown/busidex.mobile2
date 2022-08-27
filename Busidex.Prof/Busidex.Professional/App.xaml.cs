using System;
using Xamarin.Forms;
using BranchXamarinSDK;
using System.Collections.Generic;
using Newtonsoft.Json;
using Busidex.Models.Domain;
using Busidex.Http.Utils;
using Busidex.Resources.String;
using Busidex.Http;
using Busidex.Professional.ViewModels;
using System.Threading.Tasks;
using System.IO;
using Busidex.Models.Dto;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Net;
using System.Threading;
using Microsoft.AppCenter.Crashes;
using System.Linq;
using Busidex.Models.Analytics;
using Busidex.Professional.Views;
using Xamarin.Essentials;

namespace Busidex.Professional
{
    public delegate void OnEventsLoadedResult();
    public delegate void OnOrganizationsLoadedResult();
    public delegate void OnContactsLoadedResult();

    public partial class App : IBranchSessionInterface
    {
        private static readonly CardHttpService _cardHttpService = new CardHttpService();
        private static readonly OrganizationsHttpService _organizationsHttpService = new OrganizationsHttpService();

        public static event OnEventsLoadedResult OnEventsLoaded;
        public static event OnOrganizationsLoadedResult OnOrganizationsLoaded;
        public static event OnContactsLoadedResult OnContactsLoaded;
        public static List<ContactList> ContactGroups { get; set; } = new List<ContactList>();

        private static IAnalyticsManager analyticsManager;
        public static IAnalyticsManager AnalyticsManager {
            get {
                if (analyticsManager != null) return analyticsManager;

                analyticsManager = DependencyService.Get<IAnalyticsManager>();
                analyticsManager.InitWithId();
                return analyticsManager;
            }
        }

        public static bool IsCardOwnerConfirmed { get; set; }

        private static IDisplayManager displayManager;
        public static IDisplayManager DisplayManager {
            get {
                if (displayManager != null) return displayManager;

                displayManager = DependencyService.Get<IDisplayManager>();

                return displayManager;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsEnabled = true
            });
            
            InitSession();

            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var page = new LoginView();
                    await Shell.Current.Navigation.PushAsync(page);
                    //await Shell.Current.GoToAsync(AppRoutes.LOGIN);
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync(AppRoutes.HOME);
                });
            }
        }

        public static void InitSession()
        {
            Security.ReadAuthCookie();

            if (string.IsNullOrEmpty(Security.AuthToken)) return;

            Task.Factory.StartNew(async () => await Security.LoadUser());
            Task.Factory.StartNew(async () => await LoadOwnedCard());
            refreshOldData();
        }

        private static void refreshOldData()
        {
            var refreshFileName = Path.Combine(Serialization.LocalStorageFolder,
                StringResources.BUSIDEX_REFRESH_COOKIE_NAME);

            var lastRefreshFile = Serialization.LoadData<BusidexRefreshInfo>(refreshFileName) ?? new BusidexRefreshInfo();

            const int DAYS_THRESHOLD = 5;

            var today = DateTime.Now;
            if (DateTimeUtils.DateDiffDays(today, lastRefreshFile.LastMyBusidexRefresh.GetValueOrDefault()) > DAYS_THRESHOLD)
            {
                Task.Factory.StartNew(async () => await LoadMyBusidex());
            }
            //if (DateTimeUtils.DateDiffDays(today, lastRefreshFile.LastEventsRefresh.GetValueOrDefault()) > DAYS_THRESHOLD)
            //{
            //    Task.Factory.StartNew(async () => await LoadEvents());
            //}
            //if (DateTimeUtils.DateDiffDays(today, lastRefreshFile.LastOrganizationListRefresh.GetValueOrDefault()) > DAYS_THRESHOLD)
            //{
            //    Task.Factory.StartNew(async () => await LoadOrganizations());
            //}
        }


        public static async Task<bool> LoadEvents()
        {
            try
            {
                var searchService = new SearchHttpService();
                var response = await searchService.GetUserEventTags();
                var list = response != null
                    ? Newtonsoft.Json.JsonConvert.SerializeObject(response.Model)
                    : string.Empty;
                Serialization.SaveResponse(list, Path.Combine(Serialization.LocalStorageFolder, StringResources.EVENT_LIST_FILE));

                Device.BeginInvokeOnMainThread(() =>
                {
                    OnEventsLoaded?.Invoke();
                });
                Serialization.SetDataRefreshDate(RefreshItem.Events);
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public static async Task<bool> LoadOrganizations()
        {
            try
            {
                var response = await _organizationsHttpService.GetMyOrganizations();
                var list = response != null
                    ? JsonConvert.SerializeObject(response.Model)
                    : string.Empty;

                Serialization.SaveResponse(list, StringResources.MY_ORGANIZATIONS_FILE);

                Device.BeginInvokeOnMainThread(() =>
                {
                    OnOrganizationsLoaded?.Invoke();
                });

                Serialization.SetDataRefreshDate(RefreshItem.OrganizationList);
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public static async Task<bool> LoadMyBusidex()
        {
            try
            {
                var myBusidexHttpService = new MyBusidexHttpService();
                var result = await myBusidexHttpService.GetMyBusidex();
                var json = JsonConvert.SerializeObject(result.MyBusidex?.Busidex);
                Serialization.SaveResponse(json, StringResources.MY_BUSIDEX_FILE);
                Serialization.SetDataRefreshDate(RefreshItem.MyBusidex);
                return true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }

        public static void LoadCardMenuPage(ref UserCard card)
        {
            //var page = (Page)Activator.CreateInstance(typeof(EditCardMenuView), new object[] { card });
            //page.Title = ViewNames.MyBusidex;

            //var masterDetailRootPage = (MainMenu)Current.MainPage;
            //masterDetailRootPage.Detail = new NavigationPage(page);
            //masterDetailRootPage.IsPresented = false;
            //masterDetailRootPage.IsGestureEnabled = true;
        }

        public static async Task LoadContactList()
        {
            try
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync<ContactsPermission>();
                    if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        var shouldShow =
                            await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(
                                Permission.Contacts);
                        if (shouldShow || true)
                        {
                        
                                status = await CrossPermissions.Current.RequestPermissionAsync<ContactsPermission>();
                                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Denied) return;
                        }
                    }
                });

                var contacts = await Plugin.ContactService.CrossContactService.Current.GetContactListAsync();
                if (Device.RuntimePlatform == Device.iOS)
                {
                    foreach (var contact in contacts)
                    {

                        if (contact.Numbers != null)
                        {
                            for (var i = 0; i < contact.Numbers.Count; i++)
                            {
                                var number = contact.Numbers[i];
                                if (number.ToLowerInvariant().Contains("stringvalue="))
                                {
                                    var start = number.IndexOf("=", StringComparison.Ordinal) + 1;
                                    var end = number.ToLower().IndexOf(", initialcountrycode", StringComparison.Ordinal);
                                    var num = number.Substring(start, end - start);
                                    contact.Numbers[i] = num;
                                }
                            }
                        }
                    }
                }

                ContactGroups = new List<ContactList>();
                for (var i = 0; i < 26; i++)
                {
                    var letter = ((char)(65 + i)).ToString();
                    var newGroup = new ContactList();
                    var filteredList = contacts.Where(c => c.Name.StartsWith(letter, StringComparison.Ordinal)).ToList();

                    newGroup.Heading = letter.ToUpper();
                    newGroup.AddRange(filteredList);

                    ContactGroups.Add(newGroup);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    OnContactsLoaded?.Invoke();
                });
               
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);                
            }
            
        }

        private static async Task<Card> CallGetMyCard()
        {
            var myCardResponse = await _cardHttpService.GetMyCard();
            if (myCardResponse == null || myCardResponse.Model == null || !myCardResponse.Success)
            {
                return null;
            }

            return new Card(myCardResponse.Model);
        }
        public static async Task<Card> LoadOwnedCard(bool useThumbnail = true, bool mustSucceed = false)
        {
            try
            {
                var card = await CallGetMyCard();

                var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
                Serialization.SaveResponse(JsonConvert.SerializeObject(card), path);

                var fImageUrl = StringResources.THUMBNAIL_PATH + card.FrontFileId + ".jpg";
                var fName = useThumbnail
                    ? StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileId + ".jpg"
                    : card.FrontFileId + ".jpg";

                var storagePath = Serialization.LocalStorageFolder;
                var imgResult = await DownloadImage(fImageUrl, storagePath, fName);
                if (mustSucceed && string.IsNullOrEmpty(imgResult)) throw new Exception("Could not download card front image");

                if (card.BackFileId != null && card.BackFileId != Guid.Empty)
                {
                    fImageUrl = StringResources.THUMBNAIL_PATH + card.BackFileId + ".jpg";
                    fName = useThumbnail
                        ? StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileId + ".jpg"
                        : card.BackFileId + ".jpg";
                    await DownloadImage(fImageUrl, storagePath, fName);
                    fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileId + ".jpg";
                    await DownloadImage(fImageUrl, storagePath, fName);
                }

                var myBusidex = Serialization.LoadData<List<UserCard>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));

                if (myBusidex != null)
                {
                    foreach (var uc in myBusidex)
                    {
                        if (card == null || uc.Card == null || uc.Card.CardId != card.CardId) continue;

                        card.ExistsInMyBusidex = true;
                        uc.Card = new Card(card);
                        break;
                    }

                    var savedResult = JsonConvert.SerializeObject(myBusidex);

                    Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);
                }

                return card;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return null;
        }

        public static async Task<string> DownloadImage(string imageURL, string documentsPath, string fileName)
        {
            ServicePointManager.Expect100Continue = false;

            if (string.IsNullOrEmpty(imageURL) || imageURL.Contains(StringResources.NULL_CARD_ID))
            {
                return string.Empty;
            }
            var jpgFilename = Path.Combine(documentsPath, fileName);
            using (var semaphore = new SemaphoreSlim(1, 1))
            {
                await semaphore.WaitAsync();

                try
                {
                    using (var webClient = new WebClient())
                    {
                        var imageData = await webClient.DownloadDataTaskAsync(new Uri(imageURL));

                        var localPath = Path.Combine(documentsPath, fileName);
                        if (imageData != null)
                        {
                            using (var fs = new FileStream(localPath, FileMode.Append, FileAccess.Write))
                            {
                                fs.Write(imageData, 0, imageData.Length);
                                fs.Flush();
                            }
                        }
                    }
                }   
                catch (Exception ex)
                {
                    Crashes.TrackError(new Exception("Error loading " + imageURL, ex));
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return jpgFilename;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        #region IBranchSessionInterface implementation
        public void InitSessionComplete(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return;
            }

            var cardId = string.Empty;

            var sentFrom = string.Empty;
            string displayName = string.Empty;
            string personalMessage = string.Empty;
            bool? saveOwner = null;
            const string KEY_FROM = "_f";
            const string KEY_DISPLAY = "_d";
            const string KEY_MESSAGE = "_m";
            const string KEY_SAVEOWNER = "_o";
            const string KEY_CARD_ID = "cardId";

            if (data.ContainsKey(KEY_FROM))
            {
                sentFrom = System.Web.HttpUtility.UrlDecode(data[KEY_FROM].ToString());
            }
            if (data.ContainsKey(KEY_DISPLAY))
            {
                displayName = System.Web.HttpUtility.UrlDecode(data[KEY_DISPLAY].ToString());
            }
            if (data.ContainsKey(KEY_MESSAGE))
            {
                personalMessage = System.Web.HttpUtility.UrlDecode(data[KEY_MESSAGE].ToString());
            }
            if (data.ContainsKey(KEY_CARD_ID))
            {
                cardId = data[KEY_CARD_ID].ToString();
            }
            if (data.ContainsKey(KEY_SAVEOWNER))
            {
                saveOwner = bool.Parse(data[KEY_SAVEOWNER].ToString());
            }

            if (!string.IsNullOrEmpty(cardId))
            {
                var quickShareLink = new QuickShareLink
                {
                    CardId = long.Parse(cardId),
                    DisplayName = displayName,
                    From = long.Parse(sentFrom),
                    PersonalMessage = personalMessage,
                    SaveOwner  = saveOwner.GetValueOrDefault()
                };
                string json = JsonConvert.SerializeObject(quickShareLink);
                Serialization.SaveResponse(json, StringResources.QUICKSHARE_LINK);
            }
        }

        public void CloseSessionComplete()
        {
        }

        public void SessionRequestError(BranchError error)
        {
        }
        #endregion      
    }
}
