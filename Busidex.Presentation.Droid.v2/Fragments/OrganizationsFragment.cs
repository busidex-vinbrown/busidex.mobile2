

using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using System.Threading;

namespace Busidex.Presentation.Droid.v2
{
	public class OrganizationsFragment : GenericViewPagerFragment
	{
		OnMyOrganizationsLoadedEventHandler callback;
		OrganizationAdapter orgAdapter;
		OverscrollListView lstOrganizations;
		ProgressBar progressBar2;

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_ORGANIZATIONS);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate (Resource.Layout.MyOrganizations, container, false);

			lstOrganizations = view.FindViewById<OverscrollListView> (Resource.Id.lstOrganizations);
			var lblNoOrganizationsMessage = view.FindViewById<TextView> (Resource.Id.lblNoOrganizationsMessage);
			lblNoOrganizationsMessage.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
			lstOrganizations.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

			if (orgAdapter != null) {
				lstOrganizations.Adapter = orgAdapter;
			}

			progressBar2 = view.FindViewById<ProgressBar> (Resource.Id.progressBar2);
			progressBar2.Visibility = orgAdapter == null ? ViewStates.Visible : ViewStates.Gone;

			var imgRefreshOrganizations = view.FindViewById<ImageButton> (Resource.Id.imgRefreshOrganizations);
			imgRefreshOrganizations.Visibility = UISubscriptionService.OrganizationList.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
			imgRefreshOrganizations.Click += delegate {
				progressBar2.Visibility = ViewStates.Visible;
				UISubscriptionService.LoadOrganizations ();
			};

			int accumulatedDeltaY = 0;
			lstOrganizations.OverScrolled += deltaY => {

				accumulatedDeltaY += -deltaY;
				if (accumulatedDeltaY > 1000) {
					lstOrganizations.Visibility = ViewStates.Gone;
					progressBar2.Visibility = ViewStates.Visible;
					UISubscriptionService.LoadOrganizations ();
				}
			};

			lstOrganizations.Scroll += delegate {
				if (lstOrganizations.CanScrollVertically (-1)) {
					accumulatedDeltaY = 0;
				}
			};

			if (callback == null) {
				callback = list => Activity.RunOnUiThread (() => {

					orgAdapter = orgAdapter ?? new OrganizationAdapter (Activity, Resource.Id.lstOrganizations, UISubscriptionService.OrganizationList);
					orgAdapter.RedirectToOrganizationDetails += org => ((MainActivity)Activity).ShowOrganizationDetail (new OrganizationPanelFragment (org));
					orgAdapter.RedirectToOrganizationMembers += ((MainActivity)Activity).LoadOrganizationMembers;
					orgAdapter.UpdateData (UISubscriptionService.OrganizationList);
					orgAdapter.NotifyDataSetChanged ();

					lstOrganizations.Adapter = orgAdapter;

					accumulatedDeltaY = 0;
					orgAdapter.UpdateData (list);
					progressBar2.Visibility = ViewStates.Gone;
					lblNoOrganizationsMessage.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					lstOrganizations.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
				});
			}
			UISubscriptionService.OnMyOrganizationsLoaded -= callback;
			UISubscriptionService.OnMyOrganizationsLoaded += callback;

			ThreadPool.QueueUserWorkItem (tok => UISubscriptionService.LoadOrganizations ());

			BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_ORGANIZATIONS);

			return view;
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			UISubscriptionService.OnMyOrganizationsLoaded -= callback;
		}
	}
}

