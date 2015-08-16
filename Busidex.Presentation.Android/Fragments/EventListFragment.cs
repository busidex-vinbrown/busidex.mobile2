
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

namespace Busidex.Presentation.Android
{
	public class EventListFragment : BaseFragment
	{
		static EventListAdapter eventListAdapter { get; set; }
		List<EventTag> Tags { get; set; }
		EventTag SelectedEvent { get; set; }


		public override void OnResume ()
		{
			base.OnResume ();

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			var view = inflater.Inflate (Resource.Layout.EventList, container, false);

			ThreadPool.QueueUserWorkItem( o =>  LoadEventList());

			return view;
		}

		#region Get cached files
		async Task<bool> GetEventListFromFile(){

			var eventListFilePath = Path.Combine(Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.EVENT_LIST_FILE);
			if(File.Exists(eventListFilePath)){
				using(var eventListFile = File.OpenText (eventListFilePath)){
//					var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (eventListFile.ReadToEnd());
//					return responseObject;
					return await ProcessFile(eventListFile.ReadToEnd());
				}

			}
			return true;
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

		async Task<bool> LoadEventList(){

			var cookie = applicationResource.GetAuthCookie ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.EVENT_LIST_FILE);
			if (UISubscriptionService.EventList != null) {
				Activity.RunOnUiThread (() => {
					LoadUI ();	
				});
			}else if (File.Exists (fullFilePath) && applicationResource.CheckRefreshDate (Busidex.Mobile.Resources.EVENT_LIST_REFRESH_COOKIE_NAME)) {
				Activity.RunOnUiThread (async () => {
						ShowLoadingSpinner (GetString (Resource.String.Global_OneMoment));
						await GetEventListFromFile ();
					});
			} else {
				try {
					var controller = new SearchController ();
					await controller.GetEventTags (cookie).ContinueWith(r => {
						if (!string.IsNullOrEmpty (r.Result)) {

							Utils.SaveResponse (r.Result, Busidex.Mobile.Resources.EVENT_LIST_FILE);
							applicationResource.SetRefreshCookie(Busidex.Mobile.Resources.EVENT_LIST_REFRESH_COOKIE_NAME);
							Activity.RunOnUiThread (async () => await GetEventListFromFile ());
						}
					});

				} catch (Exception ignore) {

				}
			}

			return true;
		}

		static void SetEventCardRefreshCookie(EventSearchResponse eventList, EventTag tag){

			eventList.LastRefreshDate = DateTime.Now;
			var json = Newtonsoft.Json.JsonConvert.SerializeObject (eventList);
			Utils.SaveResponse(json, string.Format(Busidex.Mobile.Resources.EVENT_CARDS_FILE, tag.Text));
		}

		private void LoadUI(){
			if (UISubscriptionService.EventList != null) {
				
				Tags = UISubscriptionService.EventList;

				var lstEvents = Activity.FindViewById<ListView> (Resource.Id.lstEvents);

				eventListAdapter = new EventListAdapter (Activity, Resource.Id.lstCards, Tags);

				eventListAdapter.RedirectToEventCards += LoadEvent;

				lstEvents.Adapter = eventListAdapter;

			}
			HideLoadingSpinner ();
		}

		protected async override Task<bool> ProcessFile(string data){

			if(UISubscriptionService.EventList == null){
				var eventListResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (data);
				UISubscriptionService.EventList = eventListResponse.Model;
			}
				
			Activity.RunOnUiThread (() => {
				LoadUI ();
			});
			return true;
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
			Redirect(new EventCardsFragment(tag));
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
					Activity.RunOnUiThread (() => ShowLoadingSpinner (tag.Description));

					var ctrl = new SearchController ();
					await ctrl.SearchBySystemTag (tag.Text, cookie).ContinueWith(async r => {

						if(!string.IsNullOrEmpty(r.Result)){
							EventSearchResponse eventSearchResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (r.Result);

							var ownedCards = eventSearchResponse.SearchModel.Results.Where(c => c.OwnerId.HasValue).ToList(); 

							Utils.SaveResponse(r.Result, fileName);
							SetEventCardRefreshCookie(eventSearchResponse, tag);

							var idx = 0;
							var total = ownedCards.Count;
							//Activity.RunOnUiThread (() => {
								//HideLoadingSpinner();

								//ShowLoadingSpinner (
								//Resources.GetString (Resource.String.Global_LoadingEvent), 
								//ProgressDialogStyle.Horizontal, 
								//eventSearchResponse.SearchModel.Results.Count);
							//});


							foreach (var card in ownedCards) {
								if (card != null) {

									var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.FrontFileName;
									var bImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.BackFileName;
									var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
									var bName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;

									if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fName)){// || force) {
										await Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
											Activity.RunOnUiThread (() => UpdateLoadingSpinner (idx, total));
										});
									} else{
										Activity.RunOnUiThread (() => UpdateLoadingSpinner (idx, total));
									}

									if ((!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + bName)/* || force*/) && card.BackFileId.ToString () != Busidex.Mobile.Resources.EMPTY_CARD_ID) {
										await Utils.DownloadImage (bImageUrl, Busidex.Mobile.Resources.DocumentsPath, bName).ContinueWith (t => {
										});
									}
									idx++;
								}
							}

							Activity.RunOnUiThread (() => {
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

