
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

			var layout = view.FindViewById<RelativeLayout> (Resource.Id.homeLayout);
			layout.Visibility = ViewStates.Visible;

			//MY BUSIDEX BUTTON
			var btnMyBusidex = view.FindViewById<Button> (Resource.Id.btnMyBusidex);
			btnMyBusidex.Click += delegate {
				((MainActivity)Activity).SetTab (1);
			};
			var imgMyBusidex = view.FindViewById<ImageView> (Resource.Id.imgBusidexIcon);
			imgMyBusidex.Click += delegate {
				((MainActivity)Activity).SetTab (1);
			};

			// SEARCH BUTTON
			var btnSearch = view.FindViewById<Button> (Resource.Id.btnSearch);
			btnSearch.Click += delegate {
				((MainActivity)Activity).SetTab (2);
			};
			var imgSearch = view.FindViewById<ImageView> (Resource.Id.imgSearchIcon);
			imgSearch.Click += delegate {
				((MainActivity)Activity).SetTab (2);
			};

			// MY ORGANIZATIONS BUTTON
			var btnMyOrganizations = view.FindViewById<Button> (Resource.Id.btnMyOrganizations);
			btnMyOrganizations.Click += delegate {
				((MainActivity)Activity).SetTab (3);
			};
			var imgOrganizations = view.FindViewById<ImageView> (Resource.Id.imgOrgIcon);
			imgOrganizations.Click += delegate {
				((MainActivity)Activity).SetTab (3);
			};

			// EVENTS BUTTON
			var btnEvents = view.FindViewById<Button> (Resource.Id.btnEvents);
			btnEvents.Click += delegate {
				((MainActivity)Activity).SetTab (4);
			};
			var imgEvents = view.FindViewById<ImageView> (Resource.Id.imgEventIcon);
			imgEvents.Click += delegate {
				((MainActivity)Activity).SetTab (4);
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
			btnSharedCardsNotification.Click += delegate {
				((MainActivity)Activity).SetTab (5);
			};

			// SETTINGS BUTTON
			var btnSettingsHome = view.FindViewById<ImageButton> (Resource.Id.btnSettingsHome);
			btnSettingsHome.Click += delegate {
				((MainActivity)Activity).SetTab (6);	
			};

			if (Activity != null) {
				callback = list => Activity.RunOnUiThread (() => {

					var count = list == null ? 0 : list.Count;
					ViewPagerExtensions.UpdateNotificationCount (((MainActivity)Activity).ActionBar, count);

					var txtNotificationCount = view.FindViewById<TextView> (Resource.Id.txtNotificationCount);

					btnSharedCardsNotification.Visibility = count == 0 ? ViewStates.Gone : ViewStates.Visible;
					txtNotificationCount.Visibility = count == 0 ? ViewStates.Gone : ViewStates.Visible;
					txtNotificationCount.Text = count.ToString ();

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

