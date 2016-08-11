using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Java.IO;
using System.Threading.Tasks;
using System.Linq;
using BranchXamarinSDK;
using Android.Net;
using Plugin.Permissions;
using Android.Content.PM;

namespace Busidex.Presentation.Droid.v2
{

	public static class App
	{
		public static File _file;
		public static File _dir;
		public static Bitmap bitmap;
	}

	[Activity (
		Label = "Busidex",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
	[IntentFilter (new [] { Intent.ActionView },
		DataScheme = "busidex",
		DataPathPrefix = "/Uebo",
		//DataHost = "jqle.app.link", 
		Categories = new [] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
	public class MainActivity : FragmentActivity, IBranchSessionInterface
	{
		ViewPager pager;
		GenericFragmentPagerAdaptor tabAdapter;

		public static List<Xamarin.Contacts.Contact> Contacts { get; set; }

		public async void OpenShare ()
		{
			var userId = Utils.DecodeUserId (UISubscriptionService.AuthToken);
			var myCard = UISubscriptionService.UserCards.FirstOrDefault (c => c.Card != null && c.Card.OwnerId == userId);

			if (myCard == null) {
				await CardController.GetMyCard ().ContinueWith (r => {
					if (!string.IsNullOrEmpty (r.Result) && !r.Result.Contains ("\"Success\": false")) {
						try {
							var cardDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (r.Result);
							myCard = new UserCard (cardDetail.Model);
							if (!string.IsNullOrEmpty (myCard.Card.Name) && myCard.Card.FrontFileId != System.Guid.Empty) {
								UISubscriptionService.AddCardToMyBusidex (myCard);
								var fragment = new ShareCardFragment (myCard);
								ShareCard (fragment);
							} else {
								showNoCardMessage ();
							}
						} catch (System.Exception ex) {
							Xamarin.Insights.Report (ex);
						}
					} else {
						showNoCardMessage ();
					}
				});
			} else {
				var fragment = new ShareCardFragment (myCard);
				ShareCard (fragment);
			}
		}

		void showNoCardMessage ()
		{
			RunOnUiThread (() => ShowAlert ("Share My Card", "You have not added your card to Busidex. Would you like to do this now?", new string [] {
				"Ok",
				"Not Now"
			}, new System.EventHandler<DialogClickEventArgs> ((o, e) => {

				var dialog = o as AlertDialog;
				Button btnClicked = dialog.GetButton (e.Which);
				if (btnClicked.Text == "Ok") {
					RunOnUiThread (() => {
						FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
						LoadFragment (new ProfileFragment (UISubscriptionService.CurrentUser));
					});
				}
			})
			));
		}

		public void SetTab (int page)
		{
			pager.SetCurrentItem (page, true);
		}

		void addTabs ()
		{
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.SetDisplayShowHomeEnabled (false);
			ActionBar.SetDisplayShowTitleEnabled (true);

			ActionBar.Title = "Busidex";

			var homeTab = pager.GetViewPageTab (ActionBar, "");
			homeTab.SetCustomView (Resource.Layout.tab);
			homeTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (Resource.Drawable.Icon);


			const float DISABLED_ALPHA = .3f;

			var myBusidexTab = pager.GetViewPageTab (ActionBar, "");
			myBusidexTab.SetCustomView (Resource.Layout.tab);
			myBusidexTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (Resource.Drawable.MyBusidexIcon);
			myBusidexTab.TabReselected += delegate {

				var activity = tabAdapter.GetItem (1).Activity;
				if (activity != null) {
					var lstCards = (OverscrollListView)activity.FindViewById (Resource.Id.lstCards);
					lstCards.SmoothScrollToPosition (0);
				}
			};

			var searchTab = pager.GetViewPageTab (ActionBar, "");
			searchTab.SetCustomView (Resource.Layout.tab);
			searchTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (Resource.Drawable.SearchIconDisabled);
			searchTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			var myOrganizationsTab = pager.GetViewPageTab (ActionBar, "");
			myOrganizationsTab.SetCustomView (Resource.Layout.tab);
			myOrganizationsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (Resource.Drawable.OrganizationsIconDisabled);
			myOrganizationsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			var eventsTab = pager.GetViewPageTab (ActionBar, "");
			eventsTab.SetCustomView (Resource.Layout.tab);
			eventsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (Resource.Drawable.EventIconDisabled);
			eventsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			var notificationsTab = pager.GetViewPageTab (ActionBar, "");
			notificationsTab.SetCustomView (Resource.Layout.notification);
			notificationsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (Resource.Drawable.NotificationDisabled);
			var txtNotificationCount = notificationsTab.CustomView.FindViewById<TextView> (Resource.Id.txtNotificationCount);
			txtNotificationCount.Text = UISubscriptionService.Notifications.Count.ToString ();
			txtNotificationCount.Visibility = UISubscriptionService.Notifications.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
			notificationsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			var profileTab = pager.GetViewPageTab (ActionBar, "");
			profileTab.SetCustomView (Resource.Layout.tab);
			profileTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (Resource.Drawable.settingsDisabled);
			profileTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			ActionBar.AddTab (homeTab);
			ActionBar.AddTab (searchTab);
			ActionBar.AddTab (myBusidexTab);
			ActionBar.AddTab (myOrganizationsTab);
			ActionBar.AddTab (eventsTab);
			ActionBar.AddTab (notificationsTab);
			ActionBar.AddTab (profileTab);

		}

		void addFragments (GenericFragmentPagerAdaptor adapter)
		{

			// HOME
			adapter.AddFragment (new HomeFragment ());

			// SEARCH
			adapter.AddFragment (new SearchFragment ());

			// MY BUSIDEX
			adapter.AddFragment (new MyBusidexFragment ());

			// MY ORGANIZATIONS
			adapter.AddFragment (new OrganizationsFragment ());

			// EVENTS
			adapter.AddFragment (new EventListFragment ());

			// REFERRALS
			adapter.AddFragment (new SharedCardListFragment ());

			// PROFILE
			var profileFragment = new ProfileFragment (UISubscriptionService.CurrentUser);

			OnBusidexUserLoadedEventHandler profileCallback = user => RunOnUiThread (() => profileFragment.UpdateUser (user));

			UISubscriptionService.OnBusidexUserLoaded -= profileCallback;
			UISubscriptionService.OnBusidexUserLoaded += profileCallback;

			adapter.AddFragment (profileFragment);

			UISubscriptionService.OnNotificationCountUpdated -= UpdateNotificationCount;
			UISubscriptionService.OnNotificationCountUpdated += UpdateNotificationCount;
		}

		void Init ()
		{
			// Load Contacts if necessary
			if (Contacts == null || Contacts.Count == 0) {
				Task.Factory.StartNew (() => {
					Contacts = Contacts ?? new List<Xamarin.Contacts.Contact> ();
					if (Contacts.Count == 0) {
						var book = new Xamarin.Contacts.AddressBook (this);
						var contactList = Contacts ?? new List<Xamarin.Contacts.Contact> ();

						if (contactList.Count == 0) {
							Task.Factory.StartNew (() => {
								contactList.AddRange (book.ToList ().Where (c => c.Phones.Any () && !string.IsNullOrEmpty (c.DisplayName)).OrderBy (p => p.DisplayName).ToList ());
								Contacts = contactList;
							});
						}
					}
				});
			}

			BranchAndroid.Init (this, Mobile.Resources.BRANCH_KEY, this);

		}

		#region Override Methods
		public override void OnRequestPermissionsResult (int requestCode, string [] permissions, Permission [] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult (requestCode, permissions, grantResults);
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			if (pager != null) {
				using (var adapter = pager.Adapter as GenericFragmentPagerAdaptor) {
					if (adapter != null) {
						adapter.Clear ();
						adapter.NotifyDataSetChanged ();
						pager.Adapter = null;
						ActionBar.RemoveAllTabs ();
					}
				}
			}
			base.OnSaveInstanceState (outState);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			Task.Run (() => {
				if (!getDeviceTypeSetting ()) {
					AccountController.UpdateDeviceType (UISubscriptionService.AuthToken, DeviceType.Android).ContinueWith (r => {
						saveDeviceTypeSet ();
					});
				}
			});
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			if (string.IsNullOrEmpty (UISubscriptionService.AuthToken)) {
				DoStartUp ();
			}

			setUpPager (true);
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{

			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			RequestedOrientation = ScreenOrientation.Portrait;

			Init ();

			setUpPager ();
		}

		#endregion

		void setStartTab ()
		{
			var tab = ActionBar.GetTabAt (0);
			ActionBar.SelectTab (tab);
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_LABEL_APP_START, Mobile.Resources.GA_LABEL_APP_START, 0);
		}

		static void saveDeviceTypeSet ()
		{
			var prefs = Application.Context.GetSharedPreferences (Mobile.Resources.APPLICATION_NAME, FileCreationMode.Private);
			var prefEditor = prefs.Edit ();
			prefEditor.PutBoolean (Mobile.Resources.USER_SETTING_DEVICE_TYPE_SET, true);
			prefEditor.Commit ();
		}

		public bool getDeviceTypeSetting ()
		{
			var prefs = Application.Context.GetSharedPreferences (Mobile.Resources.APPLICATION_NAME, FileCreationMode.Private);
			return prefs.GetBoolean (Mobile.Resources.USER_SETTING_DEVICE_TYPE_SET, false);
		}

		public static void DoFilter (UserCardAdapter adapter, string filter)
		{
			if (adapter == null) {
				return;
			}
			if (string.IsNullOrEmpty (filter)) {
				adapter.CardFilter.InvokeFilter ("");
			} else {
				adapter.CardFilter.InvokeFilter (filter);
			}
		}

		void setUpPager (bool restore = false)
		{

			ActionBar.RemoveAllTabs ();
			pager = FindViewById<ViewPager> (Resource.Id.pager);

			if (restore) {
				tabAdapter = new GenericFragmentPagerAdaptor (SupportFragmentManager);

			}
			tabAdapter = tabAdapter ?? new GenericFragmentPagerAdaptor (SupportFragmentManager);

			pager.Adapter = tabAdapter;
			pager.Activated = false;
			pager.ClearOnPageChangeListeners ();
			pager.CurrentItem = -1;
			pager.Invalidate ();
			pager.RemoveViews (0, pager.Adapter.Count);

			addTabs ();
			addFragments (tabAdapter);

			pager.Adapter.NotifyDataSetChanged ();
			pager.OffscreenPageLimit = 1;

			pager.AddOnPageChangeListener (new ViewPageListenerForActionBar (ActionBar));

			pager.Visibility = ViewStates.Visible;

		}

		#region Branch Deep Linking

		public void InitSessionComplete (Dictionary<string, object> data)
		{

			var cardId = string.Empty;

			var sentFrom = string.Empty;
			string displayName = string.Empty;
			string personalMessage = string.Empty;
			const string KEY_FROM = "_f";
			const string KEY_DISPLAY = "_d";
			const string KEY_MESSAGE = "_m";
			const string KEY_CARD_ID = "cardId";

			if (data.ContainsKey (KEY_FROM)) {
				sentFrom = System.Web.HttpUtility.UrlDecode (data [KEY_FROM].ToString ());
			}
			if (data.ContainsKey (KEY_DISPLAY)) {
				displayName = System.Web.HttpUtility.UrlDecode (data [KEY_DISPLAY].ToString ());
			}
			if (data.ContainsKey (KEY_MESSAGE)) {
				personalMessage = System.Web.HttpUtility.UrlDecode (data [KEY_MESSAGE].ToString ());
			}

			if (data.ContainsKey (KEY_CARD_ID)) {
				cardId = data [KEY_CARD_ID].ToString ();
			}


			if (!string.IsNullOrEmpty (cardId)) {

				Intent.SetData (null);

				var quickShareLink = new QuickShareLink {
					CardId = long.Parse (cardId),
					DisplayName = displayName,
					From = long.Parse (sentFrom),
					PersonalMessage = personalMessage
				};

				// If the user isn't logged in, save the quickShare link to file and continue with startup.
				if (string.IsNullOrEmpty (UISubscriptionService.AuthToken)) {
					string json = Newtonsoft.Json.JsonConvert.SerializeObject (quickShareLink);
					Utils.SaveResponse (json, Mobile.Resources.QUICKSHARE_LINK);
					setStartTab ();
				} else {
					var userCard = LoadQuickShareCardData (quickShareLink);
					var fragment = new QuickShareFragment (userCard, displayName, personalMessage);
					ShowQuickShare (fragment);
				}
			}
		}

		public void SessionRequestError (BranchError error)
		{
			if (error != null) {
				Xamarin.Insights.Report (new System.Exception ("Branch Error: [" + error.ErrorCode + "]" + error.ErrorMessage));
			} else {
				Xamarin.Insights.Report (new System.Exception ("Unknow Branch Error"));
			}
		}

		#endregion

		#region Card Sharing

		public void UpdateNotificationCount (int count)
		{
			const int NOTIFICATION_TAB = 5;
			if (ActionBar.TabCount > 0) {
				var selectedTab = ActionBar.GetTabAt (NOTIFICATION_TAB);
				var txtNotificationCount = selectedTab.CustomView.FindViewById<TextView> (Resource.Id.txtNotificationCount);
				if (count > 0) {
					txtNotificationCount.Visibility = ViewStates.Visible;
					txtNotificationCount.Text = count.ToString ();
				} else {
					txtNotificationCount.Visibility = ViewStates.Gone;
				}
			}
		}

		#endregion

		#region Startup Actions

		public void DoStartUp ()
		{
			DoingLogin = true;
			UISubscriptionService.CurrentUser = null;
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new StartUpFragment ());
			ActionBar.Hide ();
		}

		public void ShowRegistration ()
		{
			DoingRegistration = true;
			UISubscriptionService.CurrentUser = null;
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new ProfileFragment ());
			ActionBar.Hide ();
		}

		#endregion

		#region Login / Logout Actions

		bool DoingLogin;
		bool DoingRegistration;

		public void ShowLogin ()
		{
			DoingLogin = true;
			UISubscriptionService.CurrentUser = null;
			DoLogin ();
		}

		public void DoLogout ()
		{
			UISubscriptionService.Clear ();
			Utils.RemoveCacheFiles ();
			BaseApplicationResource.RemoveAuthCookie ();
			UnloadFragment ();

			var tab = ActionBar.GetTabAt (0);
			ActionBar.SelectTab (tab);

			ShowLogin ();
		}

		void DoLogin ()
		{
			DoingLogin = true;
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new LoginFragment ());
			ActionBar.Hide ();
		}

		public void LoginComplete ()
		{

			UnloadFragment (showActionBar: false);

			UISubscriptionService.AuthToken = BaseApplicationResource.GetAuthCookie ();

			UISubscriptionService.Sync ();
			DoingLogin = false;
			if (DoingRegistration) {
				DoingRegistration = false;
			}

			var quickShareLink = Utils.GetQuickShareLink ();
			if (quickShareLink != null) {
				var userCard = LoadQuickShareCardData (quickShareLink);
				var fragment = new QuickShareFragment (userCard, quickShareLink.DisplayName, quickShareLink.PersonalMessage);
				ShowQuickShare (fragment);
			}
		}

		public void UpdateEmail (string email)
		{
			UISubscriptionService.ChangeEmail (email);
		}

		#endregion

		#region Event Actions

		public async Task<bool> ReloadEventCards (EventTag tag)
		{
			await UISubscriptionService.loadEventCards (tag);
			return true;
		}

		public async void LoadEventCards (EventTag tag)
		{

			if (!UISubscriptionService.EventCards.ContainsKey (tag.Text) || UISubscriptionService.EventCards [tag.Text] == null) {

				var progressBar1 = FindViewById<ProgressBar> (Resource.Id.progressBar1);

				var myBusidexProgressStatus = FindViewById<TextView> (Resource.Id.eventProgressStatus);
				progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Visible;

				OnEventCardsUpdatedEventHandler update = status => RunOnUiThread (() => {
					progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Visible;
					progressBar1.Max = status.Total;
					progressBar1.Progress = status.Count;
					myBusidexProgressStatus.Text = string.Format ("Loading {0} of {1}", status.Count, status.Total);
				});

				UISubscriptionService.OnEventCardsUpdated -= update;
				UISubscriptionService.OnEventCardsUpdated += update;

				UISubscriptionService.EventCardsLoadedEventTable [tag.Text] += goToEventCards;

				await UISubscriptionService.loadEventCards (tag);
			} else {
				goToEventCards (tag, UISubscriptionService.EventCards [tag.Text]);
			}
		}

		void goToEventCards (EventTag tag, List<UserCard> cards)
		{
			RunOnUiThread (() => {
				var fragment = new EventCardsFragment (tag, cards);
				FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
				LoadFragment (fragment, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
				ActionBar.Hide ();
			});
		}

		#endregion

		#region Organization Actions

		public void ShowOrganizationDetail (Android.Support.V4.App.Fragment panel)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (panel, Resource.Animation.SlideAnimation, Resource.Animation.SlideOutAnimation);
			ActionBar.Hide ();
		}

		public void LoadOrganizationMembers (Organization organization)
		{
			var orgMembers = new List<UserCard> ();

			if (UISubscriptionService.OrganizationMembers.ContainsKey (organization.OrganizationId)) {
				foreach (var card in UISubscriptionService.OrganizationMembers [organization.OrganizationId]) {
					var userCard = new UserCard {
						CardId = card.CardId,
						Card = card,
						Notes = string.Empty
					};
					orgMembers.Add (userCard);
				}
			}
			var logoPath = System.IO.Path.Combine (Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var fragment = new OrganizationCardsFragment (orgMembers, logoPath);

			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment, Resource.Animation.SlideAnimation, Resource.Animation.SlideOutAnimation);
			ActionBar.Hide ();
		}

		public void LoadOrganizationReferrals (Organization organization)
		{

			var logoPath = System.IO.Path.Combine (Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var fragment = new OrganizationCardsFragment (UISubscriptionService.OrganizationReferrals [organization.OrganizationId], logoPath);
			LoadFragment (fragment, Resource.Animation.SlideAnimation, Resource.Animation.SlideOutAnimation);
		}

		#endregion

		#region Card Actions

		public void SaveNotes (long userCardId, string notes)
		{
			UISubscriptionService.SaveNotes (userCardId, notes);
		}

		public void ShowCard (CardDetailFragment fragment)
		{

			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			ActivityController.SaveActivity ((long)EventSources.Details, fragment.SelectedCard.CardId, UISubscriptionService.AuthToken);

			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_DETAILS, 0);
		}

		public void HideCard ()
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Gone;
			ActionBar.Show ();
		}

		public void ShowButtonPanel (Android.Support.V4.App.Fragment panel, Uri uri, string orientation)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (panel, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
			ActionBar.Hide ();
		}

		public void ShowPhoneDialer (PhoneFragment fragment)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_PHONE, 0);

		}

		public void ShowNotes (NotesFragment fragment)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_NOTES, 0);

		}

		public void ShowQuickShare (QuickShareFragment fragment)
		{
			var holder = FindViewById (Resource.Id.fragment_holder);
			holder.Visibility = ViewStates.Visible;
			//			var progressBar1 = holder.FindViewById<RelativeLayout> (Resource.Id.homeProgressContainer);
			//			if (progressBar1 != null) {
			//				progressBar1.Visibility = ViewStates.Visible;	
			//			}

			LoadFragment (fragment);
			ActionBar.Hide ();
		}

		public void ShareCard (ShareCardFragment fragment)
		{
			RunOnUiThread (() => {
				FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
				LoadFragment (fragment);
				ActionBar.Hide ();
				BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_SHARE, 0);
			});
		}

		public void SendEmail (Intent intent, long id)
		{
			ActivityController.SaveActivity ((long)EventSources.Email, id, UISubscriptionService.AuthToken);

			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_EMAIL, 0);

			StartActivity (intent);
		}

		public void OpenBrowser (Intent intent)
		{

			var userCard = GetUserCardFromIntent (intent);

			if (userCard != null) {
				ActivityController.SaveActivity ((long)EventSources.Website, userCard.CardId, UISubscriptionService.AuthToken);
			}
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_URL, 0);

			var browserIntent = Intent.CreateChooser (intent, "Open with");
			StartActivity (browserIntent);
		}

		public void OpenMap (Intent intent)
		{

			var userCard = GetUserCardFromIntent (intent);

			ActivityController.SaveActivity ((long)EventSources.Map, userCard.CardId, UISubscriptionService.AuthToken);

			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_MAP, 0);

			StartActivity (intent);
		}

		static UserCard GetUserCardFromIntent (Intent intent)
		{

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		public void AddToMyBusidex (UserCard userCard)
		{
			UISubscriptionService.AddCardToMyBusidex (userCard);
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_ADD, 0);
		}

		public void RemoveFromMyBusidex (UserCard userCard)
		{
			UISubscriptionService.RemoveCardFromMyBusidex (userCard);
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_MY_BUSIDEX_LABEL, Mobile.Resources.GA_LABEL_REMOVED, 0);
		}

		#endregion

		#region Fragment Loading

		int ConvertPixelsToDp (float pixelValue)
		{
			var dp = (int)((pixelValue) * Resources.DisplayMetrics.Density);
			return dp;
		}

		public void LoadFragment (
			Android.Support.V4.App.Fragment fragment,
			int? openAnimation = Resource.Animation.SlideAnimation,
			int? closeAnimation = Resource.Animation.SlideOutAnimation,
			int container = Resource.Id.fragment_holder
		)
		{

			//			if (fragment.IsVisible) {
			//				return;
			//			}

			pager.Visibility = ViewStates.Visible;

			using (var transaction = SupportFragmentManager.BeginTransaction ()) {

				if (openAnimation.HasValue && closeAnimation.HasValue) {
					transaction
						.SetCustomAnimations (
						openAnimation.Value,
						closeAnimation.Value,
						openAnimation.Value,
						closeAnimation.Value
					);
				}

				string name = fragment.GetType ().Name;

				transaction
					.Replace (container, fragment, name)
					//.AddToBackStack (name)
					.Commit ();
			}
		}

		//		public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
		//		{
		////			if(keyCode == Keycode.Back){
		////				return false;
		////			}else{
		//			return base.OnKeyDown (keyCode, e);
		////			}
		//		}

		public override void OnBackPressed ()
		{
			if (SupportFragmentManager.Fragments.SingleOrDefault (f => f is ContactsFragment) != null) {

			} else if (SupportFragmentManager.Fragments.SingleOrDefault (f => f is ContactProfileFragment) != null) {

			} else if (SupportFragmentManager.Fragments.SingleOrDefault (f => f is BaseCardEditFragment) != null) {
				UnloadFragment (new CardMenuFragment ());
			} else if (UISubscriptionService.CurrentUser != null) {
				UnloadFragment ();
			} else if (DoingLogin) {
				UnloadFragment (new StartUpFragment ());
				DoingLogin = false;
			} else if (DoingRegistration) {
				UnloadFragment (new StartUpFragment ());
				DoingRegistration = false;
			} else if (pager.CurrentItem == 1) {

			} else {
				base.OnBackPressed ();
			}
		}

		public void UnloadFragment (
			Android.Support.V4.App.Fragment fragment = null,
			int? openAnimation = Resource.Animation.SlideAnimation,
			int? closeAnimation = Resource.Animation.SlideOutAnimation,
			int container = Resource.Id.fragment_holder,
			bool showActionBar = true)
		{

			if (fragment == null) {
				if (showActionBar && pager.CurrentItem != 0) {
					ActionBar.Show ();
				}
				var holder = (LinearLayout)FindViewById (container);
				if (holder != null) {
					holder.RemoveAllViews ();
					holder.Visibility = ViewStates.Gone;
				}
			} else {
				LoadFragment (fragment, openAnimation, closeAnimation, container);
			}
		}

		#endregion

		#region Alerts

		protected void ShowAlert (string title, string message, string buttonText, System.EventHandler<DialogClickEventArgs> callback)
		{
			ShowAlert (title, message, new string [] { buttonText }, callback);
		}

		protected void ShowAlert (string title, string message, string [] buttons, System.EventHandler<DialogClickEventArgs> callback)
		{
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle (title);
			builder.SetMessage (message);
			if (buttons.Length > 1) {
				builder.SetNegativeButton (buttons [1], new System.EventHandler<DialogClickEventArgs> ((o, e) => {
					return;
				}));
			} else {
				builder.SetNegativeButton (GetString (Resource.String.Global_ButtonText_Cancel), new System.EventHandler<DialogClickEventArgs> ((o, e) => {
					return;
				}));
			}
			builder.SetCancelable (true);
			builder.SetPositiveButton (buttons [0], callback);

			builder.Show ();
		}

		#endregion

		#region QuickShare

		static UserCard LoadQuickShareCardData (QuickShareLink quickShareLink)
		{

			UserCard userCard = null;

			var result = CardController.GetCardById (UISubscriptionService.AuthToken, quickShareLink.CardId);
			if (!string.IsNullOrEmpty (result)) {

				var cardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (result);
				var card = new Card (cardResponse.Model);

				userCard = new UserCard {
					Card = card,
					CardId = quickShareLink.CardId,
					ExistsInMyBusidex = true,
					OwnerId = cardResponse.Model.OwnerId,
					UserId = cardResponse.Model.OwnerId.GetValueOrDefault (),
					Notes = string.Empty
				};

				SaveFromUrl (userCard, quickShareLink);
			}
			return userCard;
		}

		static void SaveFromUrl (UserCard uc, QuickShareLink link)
		{

			var sharedCardController = new SharedCardController ();

			var result = CardController.GetCardById (UISubscriptionService.AuthToken, uc.CardId);
			if (!string.IsNullOrEmpty (result)) {

				var cardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (result);
				var card = new Card (cardResponse.Model);

				UISubscriptionService.AddCardToMyBusidex (uc);

				//var myBusidexController = new MyBusidexController ();
				//myBusidexController.AddToMyBusidex (uc.CardId, token);

				sharedCardController.AcceptQuickShare (card, UISubscriptionService.CurrentUser.Email, link.From, UISubscriptionService.AuthToken, link.PersonalMessage);
				Utils.RemoveQuickShareLink ();

			}
		}

		#endregion

		#region Card Editing
		public void ShowCardEditMenu (CardMenuFragment fragment)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
		}

		public void ShowCardEditFragment (BaseCardEditFragment fragment)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
		}

		#endregion

		#region Privacy and Terms

		public void ShowTerms (TermsAndConditionsFragment fragment)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_SCREEN_TERMS, Mobile.Resources.GA_SCREEN_TERMS, 0);
		}

		public void ShowPrivacy (PrivacyFragment fragment)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Mobile.Resources.GA_CATEGORY_ACTIVITY, Mobile.Resources.GA_SCREEN_PRIVACY, Mobile.Resources.GA_SCREEN_PRIVACY, 0);
		}

		#endregion
	}
}