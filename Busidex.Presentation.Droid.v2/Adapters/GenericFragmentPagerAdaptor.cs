using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Widget;

namespace Busidex.Presentation.Droid.v2
{

	public class GenericFragmentPagerAdaptor : FragmentStatePagerAdapter
	{
		readonly List<Android.Support.V4.App.Fragment> _fragmentList = new List<Android.Support.V4.App.Fragment> ();

		public GenericFragmentPagerAdaptor (Android.Support.V4.App.FragmentManager fm)
			: base (fm)
		{
			
		}

		public  void Clear ()
		{
			_fragmentList.RemoveAll (f => true);
			NotifyDataSetChanged ();
		}

		public override void DestroyItem (View container, int position, Java.Lang.Object objectValue)
		{
			((ViewPager)container).RemoveView ((View)objectValue);
			NotifyDataSetChanged ();
		}

		public override int Count {
			get { return _fragmentList == null ? 0 : _fragmentList.Count; }
		}

		public override Android.Support.V4.App.Fragment GetItem (int position)
		{
			return _fragmentList [position];
		}

		public void AddFragment (GenericViewPagerFragment fragment)
		{
			_fragmentList.Add (fragment);
			NotifyDataSetChanged ();
		}

		public void AddFragmentView (Func<LayoutInflater, ViewGroup, Bundle, View> view)
		{
			_fragmentList.Add (new GenericViewPagerFragment (view));
			NotifyDataSetChanged ();
		}
	}

	public class ViewPageListenerForActionBar : ViewPager.SimpleOnPageChangeListener
	{
		struct tabData
		{

			public tabData (string title, int iconActive, int iconInactive)
			{
				Title = title;
				IconActive = iconActive;
				IconInactive = iconInactive;
			}

			public string Title { get; set; }

			public int IconActive { get; set; }

			public int IconInactive { get; set; }
		}

		readonly List<tabData> tabs;

		readonly ActionBar _bar;

		public ViewPageListenerForActionBar (ActionBar bar)
		{
			_bar = bar;

			tabs = new List<tabData> ();

			tabs.Add (new tabData ("Home", Resource.Drawable.home, Resource.Drawable.home));
			tabs.Add (new tabData ("Search", Resource.Drawable.SearchIcon, Resource.Drawable.SearchIconDisabled));
			tabs.Add (new tabData ("My Busidex", Resource.Drawable.MyBusidexIcon, Resource.Drawable.MyBusidexIconDisabled));
			tabs.Add (new tabData ("Organizations", Resource.Drawable.OrganizationsIcon, Resource.Drawable.OrganizationsIconDisabled));
			tabs.Add (new tabData ("Events", Resource.Drawable.EventIcon, Resource.Drawable.EventIconDisabled));
			tabs.Add (new tabData ("Referrals Received", Resource.Drawable.notification, Resource.Drawable.NotificationDisabled));
			tabs.Add (new tabData ("Profile", Resource.Drawable.settings, Resource.Drawable.settingsDisabled));
		
		}

		public override void OnPageSelected (int position)
		{
			_bar.SetSelectedNavigationItem (position);

			for (var i = 0; i < _bar.TabCount; i++) {
				var tab = _bar.GetTabAt (i);
				tab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (tabs [i].IconInactive);	
				tab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = .3f;
			}

			var selectedTab = _bar.GetTabAt (position);
			selectedTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).SetImageResource (tabs [position].IconActive);	
			selectedTab.CustomView.FindViewById<ImageView> (Resource.Id.imgTabIcon).Alpha = 1f;

			_bar.Title = tabs [position].Title;

			switch (position) {
			case 0:
				{
					BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_HOME);
					break;
				}
			case 1:
				{
					BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_SEARCH);
					break;
				}
			case 2:
				{
					BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_MY_BUSIDEX);
					break;
				}
			case 3:
				{
					BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_ORGANIZATIONS);
					break;
				}
			case 4:
				{
					BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_EVENTS);
					break;
				}
			case 5:
				{
					BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_REFERRALS);
					break;
				}
			case 6:
				{
					BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_PROFILE);
					break;
				}
			}

		}
	}

	public static class ViewPagerExtensions
	{
		public static ActionBar.Tab GetViewPageTab (this ViewPager viewPager, ActionBar actionBar, string name)
		{
			var tab = actionBar.NewTab ();
			tab.SetText (name);
			tab.TabSelected += (o, e) => {
				viewPager.SetCurrentItem (actionBar.SelectedNavigationIndex, false);
				if (actionBar.SelectedNavigationIndex == 0) {
					actionBar.Hide ();
				} else {
					actionBar.Show ();
				}
			};

			return tab;
		}

		//		public static void UpdateNotificationCount (ActionBar actionBar, int count)
		//		{
		//
		//			const int NOTIFICATION_TAB = 5;
		//			if (actionBar.TabCount > 0) {
		//				var selectedTab = actionBar.GetTabAt (NOTIFICATION_TAB);
		//				var txtNotificationCount = selectedTab.CustomView.FindViewById<TextView> (Resource.Id.txtNotificationCount);
		//				if (count > 0) {
		//					txtNotificationCount.Visibility = ViewStates.Visible;
		//					txtNotificationCount.Text = count.ToString ();
		//				} else {
		//					txtNotificationCount.Visibility = ViewStates.Gone;
		//				}
		//			}
		//		}
	}

}