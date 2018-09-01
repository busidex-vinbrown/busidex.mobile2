using System;
using System.Linq;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class StartupController : BaseController
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

			btnStart.TouchUpInside += delegate {
				GoToCreateProfile ();
			};

			btnConnect.TouchUpInside += delegate {
				GoToLogin ();
			};
		}

		public override void ViewDidAppear (bool animated)
		{
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (true, true);
			}

			Application.MainController = NavigationController;

			NavigationController.SetToolbarHidden (true, true);

			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Startup");

			base.ViewDidAppear (animated);

			SetPosition ();
		}

		void SetPosition ()
		{

			nfloat height = UIScreen.MainScreen.Bounds.Height < 500f ? 100f : 130f;
			nfloat width = height;
			nfloat top = UIScreen.MainScreen.Bounds.Height - height - (height / 4f);
			nfloat left = (UIScreen.MainScreen.Bounds.Width / 2f) - (width / 2f);

			imgLogo.Frame = new CoreGraphics.CGRect (left, top, width, height);
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			imgLogo.Hidden = toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
			imgLogo.SetNeedsLayout ();
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			SetPosition ();
		}

		void GoToLogin ()
		{
			if (NavigationController.ViewControllers.Any (c => c as LoginController != null)) {
				NavigationController.PopToViewController (BaseNavigationController.loginController, true);
			} else {
				NavigationController.PushViewController (BaseNavigationController.loginController, true);
			}
		}

		static string EncodeUserId (long userId)
		{
			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes (userId.ToString ());
			string returnValue = Convert.ToBase64String (toEncodeAsBytes);
			return returnValue;
		}

		static string GetDeviceId ()
		{
			var thisDeviceId = UIDevice.CurrentDevice.IdentifierForVendor;
			if (thisDeviceId != null) {
				var dIdString = thisDeviceId.AsString ();
				return dIdString;
			}
			return string.Empty;
		}
	}
}
