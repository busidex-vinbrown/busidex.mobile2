using System;
using UIKit;
using System.Collections.Generic;
using Foundation;

namespace Busidex.Presentation.iOS
{
	public class ButtonPanelSource : UICollectionViewDataSource
	{
		#region implemented abstract members of UICollectionViewDataSource
		List<UIButton> Buttons { get; set; }

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return Buttons.Count;
		}

		#endregion

		static readonly NSString panelCellId = new NSString ("cellId");

		public ButtonPanelSource (List<UIButton> buttons)
		{
			Buttons = new List<UIButton> ();
			Buttons.AddRange (buttons);
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var buttonCell = (UICollectionViewCell)collectionView.DequeueReusableCell (panelCellId, indexPath);

			var button = Buttons [indexPath.Row];
			foreach(var view in buttonCell.Subviews){
				view.RemoveFromSuperview ();
			}

			buttonCell.AddSubview (button);

			//var animal = animals [indexPath.Row];

			//animalCell.Image = animal.Image;

			return buttonCell;
		}

	}
}

