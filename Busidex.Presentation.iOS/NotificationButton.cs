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
			SetBackgroundImage (UIImage.FromBundle ("Notification.png"), UIControlState.Normal);
		}

		void SetCountLabel(){

			float x = 32f;
			float y = -4f;
			float width = 25f;
			float height = 25f;
			var label = new UILabel (new CoreGraphics.CGRect (x, y, width, height));
			label.Text = NotificationCount.ToString();
			label.Font = UIFont.FromName ("Helvetica-Bold", 16f);
			label.TextColor = UIColor.White;

			AddSubview (label);
		}
	}
}

