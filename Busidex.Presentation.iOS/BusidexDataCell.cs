using System;
using System.Drawing;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	public class BusidexDataCell : UITableViewCell
	{

		public UIButton CardImageButton;
		//MyDataView myDataView;
		//UILabel headingLabel, subheadingLabel;
		//UIImageView imageView;
		//Size thisSize;
		//CardDisplay BusidexCard{ get; set; }
		public BusidexDataCell (UserCard card, NSString identKey) : base (UITableViewCellStyle.Default, identKey)
		{
			//SelectionStyle = UITableViewCellSelectionStyle.Gray;
			//thisSize = new Size (300, 50);
			//ContentView.BackgroundColor = UIColor.White;//.FromRGB (218, 255, 127);

			//BusidexCard = new CardDisplay (card);
			//ContentView.Add (BusidexCard);

//			imageView = new UIImageView();
//
//			headingLabel = new UILabel () {
//				Font = UIFont.FromName("Helvetica", 22f),
//				TextColor = UIColor.Blue,//.FromRGB (127, 51, 0),
//				BackgroundColor = UIColor.Clear
//			};
//			subheadingLabel = new UILabel () {
//				Font = UIFont.FromName("AmericanTypewriter", 12f),
//				TextColor = UIColor.FromRGB (38, 127, 0),
//				TextAlignment = UITextAlignment.Center,
//				BackgroundColor = UIColor.Clear
//			};
//			ContentView.Add (headingLabel);
//			ContentView.Add (subheadingLabel);
//			ContentView.Add (imageView);

			// Configure your cell here: selection style, colors, properties
			//myDataView = new MyDataView (myData);
			//ContentView.Add (myDataView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			//imageView.Frame = new RectangleF(ContentView.Bounds.Width - 63, 5, 100, 80);
			//headingLabel.Frame = new RectangleF(5, 4, ContentView.Bounds.Width - 63, 25);
			//subheadingLabel.Frame = new RectangleF(100, 18, 100, 20);
			//myDataView.Frame = ContentView.Bounds;
			//myDataView.SetNeedsDisplay ();
		}


		// Called by our client code when we get new data.
		public void UpdateCell (UserCard card)
		{
			//BusidexCard.UserCard = card;

			this.Accessory = UITableViewCellAccessory.DetailDisclosureButton;

			ContentView.SetNeedsDisplay ();
		}
	}
}

