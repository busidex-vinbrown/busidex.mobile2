
using System;
using System.Collections.Generic;
using System.Linq;

using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.IO;

namespace Busidex.Presentation.Droid.v2
{
	public class SearchFragment : GenericViewPagerFragment
	{
		SearchView SearchBar;
		ListView lstSearchResults;
		ProgressBar progressBar1;

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_SEARCH);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate (Resource.Layout.Search, container, false);

			SearchBar = view.FindViewById<SearchView> (Resource.Id.txtSearch);
			lstSearchResults = view.FindViewById<ListView> (Resource.Id.lstSearchResults);

			SearchBar.Iconified = false;

			SearchBar.QueryTextSubmit += delegate {
				DoSearch ();
			};

			progressBar1 = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);
			progressBar1.Visibility = ViewStates.Gone;

			SearchBar.QueryTextChange += (object sender, SearchView.QueryTextChangeEventArgs e) => {
				if (SearchBar.Query.Length == 0) {
					LoadSearchResults (new List<UserCard> ());
				}
			};

			SearchBar.SetQueryHint ("Search for a card");

			SearchBar.Touch += delegate {
				SearchBar.Focusable = true;
				SearchBar.RequestFocus ();
			};


			SearchBar.ClearFocus ();
			return view;
		}

		void LoadSearchResults (List<UserCard> cards)
		{

			var adapter = new UserCardAdapter (Activity, Resource.Id.lstCards, cards);

			adapter.Redirect += ((MainActivity)Activity).ShowCard;
			;
			adapter.ShowButtonPanel += ((MainActivity)Activity).ShowButtonPanel;

			adapter.ShowNotes = false;

			lstSearchResults.Adapter = adapter;

			lstSearchResults.RequestFocus ();
			SearchBar.ClearFocus ();

			progressBar1.Visibility = ViewStates.Gone;

			DismissKeyboard (SearchBar.WindowToken, Activity);
		}

		void DoSearch ()
		{

			progressBar1.Visibility = ViewStates.Visible;

			var ctrl = new SearchController ();
			ctrl.DoSearch (SearchBar.Query, UISubscriptionService.AuthToken).ContinueWith (response => {

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
									if (processed.Equals (total)) {
										Activity.RunOnUiThread (() => LoadSearchResults (cards));
									} 
								});
							} else {
								processed++;
								if (processed.Equals (total)) {
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

