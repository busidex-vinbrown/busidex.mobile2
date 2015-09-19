
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class MyOrganizationsFragment : GenericViewPagerFragment
	{
		ListView lstOrganizations;
		List<Organization> Organizations;
		readonly Activity context;

		public MyOrganizationsFragment(){
			
		}

		public MyOrganizationsFragment(Activity ctx, List<Organization> organizations){
			context = ctx;
			Organizations = organizations;
		}

		public void SetOrganizations(List<Organization> organizations){
			Organizations = organizations;
			if (Organizations != null) {
				context.RunOnUiThread (LoadUI);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.MyOrganizations, container, false);

			lstOrganizations = view.FindViewById<ListView> (Resource.Id.lstOrganizations);
			LoadUI ();
			return view;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				
				//TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_ORGANIZATIONS_LABEL, Busidex.Mobile.Resources.GA_LABEL_LIST, 0);
			}
		}

		void LoadUI(){
			
//			if (subscriptionService.OrganizationList != null && subscriptionService.OrganizationList.Count > 0) {
//
			var adapter = new OrganizationAdapter (context, Resource.Id.lstOrganizations, Organizations);
			adapter.RedirectToOrganizationDetails += (Organization org) => ((MainActivity)context).ShowOrganizationDetail (new OrganizationPanelFragment (org));
			lstOrganizations.Adapter = adapter;

//
//				adapter.RedirectToOrganizationDetails += async delegate {
//					Redirect(((SplashActivity)Activity).fragments[typeof(OrganizationDetailFragment).Name]);
//				};
//
//				adapter.RedirectToOrganizationMembers += async delegate {
//					Redirect(((SplashActivity)Activity).fragments[typeof(OrganizationCardsFragment).Name]);
//				};
//
//				adapter.RedirectToOrganizationReferrals += async delegate {
//					Redirect(((SplashActivity)Activity).fragments[typeof(OrganizationDetailFragment).Name]);
//				};
//
				
//			} else {
//				Toast.MakeText (Activity, Resource.String.Organization_NoOrganizations, ToastLength.Long);
//			}
			//HideLoadingSpinner ();
		}
	}
}

