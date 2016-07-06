
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
using Android.Views.InputMethods;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class BaseCardEditFragment : GenericViewPagerFragment
	{
		protected Card SelectedCard { get; set; }
		protected View view;
		protected ProgressBar progressBar1;
		protected RelativeLayout updateCover;
		protected InputMethodManager imm;

		public override void OnDetach ()
		{
			imm = null;
			view = null;

			base.OnDetach ();
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		protected void hideProgress ()
		{
			Activity.RunOnUiThread (() => {
				updateCover.Visibility = progressBar1.Visibility = ViewStates.Gone;
			});
		}

		protected void showProgress ()
		{
			Activity.RunOnUiThread (() => {
				updateCover.Visibility = progressBar1.Visibility = ViewStates.Visible;
			});
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			imm = (InputMethodManager)Activity.GetSystemService (Context.InputMethodService);

			updateCover = view.FindViewById<RelativeLayout> (Resource.Id.updateCover);
			progressBar1 = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);

			SelectedCard = UISubscriptionService.OwnedCard;

			UISubscriptionService.OnCardInfoUpdating -= CardUpdating;
			UISubscriptionService.OnCardInfoUpdating += CardUpdating;

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			UISubscriptionService.OnCardInfoSaved += CardUpdated;

			hideProgress ();

			return base.OnCreateView (inflater, container, savedInstanceState);
		}

		protected void CardUpdating ()
		{
			showProgress ();
		}

		protected virtual void CardUpdated ()
		{
			hideProgress ();
			SelectedCard = UISubscriptionService.OwnedCard;
		}

		public override void OnDestroy ()
		{
			UISubscriptionService.OnCardInfoUpdating -= CardUpdating;
			UISubscriptionService.OnCardInfoSaved -= CardUpdated;

			base.OnDestroy ();
		}
	}
}

