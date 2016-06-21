using System;

using UIKit;
using GoogleAnalytics.iOS;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Text.RegularExpressions;

namespace Busidex.Presentation.iOS
{
	public partial class ContactInfoController : BaseCardEditController
	{
		PhoneNumberTypeModel model;

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

			var source = new PhoneNumberTableSource (SelectedCard.PhoneNumbers);
			source.OnPhoneNumberEditing += editPhoneNumber;
			source.OnPhoneNumberDeleting += deletePhoneNumber;

			tblPhoneNumbers.Source = source;
			pckPhoneNumberTypes.Model = model;

			btnAddNumber.TouchUpInside += delegate {
				editPhoneNumber (null);
			};
		}

		void deletePhoneNumber (PhoneNumber number)
		{
			ShowAlert ("Delete", string.Format ("Delete {0}?", number.Number.AsPhoneNumber ()), new [] { "Ok", "Cancel" }).ContinueWith (button => {
				if (button.Result == 0) {
					SelectedCard.PhoneNumbers.RemoveAll (p =>
														 p.Number.Equals (number.Number) &&
														 p.Extension.Equals (number.Extension) &&
														 p.PhoneNumberType.Name.Equals (number.PhoneNumberType.Name));
					InvokeOnMainThread (() => {
						((PhoneNumberTableSource)tblPhoneNumbers.Source).UpdateData (SelectedCard.PhoneNumbers);
						tblPhoneNumbers.ReloadData ();
					});
				}
			});
		}

		void editPhoneNumber (PhoneNumber number)
		{
			fadeOut ();

			if (number == null) {
				txtNewPhoneNumber.Text = txtNewExtension.Text = string.Empty;
			} else {
				txtNewPhoneNumber.Text = number.Number;
				txtNewExtension.Text = number.Extension;

				model = new PhoneNumberTypeModel (null, number.PhoneNumberType);
				model.SelectedPhoneNumberType = number.PhoneNumberType;

				pckPhoneNumberTypes.Model = model;
				pckPhoneNumberTypes.Select (model.IndexOf (number.PhoneNumberType), 0, false);
			}
			vwNewPhoneNumber.Hidden = false;
		}

		void clearFields ()
		{
			txtNewExtension.Text = txtNewPhoneNumber.Text = string.Empty;
			txtNewExtension.ResignFirstResponder ();
			txtNewPhoneNumber.ResignFirstResponder ();
		}

		private void updateText (char [] digits)
		{
			var display = string.Empty;
			for (var i = 0; i < digits.Length; i++) {
				switch (i) {
				case 0: {
						display = "(" + digits [i];
						break;
					}
				case 1: {
						display += digits [i];
						break;
					}
				case 2: {
						display += digits [i] + ") ";
						break;
					}
				case 3: {
						display += "(" + digits [i];
						break;
					}
				case 4: {
						display += digits [i];
						break;
					}
				case 5: {
						display += digits [i] + ") ";
						break;
					}
				case 6: {
						display += "(" + digits [i];
						break;
					}
				case 7: {
						display += digits [i];
						break;
					}
				case 8: {
						display += digits [i];
						break;
					}
				case 9: {
						display += digits [i] + ") ";
						break;
					}
				}
			}
			txtNewPhoneNumber.Text = display;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			model = new PhoneNumberTypeModel (null, null);

			btnAddNewPhoneNumber.TouchUpInside += delegate {
				vwNewPhoneNumber.Hidden = true;
				if (model.SelectedPhoneNumberType == null) {
					model.SelectedPhoneNumberType = new PhoneNumberType {
						Name = "Business",
						PhoneNumberTypeId = 1
					};
				}
				SelectedCard.PhoneNumbers.Add (new PhoneNumber {
					Number = txtNewPhoneNumber.Text.Trim ().Replace ("(", "").Replace (")", "").Replace (" ", "."),
					Extension = txtNewExtension.Text,
					PhoneNumberType = model.SelectedPhoneNumberType
				});
				((PhoneNumberTableSource)tblPhoneNumbers.Source).UpdateData (SelectedCard.PhoneNumbers);
				tblPhoneNumbers.ReloadData ();
				clearFields ();
				fadeIn ();
			};

			txtNewPhoneNumber.ShouldChangeCharacters = (tf, range, replacementString) => {
				bool isBackspace = replacementString == "";

				var digits = Regex.Replace (txtNewPhoneNumber.Text, @"[^\d]", "").ToCharArray ();
				if (isBackspace || digits.Length > 10) {
					Array.Resize (ref digits, digits.Length - 1);
					updateText (digits);
					return false;
				}
				return true;
			};

			txtNewPhoneNumber.AllEditingEvents += (object sender, EventArgs e) => {

				var digits = Regex.Replace (txtNewPhoneNumber.Text, @"[^\d]", "").ToCharArray ();
				updateText (digits);
			};
			btnCancel.TouchUpInside += delegate {
				vwNewPhoneNumber.Hidden = true;

				clearFields ();
				fadeIn ();
			};

			txtEmail.ShouldReturn += textField => {
				textField.ResignFirstResponder ();
				return true;
			};
			txtUrl.ShouldReturn += textField => {
				textField.ResignFirstResponder ();
				return true;
			};

			btnSave.TouchUpInside += delegate {
				UISubscriptionService.SaveCardInfo (new CardDetailModel (SelectedCard));
			};

			tblPhoneNumbers.RegisterClassForCellReuse (typeof (UITableViewCell), PhoneNumberTableSource.PhoneNumberCellId);
		}

		void fadeOut ()
		{
			tblPhoneNumbers.Hidden = lblTitle.Hidden = lblDescription.Hidden = lblDescription.Hidden = true;
			UIView.Animate (
					0.5, // duration
					() => {
						lblDescription.BackgroundColor = tblPhoneNumbers.BackgroundColor = View.BackgroundColor = UIColor.UnderPageBackgroundColor;
						btnSave.Alpha = .3f;
					},
					() => {
						btnSave.Enabled = false;
					}
				);
		}

		void fadeIn ()
		{
			tblPhoneNumbers.Hidden = lblTitle.Hidden = lblDescription.Hidden = lblDescription.Hidden = false;
			UIView.Animate (
					0.5, // duration
					() => {
						lblDescription.BackgroundColor = tblPhoneNumbers.BackgroundColor = View.BackgroundColor = UIColor.White;
					},
					() => {
						btnSave.Enabled = true;
						btnSave.Alpha = 1f;
					}
				);
		}
	}
}


