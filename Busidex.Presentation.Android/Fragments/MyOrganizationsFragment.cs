
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Threading.Tasks;
using System.Threading;

namespace Busidex.Presentation.Android
{
	public class MyOrganizationsFragment : BaseFragment
	{
		ListView lstOrganizations;



		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.MyOrganizations, container, false);

			lstOrganizations = view.FindViewById<ListView> (Resource.Id.lstOrganizations);

			return view;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				if (UISubscriptionService.OrganizationList.Count > 0) {
					ThreadPool.QueueUserWorkItem (o => LoadUI());
				} else {
					ThreadPool.QueueUserWorkItem (o => LoadMyOrganizationsAsync ());
				}
				TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_ORGANIZATIONS_LABEL, Busidex.Mobile.Resources.GA_LABEL_LIST, 0);
			}
		}

		void LoadUI(){
			
			if (UISubscriptionService.OrganizationList != null && UISubscriptionService.OrganizationList.Count > 0) {

				var adapter = new OrganizationAdapter (Activity, Resource.Id.lstOrganizations, subscriptionService.OrganizationList);

				adapter.RedirectToOrganizationDetails += async delegate {
					Redirect(((SplashActivity)Activity).fragments[typeof(OrganizationDetailFragment).Name]);
				};

				adapter.RedirectToOrganizationMembers += async delegate {
					Redirect(((SplashActivity)Activity).fragments[typeof(OrganizationCardsFragment).Name]);
				};

				adapter.RedirectToOrganizationReferrals += async delegate {
					Redirect(((SplashActivity)Activity).fragments[typeof(OrganizationDetailFragment).Name]);
				};

				lstOrganizations.Adapter = adapter;
			} else {
				Toast.MakeText (Activity, Resource.String.Organization_NoOrganizations, ToastLength.Long);
			}
			HideLoadingSpinner ();
		}

		protected override async Task<bool> ProcessFile(string data){

			if(subscriptionService.OrganizationList.Count == 0){
				var myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (data);
				if(myOrganizationsResponse != null){
					List<Organization> Organizations = myOrganizationsResponse.Model;
					subscriptionService.OrganizationList = Organizations;
				}
			}
				
			Activity.RunOnUiThread (LoadUI);

			return true;
		}
	}
}

