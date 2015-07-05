
using System;
using UIKit;
using System.Threading.Tasks;
using System.IO;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;

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
				if (NavigationController != null) {
					NavigationController.SetNavigationBarHidden (false, false);
				}
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
			myBusidexButtonView.SetBackgroundImage(UIImage.FromFile (this is MyBusidexController ? "MyBusidexIcon.png" : "MyBusidexIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myBusidexButtonView.Frame = frame;
			myBusidexButtonView.TouchUpInside += async delegate {
				await LoadMyBusidexAsync();
			}; 

			frame.X *= 2;
			searchButtonView = new UIButton();
			searchButtonView.SetBackgroundImage(UIImage.FromFile (this is SearchController ? "SearchIcon.png" : "SearchIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			searchButtonView.Frame = frame;
			searchButtonView.TouchUpInside += delegate {
				GoToSearch ();	
			}; 
				
			frame.X *= 2;
			myOrganizationsButtonView = new UIButton();
			myOrganizationsButtonView.SetBackgroundImage(UIImage.FromFile (this is OrganizationsController ? "OrganizationsIcon.png" : "OrganizationsIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myOrganizationsButtonView.Frame = frame;
			myOrganizationsButtonView.TouchUpInside += async delegate {
				//GoToMyOrganizations ();	
				await LoadMyOrganizationsAsync();
			}; 
				
			frame.X *= 2;
			eventsButtonView = new UIButton();
			eventsButtonView.SetBackgroundImage(UIImage.FromFile (this is EventListController ? "EventIcon.png" : "EventIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			eventsButtonView.Frame = frame;
			eventsButtonView.TouchUpInside += async delegate {
				//GoToEvents ();	
				await LoadEventList();
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

		protected async Task<bool> LoadMyOrganizationsAsync(bool force = false){

			var cookie = GetAuthCookie ();
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_ORGANIZATIONS_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie (Resources.ORGANIZATION_REFRESH_COOKIE_NAME) && !force) {
				GoToMyOrganizations ();
			} else {
				if (cookie != null) {

					var overlay = new MyBusidexLoadingOverlay (View.Bounds);
					overlay.MessageText = "Loading Your Organizations";

					View.AddSubview (overlay);

					var controller = new OrganizationController ();
					await controller.GetMyOrganizations (cookie.Value).ContinueWith (async response => {
						if (!string.IsNullOrEmpty (response.Result)) {

							OrganizationResponse myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (response.Result);

							Utils.SaveResponse(response.Result, Resources.MY_ORGANIZATIONS_FILE);
							SetRefreshCookie(Resources.ORGANIZATION_REFRESH_COOKIE_NAME);

							foreach (Organization org in myOrganizationsResponse.Model) {
								var fileName = org.LogoFileName + "." + org.LogoType;
								var fImagePath = Resources.CARD_PATH + fileName;
								if (!File.Exists (documentsPath + "/" + fileName)) {
									await Utils.DownloadImage (fImagePath, documentsPath, fileName).ContinueWith (t => {

									});
								} 
								// load organization members
								await controller.GetOrganizationMembers(cookie.Value, org.OrganizationId).ContinueWith(async cards =>{

									OrgMemberResponse orgMemberResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (cards.Result);
									Utils.SaveResponse(cards.Result, Resources.ORGANIZATION_MEMBERS_FILE + org.OrganizationId);

									var idx = 0;
									InvokeOnMainThread (() =>{
										overlay.TotalItems = myOrganizationsResponse.Model.Count();
										overlay.UpdateProgress (idx);
									});
									foreach(var card in orgMemberResponse.Model){

										var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
										var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
										var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
										var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
										if (!File.Exists (documentsPath + "/" + fName) || force) {
											await Utils.DownloadImage (fImageUrl, documentsPath, fName).ContinueWith (t => {
												InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
											});
										}
										if (!File.Exists (documentsPath + "/" + bName) && card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
											await Utils.DownloadImage (bImageUrl, documentsPath, bName).ContinueWith (t => {

											});
										}
										idx++;
									}
								});

								await controller.GetOrganizationReferrals(cookie.Value, org.OrganizationId).ContinueWith(async cards =>{

									var orgReferralResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (cards.Result);
									Utils.SaveResponse(cards.Result, Resources.ORGANIZATION_REFERRALS_FILE + org.OrganizationId);

									var idx = 0;
									InvokeOnMainThread (() =>{
										overlay.TotalItems = myOrganizationsResponse.Model.Count();
										overlay.UpdateProgress (idx);
									});
									foreach(var card in orgReferralResponse.Model){

										var fImageUrl = Resources.THUMBNAIL_PATH + card.Card.FrontFileName;
										var bImageUrl = Resources.THUMBNAIL_PATH + card.Card.BackFileName;
										var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName;
										var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.BackFileName;
										if (!File.Exists (documentsPath + "/" + fName) || force) {
											await Utils.DownloadImage (fImageUrl, documentsPath, fName).ContinueWith (t => {
												InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
											});
										}
										if (!File.Exists (documentsPath + "/" + bName) && card.Card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
											await Utils.DownloadImage (bImageUrl, documentsPath, bName).ContinueWith (t => {

											});
										}
										idx++;
									}
								});
							}

							InvokeOnMainThread (() => {
								overlay.Hide();
								if(!force){
									GoToMyOrganizations ();
								}
							});
						}else{
							InvokeOnMainThread (() => {
								ShowAlert ("No Internet Connection", "There was a problem connecting to the internet. Please check your connection.", "Ok");
								overlay.Hide();
							});
						}
					});
				}
			}
			return true;
		}

		protected async Task<bool> LoadMyBusidexAsync(bool force = false){
			var cookie = GetAuthCookie ();
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie(Resources.BUSIDEX_REFRESH_COOKIE_NAME) && !force) {
				GoToMyBusidex ();
				return true;
			} else {
				if (cookie != null) {

					var overlay = new MyBusidexLoadingOverlay (View.Bounds);
					overlay.MessageText = "Loading Your Cards";

					View.AddSubview (overlay);

					var ctrl = new Busidex.Mobile.MyBusidexController ();
					await ctrl.GetMyBusidex (cookie.Value).ContinueWith(async r => {

						if (!string.IsNullOrEmpty (r.Result)) {
							MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);

							Utils.SaveResponse(r.Result, Resources.MY_BUSIDEX_FILE);
							SetRefreshCookie(Resources.BUSIDEX_REFRESH_COOKIE_NAME);

							var cards = new List<UserCard> ();
							var idx = 0;
							InvokeOnMainThread (() =>{
								overlay.TotalItems = myBusidexResponse.MyBusidex.Busidex.Count();
								overlay.UpdateProgress (idx);
							});

							foreach (var item in myBusidexResponse.MyBusidex.Busidex) {
								if (item.Card != null) {

									var fImageUrl = Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
									var bImageUrl = Resources.THUMBNAIL_PATH + item.Card.BackFileName;
									var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
									var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

									cards.Add (item);

									if (!File.Exists (documentsPath + "/" + fName) || force) {
										await Utils.DownloadImage (fImageUrl, documentsPath, fName).ContinueWith (t => {
											InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
										});
									} else{
										InvokeOnMainThread (() => overlay.UpdateProgress (idx));
									}

									if ((!File.Exists (documentsPath + "/" + bName) || force) && item.Card.BackFileId.ToString () != Resources.EMPTY_CARD_ID) {
										await Utils.DownloadImage (bImageUrl, documentsPath, bName).ContinueWith (t => {
										});
									}
									idx++;
								}
							}

							InvokeOnMainThread (() => {
								overlay.Hide();
								if(!force){
									GoToMyBusidex ();
								}
							});
						}else{
							InvokeOnMainThread (() => {
								//ShowAlert ("No Internet Connection", "There was a problem connecting to the internet. Please check your connection.", "Ok");
								overlay.Hide();
							});
						}
					});
				}
			}
			return true;
		}

		protected async Task<bool> LoadEventList(bool force = false){
			var cookie = GetAuthCookie ();

			var fullFilePath = Path.Combine (documentsPath, Resources.EVENT_LIST_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie (Resources.EVENT_LIST_REFRESH_COOKIE_NAME) && !force) {
				GoToEvents ();
			} else {
				try {
					var controller = new Busidex.Mobile.SearchController ();
					await controller.GetEventTags (cookie.Value).ContinueWith(async eventListResponse => {
						if (!string.IsNullOrEmpty (eventListResponse.Result)) {

							Utils.SaveResponse (eventListResponse.Result, Resources.EVENT_LIST_FILE);

							SetRefreshCookie(Resources.EVENT_LIST_REFRESH_COOKIE_NAME);

							if(!force){
								InvokeOnMainThread (GoToEvents);
							}
						}	
					});

				} catch (Exception ignore) {

				}
			}
			return true;
		}
	}
}

