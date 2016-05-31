
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class EventListFragment : GenericViewPagerFragment
	{
		OnEventListLoadedEventHandler callback;
		View view;
		ListView lstEvents;
		EventListAdapter eventListAdapter;

		public override void OnResume ()
		{
			base.OnResume ();
			if (callback == null) {
				callback = list => Activity.RunOnUiThread (() => eventListAdapter.UpdateData (list));
			}
			UISubscriptionService.OnEventListLoaded -= callback;
			UISubscriptionService.OnEventListLoaded += callback;

			UISubscriptionService.LoadEventList ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_EVENTS);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.EventList, container, false);

			eventListAdapter = new EventListAdapter (Activity, Resource.Id.lstCards, UISubscriptionService.EventList);
			eventListAdapter.RedirectToEventCards += ((MainActivity)Activity).LoadEventCards;

			lstEvents = view.FindViewById<ListView> (Resource.Id.lstEvents);

			lstEvents.Adapter = eventListAdapter;

			UISubscriptionService.LoadEventList ();

			return view;
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			UISubscriptionService.OnEventListLoaded -= callback;
		}
	}
}

