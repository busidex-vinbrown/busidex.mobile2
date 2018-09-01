using System;
using System.Collections.Generic;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Foundation;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class SharedCardListController : BaseCardViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");

		public SharedCardListController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			vwSharedCards.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);


		}


		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Shared Card List");

			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
				NavigationController.NavigationBar.SetBackgroundImage (null, UIBarMetrics.Default);
			}
			LoadSharedCards ();
		}

		void LoadSharedCards ()
		{

			vwSharedCards.Source = ConfigureTableSourceEventHandlers (UISubscriptionService.Notifications);

			Badge.Plugin.CrossBadge.Current.SetBadge (UISubscriptionService.Notifications.Count);	
		}

		static void SaveSharedCard (SharedCard sharedCard)
		{

			UISubscriptionService.SaveSharedCard (sharedCard);
		
			if (sharedCard.Accepted.GetValueOrDefault ()) {
				// track the event
				ActivityController.SaveActivity ((long)EventSources.Add, sharedCard.Card.CardId, UISubscriptionService.AuthToken);
			}
		}

		SharedCardTableSource ConfigureTableSourceEventHandlers (List<SharedCard> sharedCards)
		{

			var src = new SharedCardTableSource (sharedCards);
			src.NoCardsMessage = "There are no more shared cards to load";

			src.CardSelected += delegate {
				GoToCard ();
			};
			src.CardShared += SaveSharedCard;

			return src;
		}
	}
}

