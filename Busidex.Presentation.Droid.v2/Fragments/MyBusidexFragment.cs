using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using System.Threading;
using System;

namespace Busidex.Presentation.Droid.v2
{
	public class MyBusidexFragment : GenericViewPagerFragment
	{
		OnMyBusidexLoadedEventHandler callback;
		OnMyBusidexUpdatedEventHandler update;
		ProgressBar progressBar1;
		TextView myBusidexProgressStatus;
		TextView lblNoCardsMessage;
		OverscrollListView lstCards;
		SearchView txtFilter;
		UserCardAdapter myBusidexAdapter;

		void updateStatusCount (int total, int count)
		{
			if (myBusidexAdapter == null) {
				progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Visible;
				progressBar1.Max = total;// status.Total;
				progressBar1.Progress = count;// status.Count;
				myBusidexProgressStatus.Text = string.Format ("Loading {0} of {1}", count, total);
				lblNoCardsMessage.Visibility = ViewStates.Gone;
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.MyBusidex, container, false);

			progressBar1 = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);

			myBusidexProgressStatus = view.FindViewById<TextView> (Resource.Id.myBusidexProgressStatus);
			progressBar1.Visibility = myBusidexProgressStatus.Visibility = UISubscriptionService.UserCards.Count > 0 ? ViewStates.Gone : ViewStates.Visible;

			lblNoCardsMessage = view.FindViewById<TextView> (Resource.Id.lblNoCardsMessage);
			lblNoCardsMessage.Visibility = UISubscriptionService.UserCards.Count > 0 ? ViewStates.Gone : ViewStates.Visible;

			lstCards = view.FindViewById<OverscrollListView> (Resource.Id.lstCards);
			lstCards.Visibility = ViewStates.Visible;

			if (myBusidexAdapter != null) {
				lstCards.Adapter = myBusidexAdapter;
			}

			updateStatusCount (UISubscriptionService.UserCards.Count, 0);

			txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
			txtFilter.Visibility = ViewStates.Visible;
			txtFilter.Iconified = false;
			txtFilter.ClearFocus ();
			txtFilter.Touch += delegate {
				txtFilter.Focusable = true;
				txtFilter.RequestFocus ();
			};

			int accumulatedDeltaY = 0;
			lstCards.OverScrolled += deltaY => {

				accumulatedDeltaY += -deltaY;
				if (accumulatedDeltaY > 1000) {
					progressBar1.Visibility = ViewStates.Visible;
					myBusidexAdapter = null;
					UISubscriptionService.LoadUserCards ();
					BaseApplicationResource.TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MY_BUSIDEX_REFRESHED, 0);
				}
			};

			lstCards.Scroll += delegate {
				if (lstCards.CanScrollVertically (-1)) {
					accumulatedDeltaY = 0;
				}
			};

			if (callback == null) {
				callback = list => {

					accumulatedDeltaY = 0;
					Activity.RunOnUiThread (() => {

						try {
							myBusidexAdapter = myBusidexAdapter ?? new UserCardAdapter (Activity, Resource.Id.lstCards, UISubscriptionService.UserCards);
							myBusidexAdapter.UpdateData (UISubscriptionService.UserCards);
							myBusidexAdapter.Redirect += ((MainActivity)Activity).ShowCard;
							myBusidexAdapter.ShowButtonPanel += ((MainActivity)Activity).ShowButtonPanel;
							myBusidexAdapter.ShowNotes = true;
							txtFilter.QueryTextChange += delegate {
								MainActivity.DoFilter (myBusidexAdapter, txtFilter.Query);
							};

							lstCards.Adapter = myBusidexAdapter;
							progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Gone;
							lstCards.Visibility = ViewStates.Visible;
							((UserCardAdapter)lstCards.Adapter).NotifyDataSetChanged ();

							if (list.Count == 0) {
								lblNoCardsMessage.Visibility = ViewStates.Visible;
								lblNoCardsMessage.SetText (Resource.String.MyBusidex_NoCards);
							}
							txtFilter.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
						} catch (Exception ex) {
							Xamarin.Insights.Report (ex);
						}
					});
				};
			}

			if (update == null) {
				update = status => Activity.RunOnUiThread (() => updateStatusCount (status.Total, status.Count));
			}

			UISubscriptionService.OnMyBusidexLoaded -= callback;
			UISubscriptionService.OnMyBusidexLoaded += callback;

			UISubscriptionService.OnMyBusidexUpdated -= update;
			UISubscriptionService.OnMyBusidexUpdated += update;

			ThreadPool.QueueUserWorkItem (tok => UISubscriptionService.LoadUserCards ());

			lstCards.RequestFocus (FocusSearchDirection.Down);

			return view;
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Busidex.Mobile.Resources.GA_SCREEN_MY_BUSIDEX);
			}
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();

			UISubscriptionService.OnMyBusidexLoaded -= callback;
			UISubscriptionService.OnMyBusidexUpdated -= update;
		}
	}
}

