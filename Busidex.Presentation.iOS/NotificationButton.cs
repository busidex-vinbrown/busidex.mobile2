using System;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public sealed class NotificationButton : UIButton
	{
		int NotificationCount { get; set; }

		public NotificationButton (int count)
		{
			NotificationCount = count;
			SetBackground ();
			SetCountLabel ();
			Hidden = count == 0;
		}

		void SetBackground(){
			SetBackgroundImage (UIImage.FromBundle ("Icon-Small-50@2x.png"), UIControlState.Normal);
		}

		void SetCountLabel(){

			var label = new UILabel (new CoreGraphics.CGRect (15f, -5f, 12f, 22f));
			label.Text = NotificationCount.ToString();
			label.Font = UIFont.FromName ("Helvetica-Bold", 16f);
			label.TextColor = UIColor.Brown;

			AddSubview (label);
		}
	}
}

