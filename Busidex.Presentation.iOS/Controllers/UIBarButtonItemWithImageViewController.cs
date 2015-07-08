
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



		public UIBarButtonItemWithImageViewController (IntPtr handle) : base (handle)
		{
		}

		public UIBarButtonItemWithImageViewController () 
		{
		}
			
		protected void OnSwipeRight(){
			if(this is EventListController){
				LoadMyOrganizationsAsync(false, BaseNavigationController.NavigationDirection.Backward);
			}
			if(this is OrganizationsController){
				LoadMyBusidexAsync(false, BaseNavigationController.NavigationDirection.Backward);
			}
			if(this is MyBusidexController){
				GoToSearch (BaseNavigationController.NavigationDirection.Backward);
			}
			if(this is SearchController){
				GoHome();
			}
		}

		protected void OnSwipeLeft(){
			if(this is DataViewController){
				GoToSearch(BaseNavigationController.NavigationDirection.Forward);
			}
			if(this is SearchController){
				LoadMyBusidexAsync();
			}
			if(this is MyBusidexController){
				LoadMyOrganizationsAsync();
			}
			if(this is OrganizationsController){
				LoadEventList ();
			}
		}

		protected void OnSwipeDown(){
			Sync();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var swiperRight = new UISwipeGestureRecognizer(new Action(OnSwipeRight));
			swiperRight.Direction = UISwipeGestureRecognizerDirection.Right;

			var swiperLeft = new UISwipeGestureRecognizer(new Action(OnSwipeLeft));
			swiperLeft.Direction = UISwipeGestureRecognizerDirection.Left;

			var swiperDown = new UISwipeGestureRecognizer(new Action(OnSwipeDown));
			swiperDown.Direction = UISwipeGestureRecognizerDirection.Down;

			View.AddGestureRecognizer (swiperRight);
			View.AddGestureRecognizer (swiperLeft);
			View.AddGestureRecognizer (swiperDown);

//			SwiperLeft.PerformSelector (new ObjCRuntime.Selector ("SwipeLeftSelector:"));
//			SwiperDown.PerformSelector (new ObjCRuntime.Selector ("SwipeDownSelector:"));
//			SwiperRight.PerformSelector (new ObjCRuntime.Selector ("SwipeRightSelector:"));

			SetUpNavBarButtons();
				if (NavigationController != null) {
					NavigationController.SetNavigationBarHidden (false, false);
				}
			NavigationItem.SetHidesBackButton (true, true);
		}

		protected async Task<bool> Sync(){
			await LoadMyBusidexAsync (true);
			await LoadMyOrganizationsAsync (true);
			await LoadEventList (true);
			GetNotifications ();
			//ConfigureToolbarItems ();

			return true;
		}

		public int GetNotifications(){

			var ctrl = new Busidex.Mobile.SharedCardController ();
			var cookie = GetAuthCookie ();
			var sharedCardsResponse = ctrl.GetSharedCards (cookie.Value);
			if(sharedCardsResponse.Equals("Error")){
				return 0;
			}

			var sharedCards = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsResponse);

			if(sharedCards.SharedCards.Count > 0){
				Badge.Plugin.CrossBadge.Current.SetBadge (sharedCards.SharedCards.Count);	
			}else{
				Badge.Plugin.CrossBadge.Current.ClearBadge ();
			}

			try{
				Utils.SaveResponse (sharedCardsResponse, Resources.SHARED_CARDS_FILE);
			}catch{

			}

			foreach (SharedCard card in sharedCards.SharedCards) {
				var fileName = card.Card.FrontFileName;
				var fImagePath = Resources.CARD_PATH + fileName;
				if (!File.Exists (documentsPath + "/" + Resources.THUMBNAIL_FILE_NAME_PREFIX + fileName)) {
					Utils.DownloadImage (fImagePath, documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + fileName).ContinueWith (t => {

					});
				}
			}

			return sharedCards != null ? sharedCards.SharedCards.Count : 0;
		}

		protected void GoToMyBusidex (BaseNavigationController.NavigationDirection direction)
		{
			if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is MyBusidexController){
				return;
			}
			((BaseNavigationController)NavigationController).Direction = direction;
			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var myBusidexController = board.InstantiateViewController ("MyBusidexController") as MyBusidexController;
			if (myBusidexController != null) {
				NavigationController.PushViewController (myBusidexController, true);
			}
		}

		protected void GoToSearch (BaseNavigationController.NavigationDirection direction)
		{
			if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is SearchController){
				return;
			}
			((BaseNavigationController)NavigationController).Direction = direction;
			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var searchController = board.InstantiateViewController ("SearchController") as SearchController;
			if (searchController != null) {
				NavigationController.PushViewController (searchController, true);
			}
		}

		protected void GoToMyOrganizations (BaseNavigationController.NavigationDirection direction)
		{
			try{
				if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is OrganizationsController){
					return;
				}
				((BaseNavigationController)NavigationController).Direction = direction;
				UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
				var organizationsController = board.InstantiateViewController ("OrganizationsController") as OrganizationsController;
				if (organizationsController != null) {
					NavigationController.PushViewController (organizationsController, true);
				}
			}
			catch(Exception ex){
				new UIAlertView("Error", ex.Message, null, "OK", null).Show();
			}
		}

		protected void GoToEvents(BaseNavigationController.NavigationDirection direction){

			if(NavigationController.ViewControllers.Length > 0 && NavigationController.ViewControllers[NavigationController.ViewControllers.Length-1]  is EventListController){
				return;
			}
			((BaseNavigationController)NavigationController).Direction = direction;
			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var eventListController = board.InstantiateViewController ("EventListController") as EventListController;
			if (eventListController != null) {
				NavigationController.PushViewController (eventListController, true);
			}
		}

		protected void NoOp(){
			
		}

		protected void GoHome(){
			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			((BaseNavigationController)NavigationController).Direction = BaseNavigationController.NavigationDirection.Backward;
			var dataViewController = board.InstantiateViewController ("DataViewController") as DataViewController;
			if (dataViewController != null) {
				NavigationController.PushViewController (dataViewController, true);
			}
		}



		protected void SetUpNavBarButtons(){

			nfloat imageSize = 40f;
			var frame = new CoreGraphics.CGRect(imageSize * 2, 0, imageSize, imageSize);

			// EVENTS
			eventsButtonView = new UIButton();
			if(this is EventListController){
				eventsButtonView.SetBackgroundImage(UIImage.FromFile ("EventIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}else if(this is EventCardsController){
				eventsButtonView.SetBackgroundImage(UIImage.FromFile ("EventIconBack.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}else{
				eventsButtonView.SetBackgroundImage(UIImage.FromFile ("EventIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}
			eventsButtonView.Frame = frame;
			eventsButtonView.TouchUpInside += async delegate {
				await LoadEventList();
			};

			// ORGANIZATIONS
			frame.X *= 2;
			myOrganizationsButtonView = new UIButton();
			if(this is OrganizationsController){
				myOrganizationsButtonView.SetBackgroundImage(UIImage.FromFile ("OrganizationsIcon.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}else if(this is OrganizationDetailController || this is OrgMembersController){
				myOrganizationsButtonView.SetBackgroundImage(UIImage.FromFile ("EventIconBack.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}else{
				myOrganizationsButtonView.SetBackgroundImage(UIImage.FromFile ("OrganizationsIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			}
			myOrganizationsButtonView.Frame = frame;
			myOrganizationsButtonView.TouchUpInside += async delegate {
				await LoadMyOrganizationsAsync();
			}; 

			frame.X *= 2;
			// MY BUSIDEX
			myBusidexButtonView = new UIButton();
			myBusidexButtonView.SetBackgroundImage(UIImage.FromFile (this is MyBusidexController ? "MyBusidexIcon.png" : "MyBusidexIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			myBusidexButtonView.Frame = frame;
			myBusidexButtonView.TouchUpInside += async delegate {
				await LoadMyBusidexAsync();
			}; 

			// SEARCH
			frame.X *= 2;
			searchButtonView = new UIButton();
			searchButtonView.SetBackgroundImage(UIImage.FromFile (this is SearchController ? "SearchIcon.png" : "SearchIcon_disabled.png").Scale (new CoreGraphics.CGSize (imageSize, imageSize)), UIControlState.Normal);
			searchButtonView.Frame = frame;
			searchButtonView.TouchUpInside += delegate {
				GoToSearch (BaseNavigationController.NavigationDirection.Forward);	
			}; 

			var myBusidexButton = new UIBarButtonItem (
				myBusidexButtonView.ImageView.Image,
				UIBarButtonItemStyle.Plain,
				(s, e) => LoadMyBusidexAsync()
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
				(s, e) => LoadMyOrganizationsAsync()
			);
			myOrganizationsButton.CustomView = myOrganizationsButtonView;

			var eventsButton = new UIBarButtonItem (
				eventsButtonView.ImageView.Image,
				UIBarButtonItemStyle.Plain,
				(s, e) => GoToEvents(BaseNavigationController.NavigationDirection.Forward)
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
			homeButton.Frame = new CoreGraphics.CGRect(0, 0, 90f, imageSize);
			homeButton.TouchUpInside += delegate {
				GoHome();
			};

			NavigationItem.LeftBarButtonItem.CustomView = homeButton;
			NavigationItem.LeftBarButtonItem.CustomView.Hidden = false;

		}

		protected async Task<bool> LoadMyOrganizationsAsync(bool force = false, BaseNavigationController.NavigationDirection direction = BaseNavigationController.NavigationDirection.Forward){

			var cookie = GetAuthCookie ();
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_ORGANIZATIONS_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie (Resources.ORGANIZATION_REFRESH_COOKIE_NAME) && !force) {
				GoToMyOrganizations (direction);
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
									GoToMyOrganizations (direction);
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

		protected async Task<bool> LoadMyBusidexAsync(bool force = false, BaseNavigationController.NavigationDirection direction = BaseNavigationController.NavigationDirection.Forward){
			var cookie = GetAuthCookie ();
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie(Resources.BUSIDEX_REFRESH_COOKIE_NAME) && !force) {
				GoToMyBusidex (direction);
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
									GoToMyBusidex (direction);
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

		protected async Task<bool> LoadEventList(bool force = false, BaseNavigationController.NavigationDirection direction = BaseNavigationController.NavigationDirection.Forward){
			var cookie = GetAuthCookie ();

			var fullFilePath = Path.Combine (documentsPath, Resources.EVENT_LIST_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie (Resources.EVENT_LIST_REFRESH_COOKIE_NAME) && !force) {
				GoToEvents (direction);
			} else {
				try {
					var controller = new Busidex.Mobile.SearchController ();
					await controller.GetEventTags (cookie.Value).ContinueWith(eventListResponse => {
						if (!string.IsNullOrEmpty (eventListResponse.Result)) {

							Utils.SaveResponse (eventListResponse.Result, Resources.EVENT_LIST_FILE);

							SetRefreshCookie(Resources.EVENT_LIST_REFRESH_COOKIE_NAME);

							if(!force){
								InvokeOnMainThread (() => GoToEvents (direction));
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

