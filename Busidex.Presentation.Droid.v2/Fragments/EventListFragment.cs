
using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using System.Threading.Tasks;
using System.Threading;

namespace Busidex.Presentation.Droid.v2
{
	public class EventListFragment : GenericViewPagerFragment
	{
		static EventListAdapter eventListAdapter { get; set; }
		List<EventTag> Tags;
		EventTag SelectedEvent { get; set; }

		public EventListFragment(){
			
		}

		public EventListFragment(List<EventTag> tags){
			Tags = tags;	
		}

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				LoadUI ();

				//TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_LABEL_EVENT_LIST, Busidex.Mobile.Resources.GA_LABEL_LIST, 0);
			}
		}

		public void SetEventList(List<EventTag> tags){
			Tags = tags;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.EventList, container, false);

			return view;
		}

		static void SetEventCardRefreshCookie(EventSearchResponse eventList, EventTag tag){

			eventList.LastRefreshDate = DateTime.Now;
			var json = Newtonsoft.Json.JsonConvert.SerializeObject (eventList);
			Utils.SaveResponse(json, string.Format(Busidex.Mobile.Resources.EVENT_CARDS_FILE, tag.Text));
		}

		private void LoadUI(){

			var lstEvents = Activity.FindViewById<ListView> (Resource.Id.lstEvents);

			eventListAdapter = new EventListAdapter (Activity, Resource.Id.lstCards, Tags);

			//eventListAdapter.RedirectToEventCards += LoadEvent;

			lstEvents.Adapter = eventListAdapter;

			//HideLoadingSpinner ();

			//((SplashActivity)Activity).fragments[GetType().Name] = this;
		}


		void GoToEvent(EventTag tag){
			SelectedEvent = tag;
			//Redirect(new EventCardsFragment(tag));
		}
	}
}

