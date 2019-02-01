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

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Busidex3
{
    public partial class App : Application
    {
        private static readonly CardHttpService _cardHttpService = new CardHttpService();

        public App()
        {
            InitializeComponent();

            Security.ReadAuthCookie();
            
            Task.Factory.StartNew(async () => await LoadOwnedCard());

            MainPage = new MainMenu();// new NavigationPage(new MainMenu());            
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

        public static void LoadMainMenuPage(string title)
        {
            var initialPage = new MyBusidexView();
            var masterDetailRootPage = (MainMenu)Application.Current.MainPage;
            masterDetailRootPage.Detail = new NavigationPage(initialPage)
            {
                Title = title
            };
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
    }
}
