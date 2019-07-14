using System;
using System.Collections.Generic;
using System.Linq;
using BranchXamarinSDK;
using Foundation;
using UIKit;

namespace Busidex3.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            BranchIOS.Debug = true;

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return BranchIOS.getInstance().OpenUrl(url);
        }

        // Called when the app is opened from a Universal Link
        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            return BranchIOS.getInstance().ContinueUserActivity(userActivity);
        }

        // Called when the app receives a push notification
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            BranchIOS.getInstance().HandlePushNotification(userInfo);
        }
    }
}
