//
//using System.Collections.Generic;
//using Android.App;
//using Android.OS;
//using Android.Views;
//using Android.Widget;
//using Busidex.Mobile.Models;
//
//namespace Busidex.Presentation.Droid.v2
//{
//	public class MyOrganizationsFragment : GenericViewPagerFragment
//	{
//		ListView lstOrganizations;
//		List<Organization> Organizations;
//		readonly Activity context;
//
//		public MyOrganizationsFragment(){
//			
//		}
//
//		public MyOrganizationsFragment(Activity ctx, List<Organization> organizations){
//			context = ctx;
//			Organizations = organizations;
//		}
//
//		public void SetOrganizations(List<Organization> organizations){
//			Organizations = organizations;
//			if (Organizations != null) {
//				context.RunOnUiThread (LoadUI);
//			}
//		}
//
//		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//		{
//			// Use this to return your custom view for this Fragment
//			var view = inflater.Inflate(Resource.Layout.MyOrganizations, container, false);
//
//			lstOrganizations = view.FindViewById<ListView> (Resource.Id.lstOrganizations);
//			LoadUI ();
//			return view;
//		}
//
//		void LoadUI(){
//			
//			var adapter = new OrganizationAdapter (context, Resource.Id.lstOrganizations, Organizations);
//			adapter.RedirectToOrganizationDetails += org => ((MainActivity)context).ShowOrganizationDetail (new OrganizationPanelFragment (org));
//			adapter.RedirectToOrganizationMembers += org => ((MainActivity)Activity).LoadOrganizationMembers (org);
//			lstOrganizations.Adapter = adapter;
//		}
//	}
//}