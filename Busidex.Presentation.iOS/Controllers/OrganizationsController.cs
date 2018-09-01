using System;
using System.Collections.Generic;
using System.Linq;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Foundation;
using Google.Analytics;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class OrganizationsController : UIBarButtonItemWithImageViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		bool loadingData;

		public OrganizationsController (IntPtr handle) : base (handle)
		{
		}
		MyBusidexLoadingOverlay overlay;

		OrganizationTableSource ConfigureTableSourceEventHandlers(List<Organization> data){

			var src = new OrganizationTableSource (data);
		
			src.ViewOrganization += delegate(long orgId) {

				try{
					
					BaseNavigationController.orgDetailController.OrganizationId = orgId;

					if(NavigationController.ViewControllers.Any(c => c as OrganizationDetailController != null)){
						NavigationController.PopToViewController (BaseNavigationController.orgDetailController, true);
					}else{
						NavigationController.PushViewController (BaseNavigationController.orgDetailController, true);
					}

				}catch(Exception ex){
					Xamarin.Insights.Report(ex);
				}
			};

			src.ViewOrganizationCards += GoToOrganizationCards;

			return src;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if(!UISubscriptionService.OrganizationsLoaded){

				if(loadingData){
					return;
				}

				loadingData = true;

				View.AddSubview (overlay);

				OnMyOrganizationsUpdatedEventHandler update = status => InvokeOnMainThread (() => {
					overlay.TotalItems = status.Total;
					overlay.UpdateProgress (status.Count);
				});

				OnMyOrganizationsLoadedEventHandler callback = list => InvokeOnMainThread (() => {
					loadingData = false;
					overlay.Hide ();
					bindView(list);
				});

				UISubscriptionService.OnMyOrganizationsUpdated += update;
				UISubscriptionService.OnMyOrganizationsLoaded += callback;

				UISubscriptionService.LoadOrganizations ();
			}else{
				bindView (UISubscriptionService.OrganizationList);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "My Organizations");

			base.ViewDidAppear (animated);

		}

		void bindView(List<Organization> organizations){

			vwOrganizations.Source = ConfigureTableSourceEventHandlers(organizations);
			vwOrganizations.ReloadData ();
			vwOrganizations.AllowsSelection = true;
			vwOrganizations.SetNeedsDisplay ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			overlay = new MyBusidexLoadingOverlay (View.Bounds);
			overlay.MessageText = "Loading Your Organizations";

			vwOrganizations.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
		} 
	}
}
