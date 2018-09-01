using System;
using System.IO;
using System.Linq;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Foundation;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class OrganizationDetailController : UIBarButtonItemWithImageViewController
	{
		public long OrganizationId{ get; set;}

		Organization SelectedOrganization;

		public OrganizationDetailController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			LoadOrganiaztion ();
		}

		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Organization Detail - " + OrganizationId);

			base.ViewDidAppear (animated);

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnMembers.TouchUpInside += delegate {
				GoToOrganizationCards(SelectedOrganization, OrgMembersController.MemberMode.Members);
			};	

			btnReferrals.TouchUpInside += delegate {
				GoToOrganizationCards(SelectedOrganization, OrgMembersController.MemberMode.Referrals);
			};
			btnBrowser.TouchUpInside += delegate {
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + SelectedOrganization.Url.Replace("http://", "")));
			};
			btnTwitter.TouchUpInside += delegate {
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + SelectedOrganization.Twitter.Replace("http://", "")));
			};
			btnFacebook.TouchUpInside += delegate {
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("https://" + SelectedOrganization.Facebook.Replace("https://", "").Replace("http://", "")));
			};
		}

		void LoadOrganiaztion(){

			SelectedOrganization = UISubscriptionService.OrganizationList.SingleOrDefault (o => o.OrganizationId == OrganizationId);
			if (SelectedOrganization != null) {
				var fileName = Path.Combine (documentsPath, SelectedOrganization.LogoFileName);
				var imageFile = fileName + "." + SelectedOrganization.LogoType;
				if (File.Exists (imageFile)) {
					var data = NSData.FromFile (imageFile);
					if (data != null) {
						imgOrgImage.Image = new UIImage (data);
					}
				}

				lblContacts.Text = SelectedOrganization.Contacts;

				txtEmail.Editable = txtPhone.Editable = txtFax.Editable = false;
				txtEmail.UserInteractionEnabled = txtPhone.UserInteractionEnabled = txtFax.UserInteractionEnabled = true;
				txtEmail.DataDetectorTypes = UIDataDetectorType.Address;
				txtPhone.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
				txtFax.DataDetectorTypes = UIDataDetectorType.PhoneNumber;

				txtEmail.Text = SelectedOrganization.Email;
				txtPhone.Text = SelectedOrganization.Phone1;
				txtFax.Text = SelectedOrganization.Phone2;
				string contentDirectoryPath = Path.Combine (NSBundle.MainBundle.BundlePath, "Resources/");

				wvMessage.LoadHtmlString (string.Format ("<html><body>{0}</body></html>", SelectedOrganization.HomePage), new NSUrl(contentDirectoryPath, true));


			}
		}
	}
}
