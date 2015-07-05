using System;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using System.Threading.Tasks;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class DataViewController : UIBarButtonItemWithImageViewController
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

			UIFont font = UIFont.FromName ("Lato-Black", 22f);
		
			lblEvents.Font = lblMyBusidex.Font = lblOrganizations.Font = lblSearch.Font = lblQuestions.Font = font;

			btnGoToSearch.TouchUpInside += delegate {
				GoToSearch();
			};

			btnGoToMyBusidex.TouchUpInside += async delegate {
				var task = LoadMyBusidexAsync ();
				if (await Task.WhenAny (task, Task.Delay (15000)) == task) {
					await task;
					if(!task.Result){
						await ShowAlert ("No Internet Connection", "There was a problem connecting to the internet. Please check your connection.", "Ok");
					}
				} else {
					await ShowAlert ("No Internet Connection", "There was a problem connecting to the internet. Please check your connection.", "Ok");
				}
			};

			btnMyOrganizations.TouchUpInside += async delegate {
				
				var task = LoadMyOrganizationsAsync ();
				if (await Task.WhenAny (task, Task.Delay (15000)) == task) {
					await task;
					if(!task.Result){
						await ShowAlert ("No Internet Connection", "There was a problem connecting to the internet. Please check your connection.", "Ok");
					}
				} else {
					await ShowAlert ("No Internet Connection", "There was a problem connecting to the internet. Please check your connection.", "Ok");
				}
			};

			btnEvents.TouchUpInside += delegate {
				LoadEventList();
			};

			btnQuestions.TouchUpInside += delegate {
				OpenFaq();
			};
		}


		static string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(userId.ToString());
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		public void OpenFaq(){
			var url = new NSUrl ("https://pro.busidex.com/#/faq");
			UIApplication.SharedApplication.OpenUrl (url);

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_QUESTIONS, String.Empty, 0);
		}

		public int GetNotifications(){

			var ctrl = new Busidex.Mobile.SharedCardController ();
			var cookie = GetAuthCookie ();
			var sharedCardsResponse = ctrl.GetSharedCards (cookie.Value);
			if(sharedCardsResponse.Equals("Error")){
				return 0;
			}

			var sharedCards = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsResponse);

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

		async Task<bool> Sync(){
			await LoadMyBusidexAsync (true);
			await LoadMyOrganizationsAsync (true);
			await LoadEventList (true);
			GetNotifications ();
			ConfigureToolbarItems ();

			return true;
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

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
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
			
		static void ClearSettings(){

			var user = NSUserDefaults.StandardUserDefaults;
			user.SetString (string.Empty, Resources.USER_SETTING_DISPLAYNAME);
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
	

	}
}

