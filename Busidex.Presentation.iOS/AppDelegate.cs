
using Foundation;
using UIKit;
using WindowsAzure.Messaging;
using System;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public string _deviceToken { get; set;}
		public override UIWindow Window { get; set; }
		public IGAITracker Tracker;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Process any potential notification data from launch
			//ProcessNotification (options);

			// Register for Notifications
			//UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (
			//	UIRemoteNotificationType.Alert |
			//	UIRemoteNotificationType.Badge );

			// Optional: set Google Analytics dispatch interval to e.g. 20 seconds.
			GAI.SharedInstance.DispatchInterval = 20;

			// Optional: automatically send uncaught exceptions to Google Analytics.
			GAI.SharedInstance.TrackUncaughtExceptions = true;

			// Initialize tracker.
			Tracker = GAI.SharedInstance.GetTracker (Busidex.Mobile.Resources.GOOGLE_ANALYTICS_KEY_IOS);

			// ...
			// Your other code here
			// ...
			return true;
		}
			
		#region Google Analytics
		public static void TrackAnalyticsEvent(string category, string action, string label, NSNumber value){

			var builder = GAIDictionaryBuilder.CreateEvent (category, action, label, value);

			GAI.SharedInstance.DefaultTracker.Send(builder.Build());
		}

		#endregion

		/*public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			// Connection string from your azure dashboard
			var cs = SBConnectionString.CreateListenAccess(
				new NSUrl("sb://busidexhub-ns.servicebus.windows.net/"),
				"hilk6syUT6mPeV4uSLX2pqB3HcH3fCG5BnPNQ75/j8E=");

			// Register our info with Azure
			var hub = new SBNotificationHub (cs, "busidexhub");
			hub.RegisterNativeAsync (deviceToken, null, err => {
				if (err != null)
					Console.WriteLine("Error: " + err.Description);
				else
					Console.WriteLine("Success");
			});
		}*/

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			// Process a notification received while the app was already open
			//ProcessNotification (userInfo);
		}

		//
		// This method is invoked when the application is about to move from active to inactive state.
		//
		// OpenGL applications should use this method to pause.
		//
		public override void OnResignActivation (UIApplication application)
		{
		}

		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}

		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}

		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}
	}
}

