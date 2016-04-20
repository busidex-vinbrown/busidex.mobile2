using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Busidex.Mobile;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class OrganizationsController : UIBarButtonItemWithImageViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");

		public OrganizationsController (IntPtr handle) : base (handle)
		{
		}

		OrganizationTableSource ConfigureTableSourceEventHandlers(List<Organization> data){

			var src = new OrganizationTableSource (data);
		
			src.ViewOrganization += delegate(long orgId) {

				try{
					UIStoryboard board = UIStoryboard.FromName ("OrganizationStoryBoard_iPhone", null);

					var orgDetailController = board.InstantiateViewController ("OrganizationDetailController") as OrganizationDetailController;

					if (orgDetailController != null) {
						orgDetailController.OrganizationId = orgId;
						NavigationController.PushViewController (orgDetailController, true);
					}
				}catch(Exception ex){
					Xamarin.Insights.Report(ex);
				}
			};

			src.ViewOrganizationCards += GoToOrganizationCards;

			return src;
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "My Organizations");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			vwOrganizations.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			vwOrganizations.Source = ConfigureTableSourceEventHandlers(UISubscriptionService.OrganizationList);
		} 
	}
}
