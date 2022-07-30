using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Busidex3.ViewModels;
using BranchXamarinSDK;
using Busidex3.Views.EditCard;
using System.Linq;
using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Device = Xamarin.Forms.Device;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Http;
using Busidex.Models.Analytics;
using Busidex.Models.Dto;
using Busidex.Resources.String;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Busidex3
{
    public delegate void OnEventsLoadedResult();
    public delegate void OnOrganizationsLoadedResult();
    public delegate void OnContactsLoadedResult();
    // public delegate void OnAppLoadedResult();

    public partial class App : IBranchSessionInterface
    {
        private static readonly CardHttpService _cardHttpService = new CardHttpService();
        private static readonly OrganizationsHttpService _organizationsHttpService = new OrganizationsHttpService();

        public static event OnEventsLoadedResult OnEventsLoaded;
        public static event OnOrganizationsLoadedResult OnOrganizationsLoaded;
        public static event OnContactsLoadedResult OnContactsLoaded;
        // public static event OnAppLoadedResult OnAppLoaded;
        public static List<ContactList> ContactGroups { get; set; } = new List<ContactList>();

        // public static bool IsProfessional { get; set; }

        public App()
        {
            InitializeComponent();

            InitSession();

            // MainPage = new MainMenu(); 
            MainPage = new NavigationPage(new HomeMenuView());
            NavigationPage.SetHasNavigationBar(MainPage, false);
        }

        public static void InitSession()
        {
            Security.ReadAuthCookie();

            if (string.IsNullOrEmpty(Security.AuthToken)) return;

            Task.Factory.StartNew(async () => await Security.LoadUser());
            // Task.Factory.StartNew(async () => await LoadOwnedCard());
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
                Task.Factory.StartNew( async () =>await LoadMyBusidex());
            }
            if (DateTimeUtils.DateDiffDays(today, lastRefreshFile.LastEventsRefresh.GetValueOrDefault()) > DAYS_THRESHOLD)
            {
                Task.Factory.StartNew( async () =>await LoadEvents());
            }
            if (DateTimeUtils.DateDiffDays(today, lastRefreshFile.LastOrganizationListRefresh.GetValueOrDefault()) > DAYS_THRESHOLD)
            {
                Task.Factory.StartNew( async () =>await LoadOrganizations());
            }
        }

        public static void LoadContactList()
        {
            CrossPermissions.Current.CheckPermissionStatusAsync<ContactsPermission>()
                .ContinueWith(async (status) =>
                {
                    if (await status != PermissionStatus.Granted)
                    {
                        var shouldShow =
                            await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(
                                Permission.Contacts);
                        if (shouldShow || true)
                        {
                            await CrossPermissions.Current.RequestPermissionAsync<ContactsPermission>();
                        }
                    }
                });
            Task.Factory.StartNew(async () =>
            {
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
                                    var start = number.IndexOf("=", StringComparison.Ordinal)+1;
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
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    OnContactsLoaded?.Invoke();
                });
            });
        }

        private static IAnalyticsManager analyticsManager;
        public static IAnalyticsManager AnalyticsManager
        {
            get
            {
                if (analyticsManager != null) return analyticsManager;

                analyticsManager = DependencyService.Get<IAnalyticsManager>();
                analyticsManager.InitWithId();
                return analyticsManager;
            }
        }

        private static IDisplayManager displayManager;
        public static IDisplayManager DisplayManager
        {
            get
            {
                if (displayManager != null) return displayManager;

                displayManager = DependencyService.Get<IDisplayManager>();
              
                return displayManager;
            }
        }

        public static async Task<string> DownloadImage (string imagePath, string documentsPath, string fileName)
        {
            ServicePointManager.Expect100Continue = false;

            if (string.IsNullOrEmpty(imagePath) || imagePath.Contains (StringResources.NULL_CARD_ID)) {
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
                        var imageData = await webClient.DownloadDataTaskAsync(new Uri(imagePath));

                        var localPath = Path.Combine(documentsPath, fileName);
                        if (imageData != null)
                        {
                            using (var fs = new FileStream(localPath, FileMode.Append, FileAccess.Write))
                            {
                                fs.Write(imageData, 0, imageData.Length);
                            }

                            //File.WriteAllBytes(localPath, imageData); // writes to local storage  
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(new Exception("Error loading " + imagePath, ex));
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return jpgFilename;
        }

        //public static void LoadProfilePage()
        //{

            //var needsSetup = string.IsNullOrEmpty(Security.AuthToken);
            //var page = (Page)Activator.CreateInstance(typeof(MyProfileView));

            //var masterDetailRootPage = (MainMenu)Current.MainPage;
            //masterDetailRootPage.Detail = needsSetup 
            //    ? page
            //    : new NavigationPage(page);
            //masterDetailRootPage.IsPresented = false;
            //masterDetailRootPage.IsGestureEnabled = !needsSetup;
            //NavigationPage.SetHasNavigationBar(page, !needsSetup);
        //}

        public static void LoadStartupPage()
        {
            var page = (Page)Activator.CreateInstance(typeof(Startup));

            var masterDetailRootPage = (MainMenu)Current.MainPage;
            masterDetailRootPage.Detail = page;
            masterDetailRootPage.IsPresented = false;
            masterDetailRootPage.IsGestureEnabled = false;
            NavigationPage.SetHasNavigationBar(masterDetailRootPage.Detail, false);
        }

        public static void Reload()
        {
            InitSession();
            // Current.MainPage = new MainMenu();
            Current.MainPage = new NavigationPage(new HomeMenuView());
        }

        //public static void LoadLoginPage()
        //{
        //    var page = (Page)Activator.CreateInstance(typeof(Login));

        //    var masterDetailRootPage = (MainMenu)Current.MainPage;
        //    masterDetailRootPage.Detail = page;
        //    masterDetailRootPage.IsPresented = false;
        //    masterDetailRootPage.IsGestureEnabled = false;
        //    NavigationPage.SetHasNavigationBar(masterDetailRootPage.Detail, false);
        //}

        public static void LoadMyBusidexPage()
        {
            var page = (Page)Activator.CreateInstance(typeof(MyBusidexView));
            page.Title = ViewNames.MyBusidex;

            Current.MainPage.Navigation.PushAsync(page);
        }

        //public static void LoadHomePage()
        //{
            
            //var page = (Page)Activator.CreateInstance(typeof(HomeMenuView));
            //page.Title = ViewNames.Home;

            //var masterDetailRootPage = (MainMenu)Current.MainPage;
            //masterDetailRootPage.Detail = new NavigationPage(page);
            //masterDetailRootPage.IsPresented = false;
            //masterDetailRootPage.IsGestureEnabled = true;
        //}

        public static void LoadCardMenuPage(ref UserCard card)
        {
            var page = (Page)Activator.CreateInstance(typeof(EditCardMenuView), new object[] { card });
            page.Title = ViewNames.MyBusidex;

            var masterDetailRootPage = (MainMenu)Current.MainPage;
            masterDetailRootPage.Detail = new NavigationPage(page);
            masterDetailRootPage.IsPresented = false;
            masterDetailRootPage.IsGestureEnabled = true;
        }

        public static async Task<Card> LoadOwnedCard (bool useThumbnail = true)
        {
            try {

                var myCardResponse = await _cardHttpService.GetMyCard ();
                if(myCardResponse == null || !myCardResponse.Success){
                    return null;
                }

                var card = myCardResponse.Success && myCardResponse.Model != null
                    ? new Card(myCardResponse.Model)
                    : null;

                //IsProfessional = card?.FrontFileId != Guid.Empty && card?.FrontFileId != null;

                var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
                Serialization.SaveResponse (JsonConvert.SerializeObject (card), path);

                var fImageUrl = StringResources.THUMBNAIL_PATH + card.FrontFileId + ".jpg";
                var fName = useThumbnail 
                    ? StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileId + ".jpg"
                    : card.FrontFileId + ".jpg";

                var storagePath = Serialization.LocalStorageFolder;
                await DownloadImage(fImageUrl, storagePath, fName).ConfigureAwait(false);

                if(card.BackFileId != Guid.Empty)
                {
                    fImageUrl = StringResources.THUMBNAIL_PATH + card.BackFileId + ".jpg";
                    fName = useThumbnail 
                        ? StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileId + ".jpg"
                        : card.BackFileId + ".jpg";
                    await DownloadImage(fImageUrl, storagePath, fName).ConfigureAwait(false);
                    fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileId + ".jpg";
                    await DownloadImage(fImageUrl, storagePath, fName).ConfigureAwait(false);
                }

                //var myBusidex = Serialization.LoadData<List<UserCard>> (Path.Combine (Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));

                //if(myBusidex != null){
                //    foreach(var uc in myBusidex){
                //        if (card == null || uc.Card == null || uc.Card.CardId != card.CardId) continue;
                        
                //        card.ExistsInMyBusidex = true;
                //        uc.Card = new Card (card);
                //        break;
                //    }
                    
                //    var savedResult = JsonConvert.SerializeObject(myBusidex);

                //    Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);
                //}

                return await Task.FromResult(card);

            } catch (Exception ex) {
                Crashes.TrackError(ex);
            }
            return null;
        }

        public static async Task<bool> LoadEvents()
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

        public static async Task<bool> LoadOrganizations()
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

        public static async Task<bool> LoadMyBusidex()
        {
            var myBusidexHttpService = new MyBusidexHttpService();
            var result = await myBusidexHttpService.GetMyBusidex();
            var json = JsonConvert.SerializeObject(result.MyBusidex.Busidex);
            Serialization.SaveResponse(json, StringResources.MY_BUSIDEX_FILE);
            Serialization.SetDataRefreshDate(RefreshItem.MyBusidex);
            return true;
        }

        protected override void OnStart()
        {
            AppCenter.Start("ios=ff027836-a4f4-4fdf-9f20-b26c77fd11d9;" +
                            "uwp=6de54bfa-df57-4a05-81ef-36425c119c36;" +
                            "android=52a88625-8a62-48bd-9538-54ddf4f6ab47;",
                typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
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
            const string KEY_FROM = "_f";
            const string KEY_DISPLAY = "_d";
            const string KEY_MESSAGE = "_m";
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

            if (!string.IsNullOrEmpty(cardId))
            {
                var quickShareLink = new QuickShareLink
                {
                    CardId = long.Parse(cardId),
                    DisplayName = displayName,
                    From = long.Parse(sentFrom),
                    PersonalMessage = personalMessage
                };
                string json = JsonConvert.SerializeObject(quickShareLink);
                Serialization.SaveResponse(json, StringResources.QUICKSHARE_LINK);
                MainPage = new MainMenu();
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
