using System;
using Foundation;
using UIKit;
using CoreGraphics;
using System.CodeDom.Compiler;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Drawing;

namespace Busidex.Presentation.iOS
{
	public class SearchViewCell : UICollectionViewCell
	{
		UIImageView imageView;

		[Export ("initWithFrame:")]
		public SearchViewCell (RectangleF frame) : base (frame)	{

			BackgroundView = new UIView{BackgroundColor = UIColor.White};

			SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Blue};

			ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
			ContentView.Layer.BorderWidth = 2.0f;
			ContentView.BackgroundColor = UIColor.White;

			ContentView.Frame = new RectangleF (0, 0f, 120f, 80f);
			//ContentView.Transform = CGAffineTransform.MakeScale (0.8f, 0.8f);

			imageView = new UIImageView (UIImage.FromBundle ("defaultUserImage.png"));
			imageView.Frame = new RectangleF (0, 0f, 120f, 80f);
			imageView.Center = ContentView.Center;
			//imageView.Transform = CGAffineTransform.MakeScale (0.7f, 0.7f);

			ContentView.AddSubview (imageView);
		}

		public UIImage Image {
			set {
				imageView.Image = value;
			}
		}
	}
}

