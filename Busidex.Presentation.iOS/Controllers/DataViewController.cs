using System;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

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

		private enum LoginVisibleSetting{
			Show = 1,
			Hide = 2
		}

		//private static long UserId{ get; set; }
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		private string GetDeviceId(){
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

		private string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(userId.ToString());
			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (true, true);
			}

			if (this.NavigationController != null) {
			
				this.SetToolbarItems (new UIBarButtonItem[] {
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 },
					new UIBarButtonItem (UIBarButtonSystemItem.Compose, (s, e) => {
						var settingsController = this.Storyboard.InstantiateViewController ("SettingsController") as SettingsController;

						if (settingsController != null && this.NavigationController.ChildViewControllers.Where(c=> c is SettingsController).Count() == 0){
							this.NavigationController.PushViewController (settingsController, true);
						}
					})
				}, true);
				this.NavigationController.SetToolbarHidden(false, true);
			}
		}

		private void GoToMyBusidex ()
		{
			var myBusidexController = this.Storyboard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

			if (myBusidexController != null && this.NavigationController.ChildViewControllers.Where(c=> c is MyBusidexController).Count() == 0){
				this.NavigationController.PushViewController (myBusidexController, true);
			}
		}

		private void GoToSearch ()
		{
			var searchController = this.Storyboard.InstantiateViewController ("SearchController") as SearchController;

			if (searchController != null) {
				this.NavigationController.PushViewController (searchController, true);
			}
		}

		private void GoToMyOrganizations ()
		{
			try{
			var organizationsController = this.Storyboard.InstantiateViewController ("OrganizationsController") as OrganizationsController;

			if (organizationsController != null && this.NavigationController.ChildViewControllers.Where(c=> c is OrganizationsController).Count() == 0){
				this.NavigationController.PushViewController (organizationsController, true);
			}
			}
			catch(Exception ex){
				new UIAlertView("Error", ex.Message, null, "OK", null).Show();
			}
		}

		private bool CheckRefreshCookie(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where (c => c.Name == Busidex.Mobile.Resources.BusideRefreshCookieName).SingleOrDefault ();

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

		private void LoadMyBusidexAsync(){
			var cookie = GetAuthCookie ();
			const string EMPTY_CARD_ID = "b66ff0ee-e67a-4bbc-af3b-920cd0de56c6";
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie()) {
				//lblLoading.Hidden = true;
				//spnLoading.Hidden = true;
				GoToMyBusidex ();
			} else {
				if (cookie != null) {

					var ctrl = new Busidex.Mobile.MyBusidexController ();
					var response = ctrl.GetMyBusidex (cookie.Value);

					if (!string.IsNullOrEmpty (response.Result)) {
						MyBusidexResponse MyBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (response.Result);

						List<UserCard> cards = new List<UserCard> ();

						foreach (var item in MyBusidexResponse.MyBusidex.Busidex) {
							if (item.Card != null) {

								var fImagePath = Busidex.Mobile.Utils.CARD_PATH + item.Card.FrontFileId + "." + item.Card.FrontType;
								var bImagePath = Busidex.Mobile.Utils.CARD_PATH + item.Card.BackFileId + "." + item.Card.BackType;
								var fName = item.Card.FrontFileId + "." + item.Card.FrontType;
								var bName = item.Card.BackFileId + "." + item.Card.BackType;

								cards.Add (item);

								if (!File.Exists (documentsPath + "/" + fName)) {
									Busidex.Mobile.Utils.DownloadImage (fImagePath, documentsPath, fName).ContinueWith (t => {
									});
								} 

								if (!File.Exists (documentsPath + "/" + bName) && item.Card.BackFileId.ToString () != EMPTY_CARD_ID) {
									Busidex.Mobile.Utils.DownloadImage (bImagePath, documentsPath, bName).ContinueWith (t => {
									});
								} 
							}
						}

						//lblLoading.Hidden = true;
						//spnLoading.Hidden = true;
						GoToMyBusidex ();
					}
				}
			}
		}

	}
}

