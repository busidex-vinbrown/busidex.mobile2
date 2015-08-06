
using System;

using UIKit;
using CoreAnimation;

namespace Busidex.Presentation.iOS
{
	public partial class BaseNavigationController : UINavigationController
	{
		static CATransition transition;

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

			transition = CATransition.CreateAnimation ();
			transition.Duration = 0.25f;
			transition.Type = CAAnimation.TransitionPush;
		}

		protected void OnSwipeRight(UIGestureRecognizer sender){
			
		}

		protected void OnSwipeLeft(UIGestureRecognizer sender){

		}

		protected void OnSwipeDown(UIGestureRecognizer sender){

		}
			
		public override void PushViewController (UIViewController viewController, bool animated)
		{
			
			transition.Subtype = Direction == NavigationDirection.Backward ? CAAnimation.TransitionFromLeft : CAAnimation.TransitionFromRight;

			this.View.Layer.AddAnimation (transition, "slide");

			base.PushViewController (viewController, true);

			this.View.Layer.RemoveAnimation ( "slide");

		}

		bool ShouldAllowLandscape ()
		{
			return false;
		}
	}
}

