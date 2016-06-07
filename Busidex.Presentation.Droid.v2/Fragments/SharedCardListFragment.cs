
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Threading;

namespace Busidex.Presentation.Droid.v2
{
	public class SharedCardListFragment : GenericViewPagerFragment
	{
		OnNotificationsLoadedEventHandler callback;

		public override void OnResume ()
		{
			base.OnResume ();
			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_REFERRALS);
			}
		}

		static void SaveSharedCard (SharedCard card)
		{
			UISubscriptionService.SaveSharedCard (card);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.SharedCardList, container, false);

			var lstSharedCards = view.FindViewById<ListView> (Resource.Id.lstSharedCards);

			var imgNoNotifications = view.FindViewById<ImageView> (Resource.Id.imgNoNotifications);
			var lblNoNotificationsMessage = view.FindViewById<TextView> (Resource.Id.lblNoNotificationsMessage);

			imgNoNotifications.Visibility = UISubscriptionService.Notifications.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
			lblNoNotificationsMessage.Visibility = UISubscriptionService.Notifications.Count == 0 ? ViewStates.Visible : ViewStates.Gone;

			var sharedCardAdapter = new SharedCardListAdapter (Activity, Resource.Id.lstSharedCards, UISubscriptionService.Notifications);
			lstSharedCards.Adapter = sharedCardAdapter;

			sharedCardAdapter.SharingCard -= SaveSharedCard;
			sharedCardAdapter.SharingCard += SaveSharedCard;

			if (callback == null) {
				callback = list => Activity.RunOnUiThread (() => {
					if (Activity != null) {
						sharedCardAdapter.UpdateData (list);

						imgNoNotifications.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
						lblNoNotificationsMessage.Visibility = list.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					}
				});
			}
			UISubscriptionService.OnNotificationsLoaded -= callback;
			UISubscriptionService.OnNotificationsLoaded += callback;

			ThreadPool.QueueUserWorkItem (tok => UISubscriptionService.LoadNotifications ());

			BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_REFERRALS);

			return view;
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			UISubscriptionService.OnNotificationsLoaded -= callback;
		}
	}
}

