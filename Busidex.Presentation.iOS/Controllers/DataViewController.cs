using System;
using System.Linq;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Collections.Generic;

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

		static bool getDeviceTypeSetting ()
		{

			using (var user = NSUserDefaults.StandardUserDefaults) {
				return user.BoolForKey (Resources.USER_SETTING_DEVICE_TYPE_SET);
			}
		}

		static void saveDeviceTypeSet ()
		{
			using (var user = NSUserDefaults.StandardUserDefaults) {
				user.SetBool (true, Resources.USER_SETTING_DEVICE_TYPE_SET);
				user.Synchronize ();
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Task.Run (() => {
				UISubscriptionService.Init ();
			});

			Application.MainController = NavigationController;

			UIFont font = UIFont.FromName ("Lato-Black", 22f);
		
			lblEvents.Font = lblMyBusidex.Font = lblOrganizations.Font = lblSearch.Font = lblQuestions.Font = font;

			btnGoToSearch.TouchUpInside += delegate {
				GoToSearch (BaseNavigationController.NavigationDirection.Forward);
			};

			btnGoToMyBusidex.TouchUpInside += delegate {
				GoToMyBusidex (BaseNavigationController.NavigationDirection.Forward);
			};

			btnMyOrganizations.TouchUpInside += delegate {
				GoToMyOrganizations (BaseNavigationController.NavigationDirection.Forward);
			};

			btnEvents.TouchUpInside += delegate {
				GoToEvents (BaseNavigationController.NavigationDirection.Forward);
			};

//			btnQuestions.TouchUpInside += delegate {
//				OpenFaq();
//			};
			btnShare.TouchUpInside += async delegate {

				var userId = Utils.DecodeUserId (UISubscriptionService.AuthToken);
				var myCard = UISubscriptionService.UserCards.FirstOrDefault (c => c.Card != null && c.Card.OwnerId == userId);

				if (myCard == null) {
					await CardController.GetMyCard ().ContinueWith (r => {
						if (!string.IsNullOrEmpty (r.Result) && !r.Result.Contains ("\"Success\": false")) {
							try {
								var cardDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (r.Result);
								myCard = new UserCard (cardDetail.Model);
								if (!string.IsNullOrEmpty (myCard.Card.Name) && myCard.Card.FrontFileId != Guid.Empty) {
									UISubscriptionService.AddCardToMyBusidex (myCard);
									InvokeOnMainThread (() => ShareCard (myCard));
								} else {
									showNoCardMessage ();
								}
							} catch (Exception ex) {
								Xamarin.Insights.Report (ex);
							}
						} else {
							showNoCardMessage ();
						}
					});
				} else {
					InvokeOnMainThread (() => ShareCard (myCard));
				}
			};
		}

		void showNoCardMessage ()
		{
			InvokeOnMainThread (() => ShowAlert ("Share My Card", "You have not added your card to Busidex. Would you like to do this now?", new string[] {
				"Ok",
				"Not Now"
			}).ContinueWith (async button => {
				if (await button == 0) {
					InvokeOnMainThread (GoToSettings);
				}
			}));
		}

		static string EncodeUserId (long userId)
		{

			byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes (userId.ToString ());
			string returnValue = Convert.ToBase64String (toEncodeAsBytes);
			return returnValue;
		}

		public void OpenFaq ()
		{
			var url = new NSUrl ("https://www.busidex.com/#/faq");
			UIApplication.SharedApplication.OpenUrl (url);

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_QUESTIONS, String.Empty, 0);
		}

		void updateNotifications (List<SharedCard> sharedCards)
		{
			InvokeOnMainThread (() => {
				if (sharedCards.Count > 0) {
					ConfigureToolbarItems ();
					Badge.Plugin.CrossBadge.Current.SetBadge (UISubscriptionService.Notifications.Count);
				} else {
					Badge.Plugin.CrossBadge.Current.ClearBadge ();	
				}
			});
		}

		void ConfigureToolbarItems ()
		{
			var imgFrame = new CoreGraphics.CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 5f, 25f, 25f);

			// Sync button
			var syncImage = new UIButton (imgFrame);
			syncImage.SetBackgroundImage (UIImage.FromBundle ("sync.png"), UIControlState.Normal);

			syncImage.TouchUpInside -= ((s, e) => Sync ());
			syncImage.TouchUpInside += ((s, e) => Sync ());
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
			notificationButton.TouchUpInside += ((s, e) => GoToSharedCards ());
			notificationButton.Frame = notificationFrame;
			var notificationSystemButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
			notificationSystemButton.CustomView = notificationButton;
			notificationSystemButton.Tag = 1;

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
					try {
						GoToSettings ();
					} catch (Exception ex) {
						Xamarin.Insights.Report (ex);
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

			ConfigureToolbarItems ();

			UISubscriptionService.OnNotificationsLoaded -= updateNotifications;
			UISubscriptionService.OnNotificationsLoaded += updateNotifications;

			if (!getDeviceTypeSetting ()) {

				var deviceName = UIDevice.CurrentDevice.Name;

				var deviceType = deviceName.ToUpper ().Contains ("IPHONE") ? DeviceType.iPhone : DeviceType.iPad;

				AccountController.UpdateDeviceType (UISubscriptionService.AuthToken, deviceType).ContinueWith (r => {
					saveDeviceTypeSet ();
				});
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			try {
				if (UISubscriptionService.AppQuickShareLink != null) {
					GoToQuickShare ();
					UISubscriptionService.AppQuickShareLink = null;
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		void GoToSharedCards ()
		{
			var sharedCardListController = Storyboard.InstantiateViewController ("SharedCardListController") as SharedCardListController;

			if (sharedCardListController != null && NavigationController.ChildViewControllers.Count (c => c is SharedCardListController) == 0) {
				NavigationController.PushViewController (sharedCardListController, true);
			}
		}

		static void ClearSettings ()
		{

			var user = NSUserDefaults.StandardUserDefaults;
			user.SetString (string.Empty, Resources.USER_SETTING_DISPLAYNAME);
			user.SetString (string.Empty, Resources.USER_SETTING_USERNAME);
			user.SetString (string.Empty, Resources.USER_SETTING_PASSWORD);
			user.SetString (string.Empty, Resources.USER_SETTING_EMAIL);
			user.SetBool (false, Resources.USER_SETTING_DEVICE_TYPE_SET);
			user.Synchronize ();
		}

		void LogOut ()
		{

			ShowAlert ("Logout", "Sign out of Busidex?", "Ok", "Cancel").ContinueWith (button => {

				if (button.Result == 0) {
					InvokeOnMainThread (() => {
						ClearSettings ();
						UISubscriptionService.Clear ();
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

