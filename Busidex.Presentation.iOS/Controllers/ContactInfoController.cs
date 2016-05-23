using System;

using UIKit;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class ContactInfoController : BaseCardEditController
	{
		public ContactInfoController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Contact Info");

			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			txtEmail.Text = SelectedCard.Email;
			txtUrl.Text = SelectedCard.Url;

			tblPhoneNumbers.Source = new PhoneNumberTableSource (SelectedCard.PhoneNumbers);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			txtEmail.ShouldReturn += textField => {
				textField.ResignFirstResponder ();
				return true; 
			};
			txtUrl.ShouldReturn += textField => {
				textField.ResignFirstResponder ();
				return true; 
			};

			tblPhoneNumbers.RegisterClassForCellReuse (typeof(UITableViewCell), PhoneNumberTableSource.PhoneNumberCellId);
		}
			
	}
}


