using System;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Busidex3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        

        protected override void OnStart()
        {
            AppCenter.Start("ios=8cfa4c49-d367-4822-967b-50ea4036a284;" +
                            "uwp=9aafac90-a3df-432e-89b6-4ada484f8695;" +
                            "android=88a70679-47bd-4f95-ad7c-2ea9b53e0d1c",
                typeof(Analytics), typeof(Crashes));
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
