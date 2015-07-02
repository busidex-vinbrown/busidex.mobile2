
using System;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public partial class UIBarButtonItemWithImageViewController : BaseController
	{
		protected UIButton myBusidexButtonView;
		UIButton searchButtonView;
		UIButton myOrganizationsButtonView;
		UIButton eventsButtonView;
		UISwipeGestureRecognizer swiper;


		public UIBarButtonItemWithImageViewController (IntPtr handle) : base (handle)
		{
		}

		public UIBarButtonItemWithImageViewController () 
		{
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetUpNavBarButtons();
			NavigationItem.SetHidesBackButton (true, true);


			// Perform any additional setup after loading the view, typically from a nib.
		}

		protected void GoToSearch ()
		{
			if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is SearchController){
				return;
			}

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var searchController = board.InstantiateViewController ("SearchController") as SearchController;
			if (searchController != null) {
				NavigationController.PushViewController (searchController, true);
				resetButtonUI();
				searchButtonView.Alpha = 1f;
			}
		}

		protected void GoToMyBusidex ()
		{
			if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is MyBusidexController){
				return;
			}

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var myBusidexController = board.InstantiateViewController ("MyBusidexController") as MyBusidexController;

			if (myBusidexController != null){
				NavigationController.PushViewController (myBusidexController, true);
				resetButtonUI();
				myBusidexButtonView.Alpha = 1f;
			}
		}

		protected void GoToEvents(){

			if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is EventListController){
				return;
			}

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var eventListController = board.InstantiateViewController ("EventListController") as EventListController;
			if (eventListController != null) {
				NavigationController.PushViewController (eventListController, true);
				resetButtonUI();
				eventsButtonView.Alpha = 1f;
			}
		}

		protected void GoHome(){
			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var dataViewController = board.InstantiateViewController ("DataViewController") as DataViewController;
			if (dataViewController != null) {
				NavigationController.PushViewController (dataViewController, true);
				resetButtonUI();
			}
		}

		protected void GoToMyOrganizations ()
		{
			try{
				if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is OrganizationsController){
					return;
				}

				UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
				var organizationsController = board.InstantiateViewController ("OrganizationsController") as OrganizationsController;
				if (organizationsController != null){
					NavigationController.PushViewController (organizationsController, true);
					resetButtonUI();
					myOrganizationsButtonView.Alpha = 1f;
				}
			}
			catch(Exception ex){
				new UIAlertView("Error", ex.Message, null, "OK", null).Show();
			}
		}

		protected void SetUpNavBarButtons(){

			nfloat imageSize = 40f;
			var frame = new CoreGraphics.CGRect(imageSize * 2, 0, imageSize, imageSize);

			myBusidexButtonView = new UIButton();
			myBusidexButtonView.SetBackgroundImage(UIImage.FromFile ("MyBusidexIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myBusidexButtonView.Frame = frame;
			myBusidexButtonView.TouchUpInside += delegate {
				GoToMyBusidex ();
			}; 

			frame.X *= 2;
			searchButtonView = new UIButton();
			searchButtonView.SetBackgroundImage(UIImage.FromFile ("SearchIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			searchButtonView.Frame = frame;
			searchButtonView.TouchUpInside += delegate {
				GoToSearch ();	
			}; 
				
			frame.X *= 2;
			myOrganizationsButtonView = new UIButton();
			myOrganizationsButtonView.SetBackgroundImage(UIImage.FromFile ("OrganizationsIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myOrganizationsButtonView.Frame = frame;
			myOrganizationsButtonView.TouchUpInside += delegate {
				GoToMyOrganizations ();	
			}; 
				
			frame.X *= 2;
			eventsButtonView = new UIButton();
			eventsButtonView.SetBackgroundImage(UIImage.FromFile ("EventIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			eventsButtonView.Frame = frame;
			eventsButtonView.TouchUpInside += delegate {
				GoToEvents ();	
			};


			var myBusidexButton = new UIBarButtonItem (
				myBusidexButtonView.ImageView.Image,
				UIBarButtonItemStyle.Plain,
				(s, e) => GoToMyBusidex()
			);
			myBusidexButton.CustomView = myBusidexButtonView;

			var searchButton = new UIBarButtonItem (
				searchButtonView.ImageView.Image,
				UIBarButtonItemStyle.Plain,
				(s, e) => GoToSearch ()
			);
			searchButton.CustomView = searchButtonView;

			var myOrganizationsButton = new UIBarButtonItem (
				myOrganizationsButtonView.ImageView.Image,
				UIBarButtonItemStyle.Plain,
				(s, e) => GoToMyOrganizations()
			);
			myOrganizationsButton.CustomView = myOrganizationsButtonView;

			var eventsButton = new UIBarButtonItem (
				eventsButtonView.ImageView.Image,
				UIBarButtonItemStyle.Plain,
				(s, e) => GoToEvents()
			);
			eventsButton.CustomView = eventsButtonView;

			var buttonItems = new UIBarButtonItem[4];
			buttonItems [0] = myBusidexButton;
			buttonItems [1] = searchButton;
			buttonItems [2] = myOrganizationsButton;
			buttonItems [3] = eventsButton;

			NavigationItem.RightBarButtonItems = buttonItems;

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ();

			var homeButton = new UIButton (UIButtonType.System);
			homeButton.SetTitle ("Home", UIControlState.Normal);
			homeButton.Frame = new CoreGraphics.CGRect(0, 0, 90f, imageSize);
			homeButton.TouchUpInside += delegate {
				GoHome();
			};

			NavigationItem.LeftBarButtonItem.CustomView = homeButton;

		}

		void resetButtonUI(){
			foreach(UIBarButtonItem item in NavigationItem.RightBarButtonItems){
				item.CustomView.Alpha = .2f;
			}
		}
	}
}

