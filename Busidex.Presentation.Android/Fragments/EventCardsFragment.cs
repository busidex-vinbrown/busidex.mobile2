using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.Widget;
using Android.Views;
using Android.OS;
using Busidex.Mobile;
using System.Linq;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Presentation.Android
{
	public class EventCardsFragment : BaseFragment
	{
		List<UserCard> Cards { get; set; }
		static UserCardAdapter EventCardsAdapter { get; set; }
		static SearchView txtFilterEventCards { get; set; }
		EventTag EventTag { get; set; }
		//Activity context { get; set; }

		public EventCardsFragment (EventTag tag)
		{
			EventTag = tag;

		}

		public override void OnResume ()
		{
			base.OnResume ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, string.Format(Busidex.Mobile.Resources.EVENT_CARDS_FILE, EventTag.Text));
			ThreadPool.QueueUserWorkItem( o =>  LoadFromFile(fullFilePath));
			Activity.Title = EventTag.Text;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			//base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.EventCards, container, false);

			return view;
		}

		static void DoFilter(string filter){
			if(string.IsNullOrEmpty(filter)){
				EventCardsAdapter.Filter.InvokeFilter ("");
			}else{
				EventCardsAdapter.Filter.InvokeFilter(filter);
			}
		}

		protected override async Task<bool> ProcessFile(string data){

			var eventSearchResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (data);

			Cards = new List<UserCard> ();

			foreach (var card in eventSearchResponse.SearchModel.Results.Where(c => c.OwnerId.HasValue).ToList()) {
				if (card != null) {

					var userCard = new UserCard (card);

					userCard.ExistsInMyBusidex = card.ExistsInMyBusidex;
					userCard.Card = card;
					userCard.CardId = card.CardId;

					Cards.Add (userCard);
				}
			}

			Activity.RunOnUiThread (() => {
				txtFilterEventCards = Activity.FindViewById<SearchView> (Resource.Id.txtFilterEventCards);

				var lstEventCards = Activity.FindViewById<ListView> (Resource.Id.lstEventCards);
				EventCardsAdapter = new UserCardAdapter (Activity, Resource.Id.lstCards, Cards);

				EventCardsAdapter.Redirect += ShowCard;
				EventCardsAdapter.SendEmail += SendEmail;
				EventCardsAdapter.OpenBrowser += OpenBrowser;
				EventCardsAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
				EventCardsAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
				EventCardsAdapter.OpenMap += OpenMap;

				EventCardsAdapter.ShowNotes = false;

				lstEventCards.Adapter = EventCardsAdapter;

				txtFilterEventCards.QueryTextChange += delegate {
					DoFilter (txtFilterEventCards.Query);
				};

				txtFilterEventCards.Iconified = false;
				lstEventCards.RequestFocus (FocusSearchDirection.Down);
				//DismissKeyboard (txtFilterEventCards.WindowToken);

				txtFilterEventCards.Touch += delegate {
					txtFilterEventCards.Focusable = true;
					txtFilterEventCards.RequestFocus ();
				};
			});

			return true;
		}
	}
}

