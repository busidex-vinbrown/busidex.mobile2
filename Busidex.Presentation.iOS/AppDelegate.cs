
using Foundation;
using UIKit;
//using WindowsAzure.Messaging;
using System;
using GoogleAnalytics.iOS;
using Busidex.Mobile;

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

		UIWindow window;
		BaseNavigationController nav;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Process any potential notification data from launch
			//ProcessNotification (options);

			// Register for Notifications
			//UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (
			//	UIRemoteNotificationType.Alert |
			//	UIRemoteNotificationType.Badge );

			if (UIDevice.CurrentDevice.CheckSystemVersion(8,0))
			{
				var settings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Sound |
					UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);

				UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			}
			else
			{
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Badge |
					UIRemoteNotificationType.Sound | UIRemoteNotificationType.Alert);
			}

			// Optional: set Google Analytics dispatch interval to e.g. 20 seconds.
			GAI.SharedInstance.DispatchInterval = 5;

			// Optional: automatically send uncaught exceptions to Google Analytics.
			GAI.SharedInstance.TrackUncaughtExceptions = true;

			// Initialize tracker.
			Tracker = GAI.SharedInstance.GetTracker (Resources.GOOGLE_ANALYTICS_KEY_IOS);

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			var storyBoard = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			nav =  storyBoard.InstantiateInitialViewController() as BaseNavigationController;// UINavigationController (viewController);
			nav.id = 123;

			window.RootViewController = nav;
			window.MakeKeyAndVisible ();

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


		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			var rurl = new Rivets.AppLinkUrl (url.ToString ());

			if (rurl.InputUrl.Host.Equals ("quickshare") ) {

				var cardId = string.Empty;

				var sentFrom = string.Empty;
				string displayName = string.Empty;
				string personalMessage = string.Empty;

				if (rurl.InputQueryParameters.ContainsKey ("_f")) {
					sentFrom = System.Web.HttpUtility.UrlDecode (rurl.InputQueryParameters ["_f"]);
				}
				if (rurl.InputQueryParameters.ContainsKey ("_d")) {
					displayName = System.Web.HttpUtility.UrlDecode (rurl.InputQueryParameters ["_d"]);
				}
				if (rurl.InputQueryParameters.ContainsKey ("_m")) {
					personalMessage = System.Web.HttpUtility.UrlDecode (rurl.InputQueryParameters ["_m"]);
				}

				if (rurl.InputQueryParameters.ContainsKey ("cardId")) {
					cardId = rurl.InputQueryParameters ["cardId"];
				}

				if (!string.IsNullOrEmpty (cardId)) {

					var quickShareLink = new QuickShareLink {
						CardId = long.Parse (cardId),
						DisplayName = displayName,
						From = long.Parse(sentFrom),
						PersonalMessage = personalMessage
					};

					var storyBoard = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

					var busidexController = storyBoard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

					var cookie = busidexController.GetAuthCookie ();

					if (cookie == null) {
						// If the user is not logged in, save the shared card to file
						string json = Newtonsoft.Json.JsonConvert.SerializeObject (quickShareLink);
						Utils.SaveResponse (json, Resources.QUICKSHARE_LINK);

					} else {
						if (Application.MainController == null) {
							InvokeOnMainThread (() => UISubscriptionService.AppQuickShareLink = quickShareLink);
						} else {
							var quickShareController = storyBoard.InstantiateViewController ("QuickShareController") as QuickShareController;
							quickShareController.SetCardSharingInfo (quickShareLink);
							quickShareController.SaveFromUrl ();
							InvokeOnMainThread (() => Application.MainController.PushViewController (quickShareController, true));
							return true;
						}
					}
				}
			}
			nav.PopToRootViewController (true);
			return true;
		}
	}
}

