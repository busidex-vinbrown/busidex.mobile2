using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Busidex.Mobile.Models;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;

namespace Busidex.Presentation
{
	[Register("CardDisplay")]
	public class CardDisplay : UIView
	{

		private string imageFileName;
		private string defaultImageFileName ="defaultUserImage.png";

		public UserCard UserCard{ get; set; }

		public UILabel NameLabel;
		public UIImageView CardImage;
		public UILabel CompanyLabel;
		public UIButton EmailLabel;
		public UITextView WebsiteLabel;
		public List<UITextView> PhoneNumberLabels;
		public string SelectedControl{ get; set; }

		public CardDisplay (IntPtr p) : base(p)
		{
			Initialize ();
		}

		public CardDisplay (UserCard uc)
		{
			UserCard = uc;
			Initialize ();
		}

		void Initialize(){
			BackgroundColor = UIColor.Clear;
			Opaque =false;
			//this.Bounds = new RectangleF (new PointF (0, 10), new SizeF (250f, 120f));
			this.UpdateDisplay (UserCard);
		}

		public void UpdateDisplay(UserCard card){

			this.UserCard = card;
			this.UserInteractionEnabled = true;

			PhoneNumberLabels = new List<UITextView> ();

			CardImage = null;
			NameLabel = null;
			CompanyLabel = null;
			WebsiteLabel = null;
			EmailLabel = null;
			for(var i=0; i<	PhoneNumberLabels.Count - 1; i++){
				PhoneNumberLabels [i] = null;
			}
			PhoneNumberLabels = new List<UITextView> ();

			if (UserCard != null && UserCard.Card != null) {

				CardImage = new UIImageView ();
				//CardImage.Image = Busidex.Mobile.Utils.GetImageFromUrl (Busidex.Mobile.Utils.CARD_PATH + UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType);
				CardImage.Frame = new RectangleF (10, 10f, 120f, 80f);
				this.AddSubview (CardImage);

				float labelHeight = 21f;
				float labelWidth = 140f;

				var frame = new RectangleF (140f, 10f, labelWidth, labelHeight);

				NameLabel = new UILabel (frame); 

				NameLabel.Text = string.IsNullOrEmpty(UserCard.Card.Name) ? "(No Name)" : UserCard.Card.Name;
				NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 17f);
				frame.Y += 21;
				this.AddSubview (NameLabel);

				if (!string.IsNullOrWhiteSpace (UserCard.Card.Email)) {
					CompanyLabel = new UILabel (frame); 
					CompanyLabel.Text = UserCard.Card.CompanyName;
					CompanyLabel.Font = UIFont.FromName ("Helvetica", 12f);
					frame.Y += 21;
					this.AddSubview (CompanyLabel);
				}

				if (!string.IsNullOrWhiteSpace (UserCard.Card.Email)) {
					EmailLabel = UIButton.FromType(UIButtonType.RoundedRect);
					EmailLabel.Frame = frame;
					//EmailLabel.Editable = false;
					//EmailLabel.DataDetectorTypes = UIDataDetectorType.PhoneNumber | UIDataDetectorType.Link | UIDataDetectorType.Address;
					
					EmailLabel.SetTitle(UserCard.Card.Email, UIControlState.Normal);
					EmailLabel.Font = UIFont.FromName ("Helvetica", 12f);

					EmailLabel.UserInteractionEnabled = true;


//					var emailTap = new UITapGestureRecognizer(
//						tap => DoTap("EMAIL")
//					);
//					EmailLabel.AddGestureRecognizer(emailTap);

					frame.Y += 21;
					this.AddSubview (EmailLabel);
					EmailLabel.TouchDown += (sender, e) => {
						new UIAlertView("Touch1", "TouchUpInside handled", null, "OK", null).Show();
						SelectedControl = "EMAIL";
					};
					EmailLabel.ClipsToBounds = false;
				}

				if (!string.IsNullOrWhiteSpace (UserCard.Card.Url)) {
					WebsiteLabel = new UITextView (frame); 
					WebsiteLabel.Text = "http://" + UserCard.Card.Url.Replace("http://", "");
					WebsiteLabel.Font = UIFont.FromName ("Helvetica", 12f);
					WebsiteLabel.UserInteractionEnabled = true;

					WebsiteLabel.Editable = false;
					WebsiteLabel.DataDetectorTypes = UIDataDetectorType.Link;
					var urlTap = new UITapGestureRecognizer(
						tap => DoTap("URL")
					);
					urlTap.CancelsTouchesInView = false;
					WebsiteLabel.AddGestureRecognizer(urlTap);

					frame.Y += 21;
					this.AddSubview (WebsiteLabel);
				}

				if (UserCard.Card.PhoneNumbers != null) {

					foreach (PhoneNumber number in UserCard.Card.PhoneNumbers) {
						var newLabel = new UITextView (frame);
						newLabel.UserInteractionEnabled = true;

						newLabel.Text = number.Number;
						newLabel.Font = UIFont.FromName ("Helvetica", 12f);
						newLabel.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
						newLabel.UserInteractionEnabled = true;

						newLabel.Editable = false;

						frame.Y += 21;

						var labelTap = new UITapGestureRecognizer(
							tap => DoTap(number.Number)
						);
						newLabel.AddGestureRecognizer(labelTap);

						this.AddSubview (newLabel);

						PhoneNumberLabels.Add (newLabel);
					}
				}


			}
		}

		public void DoTap(string ctrl){

			SelectedControl = ctrl;
		}

		public override void AwakeFromNib ()
		{
			Initialize ();
		}

		[Export("ImageFileName"), Browsable(true)]
		public string ImageFileName {
			get {
				if (String.IsNullOrWhiteSpace (imageFileName))
					return defaultImageFileName;
				else if (UIImage.FromBundle(imageFileName) != null)
					return imageFileName;
				else
					return defaultImageFileName;
			}
			set {
				imageFileName = value;
				SetNeedsDisplay ();
			}
		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				g.SetFillColor ((UIColor.FromPatternImage (UIImage.FromBundle (ImageFileName)).CGColor));
					
			}
		}
	}
}

