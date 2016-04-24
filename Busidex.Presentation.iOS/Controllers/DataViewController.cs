using System;
using System.Linq;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class DataViewController : UIBarButtonItemWithImageViewController
	{
		public string DataObject { get;	set; }

		public DataViewController ()
		{

		}

		public DataViewController (IntPtr handle) : base (handle)
		{
		}

		static bool getDeviceTypeSetting(){

			using (var user = NSUserDefaults.StandardUserDefaults) {
				return user.BoolForKey (Resources.USER_SETTING_DEVICE_TYPE_SET);
			}
		}

		static void saveDeviceTypeSet(){
			using (var user = NSUserDefaults.StandardUserDefaults) {
				user.SetBool(true, Resources.USER_SETTING_DEVICE_TYPE_SET);
				user.Synchronize ();
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Task.Run (() => {
				UISubscriptionService.Init ();
			});

			ConfigureToolbarItems ();

			Application.MainController = NavigationController;

			if(!getDeviceTypeSetting()){

				var token = GetAuthCookie();

				var deviceName = UIDevice.CurrentDevice.Name;

				var deviceType = deviceName.ToUpper().Contains("IPHONE") ? DeviceType.iPhone : DeviceType.iPad;

				AccountController.UpdateDeviceType(token.Value, deviceType).ContinueWith(r =>{
					saveDeviceTypeSet ();
				});
			}

			UIFont font = UIFont.FromName ("Lato-Black", 22f);
		
			lblEvents.Font = lblMyBusidex.Font = lblOrganizations.Font = lblSearch.Font = lblQuestions.Font = font;

			btnGoToSearch.TouchUpInside += delegate {
				GoToSearch(BaseNavigationController.NavigationDirection.Forward);
			};

			btnGoToMyBusidex.TouchUpInside += delegate {
				GoToMyBusidex(BaseNavigationController.NavigationDirection.Forward);
			};

			btnMyOrganizations.TouchUpInside += delegate {
				GoToMyOrganizations(BaseNavigationController.NavigationDirection.Forward);
			};

			btnEvents.TouchUpInside += delegate {
				GoToEvents(BaseNavigationController.NavigationDirection.Forward);
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
			var url = new NSUrl ("https://www.busidex.com/#/faq");
			UIApplication.SharedApplication.OpenUrl (url);

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_QUESTIONS, String.Empty, 0);
		}

		void ConfigureToolbarItems(){
			var imgFrame = new CoreGraphics.CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 5f, 25f, 25f);

			// Sync button
			var syncImage = new UIButton (imgFrame);
			syncImage.SetBackgroundImage (UIImage.FromBundle ("sync.png"), UIControlState.Normal);

			syncImage.TouchUpInside -= ((s, e) => Sync());
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
			var notificationFrame = new CoreGraphics.CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 3f, 45f, 45f);
			var notificationCount = UISubscriptionService.Notifications.Count;
			var notificationButton = new NotificationButton (notificationCount);
			notificationButton.TouchUpInside += ((s, e) => GoToSharedCards());
			notificationButton.Frame = notificationFrame;
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
						Xamarin.Insights.Report(ex);
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

			NavigationController.SetToolbarHidden (false, true);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			try{
				if(UISubscriptionService.AppQuickShareLink != null){
					GoToQuickShare ();
					UISubscriptionService.AppQuickShareLink = null;
				}
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
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
			user.SetBool (false, Resources.USER_SETTING_DEVICE_TYPE_SET);
			user.Synchronize ();
		}

		void LogOut(){

			ShowAlert ("Logout", "Sign out of Busidex?", "Ok", "Cancel").ContinueWith (button => {

				if(button.Result == 0){
					InvokeOnMainThread( () => {
						ClearSettings ();
						UISubscriptionService.Clear();
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

