﻿using System;
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
		}

		public int TotalItems{ get; set; }

		float GetProgress(float current){
			if(current.Equals(0f)){
				return current;
			}
			return TotalItems.Equals (0f) ? 100f : (float)Math.Round (current / TotalItems, 2);
		}

		public void UpdateProgress(int currentItem){
			ProgressBar.SetProgress (GetProgress (currentItem));
			LoadingLable.Text = string.Format("{0} of {1}", currentItem, TotalItems);
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

			var width = (float)UIScreen.MainScreen.Bounds.Width;
			var height = (float)UIScreen.MainScreen.Bounds.Height;
			var progressFrame = new CoreGraphics.CGRect (width * .15, height * 55, width * .85, 5f);

			ProgressBar = new UIProgressView (progressFrame);
			UpdateProgress (0);

			AddSubview (ProgressBar);
		}
	}
}

