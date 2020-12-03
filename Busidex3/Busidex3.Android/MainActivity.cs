using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using BranchXamarinSDK;
using Busidex.Models.Constants;
using Plugin.InAppBilling;
using Plugin.Permissions;
using Xamarin.Forms;

namespace Busidex3.Droid
{
    [Activity(
        Label = "Busidex3", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
         public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Stormlion.ImageCropper.Droid.Platform.Init();

            Forms.Init(this, savedInstanceState);
            FormsMaterial.Init(this, savedInstanceState);

            var app = new App();
            BranchAndroid.GetAutoInstance(this.ApplicationContext);
            BranchAndroid.Init(this, GetString(Resource.String.branch_key), app);
            
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            LoadApplication(app);
        }

        public void SetOrientation(CardOrientation orientation)
        {
            RequestedOrientation = orientation == CardOrientation.Horizontal
                ? ScreenOrientation.Landscape
                : ScreenOrientation.Portrait;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Stormlion.ImageCropper.Droid.Platform.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        protected override void OnNewIntent(Intent intent)
        {
            this.Intent = intent;
        }
    }
}