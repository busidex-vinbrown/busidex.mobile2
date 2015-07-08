using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	partial class OrganizationDetailController : UIBarButtonItemWithImageViewController
	{
		public long OrganizationId{ get; set;}

		public OrganizationDetailController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Organization Detail - " + OrganizationId);

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			LoadOrganiaztion ();
		}

		void LoadOrganiaztion(){

			var cookie = GetAuthCookie ();
			var controller = new Busidex.Mobile.OrganizationController ();
			var overlay = new MyBusidexLoadingOverlay (View.Bounds);
			overlay.MessageText = "Loading Your Organization";

			View.AddSubview (overlay);

			controller.GetOrganizationById(cookie.Value, OrganizationId).ContinueWith(orgResult => {

				if (!string.IsNullOrEmpty (orgResult.Result)) {

					var orgResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationDetailResponse> (orgResult.Result);
					var org = orgResponse.Model;
					if (org != null) {
						var fileName = Path.Combine (documentsPath, org.LogoFileName);

						InvokeOnMainThread(() => {
						
							var imageFile = fileName + "." + org.LogoType;
							if (File.Exists (imageFile)) {
								var data = NSData.FromFile (imageFile);
								if (data != null) {
									imgOrgImage.Image = new UIImage (data);
								}
							}

							lblContacts.Text = org.Contacts;

							txtEmail.Editable = txtPhone.Editable = txtFax.Editable = false;
							txtEmail.UserInteractionEnabled = txtPhone.UserInteractionEnabled = txtFax.UserInteractionEnabled = true;
							txtEmail.DataDetectorTypes = UIDataDetectorType.Address;
							txtPhone.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
							txtFax.DataDetectorTypes = UIDataDetectorType.PhoneNumber;

							txtEmail.Text = org.Email;
							txtPhone.Text = org.Phone1;
							txtFax.Text = org.Phone2;
							string contentDirectoryPath = Path.Combine (NSBundle.MainBundle.BundlePath, "Resources/");

							wvMessage.LoadHtmlString (string.Format ("<html><body>{0}</body></html>", org.HomePage), new NSUrl(contentDirectoryPath, true));

							btnBrowser.TouchUpInside += delegate {
								UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + org.Url.Replace("http://", "")));
							};
							btnTwitter.TouchUpInside += delegate {
								UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + org.Twitter.Replace("http://", "")));
							};
							btnFacebook.TouchUpInside += delegate {
								UIApplication.SharedApplication.OpenUrl (new NSUrl ("https://" + org.Facebook.Replace("https://", "").Replace("http://", "")));
							};
							overlay.Hide();
						});
					}
				}
			});
		}
	}
}
