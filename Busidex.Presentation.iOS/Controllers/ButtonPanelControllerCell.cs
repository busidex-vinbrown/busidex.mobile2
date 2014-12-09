
using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public class ButtonPanelControllerCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("ButtonPanelControllerCell");

		[Export ("initWithFrame:")]
		public ButtonPanelControllerCell (RectangleF frame) : base (frame)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			BackgroundColor = UIColor.Cyan;
		}
	}
}

