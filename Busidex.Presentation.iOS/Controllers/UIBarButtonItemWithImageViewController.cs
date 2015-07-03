
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
			NavigationController.SetNavigationBarHidden (false, false);
			NavigationItem.SetHidesBackButton (true, true);


			// Perform any additional setup after loading the view, typically from a nib.
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
				myBusidexButtonView.Alpha = 1f;
			}
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
				searchButtonView.Alpha = 1f;
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
				}
			}
			catch(Exception ex){
				new UIAlertView("Error", ex.Message, null, "OK", null).Show();
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
				eventsButtonView.Alpha = 1f;
			}
		}

		protected void GoHome(){
			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var dataViewController = board.InstantiateViewController ("DataViewController") as DataViewController;
			if (dataViewController != null) {
				NavigationController.PushViewController (dataViewController, true);
			}
		}

		protected void SetUpNavBarButtons(){

			nfloat imageSize = 40f;
			var frame = new CoreGraphics.CGRect(imageSize * 2, 0, imageSize, imageSize);

			myBusidexButtonView = new UIButton();
			myBusidexButtonView.SetBackgroundImage(UIImage.FromFile ("MyBusidexIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myBusidexButtonView.Frame = frame;
			myBusidexButtonView.Alpha = .3f;
			myBusidexButtonView.TouchUpInside += delegate {
				GoToMyBusidex ();
			}; 

			frame.X *= 2;
			searchButtonView = new UIButton();
			searchButtonView.SetBackgroundImage(UIImage.FromFile ("SearchIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			searchButtonView.Frame = frame;
			searchButtonView.Alpha = .3f;
			searchButtonView.TouchUpInside += delegate {
				GoToSearch ();	
			}; 
				
			frame.X *= 2;
			myOrganizationsButtonView = new UIButton();
			myOrganizationsButtonView.SetBackgroundImage(UIImage.FromFile ("OrganizationsIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myOrganizationsButtonView.Frame = frame;
			myOrganizationsButtonView.Alpha = .3f;
			myOrganizationsButtonView.TouchUpInside += delegate {
				GoToMyOrganizations ();	
			}; 
				
			frame.X *= 2;
			eventsButtonView = new UIButton();
			eventsButtonView.SetBackgroundImage(UIImage.FromFile ("EventIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			eventsButtonView.Frame = frame;
			eventsButtonView.Alpha = .3f;
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

			int selectedItem = -1;

			if (this is MyBusidexController) {
				selectedItem = 0;
			}else if(this is SearchController){
				selectedItem = 1;
			}else if(this is OrganizationsController){
				selectedItem = 2;
			}else if(this is EventListController){
				selectedItem = 3;
			}
//			if(selectedItem >= 0){
//				buttonItems [selectedItem].TintColor = UIColor.FromRGB(100, 100, 100);//.Layer.BorderColor = UIColor.FromRGB (224, 224, 224).CGColor;
//			}
			//buttonItems [selectedItem] .CustomView.Layer.BorderWidth = 2.0f;


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
	}
}

