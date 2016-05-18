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
				NavigationController.PushViewController (cardImageController, true);
			} else {
				NavigationController.PopToViewController (cardImageController, true);
			}	
		}

		void GoToSearchInfo ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is SearchInfoController) == 0) {
				NavigationController.PushViewController (searchInfoController, true);
			} else {
				NavigationController.PopToViewController (searchInfoController, true);
			}	
		}

		void GoToAddressInfo ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is AddressInfoController) == 0) {
				NavigationController.PushViewController (addressInfoController, true);
			} else {
				NavigationController.PopToViewController (addressInfoController, true);
			}	
		}

		void GoToContactInfo ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is ContactInfoController) == 0) {
				NavigationController.PushViewController (contactInfoController, true);
			} else {
				NavigationController.PopToViewController (contactInfoController, true);
			}	
		}

		void GoToVisibility ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is VisibilityController) == 0) {
				NavigationController.PushViewController (visibilityController, true);
			} else {
				NavigationController.PopToViewController (visibilityController, true);
			}	
		}

		void GoToTags ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is CardTagsController) == 0) {
				NavigationController.PushViewController (cardTagsController, true);
			} else {
				NavigationController.PopToViewController (cardTagsController, true);
			}	
		}
	}
}


