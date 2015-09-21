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

		public override void OnResume ()
		{
			base.OnResume ();

			//LoadUI ();

			//Activity.Title = EventTag.Text;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.EventCards, container, false);

			var txtFilterEventCards = view.FindViewById<SearchView> (Resource.Id.txtFilterEventCards);

			var lstEventCards = view.FindViewById<ListView> (Resource.Id.lstEventCards);
			var adapter = new UserCardAdapter (Activity, Resource.Id.lstCards, Cards);

			adapter.Redirect += ShowCard;
			adapter.SendEmail += SendEmail;
			adapter.OpenBrowser += OpenBrowser;
			adapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			adapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			adapter.OpenMap += OpenMap;
			adapter.ShowButtonPanel += ((MainActivity)Activity).ShowButtonPanel;

			adapter.ShowNotes = false;

			lstEventCards.Adapter = adapter;

			txtFilterEventCards.QueryTextChange += delegate {
				//DoFilter (txtFilterEventCards.Query);
			};

			txtFilterEventCards.Iconified = false;
			txtFilterEventCards.ClearFocus ();

			lstEventCards.RequestFocus (FocusSearchDirection.Down);
			DismissKeyboard (txtFilterEventCards.WindowToken, Activity);

			txtFilterEventCards.Touch += delegate {
				txtFilterEventCards.Focusable = true;
				txtFilterEventCards.RequestFocus ();
			};

			var btnClose = view.FindViewById<ImageButton> (Resource.Id.btnClose);
			btnClose.Click += delegate {
				var panel = new EventListFragment();

				((MainActivity)Activity).UnloadFragment(panel);
			};

			return view;
		}

//		static void DoFilter(string filter){
//			if(string.IsNullOrEmpty(filter)){
//				EventCardsAdapter.Filter.InvokeFilter ("");
//			}else{
//				EventCardsAdapter.Filter.InvokeFilter(filter);
//			}
//		}

		void LoadUI(){
			

		}
	}
}

