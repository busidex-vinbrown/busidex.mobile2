using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.Drawing;

namespace Busidex.Presentation.iOS
{
	public class PhoneNumberTableSource : UITableViewSource
	{
		protected const float BASE_CELL_HEIGHT = 60f;
		public static NSString PhoneNumberCellId = new NSString ("pCellId");
		List<PhoneNumber> PhoneNumbers;

		public PhoneNumberTableSource (List<PhoneNumber> phoneNumbers)
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

				AddControls (cell, number);

				cell.SetNeedsLayout ();
				cell.SetNeedsDisplay ();
			}

			return cell;
		}

		static void AddControls (UIView cell, PhoneNumber number)
		{
			const float labelX = 10f;
			const float labelY = 10f;
			const float labelHeight = 40f;
			const float labelWidth = 80f;
			const float buttonHeight = 30f;
			const float buttonWidth = 30f;
			const float phoneFrameX = labelX + labelWidth + 10;
			const float phoneFrameWidth = 110f;

			var labelFrame = new RectangleF (labelX, labelY, labelWidth, labelHeight);
			var phoneFrame = new RectangleF (phoneFrameX, labelY, phoneFrameWidth, labelHeight);
			var editImageFrame = new RectangleF (phoneFrameX + phoneFrame.Width + 20f, labelY, buttonWidth, buttonHeight);
			var deleteImageFrame = new RectangleF (editImageFrame.X + 50f, editImageFrame.Y, editImageFrame.Width, editImageFrame.Height);

			var newLabel = new UILabel (labelFrame);
			var newNumber = new UILabel (phoneFrame);
			var editButton = UIButton.FromType (UIButtonType.Custom);
			var deleteButton = UIButton.FromType (UIButtonType.Custom);

			newLabel.Text = Enum.GetName (typeof(PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId);

			newLabel.Font = UIFont.FromName ("Helvetica", 18f);
			newLabel.UserInteractionEnabled = true;
			newLabel.TextColor = UIColor.FromRGB (66, 69, 76);
			newLabel.TextAlignment = UITextAlignment.Right;

			newNumber.Text = number.Number;
			newNumber.Font = UIFont.FromName ("Helvetica", 16f);
			newNumber.TextColor = UIColor.FromRGB (66, 69, 76);
			newNumber.TextAlignment = UITextAlignment.Left;

			editButton.Frame = editImageFrame;
			editButton.SetBackgroundImage (UIImage.FromFile ("edit.png"), UIControlState.Normal);

			deleteButton.Frame = deleteImageFrame;
			deleteButton.SetBackgroundImage (UIImage.FromFile ("delete2.png"), UIControlState.Normal);

			cell.Add (newLabel);
			cell.Add (newNumber);
			cell.Add (editButton);
			cell.Add (deleteButton);
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return 1;
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

