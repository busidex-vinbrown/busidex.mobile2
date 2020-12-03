using BranchXamarinSDK;
using Busidex.Resources.String;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Busidex3.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IBranchBUOSessionInterface
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
            FormsMaterial.Init();

            Branch.Debug = true;
            var busidexApp = new App();
            BranchIOS.Init(StringResources.BRANCH_KEY, options, busidexApp);

            Stormlion.ImageCropper.iOS.Platform.Init();

            LoadApplication(busidexApp);
            Plugin.InputKit.Platforms.iOS.Config.Init();
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

        public void InitSessionComplete(BranchUniversalObject buo, BranchLinkProperties blp)
        {
            
        }

        public void SessionRequestError(BranchError error)
        {
            
        }
    }
}
