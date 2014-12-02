using System;
using Foundation;
using UIKit;
using System.Linq;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	partial class StartupController : BaseController
	{
		const string TEST_ACCOUNT_ID = "45ef6a4e-3c9d-452c-9409-9e4d7b6e532f";
		const bool DEVELOPMENT_MODE = false;

		public StartupController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SetPosition ();
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetPosition ();
			var cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.AUTHENTICATION_COOKIE_NAME);

			long userId;
			if (cookie != null) {
				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
				if (userId <= 0) {
					UpdateSettings ();
				} else {
					GoToMain ();
				}
			}

			btnStart.TouchUpInside += delegate {
				UpdateSettings();
			};

			btnConnect.TouchUpInside += delegate {
				GoToLogin();
			};
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			SetPosition ();
		}
		void SetPosition(){
			nfloat left = (UIScreen.MainScreen.Bounds.Width / 2f) - 80f;
			nfloat top = UIScreen.MainScreen.Bounds.Height - 160f - 40f;
			imgLogo.Frame = new CoreGraphics.CGRect (left, top, 160f, 160f);
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			//base.WillRotate (toInterfaceOrientation, duration);

			imgLogo.Hidden = toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
			imgLogo.SetNeedsLayout ();
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation){

			//base.DidRotate (fromInterfaceOrientation);

			SetPosition ();
		}

		void UpdateSettings(){
			var settingsController = Storyboard.InstantiateViewController ("SettingsController") as SettingsController;

			if (settingsController != null) {

				NavigationController.PushViewController (settingsController, true);
			}
		}

		void GoToLogin ()
		{
			var loginController = Storyboard.InstantiateViewController ("LoginController") as LoginController;

			if (loginController != null) {

				NavigationController.PushViewController (loginController, true);
			}
		}

		void GoToMain ()
		{
			NavigationController.SetNavigationBarHidden (true, true);

			var dataViewController = Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

			if (dataViewController != null) {
				NavigationController.PushViewController (dataViewController, true);
			}
		}

		static string EncodeUserId(long userId){
			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(userId.ToString());
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		static string GetDeviceId(){
			var thisDeviceId = UIDevice.CurrentDevice.IdentifierForVendor;
			if (thisDeviceId != null) {
				var dIdString = thisDeviceId.AsString ();
				return dIdString;
			}
			return string.Empty;
		}

//		static void UpdateSharedStorageData(string userId){
//			var user = NSUserDefaults.StandardUserDefaults;
//
//			user.SetString(userId, Resources.USER_SETTING_PASSWORD);
//			user.SetString(userId + "@busidex.com", Resources.USER_SETTING_EMAIL);
//			user.SetBool (true, Resources.USER_SETTING_USE_STAR_82);
//			user.SetBool(true, Resources.USER_SETTING_AUTOSYNC);
//			user.Synchronize();
//		}

		static void SetAuthCookie(long userId){

			var nCookie = new System.Net.Cookie();

			nCookie.Name = Resources.AUTHENTICATION_COOKIE_NAME;
			DateTime expiration = DateTime.Now.AddYears(1);
			nCookie.Expires = expiration;
			nCookie.Value = EncodeUserId(userId);
			var cookie = new NSHttpCookie(nCookie);

			NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
		}
	}
}
