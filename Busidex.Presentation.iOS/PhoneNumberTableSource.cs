using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.Drawing;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public delegate void OnPhoneNumberEditingHandler (PhoneNumber number);
	public delegate void OnPhoneNumberDeletingHandler (int idx);

	public class PhoneNumberTableSource : UITableViewSource
	{
		public event OnPhoneNumberEditingHandler OnPhoneNumberEditing;
		public event OnPhoneNumberDeletingHandler OnPhoneNumberDeleting;

		protected const float BASE_CELL_HEIGHT = 40f;
		public static NSString PhoneNumberCellId = new NSString ("pCellId");
		List<PhoneNumber> PhoneNumbers;

		public PhoneNumberTableSource (List<PhoneNumber> phoneNumbers)
		{
			PhoneNumbers = new List<PhoneNumber> ();
			PhoneNumbers.AddRange (phoneNumbers);
		}

		public void UpdateData (List<PhoneNumber> phoneNumbers)
		{
			PhoneNumbers = new List<PhoneNumber> ();
			PhoneNumbers.AddRange (phoneNumbers);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			var cell = tableView.DequeueReusableCell (PhoneNumberCellId, indexPath);
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			cell.Tag = indexPath.Row;

			if (indexPath.Row <= PhoneNumbers.Count) {
				var number = PhoneNumbers [indexPath.Row];

				AddControls (cell, number, indexPath.Row);

				cell.SetNeedsLayout ();
				cell.SetNeedsDisplay ();
			}

			return cell;
		}

		void AddControls (UIView cell, PhoneNumber number, int idx)
		{
			const float labelX = 10f;
			const float labelY = 10f;
			const float labelHeight = 40f;
			const float labelWidth = 30f;
			const float extensionWidth = 40f;
			const float buttonHeight = 30f;
			const float buttonWidth = 30f;
			const float phoneFrameX = labelX + labelWidth + 10;
			const float phoneFrameWidth = 100f;
			const float extensionFrameX = phoneFrameX + phoneFrameWidth + 10f;
			const float editImageFrameX = extensionFrameX + extensionWidth + 20f;
			const float deleteImageFrameX = editImageFrameX + buttonWidth + 20f;

			var labelFrame = new RectangleF (labelX, labelY, labelWidth, labelHeight);
			var phoneFrame = new RectangleF (phoneFrameX, labelY, phoneFrameWidth, labelHeight);
			var extensionFrame = new RectangleF (extensionFrameX, labelY, extensionWidth, labelHeight);
			var editImageFrame = new RectangleF (editImageFrameX, extensionFrame.Y, buttonWidth, buttonHeight);
			var deleteImageFrame = new RectangleF (deleteImageFrameX, editImageFrame.Y, buttonWidth, buttonHeight);

			var newLabel = new UILabel (labelFrame);
			var newNumber = new UILabel (phoneFrame);
			var newExtension = new UILabel (extensionFrame);
			var editButton = UIButton.FromType (UIButtonType.Custom);
			var deleteButton = UIButton.FromType (UIButtonType.Custom);

			newLabel.Text = number.PhoneNumberType == null ? string.Empty : "(" + Enum.GetName (typeof (PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId).Substring (0, 1).ToUpper () + ")";

			newLabel.Font = UIFont.BoldSystemFontOfSize (18f);
			newLabel.UserInteractionEnabled = true;
			newLabel.TextColor = UIColor.FromRGB (66, 69, 76);
			newLabel.TextAlignment = UITextAlignment.Right;

			newNumber.Text = number.Number.AsPhoneNumber ();
			newNumber.Font = UIFont.FromName ("Helvetica", 16f);
			newNumber.TextColor = UIColor.FromRGB (66, 69, 76);
			newNumber.TextAlignment = UITextAlignment.Left;

			newExtension.Text = string.IsNullOrEmpty (number.Extension) ? number.Extension : "x" + number.Extension;
			newExtension.Font = UIFont.FromName ("Helvetica", 16f);
			newExtension.TextColor = UIColor.FromRGB (66, 69, 76);
			newExtension.TextAlignment = UITextAlignment.Left;

			editButton.Frame = editImageFrame;
			editButton.SetBackgroundImage (UIImage.FromFile ("edit.png"), UIControlState.Normal);

			deleteButton.Frame = deleteImageFrame;
			deleteButton.SetBackgroundImage (UIImage.FromFile ("delete2.png"), UIControlState.Normal);

			editButton.TouchUpInside += delegate {
				if (OnPhoneNumberEditing != null) {
					OnPhoneNumberEditing (number);
				}
			};

			deleteButton.TouchUpInside += delegate {
				if (OnPhoneNumberDeleting != null) {
					OnPhoneNumberDeleting (idx);
				}
			};

			foreach (var v in cell.Subviews) {
				v.RemoveFromSuperview ();
			}

			cell.Add (newLabel);
			cell.Add (newNumber);
			cell.Add (newExtension);
			cell.Add (editButton);
			cell.Add (deleteButton);
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return PhoneNumbers.Count;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return BASE_CELL_HEIGHT;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}
	}
}

