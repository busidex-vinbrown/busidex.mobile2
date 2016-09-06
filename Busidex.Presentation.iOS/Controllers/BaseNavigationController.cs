
using System;

using UIKit;
using CoreAnimation;
using System.Linq;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class BaseNavigationController : UINavigationController
	{
		public static UIStoryboard board;
		public static UIStoryboard orgBoard;
		public static UIStoryboard cardBoard;
		public static EventListController eventListController;
		public static EventCardsController eventCardsController;
		public static MyBusidexController myBusidexController;
		public static SearchController searchController;
		public static OrganizationsController organizationsController;
		public static OrgMembersController orgMembersController;
		public static HomeController homeController;
		public static QuickShareController quickShareController;
		public static ButtonPanelController buttonPanelController;
		public static SharedCardController sharedCardController;
		public static OrganizationDetailController orgDetailController;
		public static SettingsController settingsController;
		public static TermsController termsController;
		public static PrivacyController privacyController;
		public static LoginController loginController;
		public static CreateProfileController createProfileController;
		public static CardMenuController cardMenuController;
		public static CardImageController cardImageController;
		public static SearchInfoController searchInfoController;
		public static ContactInfoController contactInfoController;
		public static CardTagsController cardTagsController;
		public static AddressInfoController addressInfoController;
		public static VisibilityController visibilityController;
		public static StartupController startUpController;

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
			DoStartup ();
		}

		protected static void init ()
		{
			board = board ?? UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			orgBoard = orgBoard ?? UIStoryboard.FromName ("OrganizationStoryBoard_iPhone", null);
			cardBoard = cardBoard ?? UIStoryboard.FromName ("CardEdit_iPhone", null);

			loginController = loginController ?? board.InstantiateViewController ("LoginController") as LoginController;
			eventListController = eventListController ?? board.InstantiateViewController ("EventListController") as EventListController;
			eventCardsController = eventCardsController ?? board.InstantiateViewController ("EventCardsController") as EventCardsController;
			myBusidexController = myBusidexController ?? board.InstantiateViewController ("MyBusidexController") as MyBusidexController;
			searchController = searchController ?? board.InstantiateViewController ("SearchController") as SearchController;
			organizationsController = organizationsController ?? board.InstantiateViewController ("OrganizationsController") as OrganizationsController;
			orgMembersController = orgMembersController ?? orgBoard.InstantiateViewController ("OrgMembersController") as OrgMembersController;
			orgDetailController = orgDetailController ?? orgBoard.InstantiateViewController ("OrganizationDetailController") as OrganizationDetailController;
			homeController = homeController ?? board.InstantiateViewController ("HomeController") as HomeController;
			quickShareController = quickShareController ?? board.InstantiateViewController ("QuickShareController") as QuickShareController;
			buttonPanelController = buttonPanelController ?? board.InstantiateViewController ("ButtonPanelController") as ButtonPanelController;
			sharedCardController = sharedCardController ?? board.InstantiateViewController ("SharedCardController") as SharedCardController;
			settingsController = settingsController ?? board.InstantiateViewController ("SettingsController") as SettingsController;
			createProfileController = createProfileController ?? board.InstantiateViewController ("CreateProfileController") as CreateProfileController;
			termsController = termsController ?? board.InstantiateViewController ("TermsController") as TermsController;
			privacyController = privacyController ?? board.InstantiateViewController ("PrivacyController") as PrivacyController;
			cardMenuController = cardMenuController ?? cardBoard.InstantiateViewController ("CardMenuController") as CardMenuController;
			cardImageController = cardImageController ?? cardBoard.InstantiateViewController ("CardImageController") as CardImageController;
			searchInfoController = searchInfoController ?? cardBoard.InstantiateViewController ("SearchInfoController") as SearchInfoController;
			contactInfoController = contactInfoController ?? cardBoard.InstantiateViewController ("ContactInfoController") as ContactInfoController;
			cardTagsController = cardTagsController ?? cardBoard.InstantiateViewController ("CardTagsController") as CardTagsController;
			addressInfoController = addressInfoController ?? cardBoard.InstantiateViewController ("AddressInfoController") as AddressInfoController;
			visibilityController = visibilityController ?? cardBoard.InstantiateViewController ("VisibilityController") as VisibilityController;
			startUpController = startUpController ?? board.InstantiateViewController ("StartupController") as StartupController;
		}

		public static void Reset(){
			board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			orgBoard = UIStoryboard.FromName ("OrganizationStoryBoard_iPhone", null);
			cardBoard = UIStoryboard.FromName ("CardEdit_iPhone", null);

			//loginController = board.InstantiateViewController ("LoginController") as LoginController;
			eventListController = board.InstantiateViewController ("EventListController") as EventListController;
			eventCardsController = board.InstantiateViewController ("EventCardsController") as EventCardsController;
			myBusidexController = board.InstantiateViewController ("MyBusidexController") as MyBusidexController;
			searchController = board.InstantiateViewController ("SearchController") as SearchController;
			organizationsController = board.InstantiateViewController ("OrganizationsController") as OrganizationsController;
			orgMembersController = orgBoard.InstantiateViewController ("OrgMembersController") as OrgMembersController;
			orgDetailController = orgBoard.InstantiateViewController ("OrganizationDetailController") as OrganizationDetailController;
			//homeController = board.InstantiateViewController ("HomeController") as HomeController;
			quickShareController = board.InstantiateViewController ("QuickShareController") as QuickShareController;
			buttonPanelController = board.InstantiateViewController ("ButtonPanelController") as ButtonPanelController;
			sharedCardController = board.InstantiateViewController ("SharedCardController") as SharedCardController;
			settingsController = board.InstantiateViewController ("SettingsController") as SettingsController;
			createProfileController = board.InstantiateViewController ("CreateProfileController") as CreateProfileController;
			termsController = board.InstantiateViewController ("TermsController") as TermsController;
			privacyController = board.InstantiateViewController ("PrivacyController") as PrivacyController;
			cardMenuController = cardBoard.InstantiateViewController ("CardMenuController") as CardMenuController;
			cardImageController = cardBoard.InstantiateViewController ("CardImageController") as CardImageController;
			searchInfoController = cardBoard.InstantiateViewController ("SearchInfoController") as SearchInfoController;
			contactInfoController = cardBoard.InstantiateViewController ("ContactInfoController") as ContactInfoController;
			cardTagsController = cardBoard.InstantiateViewController ("CardTagsController") as CardTagsController;
			addressInfoController = cardBoard.InstantiateViewController ("AddressInfoController") as AddressInfoController;
			visibilityController = cardBoard.InstantiateViewController ("VisibilityController") as VisibilityController;
			startUpController = board.InstantiateViewController ("StartupController") as StartupController;
		}

		public void DoStartup(){
			transition = CATransition.CreateAnimation ();
			transition.Duration = 0.25f;
			transition.Type = CAAnimation.TransitionPush;

			init ();

			var cookie = Application.GetAuthCookie ();

			if (cookie != null) {

				UISubscriptionService.AuthToken = cookie.Value;
				var quickShareLink = Utils.GetQuickShareLink ();

				if (quickShareLink != null) {

					GoToQuickShare (quickShareLink);

				} else {

					if (ViewControllers.Any (c => c as HomeController != null)) {
						PopToViewController (homeController, true);
					} else {
						PushViewController (homeController, true);
					}
				}
			}else{

				if (ViewControllers.Any (c => c as StartupController != null)) {
					PopToViewController (startUpController, true);
				} else {
					PushViewController (startUpController, true);
				}
			}
		}

		public void GoToQuickShare (QuickShareLink link)
		{
			if (UISubscriptionService.AppQuickShareLink != null) {

				quickShareController.SetCardSharingInfo (link);

				if (ViewControllers.Any (c => c as QuickShareController != null)) {
					PopToViewController (quickShareController, true);
				} else {
					PushViewController (quickShareController, true);
				}
			}
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

				View.Layer.AddAnimation (transition, "slide");

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
			View.Layer.AddAnimation (transition, "slide");

			base.PushViewController (viewController, true);

			View.Layer.RemoveAnimation ( "slide");

		}

		bool ShouldAllowLandscape ()
		{
			return false;
		}

		//public void OpenQuickShare(QuickShareController controller){
		//	PushViewController(controller, true);
		//}
	}
}

