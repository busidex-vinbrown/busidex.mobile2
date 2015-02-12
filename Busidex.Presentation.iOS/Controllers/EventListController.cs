
using System;
using System.Drawing;

using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class EventListController : UIViewController
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

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		void LoadEventList(){

			var list = new List<EventTag> ();
			var src = ConfigureTableSourceEventHandlers (list);
			vwEventList.Source = src;
		}

		void GoToEvent(EventTag item){

		}

		EventListTableSource ConfigureTableSourceEventHandlers(List<EventTag> data){
			var src = new EventListTableSource (data);

			src.OnItemSelected += delegate {
				GoToEvent (((EventListTableSource)vwEventList.Source).SelectedEvent);
			};

			return src;
		}
	}
}

