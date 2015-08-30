using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	[Activity(Label = "Busidex", MainLauncher = true, Icon = "@drawable/icon",
		ConfigurationChanges =  global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]
	public class MainActivity : FragmentActivity
	{
		ViewPager pager;
		UISubscriptionService subscriptionService;

		void addTabs(GenericFragmentPagerAdaptor adapter){

			// HOME
//			adapter.AddFragmentView((i, v, b) =>
//				{
//					var view = i.Inflate(Resource.Layout.Home, v, false);
//					return view;
//				}
//			);
			adapter.AddFragment (new MainFragment());

			// SEARCH
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = i.Inflate(Resource.Layout.Search, v, false);
					return view;
				}
			);

			// MY BUSIDEX
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = i.Inflate(Resource.Layout.MyBusidex, v, false);

					var MyBusidexAdapter = new UserCardAdapter (this, Resource.Id.lstCards, subscriptionService.UserCards);

//					MyBusidexAdapter.Redirect += ShowCard;
//					MyBusidexAdapter.SendEmail += SendEmail;
//					MyBusidexAdapter.OpenBrowser += OpenBrowser;
//					MyBusidexAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
//					MyBusidexAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
//					MyBusidexAdapter.OpenMap += OpenMap;

					MyBusidexAdapter.ShowNotes = true;

					RunOnUiThread (() => {

						var lstCards = view.FindViewById<ListView> (Resource.Id.lstCards);
						var txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
						var lblNoCardsMessage = view.FindViewById<TextView> (Resource.Id.lblNoCardsMessage);

						var state = lstCards.OnSaveInstanceState ();

						lblNoCardsMessage.Text = GetString (Resource.String.MyBusidex_NoCards);

						lblNoCardsMessage.Visibility = subscriptionService.UserCards.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
						txtFilter.Visibility = subscriptionService.UserCards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

						lstCards.Adapter = MyBusidexAdapter;

						lstCards.OnRestoreInstanceState (state);

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

					});

					return view;
				}
			);
			//adapter.AddFragment (new MyBusidexFragment());

			// MY ORGANIZATIONS
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = i.Inflate(Resource.Layout.MyOrganizations, v, false);
					return view;
				}
			);

			// EVENTS
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = i.Inflate(Resource.Layout.EventList, v, false);
					return view;
				}
			);

			// PROFILE
			adapter.AddFragmentView((i, v, b) =>
				{
					var view = i.Inflate(Resource.Layout.Profile, v, false);
					return view;
				}
			);
		}

		public void SwitchTabs(int position){
			pager.SetCurrentItem (position, true);
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			subscriptionService = new UISubscriptionService ();
			subscriptionService.reset ("NzM=");

		}

		protected override void OnCreate(Bundle bundle)
		{

			base.OnCreate(bundle);

			this.RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Portrait;

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			ActionBar.SetDisplayShowHomeEnabled(false);
			ActionBar.SetDisplayShowTitleEnabled(false);

			pager = FindViewById<ViewPager>(Resource.Id.pager);
			var adapter = new GenericFragmentPagerAdaptor(SupportFragmentManager);

			addTabs (adapter);

			pager.Adapter = adapter;
			pager.SetOnPageChangeListener(new ViewPageListenerForActionBar(ActionBar));

			var homeTab = pager.GetViewPageTab (ActionBar, "");
			homeTab.SetCustomView (Resource.Layout.tab);
			homeTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.Icon);

			var searchTab = pager.GetViewPageTab (ActionBar, "");
			searchTab.SetCustomView (Resource.Layout.tab);
			searchTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.SearchIcon);

			var myBusidexTab = pager.GetViewPageTab (ActionBar, "");
			myBusidexTab.SetCustomView (Resource.Layout.tab);
			myBusidexTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.MyBusidexIcon);

			var myOrganizationsTab = pager.GetViewPageTab (ActionBar, "");
			myOrganizationsTab.SetCustomView (Resource.Layout.tab);
			myOrganizationsTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.OrganizationsIcon);

			var eventsTab = pager.GetViewPageTab (ActionBar, "");
			eventsTab.SetCustomView (Resource.Layout.tab);
			eventsTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.EventIcon);

			var profileTab = pager.GetViewPageTab (ActionBar, "");
			profileTab.SetCustomView (Resource.Layout.tab);
			profileTab.CustomView.FindViewById<ImageView>(Resource.Id.imgTabIcon).SetImageResource(Resource.Drawable.settings);

			ActionBar.AddTab(homeTab);
			ActionBar.AddTab(searchTab);
			ActionBar.AddTab(myBusidexTab);
			ActionBar.AddTab(myOrganizationsTab);
			ActionBar.AddTab(eventsTab);
			ActionBar.AddTab(profileTab);
		}
	}
}
