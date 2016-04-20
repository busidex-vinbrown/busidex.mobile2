using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
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

			if (UISubscriptionService.EventList.Count == 0) {
				OnEventListLoadedEventHandler callback = list => InvokeOnMainThread (LoadEventList);

				UISubscriptionService.OnEventListLoaded += callback;

				UISubscriptionService.LoadEventList ();
			}else{
				LoadEventList ();
			}
		}
			
		void LoadEventList(){
			vwEventList.Source = ConfigureTableSourceEventHandlers (UISubscriptionService.EventList);
			vwEventList.ReloadData ();
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

		EventListTableSource ConfigureTableSourceEventHandlers(List<EventTag> data){
			var src = new EventListTableSource (data);

			src.OnItemSelected += GoToEvent;

			return src;
		}
	}
}

