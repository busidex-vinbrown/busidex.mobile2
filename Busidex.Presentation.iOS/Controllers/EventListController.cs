﻿using System;
using System.Collections.Generic;
using System.Linq;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Foundation;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
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

				OnEventListLoadedEventHandler callback = list => InvokeOnMainThread (() => {
					loadingData = false;
					overlay.Hide();
					LoadEventList();
				});

				OnEventListUpdatedEventHandler update = status => InvokeOnMainThread (() => {
					if(IsViewLoaded && View.Window != null){  // no need to show anything if the view isn't visible any more
						overlay.TotalItems = status.Total;
						overlay.UpdateProgress (status.Count);
					}
				});

				UISubscriptionService.OnEventListUpdated += update;
				UISubscriptionService.OnEventListLoaded += callback;

				if(loadingData){
					return;
				}

				loadingData = true;

				View.AddSubview (overlay);

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
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "EventList");

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
			
			if (BaseNavigationController.eventCardsController != null) {
				BaseNavigationController.eventCardsController.SelectedTag = item;
				if(NavigationController.ViewControllers.Any(c => c as EventCardsController != null)){
					NavigationController.PopToViewController (BaseNavigationController.eventCardsController, true);
				}else{
					NavigationController.PushViewController (BaseNavigationController.eventCardsController, true);
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

