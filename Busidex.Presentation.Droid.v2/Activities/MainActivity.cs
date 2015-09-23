﻿using System;
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

namespace Busidex.Presentation.Droid.v2
{
	[Activity(Label = "Busidex", MainLauncher = true, ConfigurationChanges =  global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]
	public class MainActivity : FragmentActivity
	{
		ViewPager pager;
		UISubscriptionService subscriptionService;
		string authToken = string.Empty;

		public MainActivity(){


		}

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

					var myBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, subscriptionService.UserCards);

					subscriptionService.OnMyBusidexLoaded += delegate {
						RunOnUiThread(()=> {
							myBusidexAdapter.UpdateData(subscriptionService.UserCards);
							myBusidexAdapter.NotifyDataSetChanged();
						} );
					}; 

					myBusidexAdapter.Redirect += ShowCard;
					myBusidexAdapter.ShowButtonPanel += ShowButtonPanel;
					myBusidexAdapter.ShowNotes = true;

					var lstCards = view.FindViewById<ListView> (Resource.Id.lstCards);
					lstCards.Adapter = myBusidexAdapter;

					var state = lstCards.OnSaveInstanceState ();
					if(state != null){
						lstCards.OnRestoreInstanceState (state);
					}

					var txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
					txtFilter.Visibility = subscriptionService.UserCards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

					txtFilter.QueryTextChange += delegate {
						//DoFilter (txtFilter.Query);
					};

					txtFilter.Iconified = false;
					txtFilter.ClearFocus();

					lstCards.RequestFocus (FocusSearchDirection.Down);

					txtFilter.Touch += delegate {
						txtFilter.Focusable = true;
						txtFilter.RequestFocus ();
					};

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

					var lstOrganizations = view.FindViewById<ListView> (Resource.Id.lstOrganizations);

					lstOrganizations.Adapter = orgAdapter;

					subscriptionService.OnMyOrganizationsLoaded += delegate {
						RunOnUiThread(()=> {
							orgAdapter.UpdateData(subscriptionService.OrganizationList);
							orgAdapter.NotifyDataSetChanged();
						} );
					}; 

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

					subscriptionService.OnEventListLoaded += delegate {
						RunOnUiThread (() => {
							eventListAdapter.UpdateData(subscriptionService.EventList);
							eventListAdapter.NotifyDataSetChanged();
						});
					};

					return view;
				}
			);

			// PROFILE
			adapter.AddFragment (
				new ProfileFragment (subscriptionService.CurrentUser)
			);
		}

		void Init(){
			BaseApplicationResource.context = this;
			subscriptionService = subscriptionService ?? new UISubscriptionService ();
			var token = BaseApplicationResource.GetAuthCookie ();
			subscriptionService.AuthToken = token;
			subscriptionService.Init ();


		}

		protected override void OnStart ()
		{
			base.OnStart ();

			string token = BaseApplicationResource.GetAuthCookie ();
			if(string.IsNullOrEmpty(token)){
				DoLogin ();		
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
			ActionBar.SetDisplayShowTitleEnabled(false);

			pager = FindViewById<ViewPager>(Resource.Id.pager);
			var tabAdapter = new GenericFragmentPagerAdaptor(SupportFragmentManager);

			addTabs (tabAdapter);

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
				var lstCards = (ListView)tabAdapter.GetItem(0).Activity.FindViewById(Resource.Id.lstCards);
				lstCards.ScrollTo(0, 0);
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
			ActionBar.AddTab(profileTab);
			//ActionBar.AddTab (optionsTab);
		}

		#region Login Actions

		public void ShowLogin(){
			subscriptionService.CurrentUser = null;
			DoLogin ();
		}

		void DoLogin(){
			FindViewById (Resource.Id.fragment_holder).Visibility = ViewStates.Visible;
			LoadFragment (new LoginFragment ());
			ActionBar.Hide ();
		}

		public void LoginComplete(){
			subscriptionService.AuthToken = BaseApplicationResource.GetAuthCookie ();
			subscriptionService.reset ();
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

			//SupportFragmentManager.PopBackStack ();
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

		public static void TrackException(Exception ex){
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
	}
}
