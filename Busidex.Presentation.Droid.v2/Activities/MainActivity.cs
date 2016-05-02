using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Rivets;
using Android.Net;
using System.Linq;
using BranchXamarinSDK;

namespace Busidex.Presentation.Droid.v2
{
	[Activity (
		Label = "Busidex", 
		LaunchMode = Android.Content.PM.LaunchMode.SingleTask, 
		ConfigurationChanges = global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]
	[IntentFilter (new [] { Android.Content.Intent.ActionView }, 
		DataScheme = "busidex", 
		DataPathPrefix = "/Uebo",
		//DataHost = "jqle.app.link", 
		Categories = new [] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
	public class MainActivity : FragmentActivity, IBranchSessionInterface
	{
		ViewPager pager;

		public static List<Xamarin.Contacts.Contact> Contacts { get; set; }

		void openFaq ()
		{
			var OpenBrowserIntent = new Intent (Intent.ActionView);
			var uri = Uri.Parse ("https://www.busidex.com/#/faq");
			OpenBrowserIntent.SetData (uri);

			OpenBrowser (OpenBrowserIntent);
		}

		void addTabs (GenericFragmentPagerAdaptor adapter)
		{

			// HOME
			adapter.AddFragmentView ((i, v, b) => {
				var view = i.Inflate (Resource.Layout.Home, v, false);

				//MY BUSIDEX
				var btnMyBusidex = view.FindViewById<Button> (Resource.Id.btnMyBusidex);
				btnMyBusidex.Click += delegate {
					pager.SetCurrentItem (1, true);
				};
				var imgMyBusidex = view.FindViewById<ImageView> (Resource.Id.imgBusidexIcon);
				imgMyBusidex.Click += delegate {
					pager.SetCurrentItem (1, true);
				};

				// SEARCH
				var btnSearch = view.FindViewById<Button> (Resource.Id.btnSearch);
				btnSearch.Click += delegate {
					pager.SetCurrentItem (2, true);
				};
				var imgSearch = view.FindViewById<ImageView> (Resource.Id.imgSearchIcon);
				imgSearch.Click += delegate {
					pager.SetCurrentItem (2, true);
				};

				// MY ORGANIZATIONS
				var btnMyOrganizations = view.FindViewById<Button> (Resource.Id.btnMyOrganizations);
				btnMyOrganizations.Click += delegate {
					pager.SetCurrentItem (3, true);
				};
				var imgOrganizations = view.FindViewById<ImageView> (Resource.Id.imgOrgIcon);
				imgOrganizations.Click += delegate {
					pager.SetCurrentItem (3, true);
				};

				// EVENTS
				var btnEvents = view.FindViewById<Button> (Resource.Id.btnEvents);
				btnEvents.Click += delegate {
					pager.SetCurrentItem (4, true);
				};
				var imgEvents = view.FindViewById<ImageView> (Resource.Id.imgEventIcon);
				imgEvents.Click += delegate {
					pager.SetCurrentItem (4, true);
				};

				// FAQ
				var btnFaq = view.FindViewById<Button> (Resource.Id.btnQuestions);
				btnFaq.Click += delegate {
					openFaq ();
				};
				var imgFaq = view.FindViewById<ImageView> (Resource.Id.imgFAQIcon);
				imgFaq.Click += delegate {
					openFaq ();
				};

				var btnSharedCardsNotification = view.FindViewById<ImageView> (Resource.Id.btnSharedCardsNotification);
				btnSharedCardsNotification.Click += delegate {
					pager.SetCurrentItem (5, true);
				};

				OnNotificationsLoadedEventHandler callback = list => RunOnUiThread (() => {
					ViewPagerExtensions.UpdateNotificationCount (ActionBar, list.Count);
						 
					var txtNotificationCount = view.FindViewById<TextView> (Resource.Id.txtNotificationCount);

					btnSharedCardsNotification.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
					txtNotificationCount.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
					txtNotificationCount.Text = list.Count.ToString ();

				});

				UISubscriptionService.OnNotificationsLoaded -= callback;
				UISubscriptionService.OnNotificationsLoaded += callback;

				ThreadPool.QueueUserWorkItem (tok => UISubscriptionService.LoadNotifications ());

				//var btnLogout = view.FindViewById<Button>(Resource.Id.btnLogout);
				//btnLogout.Click += delegate {
				//	
				//};

				return view;
			}
			);

			// MY BUSIDEX
			adapter.AddFragmentView ((i, v, b) => {
				var view = i.Inflate (Resource.Layout.MyBusidex, v, false);

				var progressBar1 = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);
				if (progressBar1 == null) {
					return view; 
				}

				var myBusidexProgressStatus = view.FindViewById<TextView> (Resource.Id.myBusidexProgressStatus);
				progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Visible;


				var lblNoCardsMessage = view.FindViewById<TextView> (Resource.Id.lblNoCardsMessage);
				lblNoCardsMessage.Visibility = ViewStates.Gone;

				var myBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, UISubscriptionService.UserCards);

				var txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
				txtFilter.Visibility = UISubscriptionService.UserCards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
				txtFilter.QueryTextChange += delegate {
					DoFilter (myBusidexAdapter, txtFilter.Query);
				};

				txtFilter.Iconified = false;
				txtFilter.ClearFocus ();
				txtFilter.Touch += delegate {
					txtFilter.Focusable = true;
					txtFilter.RequestFocus ();
				};

				myBusidexAdapter.Redirect += ShowCard;
				myBusidexAdapter.ShowButtonPanel += ShowButtonPanel;
				myBusidexAdapter.ShowNotes = true;

				var lstCards = view.FindViewById<OverscrollListView> (Resource.Id.lstCards);
				lstCards.Adapter = myBusidexAdapter;

				int accumulatedDeltaY = 0;
				lstCards.OverScrolled += deltaY => {

					accumulatedDeltaY += -deltaY;
					if (accumulatedDeltaY > 1000) {
						lstCards.Visibility = ViewStates.Gone;
						progressBar1.Visibility = ViewStates.Visible;
						UISubscriptionService.LoadUserCards ();
						BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MY_BUSIDEX_REFRESHED, 0);

					}
				};

				lstCards.Scroll += delegate {
					if (lstCards.CanScrollVertically (-1)) {
						accumulatedDeltaY = 0;
					}
				};

				OnMyBusidexLoadedEventHandler callback = list => RunOnUiThread (() => {
					myBusidexAdapter.UpdateData (list);
					progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Gone;
					lstCards.Visibility = ViewStates.Visible;
					if (list.Count == 0) {
						lblNoCardsMessage.Visibility = ViewStates.Visible;
						lblNoCardsMessage.SetText (Resource.String.MyBusidex_NoCards);
					}
					txtFilter.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
					accumulatedDeltaY = 0;
				});

				OnMyBusidexUpdatedEventHandler update = status => RunOnUiThread (() => {
					progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Visible;
					progressBar1.Max = status.Total;
					progressBar1.Progress = status.Count;
					myBusidexProgressStatus.Text = string.Format ("Loading {0} of {1}", status.Count, status.Total);	
					lblNoCardsMessage.Visibility = ViewStates.Gone;
				});

				UISubscriptionService.OnMyBusidexLoaded -= callback;
				UISubscriptionService.OnMyBusidexLoaded += callback;

				UISubscriptionService.OnMyBusidexUpdated -= update;
				UISubscriptionService.OnMyBusidexUpdated += update;

				var state = lstCards.OnSaveInstanceState ();
				if (state != null) {
					lstCards.OnRestoreInstanceState (state);
				}

				lstCards.RequestFocus (FocusSearchDirection.Down);


				UISubscriptionService.LoadUserCards ();

				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_MY_BUSIDEX);
				return view;
			}
			);

			// SEARCH
			adapter.AddFragment (
				new SearchFragment ()
			);

			// MY ORGANIZATIONS
			adapter.AddFragmentView ((i, v, b) => {
				var view = i.Inflate (Resource.Layout.MyOrganizations, v, false);
				var orgAdapter = new OrganizationAdapter (this, Resource.Id.lstOrganizations, UISubscriptionService.OrganizationList);
				orgAdapter.RedirectToOrganizationDetails += org => ShowOrganizationDetail (new OrganizationPanelFragment (org));
				orgAdapter.RedirectToOrganizationMembers += LoadOrganizationMembers;

				var lstOrganizations = view.FindViewById<OverscrollListView> (Resource.Id.lstOrganizations);
				var lblNoOrganizationsMessage = view.FindViewById<TextView> (Resource.Id.lblNoOrganizationsMessage);
				lblNoOrganizationsMessage.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
				lstOrganizations.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

				lstOrganizations.Adapter = orgAdapter;

				var progressBar2 = view.FindViewById<ProgressBar> (Resource.Id.progressBar2);
				progressBar2.Visibility = ViewStates.Gone;

				var imgRefreshOrganizations = view.FindViewById<ImageButton> (Resource.Id.imgRefreshOrganizations);
				imgRefreshOrganizations.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
				imgRefreshOrganizations.Click += delegate {
					progressBar2.Visibility = ViewStates.Visible;
					UISubscriptionService.LoadOrganizations ();
				};

				int accumulatedDeltaY = 0;
				lstOrganizations.OverScrolled += deltaY => {

					accumulatedDeltaY += -deltaY;
					if (accumulatedDeltaY > 1000) {
						lstOrganizations.Visibility = ViewStates.Gone;
						progressBar2.Visibility = ViewStates.Visible;
						UISubscriptionService.LoadOrganizations ();
					}
				};

				lstOrganizations.Scroll += delegate {
					if (lstOrganizations.CanScrollVertically (-1)) {
						accumulatedDeltaY = 0;
					}
				};

				OnMyOrganizationsLoadedEventHandler callback = list => RunOnUiThread (() => {
					accumulatedDeltaY = 0;
					orgAdapter.UpdateData (list);
					progressBar2.Visibility = ViewStates.Gone;
					lblNoOrganizationsMessage.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					lstOrganizations.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
				});

				UISubscriptionService.OnMyOrganizationsLoaded -= callback;
				UISubscriptionService.OnMyOrganizationsLoaded += callback;

				ThreadPool.QueueUserWorkItem (tok => UISubscriptionService.LoadOrganizations ());

				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_ORGANIZATIONS);

				return view;
			}	
			);

			// EVENTS
			adapter.AddFragmentView ((i, v, b) => {
				var view = i.Inflate (Resource.Layout.EventList, v, false);
				var eventListAdapter = new EventListAdapter (this, Resource.Id.lstCards, UISubscriptionService.EventList);
				eventListAdapter.RedirectToEventCards += LoadEventCards;

				var lstEvents = view.FindViewById<ListView> (Resource.Id.lstEvents);

				lstEvents.Adapter = eventListAdapter;

				OnEventListLoadedEventHandler callback = list => RunOnUiThread (() => eventListAdapter.UpdateData (list));

				UISubscriptionService.OnEventListLoaded -= callback;
				UISubscriptionService.OnEventListLoaded += callback;

				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_EVENTS);

				return view;
			}
			);

			// REFERRALS
			adapter.AddFragmentView ((i, v, b) => {
					
				var view = i.Inflate (Resource.Layout.SharedCardList, v, false);
				var lstSharedCards = view.FindViewById<ListView> (Resource.Id.lstSharedCards);

				var imgNoNotifications = view.FindViewById<ImageView> (Resource.Id.imgNoNotifications);
				var lblNoNotificationsMessage = view.FindViewById<TextView> (Resource.Id.lblNoNotificationsMessage);

				imgNoNotifications.Visibility = UISubscriptionService.Notifications.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
				lblNoNotificationsMessage.Visibility = UISubscriptionService.Notifications.Count == 0 ? ViewStates.Visible : ViewStates.Gone;

				var sharedCardAdapter = new SharedCardListAdapter (this, Resource.Id.lstSharedCards, UISubscriptionService.Notifications);
				lstSharedCards.Adapter = sharedCardAdapter;

				sharedCardAdapter.SharingCard -= SaveSharedCard;
				sharedCardAdapter.SharingCard += SaveSharedCard;

				OnNotificationsLoadedEventHandler callback = list => RunOnUiThread (() => {
					ViewPagerExtensions.UpdateNotificationCount (ActionBar, list.Count);
					sharedCardAdapter.UpdateData (list);

					imgNoNotifications.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					lblNoNotificationsMessage.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;

				});

				UISubscriptionService.OnNotificationsLoaded -= callback;
				UISubscriptionService.OnNotificationsLoaded += callback;

				ThreadPool.QueueUserWorkItem (tok => UISubscriptionService.LoadNotifications ());

				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_REFERRALS);

				return view;
			}
			);

			// PROFILE
			var profileFragment = new ProfileFragment (UISubscriptionService.CurrentUser);

			OnBusidexUserLoadedEventHandler profileCallback = user => RunOnUiThread (() => profileFragment.UpdateUser (user));

			UISubscriptionService.OnBusidexUserLoaded -= profileCallback;
			UISubscriptionService.OnBusidexUserLoaded += profileCallback;

			adapter.AddFragment (
				profileFragment
			);
		}

		void Init ()
		{
			BaseApplicationResource.Init (this);
			var token = BaseApplicationResource.GetAuthCookie ();

			UISubscriptionService.AuthToken = token;

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

		void setStartTab ()
		{
			var tab = ActionBar.GetTabAt (0);
			ActionBar.SelectTab (tab);
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_LABEL_APP_START, Busidex.Mobile.Resources.GA_LABEL_APP_START, 0);
		}

		static void saveDeviceTypeSet ()
		{
			var prefs = Application.Context.GetSharedPreferences (Busidex.Mobile.Resources.APPLICATION_NAME, FileCreationMode.Private);
			var prefEditor = prefs.Edit ();
			prefEditor.PutBoolean (Busidex.Mobile.Resources.USER_SETTING_DEVICE_TYPE_SET, true);
			prefEditor.Commit ();	
		}

		public bool getDeviceTypeSetting ()
		{
			var prefs = Android.App.Application.Context.GetSharedPreferences (Busidex.Mobile.Resources.APPLICATION_NAME, FileCreationMode.Private);
			return prefs.GetBoolean (Busidex.Mobile.Resources.USER_SETTING_DEVICE_TYPE_SET, false);	
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			if (string.IsNullOrEmpty (UISubscriptionService.AuthToken)) {
				DoStartUp ();		
			}
		}

		static void DoFilter (UserCardAdapter adapter, string filter)
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

		protected override void OnCreate (Bundle savedInstanceState)
		{

			base.OnCreate (savedInstanceState);

			Xamarin.Insights.Initialize (GetString (Resource.String.InsightsApiKey), ApplicationContext);

			BranchAndroid.Init (this, Busidex.Mobile.Resources.BRANCH_KEY, this);

			RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Portrait;

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			pager = FindViewById<ViewPager> (Resource.Id.pager);
			var tabAdapter = new GenericFragmentPagerAdaptor (SupportFragmentManager);

			Init ();

			addTabs (tabAdapter);

			pager.Adapter = tabAdapter;
			pager.SetOnPageChangeListener (new ViewPageListenerForActionBar (ActionBar));

			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.SetDisplayShowHomeEnabled (false);
			ActionBar.SetDisplayShowTitleEnabled (true);

			ActionBar.Title = "My Busidex";

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

//			var optionsTab = pager.GetViewPageTab (ActionBar, "Options");
//			optionsTab.SetCustomView (Resource.Layout.OptionsTab);
//			optionsTab.TabSelected += (object sender, ActionBar.TabEventArgs e) => {
//				showActionBarTitle = !showActionBarTitle;
//				for(var i=0; i< ActionBar.TabCount; i++){
//					var tab = ActionBar.GetTabAt(i);
//					var title = (TextView)tab.CustomView.FindViewById(Resource.Id.txtTabTitle);
//					if(title != null){
//						if(showActionBarTitle){
//							title.Text = titles[i];
//						}else{
//							title.Text = "";
//						}
//					}
//				}
//			};

			ActionBar.AddTab (homeTab);
			ActionBar.AddTab (myBusidexTab);
			ActionBar.AddTab (searchTab);
			ActionBar.AddTab (myOrganizationsTab);
			ActionBar.AddTab (eventsTab);
			ActionBar.AddTab (notificationsTab);
			ActionBar.AddTab (profileTab);
			//ActionBar.AddTab (optionsTab);


		}

		protected override void OnNewIntent (Intent intent)
		{
			base.OnNewIntent (intent);
			//BranchAndroid.getInstance ().SetNewUrl (intent.Data);
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
					Utils.SaveResponse (json, Busidex.Mobile.Resources.QUICKSHARE_LINK);
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

		public void SaveSharedCard (SharedCard card)
		{
			UISubscriptionService.SaveSharedCard (card);
			ViewPagerExtensions.UpdateNotificationCount (ActionBar, UISubscriptionService.Notifications.Count);
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

				UISubscriptionService.OnEventCardsLoaded -= goToEventCards;
				UISubscriptionService.OnEventCardsLoaded += goToEventCards;

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
			LoadFragment (panel, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
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
			var logoPath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var fragment = new OrganizationCardsFragment (orgMembers, logoPath);

			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
			ActionBar.Hide ();
		}

		public void LoadOrganizationReferrals (Organization organization)
		{
			
			var logoPath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var fragment = new OrganizationCardsFragment (UISubscriptionService.OrganizationReferrals [organization.OrganizationId], logoPath);
			LoadFragment (fragment, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
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

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_DETAILS, 0);
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
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_PHONE, 0);

		}

		public void ShowNotes (NotesFragment fragment)
		{
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_NOTES, 0);

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
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_SHARE, 0);
		}

		public void SendEmail (Intent intent, long id)
		{
			ActivityController.SaveActivity ((long)EventSources.Email, id, UISubscriptionService.AuthToken);

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_EMAIL, 0);

			StartActivity (intent);
		}

		public void OpenBrowser (Intent intent)
		{

			var userCard = GetUserCardFromIntent (intent);
		
			if (userCard != null) {
				ActivityController.SaveActivity ((long)EventSources.Website, userCard.CardId, UISubscriptionService.AuthToken);
			}
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_URL, 0);

			var browserIntent = Intent.CreateChooser (intent, "Open with");
			StartActivity (browserIntent);
		}

		public void OpenMap (Intent intent)
		{

			var userCard = GetUserCardFromIntent (intent);
		
			ActivityController.SaveActivity ((long)EventSources.Map, userCard.CardId, UISubscriptionService.AuthToken);

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MAP, 0);

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
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_ADD, 0);
		}

		public void RemoveFromMyBusidex (UserCard userCard)
		{
			UISubscriptionService.RemoveCardFromMyBusidex (userCard);
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_REMOVED, 0);
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

			if (fragment.IsVisible) {
				return;
			}
				
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
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle (title);
			builder.SetMessage (message);
			builder.SetNegativeButton (GetString (Resource.String.Global_ButtonText_Cancel), new System.EventHandler<DialogClickEventArgs> ((o, e) => {
				return;
			}));
			builder.SetCancelable (true);
			builder.SetPositiveButton (buttonText, callback);
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
   
			#endregion
		}
	}
}