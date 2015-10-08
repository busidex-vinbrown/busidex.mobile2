using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.Widget;
using Android.Views;
using Android.OS;

namespace Busidex.Presentation.Droid.v2
{
	public class EventCardsFragment : GenericViewPagerFragment
	{
		readonly List<UserCard> Cards;
		//static UserCardAdapter EventCardsAdapter { get; set; }
		//static SearchView txtFilterEventCards { get; set; }
		readonly EventTag EventTag;

		public EventCardsFragment (EventTag tag, List<UserCard> cards)
		{
			EventTag = tag;
			Cards = cards;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.EventCards, container, false);

			//var txtFilterEventCards = view.FindViewById<SearchView> (Resource.Id.txtFilterEventCards);

			var lblEventDescription = view.FindViewById<TextView> (Resource.Id.lblEventDescription);
			lblEventDescription.Text = EventTag.Description;

			var progressBar1 = view.FindViewById<ProgressBar>(Resource.Id.progressBar1);
			if(progressBar1 == null){
				return view; 
			}

			progressBar1.Visibility = ViewStates.Gone;

			var lblPrivateEventMessage = view.FindViewById<TextView> (Resource.Id.lblPrivateEventMessage);

			var lstEventCards = view.FindViewById<OverscrollListView> (Resource.Id.lstEventCards);
			lblPrivateEventMessage.Visibility = Cards.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
			lstEventCards.Visibility = Cards.Count > 0 ? ViewStates.Visible : ViewStates.Gone;

			var adapter = new UserCardAdapter (Activity, Resource.Id.lstCards, Cards);

			adapter.Redirect += ((MainActivity)Activity).ShowCard;
			adapter.ShowButtonPanel += ((MainActivity)Activity).ShowButtonPanel;

			adapter.ShowNotes = false;

			lstEventCards.Adapter = adapter;

			int accumulatedDeltaY = 0;
			lstEventCards.OverScrolled += deltaY=> {

				accumulatedDeltaY += -deltaY;
				if(accumulatedDeltaY > 1000){
					lstEventCards.Visibility = ViewStates.Gone;
					progressBar1.Visibility = ViewStates.Visible;
					((MainActivity)Activity).ReloadEventCards(EventTag).ContinueWith((r) => {
						((MainActivity)Activity).RunOnUiThread(() =>{
							progressBar1.Visibility = ViewStates.Gone;
							lstEventCards.Visibility = ViewStates.Visible;
						});
					});
					BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MY_BUSIDEX_REFRESHED, 0);
				}
			};

			lstEventCards.Scroll+= delegate {
				if( lstEventCards.CanScrollVertically(-1)){
					accumulatedDeltaY = 0;
				}
			};

			var btnClose = view.FindViewById<ImageButton> (Resource.Id.btnClose);
			btnClose.Click += delegate {

				((MainActivity)Activity).UnloadFragment();
			};

			return view;
		}
	}
}

