
using System;
using System.Collections.Generic;
using System.Linq;

using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.IO;

namespace Busidex.Presentation.Android
{
	public class SearchFragment : BaseFragment
	{
		SearchView SearchBar;
		ListView lstSearchResults;
	
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.Search, container, false);

			SearchBar = view.FindViewById<SearchView> (Resource.Id.txtSearch);
			lstSearchResults = view.FindViewById<ListView> (Resource.Id.lstSearchResults);

			SearchBar.Iconified = false;

			SearchBar.QueryTextSubmit += delegate {
				DoSearch();
			};


			SearchBar.SetQueryHint ("Search for a card");

			SearchBar.Touch += delegate {
				SearchBar.Focusable = true;
				SearchBar.RequestFocus();
			};

			SearchBar.ClearFocus ();
			return view;
		}

		async void ClearFocus(){

			lstSearchResults.RequestFocus ();
			DismissKeyboard (SearchBar.WindowToken, Activity);
		}

		async void LoadSearchResults(List<UserCard> cards){

			var adapter = new UserCardAdapter (Activity, Resource.Id.lstCards, cards);

			adapter.Redirect += ShowCard;
			adapter.SendEmail += SendEmail;
			adapter.OpenBrowser += OpenBrowser;
			adapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			adapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			adapter.OpenMap += OpenMap;

			adapter.ShowNotes = false;

			lstSearchResults.Adapter = adapter;

			HideLoadingSpinner ();
			lstSearchResults.RequestFocus ();
			SearchBar.ClearFocus();

			DismissKeyboard (SearchBar.WindowToken, Activity);
		}

		void DoSearch(){

			string token = applicationResource.GetAuthCookie ();
			ShowLoadingSpinner ();

			var ctrl = new SearchController ();
			ctrl.DoSearch (SearchBar.Query, token).ContinueWith(response => {

				SearchResponse Search = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResponse> (response.Result);
				var cards = new List<UserCard> ();
				float total = Search.SearchModel.Results.Count;
				float processed = 0;

				if (!Search.SearchModel.Results.Any ()) {
					Activity.RunOnUiThread (() => LoadSearchResults (new List<UserCard> ()));
				} else {
					foreach (var item in Search.SearchModel.Results) {
						if (item != null) {

							var imageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + item.FrontFileName;
							var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.FrontFileName;

							var userCard = new UserCard ();
							userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
							userCard.Card = item;
							userCard.CardId = item.CardId;
							cards.Add (userCard);

							if (!File.Exists (Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.FrontFileName))) {
								Utils.DownloadImage (imageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
									processed++;
									if (processed.Equals(total)) {
										Activity.RunOnUiThread (() => LoadSearchResults (cards));
									} 
								});
							} else {
								processed++;
								if (processed.Equals(total)) {
									Activity.RunOnUiThread (() => LoadSearchResults (cards));
								}
							}
						}
					}
				}
			});
		}
	}
}

