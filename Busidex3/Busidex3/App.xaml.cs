using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Busidex3.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Busidex3.ViewModels;
using BranchXamarinSDK;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Busidex3
{
    public partial class App : Application, IBranchSessionInterface
    {
        private static readonly CardHttpService _cardHttpService = new CardHttpService();

        public App()
        {
            InitializeComponent();

            InitSession();

            MainPage = new MainMenu(); 
        }

        private static void InitSession()
        {
            Security.ReadAuthCookie();

            Task.Factory.StartNew(async () => await LoadOwnedCard());
            Task.Factory.StartNew(async () => await Security.LoadUser());
            Task.Factory.StartNew(async () => await LoadEvents());
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

            var semaphore = new SemaphoreSlim (1, 1);
            await semaphore.WaitAsync ();

            var jpgFilename = Path.Combine (documentsPath, fileName);

            try {
                using (var webClient = new WebClient ()) {

                    var imageData = await webClient.DownloadDataTaskAsync (new Uri (imagePath));

                    var localPath = Path.Combine (documentsPath, fileName);
                    if (imageData != null) {
                        File.WriteAllBytes (localPath, imageData); // writes to local storage  
                    }
                }
            } catch (Exception ex) {
                Crashes.TrackError(new Exception ("Error loading " + imagePath, ex));
            } finally {
                semaphore.Release ();
            }

            return jpgFilename;
        }

        public static void LoadProfilePage()
        {
            var needsSetup = string.IsNullOrEmpty(Security.AuthToken);
            var page = (Page)Activator.CreateInstance(typeof(MyProfileView));

            var masterDetailRootPage = (MainMenu)Current.MainPage;
            masterDetailRootPage.Detail = needsSetup 
                ? page
                : new NavigationPage(page);
            masterDetailRootPage.IsPresented = false;
            masterDetailRootPage.IsGestureEnabled = !needsSetup;
            NavigationPage.SetHasNavigationBar(masterDetailRootPage.Detail, !needsSetup);
        }

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
            Current.MainPage = new MainMenu();
        }

        public static void LoadLoginPage()
        {
            var page = (Page)Activator.CreateInstance(typeof(Login));

            var masterDetailRootPage = (MainMenu)Current.MainPage;
            masterDetailRootPage.Detail = page;
            masterDetailRootPage.IsPresented = false;
            masterDetailRootPage.IsGestureEnabled = false;
            NavigationPage.SetHasNavigationBar(masterDetailRootPage.Detail, false);
        }

        public static void LoadMyBusidexPage()
        {
            var page = (Page)Activator.CreateInstance(typeof(MyBusidexView));
            page.Title = ViewNames.MyBusidex;

            var masterDetailRootPage = (MainMenu)Current.MainPage;
            masterDetailRootPage.Detail = new NavigationPage(page);
            masterDetailRootPage.IsPresented = false;
            masterDetailRootPage.IsGestureEnabled = true;
        }

        public static async Task<Card> LoadOwnedCard ()
        {
            try {

                var myCardResponse = await _cardHttpService.GetMyCard ();
                if(myCardResponse == null){
                    return null;
                }

                var card = myCardResponse.Success && myCardResponse.Model != null
                    ? new Card(myCardResponse.Model)
                    : null;
                var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
                Serialization.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (card), path);

                var myBusidex = Serialization.LoadData<List<UserCard>> (Path.Combine (Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));

                if(myBusidex != null){
                    foreach(var uc in myBusidex){
                        if (card == null || uc.Card == null || uc.Card.CardId != card.CardId) continue;
                        
                        card.ExistsInMyBusidex = true;
                        uc.Card = new Card (card);
                        break;
                    }
                    
                    var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidex);

                    Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);
                }

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
            var list = Newtonsoft.Json.JsonConvert.SerializeObject(response.Model);
            Serialization.SaveResponse(list, Path.Combine(Serialization.LocalStorageFolder, StringResources.EVENT_LIST_FILE));
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
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(quickShareLink);
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
