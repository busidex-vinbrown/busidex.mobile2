
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class EventListFragment : GenericViewPagerFragment
	{
		static EventListAdapter eventListAdapter { get; set; }
		List<EventTag> Tags;
		EventTag SelectedEvent { get; set; }
		ListView lstEvents;

		public EventListFragment(){
			
		}

		public EventListFragment(List<EventTag> tags){
			Tags = tags;	
		}

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				//TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_LABEL_EVENT_LIST, Busidex.Mobile.Resources.GA_LABEL_LIST, 0);
			}
		}

		public void SetEventList(List<EventTag> tags){
			Tags = tags;
			eventListAdapter.UpdateData (tags);
			eventListAdapter.NotifyDataSetChanged ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.EventList, container, false);

			lstEvents = view.FindViewById<ListView> (Resource.Id.lstEvents);

			eventListAdapter = new EventListAdapter (Activity, Resource.Id.lstCards, Tags);

			eventListAdapter.RedirectToEventCards -= GoToEvent;
			eventListAdapter.RedirectToEventCards += GoToEvent;

			lstEvents.Adapter = eventListAdapter;

			return view;
		}

		void GoToEvent(EventTag tag){
			SelectedEvent = tag;
			((MainActivity)Activity).LoadEventCards (tag);
		}
	}
}

