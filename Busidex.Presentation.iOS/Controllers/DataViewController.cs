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
				//GoToMyOrganizations();
				LoadMyOrganizationsAsync();
			};
		}

		static string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(userId.ToString());
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
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

			var imgFrame = new CoreGraphics.CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 5f, 25f, 25f);
			var syncImage = new UIButton (imgFrame);
			syncImage.SetBackgroundImage (UIImage.FromBundle ("sync.png"), UIControlState.Normal);
			syncImage.TouchUpInside += ((s, e) => LoadMyBusidexAsync (true));
			var syncButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
			syncButton.CustomView = syncImage;

			var logOutButton = UIButton.FromType (UIButtonType.System);
			logOutButton.Frame = imgFrame;

			logOutButton.SetBackgroundImage (UIImage.FromBundle ("Exit.png"), UIControlState.Normal);
			logOutButton.TouchUpInside += ((s, e) => LogOut ());
			var logOutSystemButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
			logOutSystemButton.CustomView = logOutButton;

			SetToolbarItems (new[] {
				logOutSystemButton,
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace) {
					Width = 100
				},
				syncButton,
				new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace) {
					Width = 10
				},
				new UIBarButtonItem (UIBarButtonSystemItem.Compose, (s, e) => {
					var settingsController = Storyboard.InstantiateViewController ("SettingsController") as SettingsController;
					if (settingsController != null && NavigationController.ChildViewControllers.Count (c => c is SettingsController) == 0) {
						NavigationController.PushViewController (settingsController, true);
					}
				})
			}, true);
			NavigationController.SetToolbarHidden (false, true);
		}

		void GoToMyBusidex ()
		{
			var myBusidexController = Storyboard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

			if (myBusidexController != null && NavigationController.ChildViewControllers.Count (c => c is MyBusidexController) == 0){
				NavigationController.PushViewController (myBusidexController, true);
			}
		}

		void LogOut(){

			ShowAlert ("Logout", "Sign out of Busidex?", "Ok", "Cancel").ContinueWith (button => {

				if(button.Result == 0){
					InvokeOnMainThread( () => {
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

		static void SetRefreshCookie(){
			var nCookie = new System.Net.Cookie();
			nCookie.Name = Resources.BUSIDEX_REFRESH_COOKIE_NAME;
			DateTime expiration = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0, 0, 1).AddDays(1);
			nCookie.Expires = expiration;

			var cookie = new NSHttpCookie (nCookie);

			NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
		}

		static bool CheckRefreshCookie(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Resources.BUSIDEX_REFRESH_COOKIE_NAME);

			if (cookie == null || cookie.ExpiresDate < DateTime.Now) {

				SetRefreshCookie ();
				return false;
			}
			return true;
		}

		void SaveMyBusidexResponse(string response){
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
			File.WriteAllText (fullFilePath, response);
		}
			
		void SaveMyOrganizationsResponse(string response){
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_ORGANIZATIONS_FILE);
			File.WriteAllText (fullFilePath, response);
		}

		void SaveOrganizationCardsResponse(string response, string fileName){
			var fullFilePath = Path.Combine (documentsPath, fileName);
			File.WriteAllText (fullFilePath, response);
		}

		public async Task<bool> LoadMyOrganizationsAsync(bool force = false){

			var cookie = GetAuthCookie ();
			var fullFilePath = Path.Combine (documentsPath, Resources.MY_ORGANIZATIONS_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie () && !force) {
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

							SaveMyOrganizationsResponse(response.Result);
							SetRefreshCookie();

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
									SaveOrganizationCardsResponse(cards.Result, Resources.ORGANIZATION_MEMBERS_FILE + org.OrganizationId);

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
									SaveOrganizationCardsResponse(cards.Result, Resources.ORGANIZATION_REFERRALS_FILE + org.OrganizationId);

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
								GoToMyOrganizations ();
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
			if (File.Exists (fullFilePath) && CheckRefreshCookie() && !force) {
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

							SaveMyBusidexResponse(r.Result);
							SetRefreshCookie();

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
								GoToMyBusidex ();
							});

						}
					});
				}

			}
			return true;
		}

	}
}

