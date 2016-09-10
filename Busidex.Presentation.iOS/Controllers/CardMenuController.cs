using System;

using System.Linq;
using GoogleAnalytics.iOS;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class CardMenuController : BaseCardEditController
	{
		public CardMenuController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			NavigationItem.SetRightBarButtonItem (null, true);
		}
	
		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Card Menu");

			base.ViewDidAppear (animated);

			SelectedCard = UISubscriptionService.OwnedCard;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnTags.TouchUpInside += delegate {
				GoToTags ();
			};

			btnVisibility.TouchUpInside += delegate {
				GoToVisibility ();
			};

			btnSearchInfo.TouchUpInside += delegate {
				GoToSearchInfo ();
			};

			btnContactInfo.TouchUpInside += delegate {
				GoToContactInfo ();
			};

			btnCardImage.TouchUpInside += delegate {
				GoToImageEdit ();
			};

			btnAddressInfo.TouchUpInside += delegate {
				GoToAddressInfo ();
			};
		}

		void GoToImageEdit ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is CardImageController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.cardImageController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.cardImageController, true);
			}	
		}

		void GoToSearchInfo ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is SearchInfoController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.searchInfoController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.searchInfoController, true);
			}	
		}

		void GoToAddressInfo ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is AddressInfoController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.addressInfoController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.addressInfoController, true);
			}	
		}

		void GoToContactInfo ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is ContactInfoController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.contactInfoController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.contactInfoController, true);
			}	
		}

		void GoToVisibility ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is VisibilityController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.visibilityController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.visibilityController, true);
			}	
		}

		void GoToTags ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is CardTagsController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.cardTagsController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.cardTagsController, true);
			}	
		}
	}
}


