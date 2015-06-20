
using System;

using UIKit;

namespace Busidex.Presentation.iOS
{
	public partial class BaseNavigationController : UINavigationController
	{
		public BaseNavigationController  (IntPtr handle) : base (handle)
		{
		}

		public override bool ShouldAutorotate ()
		{
			return false;//ShouldAllowLandscape(); // implemet this method to return true only when u want it to
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			var shouldAllowOtherOrientation = ShouldAllowLandscape (); // same here
			if (shouldAllowOtherOrientation) 
			{
				var cardViewController = TopViewController as CardViewController;
				if(cardViewController != null){
					if(cardViewController.IsLandscape){
						return UIInterfaceOrientationMask.Landscape;
					}
					if(cardViewController.IsPortrate){
						return UIInterfaceOrientationMask.Portrait;
					}
				}
				return UIInterfaceOrientationMask.AllButUpsideDown;
			} 

			return UIInterfaceOrientationMask.Portrait;
		}

		bool ShouldAllowLandscape ()
		{
			return TopViewController is CardViewController; // implement this to return true when u want it
		}
	}
}

