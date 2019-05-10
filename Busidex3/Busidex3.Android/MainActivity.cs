using Android.App;
using Android.Content.PM;
using Android.OS;
using Busidex3.ViewModels;

namespace Busidex3.Droid
{
    [Activity(Label = "Busidex3", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarians.CropImage.Droid.CropImageServiceAndroid.Initialize(this);
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