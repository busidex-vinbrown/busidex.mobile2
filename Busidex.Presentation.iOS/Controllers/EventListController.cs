using System;
using System.Linq;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Busidex.Mobile;
using GoogleAnalytics.iOS;


namespace Busidex.Presentation.iOS
{
	public partial class EventListController : UIBarButtonItemWithImageViewController
	{
		public static NSString cellID = new NSString ("cellId");

		public EventListController (IntPtr handle) : base (handle){

		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "EventList");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		
			vwEventList.RegisterClassForCellReuse (typeof(UITableViewCell), cellID);
			LoadEventList ();
		}


		#region Get cached files
		static EventListResponse GetEventListFromFile(){

			var eventListFilePath = Path.Combine(Resources.DocumentsPath, Resources.EVENT_LIST_FILE);
			if(File.Exists(eventListFilePath)){
				using(var eventListFile = File.OpenText (eventListFilePath)){
					var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (eventListFile.ReadToEnd());
					return responseObject;
				}

			}
			return null;
		}

		static EventSearchResponse GetEventCardsFromFile(EventTag tag){

			var eventCardsFilePath = Path.Combine(Resources.DocumentsPath, string.Format(Resources.EVENT_CARDS_FILE, tag.Text));
			if(File.Exists(eventCardsFilePath)){
				using(var eventCardsFile = File.OpenText (eventCardsFilePath)){
					var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (eventCardsFile.ReadToEnd());
					return responseObject;
				}

			}
			return null;
		}
		#endregion

		void LoadEventList(){

			var list = GetEventListFromFile ();
			var src = ConfigureTableSourceEventHandlers (list.Model);
			vwEventList.Source = src;
		}
			
		void GoToEvent(EventTag item){
			var eventCardsController = Storyboard.InstantiateViewController ("EventCardsController") as EventCardsController;
			if (eventCardsController != null) {
				eventCardsController.SelectedTag = item;
				NavigationController.PushViewController (eventCardsController, true);
			}
		}

		static void SetEventCardRefreshCookie(EventSearchResponse eventList, EventTag tag){

			eventList.LastRefreshDate = DateTime.Now;
			var json = Newtonsoft.Json.JsonConvert.SerializeObject (eventList);
			Utils.SaveResponse(json, string.Format(Resources.EVENT_CARDS_FILE, tag.Text));
		}

		/// <summary>
		/// Check the last refresh date of the cached object.
		/// </summary>
		/// <returns><c>true</c>, if event search refresh cookie was checked, <c>false</c> otherwise.</returns>
		/// <param name="tag">Tag.</param>
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

		async Task<bool> LoadEvent(EventTag tag){

			var cookie = GetAuthCookie ();

			try {
				string fileName = string.Format(Resources.EVENT_CARDS_FILE, tag.Text);
				var fullFilePath = Path.Combine (documentsPath, fileName);
				if (File.Exists (fullFilePath) && CheckEventSearchRefreshDate(tag)) {
					GoToEvent(tag);
				} else {
					var overlay = new MyBusidexLoadingOverlay (View.Bounds);
					overlay.MessageText = tag.Description;

					View.AddSubview (overlay);

					var ctrl = new Busidex.Mobile.SearchController ();
					await ctrl.SearchBySystemTag (tag.Text, cookie.Value).ContinueWith(async r => {

						if(!string.IsNullOrEmpty(r.Result)){
							EventSearchResponse eventSearchResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (r.Result);

							var ownedCards = eventSearchResponse.SearchModel.Results.Where(c => c.OwnerId.HasValue).ToList(); 

							Utils.SaveResponse(r.Result, fileName);
							SetEventCardRefreshCookie(eventSearchResponse, tag);

							var idx = 0;
							InvokeOnMainThread (() =>{
								overlay.TotalItems = ownedCards.Count;
								overlay.UpdateProgress (idx);
							});


							foreach (var card in ownedCards) {
								if (card != null) {

									var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
									var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
									var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
									var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;

									if (!File.Exists (documentsPath + "/" + fName)){// || force) {
										await Utils.DownloadImage (fImageUrl, documentsPath, fName).ContinueWith (t => {
											InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
										});
									} else{
										InvokeOnMainThread (() => overlay.UpdateProgress (idx));
									}

									if ((!File.Exists (documentsPath + "/" + bName)/* || force*/) && card.BackFileId.ToString () != Resources.EMPTY_CARD_ID) {
										await Utils.DownloadImage (bImageUrl, documentsPath, bName).ContinueWith (t => {
										});
									}
									idx++;
								}
							}

							InvokeOnMainThread (() => {
								overlay.Hide();
								GoToEvent (tag);
							});

						}
					});
				}
				
			}
			catch(Exception ignore){

			}

			return true;
		}

		EventListTableSource ConfigureTableSourceEventHandlers(List<EventTag> data){
			var src = new EventListTableSource (data);

			src.OnItemSelected += async delegate {
				await LoadEvent (((EventListTableSource)vwEventList.Source).SelectedEvent);
			};

			return src;
		}
	}
}

