
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using System.Threading;

namespace Busidex.Presentation.Droid.v2
{
	public class HomeFragment : GenericViewPagerFragment
	{
		OnNotificationsLoadedEventHandler callback;

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_HOME);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			var view = inflater.Inflate (Resource.Layout.Home, container, false);

			const int TAB_SEARCH = 1;
			const int TAB_MY_BUSIDEX = 2;
			const int TAB_ORGANIZATIONS = 3;
			const int TAB_EVENTS = 4;
			const int TAB_NOTIFICATIONS = 5;
			const int TAB_PROFILE = 6;

			var layout = view.FindViewById<RelativeLayout> (Resource.Id.homeLayout);
			layout.Visibility = ViewStates.Visible;

			// SEARCH BUTTON
			var btnSearch = view.FindViewById<Button> (Resource.Id.btnSearch);
			btnSearch.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_SEARCH);
			};
			var imgSearch = view.FindViewById<ImageView> (Resource.Id.imgSearchIcon);
			imgSearch.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_SEARCH);
			};

			//MY BUSIDEX BUTTON
			var btnMyBusidex = view.FindViewById<Button> (Resource.Id.btnMyBusidex);
			btnMyBusidex.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_MY_BUSIDEX);
			};
			var imgMyBusidex = view.FindViewById<ImageView> (Resource.Id.imgBusidexIcon);
			imgMyBusidex.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_MY_BUSIDEX);
			};

			// MY ORGANIZATIONS BUTTON
			var btnMyOrganizations = view.FindViewById<Button> (Resource.Id.btnMyOrganizations);
			btnMyOrganizations.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_ORGANIZATIONS);
			};
			var imgOrganizations = view.FindViewById<ImageView> (Resource.Id.imgOrgIcon);
			imgOrganizations.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_ORGANIZATIONS);
			};

			// EVENTS BUTTON
			var btnEvents = view.FindViewById<Button> (Resource.Id.btnEvents);
			btnEvents.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_EVENTS);
			};
			var imgEvents = view.FindViewById<ImageView> (Resource.Id.imgEventIcon);
			imgEvents.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_EVENTS);
			};

			// SHARE BUTTON
			var btnShare = view.FindViewById<Button> (Resource.Id.btnShare);
			btnShare.Click += delegate {
				((MainActivity)Activity).OpenShare ();
			};
			var imgShare = view.FindViewById<ImageView> (Resource.Id.imgShareIcon);
			imgShare.Click += delegate {
				((MainActivity)Activity).OpenShare ();
			};

			// SHARED CARDS NOTIFICATION
			var btnSharedCardsNotification = view.FindViewById<ImageView> (Resource.Id.btnSharedCardsNotification);
			btnSharedCardsNotification.Visibility = ViewStates.Gone;

			btnSharedCardsNotification.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_NOTIFICATIONS);
			};

			// NOTIFICATION COUNT
			var txtNotificationCount = view.FindViewById<TextView> (Resource.Id.txtNotificationCount);
			txtNotificationCount.Visibility = ViewStates.Gone;

			// SETTINGS BUTTON
			var btnSettingsHome = view.FindViewById<ImageButton> (Resource.Id.btnSettingsHome);
			btnSettingsHome.Click += delegate {
				((MainActivity)Activity).SetTab (TAB_PROFILE);	
			};

			if (Activity != null) {
				callback = list => Activity.RunOnUiThread (() => {

					if (Activity != null) {
						var count = list == null ? 0 : list.Count;

						btnSharedCardsNotification.Visibility = count == 0 ? ViewStates.Gone : ViewStates.Visible;
						txtNotificationCount.Visibility = count == 0 ? ViewStates.Gone : ViewStates.Visible;
						txtNotificationCount.Text = count.ToString ();
					}

				});
			}
			UISubscriptionService.OnNotificationsLoaded -= callback;
			UISubscriptionService.OnNotificationsLoaded += callback;

			ThreadPool.QueueUserWorkItem (tok => UISubscriptionService.LoadNotifications ());
			view.Visibility = ViewStates.Visible;
			return view;
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			UISubscriptionService.OnNotificationsLoaded -= callback;
		}
	}
}

