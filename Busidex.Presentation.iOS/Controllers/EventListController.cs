using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Busidex.Mobile;
using GoogleAnalytics.iOS;
using System.Linq;

namespace Busidex.Presentation.iOS
{
	public partial class EventListController : UIBarButtonItemWithImageViewController
	{
		public static NSString cellID = new NSString ("cellId");
		bool loadingData;

		public EventListController (IntPtr handle) : base (handle){

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			var overlay = new MyBusidexLoadingOverlay (View.Bounds);
			overlay.MessageText = "Loading Your Events";

			if (!UISubscriptionService.EventListLoaded) {

				if(loadingData){
					return;
				}

				loadingData = true;

				View.AddSubview (overlay);

				OnEventListLoadedEventHandler callback = list => InvokeOnMainThread (() => {
					loadingData = false;
					overlay.Hide();
					LoadEventList();
				});

				OnEventListUpdatedEventHandler update = status => InvokeOnMainThread (() => {
					overlay.TotalItems = status.Total;
					overlay.UpdateProgress (status.Count);
				});

				UISubscriptionService.OnEventListUpdated += update;
				UISubscriptionService.OnEventListLoaded += callback;

				UISubscriptionService.LoadEventList ();
			}else{
				InvokeOnMainThread (() => {
					overlay.Hide();
					LoadEventList();
				});
			}
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
		}
			
		void LoadEventList(){
			vwEventList.Source = ConfigureTableSourceEventHandlers (UISubscriptionService.EventList);
			vwEventList.ReloadData ();
			vwEventList.AllowsSelection = true;
			vwEventList.SetNeedsDisplay ();
		}
			
		void GoToEvent(EventTag item){
			
			if (eventCardsController != null) {
				eventCardsController.SelectedTag = item;
				if(NavigationController.ViewControllers.Any(c => c as EventCardsController != null)){
					NavigationController.PopToViewController (eventCardsController, true);
				}else{
					NavigationController.PushViewController (eventCardsController, true);
				}
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

