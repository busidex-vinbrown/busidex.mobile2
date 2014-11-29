using System;
using System.Drawing;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class MyBusidexLoadingOverlay : LoadingOverlay
	{
		UIProgressView ProgressBar;
		UILabel ProgressLabel;

		public MyBusidexLoadingOverlay  (CoreGraphics.CGRect frame) : base (frame)
		{
			base.Init ();
			init ();
		}

		public int TotalItems{ get; set; }

		float GetProgress(float current){
			if(current.Equals(0f)){
				return current;
			}
			return TotalItems.Equals (0f) ? 100f : (float)Math.Round (current / TotalItems, 2);
		}

		public void UpdateProgress(int currentItem){
			ProgressBar.SetProgress (GetProgress (currentItem), true);
			ProgressLabel.Text = string.Format("{0} of {1}", currentItem, TotalItems);
		}

		void init(){

			ProgressLabel = new UILabel(new RectangleF (
				centerX - (labelWidth / 2),
				centerY + 50 ,
				labelWidth ,
				LABEL_HEIGHT
			));
			LoadingLabel.Text = "Loading your cards...";
			ProgressLabel.BackgroundColor = UIColor.Clear;
			ProgressLabel.TextColor = UIColor.White;
			ProgressLabel.TextAlignment = UITextAlignment.Center;
			ProgressLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			AddSubview (ProgressLabel);

			nfloat width = UIScreen.MainScreen.Bounds.Width;
			nfloat height = UIScreen.MainScreen.Bounds.Height;
			var progressFrame = new CoreGraphics.CGRect (width * .05f, centerY + 80f, width * .90f, 5f);

			ProgressBar = new UIProgressView (progressFrame);
			ProgressBar.ProgressTintColor = UIColor.Blue;

			ProgressBar.Hidden = false;
		   	AddSubview (ProgressBar);
			UpdateProgress (0);
		}
	}
}

