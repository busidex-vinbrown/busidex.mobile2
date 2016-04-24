
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
			Backward = 2,
			Up = 3,
			Down = 4
		}
		public int id { get; set; }

		public NavigationDirection Direction{ get; set; }

		public BaseNavigationController  (UIViewController controller) : base (controller)
		{
		}

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
			
		public override UIViewController PopViewController (bool animated)
		{
			if(transition != null){
				switch(Direction){
				case NavigationDirection.Backward: {
						transition.Subtype = CAAnimation.TransitionFromLeft;
						break;
					}
				case NavigationDirection.Forward: {
						transition.Subtype = CAAnimation.TransitionFromRight;
						break;
					}
				case NavigationDirection.Up: {
						transition.Subtype = CAAnimation.TransitionReveal;
						Direction = NavigationDirection.Down;
						break;
					}
				case NavigationDirection.Down: {
						transition.Subtype = CAAnimation.TransitionFromBottom;
						Direction = NavigationDirection.Backward;
						break;
					}
				default: {
						transition.Subtype = CAAnimation.TransitionFromLeft;
						break;
					}
				}
			}
			this.View.Layer.AddAnimation (transition, "slide");

			return base.PopViewController (animated);
		}

		public override void PushViewController (UIViewController viewController, bool animated)
		{
			if(transition != null){
				switch(Direction){
				case NavigationDirection.Backward: {
						transition.Subtype = CAAnimation.TransitionFromLeft;
						break;
					}
				case NavigationDirection.Forward: {
						transition.Subtype = CAAnimation.TransitionFromRight;
						break;
					}
				case NavigationDirection.Up: {
						transition.Subtype = CAAnimation.TransitionReveal;
						Direction = NavigationDirection.Down;
						break;
					}
				case NavigationDirection.Down: {
						transition.Subtype = CAAnimation.TransitionFromBottom;
						Direction = NavigationDirection.Backward;
						break;
					}
				default: {
						transition.Subtype = CAAnimation.TransitionFromLeft;
						break;
					}
				}
			}
			this.View.Layer.AddAnimation (transition, "slide");

			base.PushViewController (viewController, true);

			this.View.Layer.RemoveAnimation ( "slide");

		}

		bool ShouldAllowLandscape ()
		{
			return false;
		}

		public void OpenQuickShare(QuickShareController controller){
			PushViewController(controller, true);
		}
	}
}

