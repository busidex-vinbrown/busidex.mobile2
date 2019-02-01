using Android.App;
using Android.Content.PM;
using Android.OS;
using Busidex3.ViewModels;

namespace Busidex3.Droid
{
    [Activity(Label = "Busidex3", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public void SetOrientation(UserCardDisplay.CardOrientation orientation)
        {
            RequestedOrientation = orientation == UserCardDisplay.CardOrientation.Horizontal
                ? ScreenOrientation.Landscape
                : ScreenOrientation.Portrait;
        }
    }
}