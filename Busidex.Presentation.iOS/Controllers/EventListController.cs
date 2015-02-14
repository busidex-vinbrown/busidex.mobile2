
using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Collections.Generic;
using GoogleAnalytics.iOS;
using System.IO;

namespace Busidex.Presentation.iOS
{
	public partial class EventListController : BaseController
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

			if(NavigationController != null){
				NavigationController.SetNavigationBarHidden(true, true);
			}


			vwEventList.RegisterClassForCellReuse (typeof(UITableViewCell), cellID);
			LoadEventList ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
				NavigationController.SetToolbarHidden (true, false);
			}
		}

		#region Get cached files
		EventListResponse GetEventListFromFile(){

			var eventListFilePath = Path.Combine(Resources.DocumentsPath, Resources.EVENT_LIST_FILE);
			if(File.Exists(eventListFilePath)){
				using(var eventListFile = File.OpenText (eventListFilePath)){
					var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (eventListFile.ReadToEnd());
					return responseObject;
				}

			}
			return null;
		}

		EventSearchResponse GetEventCardsFromFile(EventTag tag){

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
			Busidex.Mobile.Utils.SaveResponse(json, string.Format(Resources.EVENT_CARDS_FILE, tag.Text));
		}

		bool CheckEventSearchRefreshCookie(EventTag tag){

			var eventList = GetEventCardsFromFile(tag);
			if (eventList == null){
				SetEventCardRefreshCookie (eventList, tag);
				return false;
			}

			if(eventList.LastRefreshDate < DateTime.Now) {

				SetEventCardRefreshCookie (eventList, tag);
				return false;
			}
			return true;
		}

		void LoadEvent(EventTag item){

			var cookie = GetAuthCookie ();

			try{
				var fullFilePath = Path.Combine (documentsPath, Resources.EVENT_CARDS_FILE);
				if (File.Exists (fullFilePath) && CheckEventSearchRefreshCookie(item)) {
					GoToEvent(item);
				} else {
					var controller = new Busidex.Mobile.SearchController ();
					controller.SearchBySystemTag (item.Text, cookie.Value).ContinueWith(eventSearchResponse => {
						if(!string.IsNullOrEmpty(eventSearchResponse.Result)){

							Busidex.Mobile.Utils.SaveResponse(eventSearchResponse.Result, Resources.EVENT_CARDS_FILE);

							InvokeOnMainThread (() =>{
								GoToEvent(item);
							});
						}
					});
				}
			}
			catch(Exception ex){

			}
		}

		EventListTableSource ConfigureTableSourceEventHandlers(List<EventTag> data){
			var src = new EventListTableSource (data);

			src.OnItemSelected += delegate {
				LoadEvent (((EventListTableSource)vwEventList.Source).SelectedEvent);
			};

			return src;
		}
	}
}

