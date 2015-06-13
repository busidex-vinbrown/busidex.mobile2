
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using System.Threading.Tasks;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Busidex Events")]			
	public class EventListActivity : BaseActivity
	{
		static EventListAdapter eventListAdapter { get; set; }
		List<EventTag> Tags { get; set; }
		EventTag SelectedEvent { get; set; }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.EventList);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_LABEL_EVENT_LIST, Busidex.Mobile.Resources.GA_LABEL_EVENT_LIST, 0);


			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.EVENT_LIST_FILE);
			LoadFromFile (fullFilePath);
		}

		public override void OnBackPressed ()
		{
			Redirect(new Intent(this, typeof(MainActivity)));
		}

		#region Get cached files
		static EventListResponse GetEventListFromFile(){

			var eventListFilePath = Path.Combine(Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.EVENT_LIST_FILE);
			if(File.Exists(eventListFilePath)){
				using(var eventListFile = File.OpenText (eventListFilePath)){
					var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (eventListFile.ReadToEnd());
					return responseObject;
				}

			}
			return null;
		}

		static EventSearchResponse GetEventCardsFromFile(EventTag tag){

			var eventCardsFilePath = Path.Combine(Busidex.Mobile.Resources.DocumentsPath, string.Format(Busidex.Mobile.Resources.EVENT_CARDS_FILE, tag.Text));
			if(File.Exists(eventCardsFilePath)){
				using(var eventCardsFile = File.OpenText (eventCardsFilePath)){
					string json = eventCardsFile.ReadToEnd ();
					if(!string.IsNullOrEmpty(json)){
						var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (json);
						return responseObject;
					}
				}
			}
			return null;
		}
		#endregion

		static void SetEventCardRefreshCookie(EventSearchResponse eventList, EventTag tag){

			eventList.LastRefreshDate = DateTime.Now;
			var json = Newtonsoft.Json.JsonConvert.SerializeObject (eventList);
			Utils.SaveResponse(json, string.Format(Busidex.Mobile.Resources.EVENT_CARDS_FILE, tag.Text));
		}

		protected override void ProcessFile(string data){

			var eventListResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (data);

			Tags = eventListResponse.Model;

			var lstEvents = FindViewById<ListView> (Resource.Id.lstEvents);

			eventListAdapter = new EventListAdapter (this, Resource.Id.lstCards, Tags);

			eventListAdapter.RedirectToEventCards += LoadEvent;

			lstEvents.Adapter = eventListAdapter;
		}

		static bool CheckEventSearchRefreshDate(EventTag tag){

			var eventList = GetEventCardsFromFile(tag);
			if (eventList == null){
				return false;
			}

			if(eventList.LastRefreshDate < DateTime.Now) {

				SetEventCardRefreshCookie (eventList, tag);
				return false;
			}
			return true;
		}

		void GoToEvent(EventTag tag){
			SelectedEvent = tag;
			var eventCardsIntent = new Intent (this, typeof(EventCardsActivity));

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(tag);
			eventCardsIntent.PutExtra ("Event", data);
			Redirect(eventCardsIntent);
		}

		// need this wrapper because a delegate can't be Task
		void LoadEvent(EventTag tag){
			LoadEventAsync (tag);
		}

		async Task<bool> LoadEventAsync(EventTag tag){

			var cookie = applicationResource.GetAuthCookie ();

			try {
				string fileName = string.Format(Busidex.Mobile.Resources.EVENT_CARDS_FILE, tag.Text);
				var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, fileName);
				if (File.Exists (fullFilePath) && CheckEventSearchRefreshDate(tag)) {
					GoToEvent(tag);
				} else {
					RunOnUiThread (() => ShowLoadingSpinner (tag.Description));

					var ctrl = new SearchController ();
					await ctrl.SearchBySystemTag (tag.Text, cookie).ContinueWith(async r => {

						if(!string.IsNullOrEmpty(r.Result)){
							EventSearchResponse eventSearchResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (r.Result);

							var ownedCards = eventSearchResponse.SearchModel.Results.Where(c => c.OwnerId.HasValue).ToList(); 

							Utils.SaveResponse(r.Result, fileName);
							SetEventCardRefreshCookie(eventSearchResponse, tag);

							var idx = 0;
							var total = ownedCards.Count;
							RunOnUiThread (() => {
								HideLoadingSpinner();

								ShowLoadingSpinner (
									Resources.GetString (Resource.String.Global_LoadingEvent), 
									ProgressDialogStyle.Horizontal, 
									eventSearchResponse.SearchModel.Results.Count);
							});


							foreach (var card in ownedCards) {
								if (card != null) {

									var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.FrontFileName;
									var bImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.BackFileName;
									var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
									var bName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;

									if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fName)){// || force) {
										await Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
											UpdateLoadingSpinner (idx, total);
										});
									} else{
										UpdateLoadingSpinner (idx, total);
									}

									if ((!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + bName)/* || force*/) && card.BackFileId.ToString () != Busidex.Mobile.Resources.EMPTY_CARD_ID) {
										await Utils.DownloadImage (bImageUrl, Busidex.Mobile.Resources.DocumentsPath, bName).ContinueWith (t => {
										});
									}
									idx++;
								}
							}

							RunOnUiThread (() => {
								HideLoadingSpinner();
								GoToEvent (tag);
							});

						}
					});
				}

			}
			catch(Exception ex){

			}

			return true;
		}
	}
}

