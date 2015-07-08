
using System;

using UIKit;
using CoreAnimation;

namespace Busidex.Presentation.iOS
{
	public partial class BaseNavigationController : UINavigationController
	{
		public enum NavigationDirection{
			Forward = 1,
			Backward = 2
		}

		public NavigationDirection Direction{ get; set; }

		public BaseNavigationController  (IntPtr handle) : base (handle)
		{
		}

		public override bool ShouldAutorotate ()
		{
			return false;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}

		protected void OnSwipeRight(UIGestureRecognizer sender){
			
		}

		protected void OnSwipeLeft(UIGestureRecognizer sender){

		}

		protected void OnSwipeDown(UIGestureRecognizer sender){

		}
			
		public override void PushViewController (UIViewController viewController, bool animated)
		{
			var transition = CATransition.CreateAnimation ();
			transition.Duration = 0.25f;
			transition.Type = CAAnimation.TransitionPush;
			transition.Subtype = Direction == NavigationDirection.Backward ? CAAnimation.TransitionFromLeft : CAAnimation.TransitionFromRight;

			this.View.Layer.AddAnimation (transition, "slide");

			base.PushViewController (viewController, animated);

			this.View.Layer.RemoveAnimation ( "slide");

		}

		bool ShouldAllowLandscape ()
		{
			return false;
		}
	}
}

