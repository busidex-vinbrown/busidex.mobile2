using System;
using System.Drawing;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class MyBusidexLoadingOverlay : LoadingOverlay
	{
		UIProgressView ProgressBar;
		UILabel LoadingLable;

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
			LoadingLable.Text = string.Format("{0} of {1}", currentItem, TotalItems);
		}

//		protected override void Dispose (bool disposing)
//		{
//			Hide ();
//			base.Dispose (disposing);
//		}

		public void Hide(){
			base.Hide();
		}

		void init(){

			LoadingLable = new UILabel(new RectangleF (
				centerX - (labelWidth / 2),
				centerY + 20 ,
				labelWidth ,
				LABEL_HEIGHT
			));
			LoadingLable.BackgroundColor = UIColor.Clear;
			LoadingLable.TextColor = UIColor.White;
			LoadingLable.Text = "Loading your cards...";
			LoadingLable.TextAlignment = UITextAlignment.Center;
			LoadingLable.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			AddSubview (LoadingLable);

			nfloat width = UIScreen.MainScreen.Bounds.Width;
			nfloat height = UIScreen.MainScreen.Bounds.Height;
			var progressFrame = new CoreGraphics.CGRect (width * .15f, height * 55f, width * .85f, 5f);

			ProgressBar = new UIProgressView (progressFrame);
			ProgressBar.Hidden = false;
			AddSubview (ProgressBar);
			UpdateProgress (0);
		}
	}
}

