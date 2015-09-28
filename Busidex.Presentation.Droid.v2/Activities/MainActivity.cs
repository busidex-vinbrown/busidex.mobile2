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
using Android.Gms.Analytics;
using System.IO;
using System.Threading;

namespace Busidex.Presentation.Droid.v2
{
	[Activity(Label = "Busidex", MainLauncher = true, ConfigurationChanges =  global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]
	public class MainActivity : FragmentActivity
	{
		ViewPager pager;
		UISubscriptionService subscriptionService;
		bool OnMyBusidexLoadedAssigned, OnMyOrganizationsLoadedAssigned, OnEventListLoadedAssigned, OnBusidexUserLoadedAssigned;

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
					lblNoCardsMessage.Visibility = ViewStates.Gone;// subscriptionService.UserCards.Count > 0 ? ViewStates.Gone : ViewStates.Visible;

					var myBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, subscriptionService.UserCards);

					var txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
					txtFilter.Visibility = subscriptionService.UserCards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
					txtFilter.QueryTextChange += delegate {
						//DoFilter (txtFilter.Query);
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
							subscriptionService.LoadUserCards();
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
					subscriptionService.OnMyBusidexLoaded -= callback;
					subscriptionService.OnMyBusidexLoaded += callback;

					var state = lstCards.OnSaveInstanceState ();
					if(state != null){
						lstCards.OnRestoreInstanceState (state);
					}

					lstCards.RequestFocus (FocusSearchDirection.Down);

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
					var orgAdapter = new OrganizationAdapter (this, Resource.Id.lstOrganizations, subscriptionService.OrganizationList);
					orgAdapter.RedirectToOrganizationDetails += org => ShowOrganizationDetail (new OrganizationPanelFragment (org));

					var lstOrganizations = view.FindViewById<OverscrollListView> (Resource.Id.lstOrganizations);
					var lblNoOrganizationsMessage = view.FindViewById<TextView>(Resource.Id.lblNoOrganizationsMessage);
					lblNoOrganizationsMessage.Visibility = subscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					lstOrganizations.Visibility = subscriptionService.OrganizationList.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

					lstOrganizations.Adapter = orgAdapter;

					var progressBar2 = view.FindViewById<ProgressBar>(Resource.Id.progressBar2);
					progressBar2.Visibility = ViewStates.Gone;

					var imgRefreshOrganizations = view.FindViewById<ImageButton>(Resource.Id.imgRefreshOrganizations);
					imgRefreshOrganizations.Visibility = subscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					imgRefreshOrganizations.Click += delegate {
						progressBar2.Visibility = ViewStates.Visible;
						subscriptionService.LoadOrganizations();
					};

					int accumulatedDeltaY = 0;
					lstOrganizations.OverScrolled += deltaY=> {

						accumulatedDeltaY += -deltaY;
						if(accumulatedDeltaY > 1000){
							lstOrganizations.Visibility = ViewStates.Gone;
							progressBar2.Visibility = ViewStates.Visible;
							subscriptionService.LoadOrganizations();
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

					subscriptionService.OnMyOrganizationsLoaded -= callback;
					subscriptionService.OnMyOrganizationsLoaded += callback;

					ThreadPool.QueueUserWorkItem( (tok)=>{
						subscriptionService.LoadOrganizations();
					});

					return view;
				}	
			);

			// EVENTS
			adapter.AddFragmentView ((i, v, b) => 
				{
					var view = i.Inflate (Resource.Layout.EventList, v, false);
					var eventListAdapter = new EventListAdapter (this, Resource.Id.lstCards, subscriptionService.EventList);
					eventListAdapter.RedirectToEventCards += LoadEventCards;

					var lstEvents = view.FindViewById<ListView> (Resource.Id.lstEvents);

					lstEvents.Adapter = eventListAdapter;

					OnEventListLoadedEventHandler callback = list => RunOnUiThread (() => eventListAdapter.UpdateData (list));

					subscriptionService.OnEventListLoaded -= callback;
					subscriptionService.OnEventListLoaded += callback;

					return view;
				}
			);

			// NOTIFICATIONS
//			var sharedCardListFragment = new SharedCardListFragment (subscriptionService.Notifications);
//			OnBusidexUserLoadedAssigned = true;	
//			subscriptionService.OnBusidexUserLoaded += delegate {
//				RunOnUiThread (() => sharedCardListFragment.UpdateData (subscriptionService.Notifications));
//			}; 
//
//			adapter.AddFragment (
//				sharedCardListFragment
//			);
			adapter.AddFragmentView ((i, v, b) => 
				{
					var view = i.Inflate (Resource.Layout.SharedCardList, v, false);
					var lstSharedCards = view.FindViewById<ListView>(Resource.Id.lstSharedCards);

					var sharedCardAdapter = new SharedCardListAdapter(this, Resource.Id.lstSharedCards, subscriptionService.Notifications);
					lstSharedCards.Adapter = sharedCardAdapter;

					OnNotificationsLoadedEventHandler callback = list => RunOnUiThread (() => sharedCardAdapter.UpdateData (list));

					subscriptionService.OnNotificationsLoaded -= callback;
					subscriptionService.OnNotificationsLoaded += callback;

					ThreadPool.QueueUserWorkItem( (tok)=>{
						subscriptionService.LoadNotifications();
					});


					return view;
				}
			);

			// PROFILE
			var profileFragment = new ProfileFragment (subscriptionService.CurrentUser);

			OnBusidexUserLoadedEventHandler profileCallback = user => RunOnUiThread (() => profileFragment.UpdateUser (user));

			subscriptionService.OnBusidexUserLoaded -= profileCallback;
			subscriptionService.OnBusidexUserLoaded += profileCallback;


			adapter.AddFragment (
				profileFragment
			);
		}

		void Init(){
			BaseApplicationResource.context = this;
			subscriptionService = subscriptionService ?? new UISubscriptionService ();
			var token = BaseApplicationResource.GetAuthCookie ();
			subscriptionService.AuthToken = token;
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			subscriptionService.Sync ();

			var tab = ActionBar.GetTabAt (0);
			ActionBar.SelectTab (tab);
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			string token = BaseApplicationResource.GetAuthCookie ();
			if(string.IsNullOrEmpty(token)){
				DoStartUp ();		
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{

			base.OnCreate(savedInstanceState);

			_tracker = _tracker ?? GoogleAnalytics.GetInstance (this).NewTracker (Busidex.Mobile.Resources.GOOGLE_ANALYTICS_KEY_ANDROID);

			Init ();

			RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Portrait;

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.SetDisplayShowHomeEnabled(false);
			ActionBar.SetDisplayShowTitleEnabled(true);

			ActionBar.Title = "My Busidex";

			pager = FindViewById<ViewPager>(Resource.Id.pager);
			var tabAdapter = new GenericFragmentPagerAdaptor(SupportFragmentManager);

			addTabs (tabAdapter);

			subscriptionService.Init ();

			pager.Adapter = tabAdapter;
			pager.SetOnPageChangeListener(new ViewPageListenerForActionBar(ActionBar));
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
					lstCards.ScrollTo(0, 0);
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
			notificationsTab.CustomView.FindViewById<TextView> (Resource.Id.txtNotificationCount).Text = subscriptionService.Notifications.Count.ToString();
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

		#region Startup Actions
		public void DoStartUp(){
			DoingLogin = true;
			subscriptionService.CurrentUser = null;
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new StartUpFragment ());
			ActionBar.Hide ();
		}

		public void ShowRegistration(){
			DoingRegistration = true;
			subscriptionService.CurrentUser = null;
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new ProfileFragment ());
			ActionBar.Hide ();
		}
		#endregion

		#region Login / Logout Actions
		bool DoingLogin = false;
		bool DoingRegistration = false;

		public void ShowLogin(){
			DoingLogin = true;
			subscriptionService.CurrentUser = null;
			DoLogin ();
		}
 
		public void DoLogout(){
			subscriptionService.Clear ();
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

			subscriptionService.AuthToken = BaseApplicationResource.GetAuthCookie ();

			subscriptionService.Sync ();
			DoingLogin = false;
			if(DoingRegistration){
				
				DoingRegistration = false;
			}
		}			
		#endregion

		#region Event Actions
		public void LoadEventCards(EventTag tag){

			if (!subscriptionService.EventCards.ContainsKey (tag.Text)) {
				subscriptionService.OnEventCardsLoaded -= goToEventCards;
				subscriptionService.OnEventCardsLoaded += goToEventCards;

				subscriptionService.loadEventCards (tag);
			}else{
				goToEventCards (tag, subscriptionService.EventCards [tag.Text]);
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
			foreach(var card in subscriptionService.OrganizationMembers [organization.OrganizationId]){
				var userCard = new UserCard {
					CardId = card.CardId,
					Card = card,
					Notes = string.Empty
				};
				orgMembers.Add (userCard);
			}

			var logoPath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var fragment = new OrganizationCardsFragment (orgMembers, logoPath);
			LoadFragment (fragment, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
		}

		public void LoadOrganizationReferrals(Organization organization){
			
			var logoPath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var fragment = new OrganizationCardsFragment (subscriptionService.OrganizationReferrals [organization.OrganizationId], logoPath);
			LoadFragment (fragment, Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation);
		}

		#endregion

		#region Card Actions
		public void ShowCard(CardDetailFragment fragment){

			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
			string token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Details, fragment.UserCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_DETAILS, 0);
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
		}

		public void ShowNotes(NotesFragment fragment){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
		}

		public void ShareCard(ShareCardFragment fragment){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (fragment);
			ActionBar.Hide ();
		}

		public void SendEmail(Intent intent, long id){

			var token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Email, id, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_EMAIL, 0);

			StartActivity(intent);
		}

		public void OpenBrowser(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Website, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_URL, 0);

			var browserIntent = Intent.CreateChooser(intent, "Open with");
			StartActivity (browserIntent);
		}

		public void OpenMap(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = BaseApplicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Map, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MAP, 0);

			StartActivity (intent);
		}

		static UserCard GetUserCardFromIntent(Intent intent){

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		public void AddToMyBusidex(UserCard userCard){
			subscriptionService.AddCardToMyBusidex (userCard);
		}

		public void RemoveFromMyBusidex(UserCard userCard){
			subscriptionService.RemoveCardFromMyBusidex (userCard);
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
			if (subscriptionService.CurrentUser != null) {
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

		#region Google Analytics
		public static void TrackAnalyticsEvent(string category, string label, string action, int value){

			var build = new HitBuilders.EventBuilder ()
				.SetCategory (category)
				.SetLabel (label)	
				.SetAction (action)
				.SetValue (value) 
				.Build ();
			var build2 = new Dictionary<string,string>();
			foreach (var key in build.Keys)
			{
				build2.Add (key, build [key]);
			}
			GATracker.Send (build2);
		}

		public static void TrackException(System.Exception ex){
			try{
				var build = new HitBuilders.ExceptionBuilder ()
					.SetDescription (ex.Message)
					.SetFatal (false) // This is useful for uncaught exceptions
					.Build();
				var build2 = new Dictionary<string,string>();
				foreach (var key in build.Keys)
				{
					build2.Add (key, build [key]);
				}
				GATracker.Send(build2);
			}catch{

			}
		}

		static Tracker _tracker;
		protected static Tracker GATracker { 
			get { 
				return _tracker; 
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
