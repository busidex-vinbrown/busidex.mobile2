
using System;
using System.Drawing;

using Foundation;
using UIKit;
using System.Collections.Generic;

namespace Busidex.Presentation.iOS
{
	public class ButtonPanelControllerController : UICollectionViewController
	{

		const float ANIMATION_SPEED = 0.5f;
		bool visible;
		List<UIButton> Buttons { get; set; }

		public ButtonPanelControllerController (UICollectionViewLayout layout) : base (layout)
		{
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register any custom UICollectionViewCell classes
			CollectionView.RegisterClassForCell (typeof(ButtonPanelControllerCell), ButtonPanelControllerCell.Key);
			

		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (ButtonPanelControllerCell.Key, indexPath) as ButtonPanelControllerCell;

			var button = Buttons [indexPath.Row];
			foreach(var view in cell.Subviews){
				view.RemoveFromSuperview ();
			}

			cell.AddSubview (button);

			return cell;
		}

		public void Toggle(){

			UIView.Animate (ANIMATION_SPEED, () => {

				nfloat x = visible ? View.Frame.Width : 0;
				visible = !visible;
				View.Frame = new CoreGraphics.CGRect(x, View.Frame.Location.Y, View.Frame.Size.Width, View.Frame.Size.Height);
			});
		}

		public void Hide(){

			if (!visible)
				return;

			UIView.Animate (ANIMATION_SPEED, () => {
				View.Frame = new CoreGraphics.CGRect(View.Frame.Width, View.Frame.Location.Y, View.Frame.Size.Width, View.Frame.Size.Height);
				visible = false;
			});
		}
	}
}

