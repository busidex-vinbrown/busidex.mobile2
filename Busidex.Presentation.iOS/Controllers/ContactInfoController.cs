using System;

using UIKit;
using GoogleAnalytics.iOS;
using System.Collections.Generic;
using Busidex.Mobile.Models;

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
			vwNewPhoneNumber.Hidden = true;


			tblPhoneNumbers.Source = new PhoneNumberTableSource (SelectedCard.PhoneNumbers);

			btnAddNumber.TouchUpInside += delegate {

				vwNewPhoneNumber.Hidden = false;
				txtNewPhoneNumber.Text = txtNewExtension.Text = string.Empty;

				var model = new PhoneNumberTypeModel (null, null);
				btnAddNewPhoneNumber.TouchUpInside += delegate {
					vwNewPhoneNumber.Hidden = true;
					if (model.selectedPhoneNumberType == null) {
						model.selectedPhoneNumberType = new PhoneNumberType {
							Name = "Business",
							PhoneNumberTypeId = 1
						};
					}
					SelectedCard.PhoneNumbers.Add (new PhoneNumber {
						Number = txtNewPhoneNumber.Text,
						Extension = txtNewExtension.Text,
						PhoneNumberType = model.selectedPhoneNumberType
					});
					tblPhoneNumbers.Source = new PhoneNumberTableSource (SelectedCard.PhoneNumbers);
					tblPhoneNumbers.ReloadData ();
				};

				pckPhoneNumberTypes.Model = model;

			};

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


