
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class MyBusidexFragment : GenericViewPagerFragment
	{
		OnMyBusidexLoadedEventHandler callback;
		OnMyBusidexUpdatedEventHandler update;
		View _view;

		void init (View view)
		{
			var progressBar1 = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);

			var myBusidexProgressStatus = view.FindViewById<TextView> (Resource.Id.myBusidexProgressStatus);
			progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Visible;

			var lblNoCardsMessage = view.FindViewById<TextView> (Resource.Id.lblNoCardsMessage);
			lblNoCardsMessage.Visibility = ViewStates.Gone;

			var myBusidexAdapter = new UserCardAdapter (Activity, Resource.Id.lstCards, UISubscriptionService.UserCards);

			var txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
			txtFilter.Visibility = ViewStates.Visible;// UISubscriptionService.UserCards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
			txtFilter.QueryTextChange += delegate {
				MainActivity.DoFilter (myBusidexAdapter, txtFilter.Query);
			};

			txtFilter.Iconified = false;
			txtFilter.ClearFocus ();
			txtFilter.Touch += delegate {
				txtFilter.Focusable = true;
				txtFilter.RequestFocus ();
			};

			myBusidexAdapter.Redirect += ((MainActivity)Activity).ShowCard;
			myBusidexAdapter.ShowButtonPanel += ((MainActivity)Activity).ShowButtonPanel;
			myBusidexAdapter.ShowNotes = true;

			var lstCards = view.FindViewById<OverscrollListView> (Resource.Id.lstCards);
			lstCards.Adapter = myBusidexAdapter;

			int accumulatedDeltaY = 0;
			lstCards.OverScrolled += deltaY => {

				accumulatedDeltaY += -deltaY;
				if (accumulatedDeltaY > 1000) {
					lstCards.Visibility = ViewStates.Gone;
					progressBar1.Visibility = ViewStates.Visible;
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
				callback = list => Activity.RunOnUiThread (() => {
					myBusidexAdapter.UpdateData (list);
					progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Gone;
					lstCards.Visibility = ViewStates.Visible;
					if (list.Count == 0) {
						lblNoCardsMessage.Visibility = ViewStates.Visible;
						lblNoCardsMessage.SetText (Resource.String.MyBusidex_NoCards);
					}
					txtFilter.Visibility = list.Count == 0 ? ViewStates.Gone : ViewStates.Visible;
					accumulatedDeltaY = 0;
				});
			}

			if (update == null) {
				update = status => Activity.RunOnUiThread (() => {
					progressBar1.Visibility = myBusidexProgressStatus.Visibility = ViewStates.Visible;
					progressBar1.Max = status.Total;
					progressBar1.Progress = status.Count;
					myBusidexProgressStatus.Text = string.Format ("Loading {0} of {1}", status.Count, status.Total);	
					lblNoCardsMessage.Visibility = ViewStates.Gone;
				});
			}

			UISubscriptionService.OnMyBusidexLoaded -= callback;
			UISubscriptionService.OnMyBusidexLoaded += callback;

			UISubscriptionService.OnMyBusidexUpdated -= update;
			UISubscriptionService.OnMyBusidexUpdated += update;

			lstCards.RequestFocus (FocusSearchDirection.Down);
			lstCards.Visibility = ViewStates.Visible;

			view.Id = Resource.Id.MyBusidexLayout;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_view = inflater.Inflate (Resource.Layout.MyBusidex, container, false);

			init (_view);
			container.Visibility = ViewStates.Visible;
			return _view;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			UISubscriptionService.LoadUserCards ();

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

