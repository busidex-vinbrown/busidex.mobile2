using Busidex3.Analytics;
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
        public App()
        {
            InitializeComponent();

            Security.ReadAuthCookie();

            MainPage = new MainMenu();// new NavigationPage(new MainMenu());            
        }

        private static IAnalyticsManager analyticsManager;
        public static IAnalyticsManager AnalyticsManager
        {
            get
            {
                if (analyticsManager == null)
                {
                    analyticsManager = DependencyService.Get<IAnalyticsManager>();
                    analyticsManager.InitWithId();
                }
                return analyticsManager;
            }
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
