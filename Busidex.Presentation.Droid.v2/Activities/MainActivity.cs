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

namespace Busidex.Presentation.Droid.v2
{
	[Activity(Label = "Busidex", ConfigurationChanges =  global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]
	public class MainActivity : FragmentActivity
	{
		ViewPager pager;

		void addTabs(GenericFragmentPagerAdaptor adapter){

			// HOME
//			adapter.AddFragmentView((i, v, b) =>
//				{
//					var view = i.Inflate(Resource.Layout.Home, v, false);
//					return view;
//				}
//			);

			// MY BUSIDEX
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = i.Inflate(Resource.Layout.MyBusidex, v, false);

					var progressBar1 = view.FindViewById<ProgressBar>(Resource.Id.progressBar1);
					if(progressBar1 == null){
						return view; 
					}

					progressBar1.Visibility = ViewStates.Visible;

					var lblNoCardsMessage = view.FindViewById<TextView>(Resource.Id.lblNoCardsMessage);
					lblNoCardsMessage.Visibility = ViewStates.Gone;

					var myBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, UISubscriptionService.UserCards);

					var txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
					txtFilter.Visibility = UISubscriptionService.UserCards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
					txtFilter.QueryTextChange += delegate {
						DoFilter (myBusidexAdapter, txtFilter.Query);
					};

					txtFilter.Iconified = false;
					txtFilter.ClearFocus();
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
					lstCards.OverScrolled += deltaY=> {

						accumulatedDeltaY += -deltaY;
						if(accumulatedDeltaY > 1000){
							lstCards.Visibility = ViewStates.Gone;
							progressBar1.Visibility = ViewStates.Visible;
							UISubscriptionService.LoadUserCards();
						 	BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MY_BUSIDEX_REFRESHED, 0);

						}
					};

					lstCards.Scroll+= delegate {
						if( lstCards.CanScrollVertically(-1)){
							accumulatedDeltaY = 0;
						}
					};

					OnMyBusidexLoadedEventHandler callback = list => RunOnUiThread (() => {
						myBusidexAdapter.UpdateData (list);
						progressBar1.Visibility = ViewStates.Gone;
						lstCards.Visibility = ViewStates.Visible;
						if (list.Count == 0) {
							lblNoCardsMessage.Visibility = ViewStates.Visible;
							lblNoCardsMessage.SetText (Resource.String.MyBusidex_NoCards);
						}
						txtFilter.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
						accumulatedDeltaY = 0;
					});
					UISubscriptionService.OnMyBusidexLoaded -= callback;
					UISubscriptionService.OnMyBusidexLoaded += callback;

					var state = lstCards.OnSaveInstanceState ();
					if(state != null){
						lstCards.OnRestoreInstanceState (state);
					}

					lstCards.RequestFocus (FocusSearchDirection.Down);

					UISubscriptionService.LoadUserCards();

					BaseApplicationResource.TrackScreenView(Busidex.Mobile.Resources.GA_SCREEN_MY_BUSIDEX);
					return view;
				}
			);

			// SEARCH
			adapter.AddFragment (
				new SearchFragment ()
			);

			// MY ORGANIZATIONS
			adapter.AddFragmentView ((i, v, b) => 
				{

					var view = i.Inflate (Resource.Layout.MyOrganizations, v, false);
					var orgAdapter = new OrganizationAdapter (this, Resource.Id.lstOrganizations, UISubscriptionService.OrganizationList);
					orgAdapter.RedirectToOrganizationDetails += org => ShowOrganizationDetail (new OrganizationPanelFragment (org));
					orgAdapter.RedirectToOrganizationMembers += LoadOrganizationMembers;

					var lstOrganizations = view.FindViewById<OverscrollListView> (Resource.Id.lstOrganizations);
					var lblNoOrganizationsMessage = view.FindViewById<TextView>(Resource.Id.lblNoOrganizationsMessage);
					lblNoOrganizationsMessage.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					lstOrganizations.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

					lstOrganizations.Adapter = orgAdapter;

					var progressBar2 = view.FindViewById<ProgressBar>(Resource.Id.progressBar2);
					progressBar2.Visibility = ViewStates.Gone;

					var imgRefreshOrganizations = view.FindViewById<ImageButton>(Resource.Id.imgRefreshOrganizations);
					imgRefreshOrganizations.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					imgRefreshOrganizations.Click += delegate {
						progressBar2.Visibility = ViewStates.Visible;
						UISubscriptionService.LoadOrganizations();
					};

					int accumulatedDeltaY = 0;
					lstOrganizations.OverScrolled += deltaY=> {

						accumulatedDeltaY += -deltaY;
						if(accumulatedDeltaY > 1000){
							lstOrganizations.Visibility = ViewStates.Gone;
							progressBar2.Visibility = ViewStates.Visible;
							UISubscriptionService.LoadOrganizations();
						}
					};

					lstOrganizations.Scroll+= delegate {
						if( lstOrganizations.CanScrollVertically(-1)){
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

					ThreadPool.QueueUserWorkItem(tok => UISubscriptionService.LoadOrganizations ());

					BaseApplicationResource.TrackScreenView(Busidex.Mobile.Resources.GA_SCREEN_ORGANIZATIONS);

					return view;
				}	
			);

			// EVENTS
			adapter.AddFragmentView ((i, v, b) => 
				{
					var view = i.Inflate (Resource.Layout.EventList, v, false);
					var eventListAdapter = new EventListAdapter (this, Resource.Id.lstCards, UISubscriptionService.EventList);
					eventListAdapter.RedirectToEventCards += LoadEventCards;

					var lstEvents = view.FindViewById<ListView> (Resource.Id.lstEvents);

					lstEvents.Adapter = eventListAdapter;

					OnEventListLoadedEventHandler callback = list => RunOnUiThread (() => eventListAdapter.UpdateData (list));

					UISubscriptionService.OnEventListLoaded -= callback;
					UISubscriptionService.OnEventListLoaded += callback;

					BaseApplicationResource.TrackScreenView(Busidex.Mobile.Resources.GA_SCREEN_EVENTS);

					return view;
				}
			);

			// REFERRALS
			adapter.AddFragmentView ((i, v, b) => 
				{
					var view = i.Inflate (Resource.Layout.SharedCardList, v, false);
					var lstSharedCards = view.FindViewById<ListView>(Resource.Id.lstSharedCards);

					var imgNoNotifications = view.FindViewById<ImageView>(Resource.Id.imgNoNotifications);
					var lblNoNotificationsMessage = view.FindViewById<TextView>(Resource.Id.lblNoNotificationsMessage);

					imgNoNotifications.Visibility = UISubscriptionService.Notifications.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					lblNoNotificationsMessage.Visibility = UISubscriptionService.Notifications.Count == 0 ? ViewStates.Visible : ViewStates.Gone;

					var sharedCardAdapter = new SharedCardListAdapter(this, Resource.Id.lstSharedCards, UISubscriptionService.Notifications);
					lstSharedCards.Adapter = sharedCardAdapter;

					sharedCardAdapter.SharingCard -= SaveSharedCard;
					sharedCardAdapter.SharingCard += SaveSharedCard;

					OnNotificationsLoadedEventHandler callback = list => RunOnUiThread (() => {
						ViewPagerExtensions.UpdateNotificationCount(ActionBar, list.Count);
						sharedCardAdapter.UpdateData (list);

						imgNoNotifications.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
						lblNoNotificationsMessage.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;

					});

					UISubscriptionService.OnNotificationsLoaded -= callback;
					UISubscriptionService.OnNotificationsLoaded += callback;

					ThreadPool.QueueUserWorkItem(tok => UISubscriptionService.LoadNotifications ());

					BaseApplicationResource.TrackScreenView(Busidex.Mobile.Resources.GA_SCREEN_REFERRALS);

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

		void Init(){
			BaseApplicationResource.Init (this);
			var token = BaseApplicationResource.GetAuthCookie ();

			UISubscriptionService.AuthToken = token;
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			if(!getDeviceTypeSetting()){

				string token = BaseApplicationResource.GetAuthCookie();
				var deviceType = Busidex.Mobile.DeviceType.Android;

				AccountController.UpdateDeviceType(token, deviceType).ContinueWith(r =>{
					saveDeviceTypeSet ();
				});
			}

			var tab = ActionBar.GetTabAt (0);
			ActionBar.SelectTab (tab);
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_LABEL_APP_START, Busidex.Mobile.Resources.GA_LABEL_APP_START, 0);
		}

		void saveDeviceTypeSet(){
			var prefs = Application.Context.GetSharedPreferences(Busidex.Mobile.Resources.APPLICATION_NAME, FileCreationMode.Private);
			var prefEditor = prefs.Edit();
			prefEditor.PutBoolean(Busidex.Mobile.Resources.USER_SETTING_DEVICE_TYPE_SET, true);
			prefEditor.Commit();	
		}

		public bool getDeviceTypeSetting(){
			var prefs = Android.App.Application.Context.GetSharedPreferences(Busidex.Mobile.Resources.APPLICATION_NAME, FileCreationMode.Private);
			return prefs.GetBoolean (Busidex.Mobile.Resources.USER_SETTING_DEVICE_TYPE_SET, false);	
		}

		protected override void OnStart ()
		{
			base.OnStart ();



			string token = BaseApplicationResource.GetAuthCookie ();
			if(string.IsNullOrEmpty(token)){
				DoStartUp ();		
			}

		}

		static void DoFilter(UserCardAdapter adapter, string filter){
			if (adapter == null) {
				return;
			}
			if(string.IsNullOrEmpty(filter)){
				adapter.CardFilter.InvokeFilter ("");
			}else{
				adapter.CardFilter.InvokeFilter(filter);
			}
		}
			
		protected override void OnCreate(Bundle savedInstanceState)
		{

			base.OnCreate(savedInstanceState);

			Xamarin.Insights.Initialize(GetString(Resource.String.InsightsApiKey), ApplicationContext);

			RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Portrait;

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			pager = FindViewById<ViewPager>(Resource.Id.pager);
			var tabAdapter = new GenericFragmentPagerAdaptor (SupportFragmentManager);

			Init ();

			addTabs (tabAdapter);

			pager.Adapter = tabAdapter;
			pager.SetOnPageChangeListener(new ViewPageListenerForActionBar(ActionBar));

			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.SetDisplayShowHomeEnabled(false);
			ActionBar.SetDisplayShowTitleEnabled(true);

			ActionBar.Title = "My Busidex";

//			var homeTab = pager.GetViewPageTab (ActionBar, "");
//			homeTab.SetCustomView (Resource.Layout.tab);
//			homeTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.Icon);


			const float DISABLED_ALPHA = .3f;

			var myBusidexTab = pager.GetViewPageTab (ActionBar, "");
			myBusidexTab.SetCustomView (Resource.Layout.tab);
			myBusidexTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.MyBusidexIcon);
			myBusidexTab.TabReselected += delegate {

				var activity = tabAdapter.GetItem(0).Activity;
				if(activity != null){				
					var lstCards = (OverscrollListView)activity.FindViewById(Resource.Id.lstCards);
					lstCards.SmoothScrollToPosition(0);
				}
			};

			var searchTab = pager.GetViewPageTab (ActionBar, "");
			searchTab.SetCustomView (Resource.Layout.tab);
			searchTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.SearchIconDisabled);
			searchTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			var myOrganizationsTab = pager.GetViewPageTab (ActionBar, "");
			myOrganizationsTab.SetCustomView (Resource.Layout.tab);
			myOrganizationsTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.OrganizationsIconDisabled);
			myOrganizationsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			var eventsTab = pager.GetViewPageTab (ActionBar, "");
			eventsTab.SetCustomView (Resource.Layout.tab);
			eventsTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.EventIconDisabled);
			eventsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;

			var notificationsTab = pager.GetViewPageTab (ActionBar, "");
			notificationsTab.SetCustomView (Resource.Layout.notification);
			notificationsTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.NotificationDisabled);
			var txtNotificationCount = notificationsTab.CustomView.FindViewById<TextView> (Resource.Id.txtNotificationCount);
			txtNotificationCount.Text = UISubscriptionService.Notifications.Count.ToString();
			txtNotificationCount.Visibility = UISubscriptionService.Notifications.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
			notificationsTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = DISABLED_ALPHA;


			var profileTab = pager.GetViewPageTab (ActionBar, "");
			profileTab.SetCustomView (Resource.Layout.tab);
			profileTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.settingsDisabled);
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

			//ActionBar.AddTab(homeTab);
			ActionBar.AddTab(myBusidexTab);
			ActionBar.AddTab(searchTab);
			ActionBar.AddTab(myOrganizationsTab);
			ActionBar.AddTab(eventsTab);
			ActionBar.AddTab (notificationsTab);
			ActionBar.AddTab(profileTab);
			//ActionBar.AddTab (optionsTab);


		}

		#region Card Sharing
		public void SaveSharedCard(SharedCard card){
			UISubscriptionService.SaveSharedCard (card);
			ViewPagerExtensions.UpdateNotificationCount(ActionBar, UISubscriptionService.Notifications.Count);
		}
		#endregion

		#region Startup Actions
		public void DoStartUp(){
			DoingLogin = true;
			UISubscriptionService.CurrentUser = null;
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new StartUpFragment ());
			ActionBar.Hide ();
		}

		public void ShowRegistration(){
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

		public void ShowLogin(){
			DoingLogin = true;
			UISubscriptionService.CurrentUser = null;
			DoLogin ();
		}
 
		public void DoLogout(){
			UISubscriptionService.Clear ();
			Utils.RemoveCacheFiles ();
			BaseApplicationResource.RemoveAuthCookie ();
			UnloadFragment ();

			var tab = ActionBar.GetTabAt (0);
			ActionBar.SelectTab (tab);

			ShowLogin ();
		}

		void DoLogin(){
			DoingLogin = true;
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new LoginFragment ());
			ActionBar.Hide ();
		}

		public void LoginComplete(){

			UnloadFragment ();

			UISubscriptionService.AuthToken = BaseApplicationResource.GetAuthCookie ();

			UISubscriptionService.Sync ();
			DoingLogin = false;
			if(DoingRegistration){
				
				DoingRegistration = false;
			}
		}		

		public void UpdateEmail(string email){
			UISubscriptionService.ChangeEmail (email);
		}

		#endregion

		#region Event Actions
		public async Task<bool> ReloadEventCards(EventTag tag){
			await UISubscriptionService.loadEventCards (tag);
			return true;
		}

		public void LoadEventCards(EventTag tag){

			if (!UISubscriptionService.EventCards.ContainsKey (tag.Text)) {
				UISubscriptionService.OnEventCardsLoaded -= goToEventCards;
				UISubscriptionService.OnEventCardsLoaded += goToEventCards;

				UISubscriptionService.loadEventCards (tag);
			}else{
				goToEventCards (tag, UISubscriptionService.EventCards [tag.Text]);
			}
		}

		void goToEventCards(EventTag tag, List<UserCard> cards){
			RunOnUiThread (() => {
				var fragment = new EventCardsFragment (tag, cards);
				FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
				LoadFragment (fragment, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
				ActionBar.Hide ();
			});
		}
		#endregion

		#region Organization Actions
		public void ShowOrganizationDetail(Android.Support.V4.App.Fragment panel){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (panel, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
			ActionBar.Hide ();
		}

		public void LoadOrganizationMembers(Organization organization){
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

		public void LoadOrganizationReferrals(Organization organization){
			
			var logoPath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var fragment = new OrganizationCardsFragment (UISubscriptionService.OrganizationReferrals [organization.OrganizationId], logoPath);
			LoadFragment (fragment, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
		}

		#endregion

		#region Card Actions
		public void SaveNotes(long userCardId, string notes){
			UISubscriptionService.SaveNotes (userCardId, notes);
		}

		public void ShowCard(CardDetailFragment fragment){

			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			string token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Details, fragment.UserCard.CardId, token);

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_DETAILS, 0);
		}

		public void HideCard(){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Gone;
			ActionBar.Show ();
		}

		public void ShowButtonPanel(Android.Support.V4.App.Fragment panel, Android.Net.Uri uri, string orientation){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (panel, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
			ActionBar.Hide ();
		}

		public void ShowPhoneDialer(PhoneFragment fragment){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_PHONE, 0);

		}

		public void ShowNotes(NotesFragment fragment){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_NOTES, 0);

		}

		public void ShareCard(ShareCardFragment fragment){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_SHARE, 0);
		}

		public void SendEmail(Intent intent, long id){

			var token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Email, id, token);

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_EMAIL, 0);

			StartActivity(intent);
		}

		public void OpenBrowser(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Website, userCard.CardId, token);

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_URL, 0);

			var browserIntent = Intent.CreateChooser(intent, "Open with");
			StartActivity (browserIntent);
		}

		public void OpenMap(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Map, userCard.CardId, token);

			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MAP, 0);

			StartActivity (intent);
		}

		static UserCard GetUserCardFromIntent(Intent intent){

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		public void AddToMyBusidex(UserCard userCard){
			UISubscriptionService.AddCardToMyBusidex (userCard);
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_ADD, 0);
		}

		public void RemoveFromMyBusidex(UserCard userCard){
			UISubscriptionService.RemoveCardFromMyBusidex (userCard);
			BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_REMOVED, 0);
		}
		#endregion

		#region Fragment Loading
		int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int) ((pixelValue) * Resources.DisplayMetrics.Density);
			return dp;
		}

		public void LoadFragment(
			Android.Support.V4.App.Fragment fragment, 
			int? openAnimation = Resource.Animation.SlideAnimation, 
			int? closeAnimation = Resource.Animation.SlideOutAnimation,
			int container = Resource.Id.fragment_holder
		){

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

		public override void OnBackPressed ()
		{
			if (UISubscriptionService.CurrentUser != null) {
				//base.OnBackPressed ();
				UnloadFragment ();
			}else if(DoingLogin){
				UnloadFragment (new StartUpFragment ());
				DoingLogin = false;
			}else if(DoingRegistration){
				UnloadFragment (new StartUpFragment ());
				DoingRegistration = false;
			}
		}

		public void UnloadFragment(
			Android.Support.V4.App.Fragment fragment = null,
			int? openAnimation = Resource.Animation.SlideAnimation, 
			int? closeAnimation = Resource.Animation.SlideOutAnimation,
			int container = Resource.Id.fragment_holder){

			if(fragment == null){
				ActionBar.Show ();
				var holder = (LinearLayout)FindViewById (container);
				if (holder != null) {
					holder.RemoveAllViews ();
					holder.Visibility = ViewStates.Gone;
				}
			}else{
				LoadFragment (fragment, openAnimation, closeAnimation, container);
			}
		}
		#endregion

		#region Alerts
		protected void ShowAlert(string title, string message, string buttonText, System.EventHandler<DialogClickEventArgs> callback){
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle(title);
			builder.SetMessage(message);
			builder.SetNegativeButton (GetString (Resource.String.Global_ButtonText_Cancel), new System.EventHandler<DialogClickEventArgs>((o,e) => {
				return;
			}));
			builder.SetCancelable(true);
			builder.SetPositiveButton(buttonText, callback);
			builder.Show();
		}
		#endregion 
	}
}