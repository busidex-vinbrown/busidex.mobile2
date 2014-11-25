using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Linq;

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
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies
				.Where(c=>c.Name == Busidex.Mobile.Resources.AuthenticationCookieName)
				.SingleOrDefault();

			long userId;
			if (cookie != null) {
				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
				if (userId <= 0) {
					DoLogin ();
				} 
				GoToMain ();
			}

			btnStart.TouchUpInside += delegate {
				DoLogin();
				GoToMain();
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
		private void SetPosition(){
			nfloat left = (UIScreen.MainScreen.Bounds.Width / 2f) - 80f;
			nfloat top = UIScreen.MainScreen.Bounds.Height - 160f - 40f;
			imgLogo.Frame = new CoreGraphics.CGRect (left, top, 160f, 160f);
		}
		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			imgLogo.Hidden = toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
			imgLogo.SetNeedsLayout ();

			base.WillRotate (toInterfaceOrientation, duration);
		}

		public override void DidRotate(UIInterfaceOrientation orientation){

			base.DidRotate (orientation);

			SetPosition ();
		}

		private void GoToLogin ()
		{
			var loginController = this.Storyboard.InstantiateViewController ("LoginController") as LoginController;

			if (loginController != null) {

				this.NavigationController.PushViewController (loginController, true);
			}
		}

		private void GoToMain ()
		{
			this.NavigationController.SetNavigationBarHidden (true, true);

			var dataViewController = this.Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

			if (dataViewController != null) {
				this.NavigationController.PushViewController (dataViewController, true);
			}
		}

		private string EncodeUserId(long userId){
			byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(userId.ToString());
			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		private string GetDeviceId(){
			var thisDeviceId = UIDevice.CurrentDevice.IdentifierForVendor;
			if (thisDeviceId != null) {
				var dIdString = thisDeviceId.AsString ();
				return dIdString;
			}
			return string.Empty;
		}

		private void UpdateSharedStorageData(string userId){
			var user = NSUserDefaults.StandardUserDefaults;

			user.SetString(userId, Busidex.Mobile.Resources.USER_SETTING_USERNAME);
			user.SetString(userId, Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
			user.SetString(userId + "@busidex.com", Busidex.Mobile.Resources.USER_SETTING_EMAIL);
			user.SetBool (true, Busidex.Mobile.Resources.USER_SETTING_USE_STAR_82);
			user.SetBool(true, Busidex.Mobile.Resources.USER_SETTING_AUTOSYNC);
			user.Synchronize();
		}

		private void SetAuthCookie(long userId){

			var nCookie = new System.Net.Cookie();

			nCookie.Name = Busidex.Mobile.Resources.AuthenticationCookieName;
			DateTime expiration = DateTime.Now.AddYears(1);
			nCookie.Expires = expiration;
			nCookie.Value = EncodeUserId(userId);
			var cookie = new NSHttpCookie(nCookie);

			NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
		}

		private void DoLogin(){
			string uidId = GetDeviceId ();
			uidId = (DEVELOPMENT_MODE ? TEST_ACCOUNT_ID : uidId);

			var userId = Busidex.Mobile.LoginController.AutoLogin (uidId);
			if (userId > 0) {

				SetAuthCookie (userId);

				UpdateSharedStorageData (uidId);
			}
		}
	}
}
