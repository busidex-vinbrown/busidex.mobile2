﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Busidex.Models.Constants;
using BranchXamarinSDK;
using Plugin.Permissions;

namespace Busidex.Professional.Droid
{
    [Activity(Label = "Busidex.Professional", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}