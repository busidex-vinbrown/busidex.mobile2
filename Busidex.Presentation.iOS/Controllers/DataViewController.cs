using System;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class DataViewController : BaseController
	{
		public DataViewController ()
		{

		}

		public DataViewController (IntPtr handle) : base (handle)
		{
		}

		public string DataObject {
			get;
			set;
		}

		enum LoginVisibleSetting{
			Show = 1,
			Hide = 2
		}

		static string GetDeviceId(){
			var thisDeviceId = UIDevice.CurrentDevice.IdentifierForVendor;
			if (thisDeviceId != null) {
				var dIdString = thisDeviceId.AsString ();
				return dIdString;
			}
			return string.Empty;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			/*
			 * Check the authentication cookie.
			 * If it is null, use the device id to AutoRegister. This will
			 * use the device ID as the username and password and create the
			 * user account if one does not already exist. Then it will return 
			 * the userId. Set the authentication cookie and continue.
			 */


			btnGoToSearch.TouchUpInside += delegate {
				GoToSearch();
			};

			btnGoToMyBusidex.TouchUpInside += delegate {
				LoadMyBusidexAsync();
			};

			btnMyOrganizations.TouchUpInside += delegate {
				LoadMyOrganizationsAsync();
			};

			btnEvents.TouchUpInside += delegate {
				LoadEventList();
			};
		}

		static string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(userId.ToString());
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		public int GetNotifications(){

			var ctrl = new Busidex.Mobile.SharedCardController ();
			var cookie = GetAuthCookie ();
			var sharedCardsResponse = ctrl.GetSharedCards (cookie.Value);
			var sharedCards = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsResponse);

			try{
				Busidex.Mobile.Utils.SaveResponse (sharedCardsResponse, Resources.SHARED_CARDS_FILE);
			}catch{

			}

			foreach (SharedCard card in sharedCards.SharedCards) {
				var fileName = card.Card.FrontFileName;
				var fImagePath = Resources.CARD_PATH + fileName;
				if (!File.Exists (documentsPath + "/" + Resources.THUMBNAIL_FILE_NAME_PREFIX + fileName)) {
					Busidex.Mobile.Utils.DownloadImage (fImagePath, documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + fileName).ContinueWith (t => {

					});
				}
			}

			return sharedCards != null ? sharedCards.SharedCards.Count : 0;
		}

		void Sync(){
			LoadMyBusidexAsync (true);
			LoadMyOrganizationsAsync (true);
			LoadEventList (true);
			GetNotifications ();
			ConfigureToolbarItems ();
		}

		void ConfigureToolbarItems(){
			var imgFrame = new CoreGraphics.CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 5f, 25f, 25f);

			// Sync button
			var syncImage = new UIButton (imgFrame);
			syncImage.SetBackgroundImage (UIImage.FromBundle ("sync.png"), UIControlState.Normal);
			syncImage.TouchUpInside += ((s, e) => Sync());
			var syncButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
			syncButton.CustomView = syncImage;

			// Logout button
			var logOutButton = UIButton.FromType (UIButtonType.System);
			logOutButton.Frame = imgFrame;
			logOutButton.SetBackgroundImage (UIImage.FromBundle ("Exit.png"), UIControlState.Normal);
			logOutButton.TouchUpInside += ((s, e) => LogOut ());
			var logOutSystemButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
			logOutSystemButton.CustomView = logOutButton;

			// Notifications
			var notificationCount = GetNotifications();
			var notificationButton = new NotificationButton (notificationCount);
			notificationButton.TouchUpInside += ((s, e) => GoToSharedCards());
			notificationButton.Frame = imgFrame;
			var notificationSystemButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
			notificationSystemButton.CustomView = notificationButton;

			SetToolbarItems (new[] {
				logOutSystemButton,
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace) {
					Width = 100
				},
				notificationSystemButton,
				new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace) {
					Width = 10
				},
				syncButton,
				new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace) {
					Width = 10
				},
				new UIBarButtonItem (UIBarButtonSystemItem.Compose, (s, e) => {
					try{
						var settingsController = Storyboard.InstantiateViewController ("SettingsController") as SettingsController;
						if (settingsController != null /* && NavigationController.ChildViewControllers.Count (c => c is SettingsController) == 0*/) {
							NavigationController.PushViewController (settingsController, true);
						}
					}catch(Exception ex){

					}
				})
			}, true);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (true, true);
			}

			if (NavigationController == null) {
				return;
			}

			ConfigureToolbarItems ();

			NavigationController.SetToolbarHidden (false, true);
		}

		void GoToSharedCards(){
			var sharedCardListController = Storyboard.InstantiateViewController ("SharedCardListController") as SharedCardListController;

			if (sharedCardListController != null && NavigationController.ChildViewControllers.Count (c => c is SharedCardListController) == 0){
				NavigationController.PushViewController (sharedCardListController, true);
			}
		}

		void GoToMyBusidex ()
		{
			var myBusidexController = Storyboard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

			if (myBusidexController != null && NavigationController.ChildViewControllers.Count (c => c is MyBusidexController) == 0){
				NavigationController.PushViewController (myBusidexController, true);
			}
		}

		void ClearSettings(){

			var user = NSUserDefaults.StandardUserDefaults;
			user.SetString (string.Empty, Resources.USER_SETTING_USERNAME);
			user.SetString (string.Empty, Resources.USER_SETTING_PASSWORD);
			user.SetString (string.Empty, Resources.USER_SETTING_EMAIL);
			user.Synchronize ();
		}

		void LogOut(){

			ShowAlert ("Logout", "Sign out of Busidex?", "Ok", "Cancel").ContinueWith (button => {

				if(button.Result == 0){
					InvokeOnMainThread( () => {
						ClearSettings ();
						RemoveAuthCookie ();
						var startUpController = Storyboard.InstantiateViewController ("StartupController") as StartupController;

						if (startUpController != null) {
							NavigationController.PushViewController (startUpController, true);
						}
					});
				}
			});
		}

		void GoToSearch ()
		{
			var searchController = Storyboard.InstantiateViewController ("SearchController") as SearchController;

			if (searchController != null) {
				NavigationController.PushViewController (searchController, true);
			}
		}

		void GoToMyOrganizations ()
		{
			try{
			var organizationsController = Storyboard.InstantiateViewController ("OrganizationsController") as OrganizationsController;

			if (organizationsController != null && NavigationController.ChildViewControllers.Count (c => c is OrganizationsController) == 0){
				NavigationController.PushViewController (organizationsController, true);
			}
			}
			catch(Exception ex){
				new UIAlertView("Error", ex.Message, null, "OK", null).Show();
			}
		}
	
		public async Task<bool> LoadMyOrganizationsAsync(bool force = false){

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

							Busidex.Mobile.Utils.SaveResponse(response.Result, Resources.MY_ORGANIZATIONS_FILE);
							SetRefreshCookie(Resources.ORGANIZATION_REFRESH_COOKIE_NAME);

							foreach (Organization org in myOrganizationsResponse.Model) {
								var fileName = org.LogoFileName + "." + org.LogoType;
								var fImagePath = Resources.CARD_PATH + fileName;
								if (!File.Exists (documentsPath + "/" + fileName)) {
									await Busidex.Mobile.Utils.DownloadImage (fImagePath, documentsPath, fileName).ContinueWith (t => {

									});
								} 
								// load organization members
								await controller.GetOrganizationMembers(cookie.Value, org.OrganizationId).ContinueWith(async cards =>{

									OrgMemberResponse orgMemberResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (cards.Result);
									Busidex.Mobile.Utils.SaveResponse(cards.Result, Resources.ORGANIZATION_MEMBERS_FILE + org.OrganizationId);

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
											await Busidex.Mobile.Utils.DownloadImage (fImageUrl, documentsPath, fName).ContinueWith (t => {
												InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
											});
										}
										if (!File.Exists (documentsPath + "/" + bName) && card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
											await Busidex.Mobile.Utils.DownloadImage (bImageUrl, documentsPath, bName).ContinueWith (t => {

											});
										}
										idx++;
									}
								});

								await controller.GetOrganizationReferrals(cookie.Value, org.OrganizationId).ContinueWith(async cards =>{

									var orgReferralResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (cards.Result);
									Busidex.Mobile.Utils.SaveResponse(cards.Result, Resources.ORGANIZATION_REFERRALS_FILE + org.OrganizationId);

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
											await Busidex.Mobile.Utils.DownloadImage (fImageUrl, documentsPath, fName).ContinueWith (t => {
												InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
											});
										}
										if (!File.Exists (documentsPath + "/" + bName) && card.Card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
											await Busidex.Mobile.Utils.DownloadImage (bImageUrl, documentsPath, bName).ContinueWith (t => {

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
						}
					});
				}
			}
			return true;
		}

		async Task<bool> LoadMyBusidexAsync(bool force = false){
			var cookie = GetAuthCookie ();
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie(Resources.BUSIDEX_REFRESH_COOKIE_NAME) && !force) {
				GoToMyBusidex ();
			} else {
				if (cookie != null) {

					var overlay = new MyBusidexLoadingOverlay (View.Bounds);
					overlay.MessageText = "Loading Your Cards";

					View.AddSubview (overlay);

					var ctrl = new Busidex.Mobile.MyBusidexController ();
					await ctrl.GetMyBusidex (cookie.Value).ContinueWith(async r => {

						if (!string.IsNullOrEmpty (r.Result)) {
							MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);

						    Busidex.Mobile.Utils.SaveResponse(r.Result, Resources.MY_BUSIDEX_FILE);
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
										await Busidex.Mobile.Utils.DownloadImage (fImageUrl, documentsPath, fName).ContinueWith (t => {
											InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
										});
									} else{
										InvokeOnMainThread (() => overlay.UpdateProgress (idx));
									}

									if ((!File.Exists (documentsPath + "/" + bName) || force) && item.Card.BackFileId.ToString () != Resources.EMPTY_CARD_ID) {
										await Busidex.Mobile.Utils.DownloadImage (bImageUrl, documentsPath, bName).ContinueWith (t => {
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

						}
					});
				}

			}
			return true;
		}

		void LoadEventList(bool force = false){
			var cookie = GetAuthCookie ();

			var fullFilePath = Path.Combine (documentsPath, Resources.EVENT_LIST_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie (Resources.EVENT_LIST_REFRESH_COOKIE_NAME) && !force) {
				GoToEvents ();
			} else {
				try {
					var controller = new Busidex.Mobile.SearchController ();
					var eventListResponse = controller.GetEventTags (cookie.Value);
					if (!string.IsNullOrEmpty (eventListResponse)) {

						Busidex.Mobile.Utils.SaveResponse (eventListResponse, Resources.EVENT_LIST_FILE);

						SetRefreshCookie(Resources.EVENT_LIST_REFRESH_COOKIE_NAME);

						if(!force){
							GoToEvents ();
						}
					}
				} catch (Exception ex) {

				}
			}
		}

		void GoToEvents(){
			var eventListController = Storyboard.InstantiateViewController ("EventListController") as EventListController;
			if (eventListController != null) {
				NavigationController.PushViewController (eventListController, true);
			}
		}
	}
}

