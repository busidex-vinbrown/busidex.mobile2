
using System;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;

namespace Busidex.Presentation.iOS
{
	public partial class UIBarButtonItemWithImageViewController : BaseController
	{
		protected UIButton myBusidexButtonView;
		UIButton searchButtonView;
		UIButton myOrganizationsButtonView;
		UIButton eventsButtonView;

		public UIBarButtonItemWithImageViewController (IntPtr handle) : base (handle)
		{
		}

		public UIBarButtonItemWithImageViewController ()
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		protected void OnSwipeRight ()
		{
			if (this is SearchController) {
				GoHome ();
			}
			if (this is MyBusidexController) {
				GoToSearch (BaseNavigationController.NavigationDirection.Backward);
			}
			if (this is OrganizationsController) {
				GoToMyBusidex (BaseNavigationController.NavigationDirection.Backward);
			}
			if (this is EventListController) {
				GoToMyOrganizations (BaseNavigationController.NavigationDirection.Backward);
			}
		}

		protected void OnSwipeLeft ()
		{
			if (this is HomeController) {
				GoToSearch (BaseNavigationController.NavigationDirection.Forward);
			}
			if (this is SearchController) {
				GoToMyBusidex (BaseNavigationController.NavigationDirection.Forward);
			}
			if (this is MyBusidexController) {
				GoToMyOrganizations (BaseNavigationController.NavigationDirection.Forward);
			}
			if (this is OrganizationsController) {
				GoToEvents (BaseNavigationController.NavigationDirection.Forward);
			}
		}

		protected void OnSwipeDown ()
		{
			Sync ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var swiperRight = new UISwipeGestureRecognizer (new Action (OnSwipeRight));
			swiperRight.Direction = UISwipeGestureRecognizerDirection.Right;

			var swiperLeft = new UISwipeGestureRecognizer (new Action (OnSwipeLeft));
			swiperLeft.Direction = UISwipeGestureRecognizerDirection.Left;

			var swiperDown = new UISwipeGestureRecognizer (new Action (OnSwipeDown));
			swiperDown.Direction = UISwipeGestureRecognizerDirection.Down;

			View.AddGestureRecognizer (swiperRight);
			View.AddGestureRecognizer (swiperLeft);

			if (this is HomeController) {
				View.AddGestureRecognizer (swiperDown);
			}

			SetUpNavBarButtons ();
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, false);
			}
			NavigationItem.SetHidesBackButton (true, true);
		}

		protected void Sync ()
		{

			UISubscriptionService.Sync ();

			var overlay = new MyBusidexLoadingOverlay (View.Bounds);
			const int TOTAL_TASKS = 4;
			var completedTasks = 0;
			const string MESSAGE = "Synchronizing your account items...";
			overlay.MessageText = string.Format (MESSAGE, 0);

			View.AddSubview (overlay);
			overlay.TotalItems = TOTAL_TASKS;

			OnMyBusidexUpdatedEventHandler update = status => InvokeOnMainThread (() => {
				

			});

			OnMyBusidexLoadedEventHandler callback1 = list => InvokeOnMainThread (() => {
				overlay.UpdateProgress (completedTasks++);
				if (completedTasks == TOTAL_TASKS) {
					overlay.Hide ();
				}
			});

			OnEventListLoadedEventHandler callback2 = list => InvokeOnMainThread (() => {
				overlay.UpdateProgress (completedTasks++);
				if (completedTasks == TOTAL_TASKS) {
					overlay.Hide ();
				}
			});

			OnMyOrganizationsLoadedEventHandler callback3 = list => InvokeOnMainThread (() => {
				overlay.UpdateProgress (completedTasks++);
				if (completedTasks == TOTAL_TASKS) {
					overlay.Hide ();
				}
			});

			OnNotificationsLoadedEventHandler callback4 = list => InvokeOnMainThread (() => {
				overlay.UpdateProgress (completedTasks++);
				if (completedTasks == TOTAL_TASKS) {
					overlay.Hide ();
				}
			});

			UISubscriptionService.OnMyBusidexUpdated += update;
			UISubscriptionService.OnMyBusidexLoaded += callback1;
			UISubscriptionService.OnEventListLoaded += callback2;
			UISubscriptionService.OnMyOrganizationsLoaded += callback3;
			UISubscriptionService.OnNotificationsLoaded += callback4;

		}

		public void GoToMyBusidex (BaseNavigationController.NavigationDirection direction)
		{
			if (NavigationController == null || (NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers [NavigationController.ViewControllers.Length - 1]  is MyBusidexController)) {
				return;
			}
			((BaseNavigationController)NavigationController).Direction = direction;

			if (NavigationController.ViewControllers.Any (c => c as MyBusidexController != null)) {
				NavigationController.PopToViewController (BaseNavigationController.myBusidexController, true);
			} else {
				NavigationController.PushViewController (BaseNavigationController.myBusidexController, true);
			}
		}

		protected void GoToSearch (BaseNavigationController.NavigationDirection direction)
		{
			if (NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers [NavigationController.ViewControllers.Length - 1]  is SearchController) {
				return;
			}
			((BaseNavigationController)NavigationController).Direction = direction;

			if (NavigationController.ViewControllers.Any (c => c as SearchController != null)) {
				NavigationController.PopToViewController (BaseNavigationController.searchController, true);
			} else {
				NavigationController.PushViewController (BaseNavigationController.searchController, true);
			}
		}

		protected void GoToMyOrganizations (BaseNavigationController.NavigationDirection direction)
		{
			try {
				if (NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers [NavigationController.ViewControllers.Length - 1]  is OrganizationsController) {
					return;
				}
				((BaseNavigationController)NavigationController).Direction = direction;

				if (NavigationController.ViewControllers.Any (c => c as OrganizationsController != null)) {
					NavigationController.PopToViewController (BaseNavigationController.organizationsController, true);
				} else {
					NavigationController.PushViewController (BaseNavigationController.organizationsController, true);
				}
			} catch (Exception ex) {
				new UIAlertView ("Error", ex.Message, null, "OK", null).Show ();
			}
		}

		protected void GoToEvents (BaseNavigationController.NavigationDirection direction)
		{

			if (NavigationController == null || NavigationController.ViewControllers == null) {
				return;
			}
			if (NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers [NavigationController.ViewControllers.Length - 1]  is EventListController) {
				return;
			}

			((BaseNavigationController)NavigationController).Direction = direction;

			if (NavigationController.ViewControllers.Any (c => c as EventListController != null)) {
				NavigationController.PopToViewController (BaseNavigationController.eventListController, true);
			} else {
				NavigationController.PushViewController (BaseNavigationController.eventListController, true);
			}
		}

		protected void GoToOrganizationCards (Organization org, OrgMembersController.MemberMode mode)
		{
			try {

				if (NavigationController == null || NavigationController.ViewControllers == null) {
					return;
				}
				if (NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers [NavigationController.ViewControllers.Length - 1]  is EventListController) {
					return;
				}
				BaseNavigationController.orgMembersController.OrganizationId = org.OrganizationId;
				BaseNavigationController.orgMembersController.OrganizationMemberMode = mode;
				BaseNavigationController.orgMembersController.OrganizationName = org.Name;
				BaseNavigationController.orgMembersController.OrganizationLogo = org.LogoFileName + "." + org.LogoType;
				BaseNavigationController.orgMembersController.IsOrgMember = org.IsMember;

				if (NavigationController.ViewControllers.Any (c => c as OrgMembersController != null)) {
					NavigationController.PopToViewController (BaseNavigationController.orgMembersController, true);
				} else {
					NavigationController.PushViewController (BaseNavigationController.orgMembersController, true);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		protected void GoHome ()
		{
			((BaseNavigationController)NavigationController).Direction = BaseNavigationController.NavigationDirection.Backward;

			if (NavigationController.ViewControllers.Any (c => c as HomeController != null)) {
				NavigationController.PopToViewController (BaseNavigationController.homeController, true);
			} else {
				NavigationController.PushViewController (BaseNavigationController.homeController, true);
			}
		}

		protected void SetUpNavBarButtons ()
		{

			nfloat imageSize = 40f;
			var frame = new CoreGraphics.CGRect (imageSize * 2, 0, imageSize, imageSize);

			// EVENTS
			eventsButtonView = new UIButton ();
			if (this is EventListController) {
				eventsButtonView.SetBackgroundImage (UIImage.FromFile ("EventIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			} else if (this is EventCardsController) {
				eventsButtonView.SetBackgroundImage (UIImage.FromFile ("EventIconBack.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			} else {
				eventsButtonView.SetBackgroundImage (UIImage.FromFile ("EventIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}
			eventsButtonView.Frame = frame;
			eventsButtonView.TouchUpInside += delegate {
				GoToEvents (BaseNavigationController.NavigationDirection.Forward);
			};

			// ORGANIZATIONS
			frame.X *= 2;
			myOrganizationsButtonView = new UIButton ();
			if (this is OrganizationsController) {
				myOrganizationsButtonView.SetBackgroundImage (UIImage.FromFile ("OrganizationsIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			} else if (this is OrganizationDetailController || this is OrgMembersController) {
				myOrganizationsButtonView.SetBackgroundImage (UIImage.FromFile ("EventIconBack.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			} else {
				myOrganizationsButtonView.SetBackgroundImage (UIImage.FromFile ("OrganizationsIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}
			myOrganizationsButtonView.Frame = frame;
			myOrganizationsButtonView.TouchUpInside += delegate {
				GoToMyOrganizations (BaseNavigationController.NavigationDirection.Forward);
			}; 

			frame.X *= 2;
			// MY BUSIDEX
			myBusidexButtonView = new UIButton ();
			myBusidexButtonView.SetBackgroundImage (UIImage.FromFile (this is MyBusidexController ? "MyBusidexIcon.png" : "MyBusidexIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myBusidexButtonView.Frame = frame;
			myBusidexButtonView.TouchUpInside += delegate {
				var controller = this as MyBusidexController;
				if (controller != null) {
					controller.GoToTop ();
				} else {
					GoToMyBusidex (BaseNavigationController.NavigationDirection.Forward);
				}
			}; 

			// SEARCH
			frame.X *= 2;
			searchButtonView = new UIButton ();
			searchButtonView.SetBackgroundImage (UIImage.FromFile (this is SearchController ? "SearchIcon.png" : "SearchIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			searchButtonView.Frame = frame;
			searchButtonView.TouchUpInside += delegate {
				GoToSearch (BaseNavigationController.NavigationDirection.Forward);	
			}; 

			var myBusidexButton = new UIBarButtonItem (
				                      myBusidexButtonView.ImageView.Image,
				                      UIBarButtonItemStyle.Plain,
				                      (s, e) => GoToMyBusidex (BaseNavigationController.NavigationDirection.Forward)
			                      );
			myBusidexButton.CustomView = myBusidexButtonView;

			var searchButton = new UIBarButtonItem (
				                   searchButtonView.ImageView.Image,
				                   UIBarButtonItemStyle.Plain,
				                   (s, e) => GoToSearch (BaseNavigationController.NavigationDirection.Forward)
			                   );
			searchButton.CustomView = searchButtonView;

			var myOrganizationsButton = new UIBarButtonItem (
				                            myOrganizationsButtonView.ImageView.Image,
				                            UIBarButtonItemStyle.Plain,
				                            (s, e) => GoToMyOrganizations (BaseNavigationController.NavigationDirection.Forward)
			                            );
			myOrganizationsButton.CustomView = myOrganizationsButtonView;

			var eventsButton = new UIBarButtonItem (
				                   eventsButtonView.ImageView.Image,
				                   UIBarButtonItemStyle.Plain,
				                   (s, e) => GoToEvents (BaseNavigationController.NavigationDirection.Forward)
			                   );
			eventsButton.CustomView = eventsButtonView;

			var buttonItems = new UIBarButtonItem[4];
			buttonItems [0] = eventsButton;
			buttonItems [1] = myOrganizationsButton;
			buttonItems [2] = myBusidexButton;
			buttonItems [3] = searchButton;

			NavigationItem.RightBarButtonItems = buttonItems;

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ();

			// HOME BUTTON
			var homeButton = new UIButton (UIButtonType.System);
			homeButton.SetTitle ("Home", UIControlState.Normal);
			homeButton.Frame = new CoreGraphics.CGRect (0, 0, 90f, imageSize);
			homeButton.TouchUpInside += delegate {
				GoHome ();
			};

			NavigationItem.LeftBarButtonItem.CustomView = homeButton;
			NavigationItem.LeftBarButtonItem.CustomView.Hidden = false;
		}
	}
}

