
using System;
using System.Drawing;

using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using GoogleAnalytics.iOS;
using System.IO;

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

		List<EventTag> GetEventListFromFile(){

			var eventListFilePath = Path.Combine(Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.EVENT_LIST_FILE);
			if(File.Exists(eventListFilePath)){
				using(var eventListFile = File.OpenText (eventListFilePath)){
					var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (eventListFile.ReadToEnd());
					return responseObject.Model;
				}

			}
			return new List<EventTag> ();
		}

		void LoadEventList(){

			var list = GetEventListFromFile ();
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

