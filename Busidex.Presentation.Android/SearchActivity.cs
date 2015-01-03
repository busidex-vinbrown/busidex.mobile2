

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.IO;
using System.Linq;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Search")]			
	public class SearchActivity : BaseCardActivity
	{
		SearchView SearchBar;
		ListView lstSearchResults;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Search);

			SearchBar = FindViewById<SearchView> (Resource.Id.txtSearch);
			lstSearchResults = FindViewById<ListView> (Resource.Id.lstSearchResults);

			SearchBar.QueryTextSubmit += delegate {
				DoSearch();
			};

			SearchBar.Touch += delegate {
				SearchBar.Focusable = true;
				SearchBar.RequestFocus();
			};
		}

		async void ClearFocus(){

			lstSearchResults.RequestFocus ();
			await DismissKeyboard (SearchBar.WindowToken);
		}

		async void LoadSearchResults(List<UserCard> cards){

			var adapter = new UserCardAdapter (this, Resource.Id.lstCards, cards);

			adapter.Redirect += ShowCard;
			adapter.SendEmail += SendEmail;
			adapter.OpenBrowser += OpenBrowser;
			adapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			adapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			adapter.OpenMap += OpenMap;

			adapter.ShowNotes = true;

			lstSearchResults.Adapter = adapter;

			HideLoadingSpinner ();
			lstSearchResults.RequestFocus ();
			SearchBar.Focusable = false;

			await DismissKeyboard (SearchBar.WindowToken);
		}

		void DoSearch(){
		
			string token = GetAuthCookie ();
			ShowLoadingSpinner ();

			var ctrl = new SearchController ();
			ctrl.DoSearch (SearchBar.Query, token).ContinueWith(response => {

				SearchResponse Search = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResponse> (response.Result);
				var cards = new List<UserCard> ();
				float total = Search.SearchModel.Results.Count;
				float processed = 0;

				if (!Search.SearchModel.Results.Any ()) {
					RunOnUiThread (() => LoadSearchResults (new List<UserCard> ()));
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
										RunOnUiThread (() => LoadSearchResults (cards));
									} 
								});
							} else {
								processed++;
								if (processed.Equals(total)) {
									RunOnUiThread (() => LoadSearchResults (cards));
								}
							}
						}
					}
				}
			});
		}
	}
}

