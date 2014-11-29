using System;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Presentation.iOS
{
	public partial class DataViewController : BaseController
	{

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

			btnGoToMyBusidex.TouchDown += delegate {
				//lblLoading.Hidden = false;
				//spnLoading.Hidden = false;
			};

			btnMyOrganizations.TouchUpInside += delegate {
				GoToMyOrganizations();
			};

			//lblLoading.Hidden = true;
			//spnLoading.Hidden = true;
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

			if (NavigationController != null) {
			
				SetToolbarItems (new [] {
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 },
					new UIBarButtonItem (UIBarButtonSystemItem.Compose, (s, e) => {
						var settingsController = Storyboard.InstantiateViewController ("SettingsController") as SettingsController;

						if (settingsController != null && NavigationController.ChildViewControllers.Count (c => c is SettingsController) == 0){
							NavigationController.PushViewController (settingsController, true);
						}
					})
				}, true);
				NavigationController.SetToolbarHidden(false, true);
			}
		}

		void GoToMyBusidex ()
		{
			var myBusidexController = Storyboard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

			if (myBusidexController != null && NavigationController.ChildViewControllers.Count (c => c is MyBusidexController) == 0){
				NavigationController.PushViewController (myBusidexController, true);
			}
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

		static bool CheckRefreshCookie(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.SingleOrDefault (c => c.Name == Busidex.Mobile.Resources.BusideRefreshCookieName);

			if (cookie == null || cookie.ExpiresDate < DateTime.Now) {
				var nCookie = new System.Net.Cookie();
				nCookie.Name = Busidex.Mobile.Resources.BusideRefreshCookieName;
				DateTime expiration = DateTime.Now.AddDays(1);
				nCookie.Expires = expiration;

				cookie = new NSHttpCookie (nCookie);

				NSHttpCookieStorage.SharedStorage.SetCookie(cookie);

				return false;
			}

			return true;
		}

		void SaveMyBusidexResponse(string response){
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			File.WriteAllText (fullFilePath, response);
		}

		async Task<bool> LoadMyBusidexAsync(){
			var cookie = GetAuthCookie ();
			const string EMPTY_CARD_ID = "b66ff0ee-e67a-4bbc-af3b-920cd0de56c6";
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie()) {
				//lblLoading.Hidden = true;
				//spnLoading.Hidden = true;
				GoToMyBusidex ();
			} else {
				if (cookie != null) {

					var overlay = new MyBusidexLoadingOverlay (View.Bounds);

					View.AddSubview (overlay);

					var ctrl = new Busidex.Mobile.MyBusidexController ();
					await ctrl.GetMyBusidex (cookie.Value).ContinueWith(r => {

						if (!string.IsNullOrEmpty (r.Result)) {
							MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);

							SaveMyBusidexResponse(r.Result);

							var cards = new List<UserCard> ();
							var idx = 0;
							InvokeOnMainThread (() =>{
								overlay.TotalItems = myBusidexResponse.MyBusidex.Busidex.Count();
								overlay.UpdateProgress (idx);
							});

							foreach (var item in myBusidexResponse.MyBusidex.Busidex) {
								if (item.Card != null) {

									var fImagePath = Busidex.Mobile.Utils.CARD_PATH + item.Card.FrontFileId + "." + item.Card.FrontType;
									var bImagePath = Busidex.Mobile.Utils.CARD_PATH + item.Card.BackFileId + "." + item.Card.BackType;
									var fName = item.Card.FrontFileId + "." + item.Card.FrontType;
									var bName = item.Card.BackFileId + "." + item.Card.BackType;

									cards.Add (item);

									if (!File.Exists (documentsPath + "/" + fName)) {
										Busidex.Mobile.Utils.DownloadImage (fImagePath, documentsPath, fName).ContinueWith (t => {
											InvokeOnMainThread (() => overlay.UpdateProgress (idx));
										});
									} else{
										InvokeOnMainThread (() => overlay.UpdateProgress (idx));
									}

									if (!File.Exists (documentsPath + "/" + bName) && item.Card.BackFileId.ToString () != EMPTY_CARD_ID) {
										Busidex.Mobile.Utils.DownloadImage (bImagePath, documentsPath, bName).ContinueWith (t => {
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

