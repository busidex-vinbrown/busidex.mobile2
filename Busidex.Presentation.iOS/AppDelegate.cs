
using Foundation;
using UIKit;
using System;
using GoogleAnalytics.iOS;
using Busidex.Mobile;
using BranchXamarinSDK;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Busidex.Presentation.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate, IBranchSessionInterface
	{
		// class-level declarations
		const string KEY_FROM = "_f";
		const string KEY_DISPLAY = "_d";
		const string KEY_MESSAGE = "_m";
		const string KEY_CARD_ID = "cardid";
		const string KEY_CARD_ID_ALT = "cardId";

		public string _deviceToken { get; set; }

		public override UIWindow Window { get; set; }

		public IGAITracker Tracker;

		UIWindow window;
		BaseNavigationController nav;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var settings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Sound |
				               UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);

				UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			} else {
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (UIRemoteNotificationType.Badge |
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
			nav = storyBoard.InstantiateInitialViewController () as BaseNavigationController;

			window.RootViewController = nav;
			window.MakeKeyAndVisible ();

			BranchIOS.Init (Resources.BRANCH_KEY, launchOptions, this);

			CheckAppVersion ();

			return true;
		}

		#region Google Analytics

		public static void TrackAnalyticsEvent (string category, string action, string label, NSNumber value)
		{

			var builder = GAIDictionaryBuilder.CreateEvent (category, action, label, value);

			GAI.SharedInstance.DefaultTracker.Send (builder.Build ());
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

		//
		// This method is invoked when the application is about to move from active to inactive state.
		//
		// OpenGL applications should use this method to pause.
		//
		public override void OnResignActivation (UIApplication application)
		{
		}

		async void UpdateAppVersion(){
			await UserDeviceController.UpdateDeviceDetails (DeviceType.iPhone, Application.APP_VERSION, UISubscriptionService.AuthToken);
		}

		void CheckAppVersion(){
			UserDeviceController.GetCurrentAppInfo (UISubscriptionService.AuthToken).ContinueWith ((device) => {
				if (device == null || device.Result == null || device.Result.iOS > Application.APP_VERSION) {
					const string MESSAGE = "There are critical updates available. You need to update the app now to get the latest features and bug fixes.";
					InvokeOnMainThread (() => {
						Application.ShowAlert ("Critical Updates", MESSAGE, new [] { "Update Now", "Update Later" }).ContinueWith (button => {
							if (button.Result == 0) {
								InvokeOnMainThread (() => {
									var url = new NSUrl (Resources.IOS_UPDATE_URL);
									UIApplication.SharedApplication.OpenUrl (url);
								});
							}
						});
					});
				}else{
					UpdateAppVersion ();
				}
			});
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

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			BranchIOS.getInstance ().HandlePushNotification (userInfo);
		}

		public override bool ContinueUserActivity (UIApplication application,
		                                           NSUserActivity userActivity,
		                                           UIApplicationRestorationHandler completionHandler)
		{
			AddOverlay ();
			bool handledByBranch = BranchIOS.getInstance ().ContinueUserActivity (userActivity);
			return handledByBranch;
		}

		void AddOverlay(){
			if (Application.Overlay == null) {
				var w = UIApplication.SharedApplication.KeyWindow;

				Application.Overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				Application.Overlay.Tag = (int)Resources.UIElements.QuickShare;
				w.Add (Application.Overlay);
				w.SetNeedsLayout ();
			}
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			var rUrl = new Rivets.AppLinkUrl (url.ToString ());

			if (rUrl.InputUrl.Equals ("jqle.app.link")) {

				var cardId = string.Empty;
				var sentFrom = string.Empty;
				string displayName = string.Empty;
				string personalMessage = string.Empty;

				if (rUrl.InputQueryParameters.ContainsKey (KEY_FROM)) {
					sentFrom = System.Web.HttpUtility.UrlDecode (rUrl.InputQueryParameters [KEY_FROM]);
				}
				if (rUrl.InputQueryParameters.ContainsKey (KEY_DISPLAY)) {
					displayName = System.Web.HttpUtility.UrlDecode (rUrl.InputQueryParameters [KEY_DISPLAY]);
				}
				if (rUrl.InputQueryParameters.ContainsKey (KEY_MESSAGE)) {
					personalMessage = System.Web.HttpUtility.UrlDecode (rUrl.InputQueryParameters [KEY_MESSAGE]);
				}

				if (rUrl.InputQueryParameters.ContainsKey (KEY_CARD_ID)) {
					cardId = rUrl.InputQueryParameters [KEY_CARD_ID];
				}
				if (rUrl.InputQueryParameters.ContainsKey (KEY_CARD_ID_ALT)) {
					cardId = rUrl.InputQueryParameters [KEY_CARD_ID_ALT];
				}
				if (!string.IsNullOrEmpty (cardId) && !string.IsNullOrEmpty (sentFrom)) {
					handleQuickShareRouting (long.Parse (cardId), displayName, long.Parse (sentFrom), personalMessage);
				}
			}

			return BranchIOS.getInstance ().OpenUrl (url);
		}

		#region Branch Event Handling

		public void InitSessionComplete (Dictionary<string, object> data)
		{
			var cardId = string.Empty;
			var sentFrom = string.Empty;
			string displayName = string.Empty;
			string personalMessage = string.Empty;

			if (data.ContainsKey (KEY_FROM)) {
				sentFrom = System.Web.HttpUtility.UrlDecode (data [KEY_FROM].ToString ());
			}
			if (data.ContainsKey (KEY_DISPLAY)) {
				displayName = System.Web.HttpUtility.UrlDecode (data [KEY_DISPLAY].ToString ());
			}
			if (data.ContainsKey (KEY_MESSAGE)) {
				personalMessage = System.Web.HttpUtility.UrlDecode (data [KEY_MESSAGE].ToString ());
			}

			if (data.ContainsKey (KEY_CARD_ID)) {
				cardId = data [KEY_CARD_ID].ToString ();
			}
			if (data.ContainsKey (KEY_CARD_ID_ALT)) {
				cardId = data [KEY_CARD_ID_ALT].ToString ();
			}
			if (!string.IsNullOrEmpty (cardId) && !string.IsNullOrEmpty (sentFrom)) {
				handleQuickShareRouting (long.Parse (cardId), displayName, long.Parse (sentFrom), personalMessage);
			}
		}

		void handleQuickShareRouting (long cardId, string displayName, long sentFrom, string personalMessage)
		{
			if (cardId > 0) {

				AddOverlay ();

				var quickShareLink = new QuickShareLink {
					CardId = cardId,
					DisplayName = displayName,
					From = sentFrom,
					PersonalMessage = personalMessage
				};

				Task.Run (() => {
					string json = Newtonsoft.Json.JsonConvert.SerializeObject (quickShareLink);
					Utils.SaveResponse (json, Resources.QUICKSHARE_LINK);
				});

				UISubscriptionService.AppQuickShareLink = quickShareLink;
			}
		}

		public void SessionRequestError (BranchError error)
		{
			if (error != null) {
				Xamarin.Insights.Report (new Exception ("Branch Error: [" + error.ErrorCode + "]" + error.ErrorMessage));
			} else {
				Xamarin.Insights.Report (new Exception ("Unknow Branch Error"));
			}
		}

		#endregion
	}
}

