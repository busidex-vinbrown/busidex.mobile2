using System;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public sealed class NotificationButton : UIButton
	{
		int Count { get; set; }

		public NotificationButton (int count)
		{
			Count = count;
			SetBackground ();
			SetCountLabel ();
		}

		void SetBackground(){
			SetBackgroundImage (UIImage.FromBundle ("Icon-Small-50@2x.png"), UIControlState.Normal);
		}

		void SetCountLabel(){

			var label = new UILabel (new CoreGraphics.CGRect (15f, -5f, 12f, 22f));
			label.Text = Count.ToString();
			label.Font = UIFont.FromName ("Helvetica-Bold", 16f);
			label.TextColor = UIColor.Brown;

			AddSubview (label);
		}
	}
}

