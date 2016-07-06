using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class CardContactInfoFragment : BaseCardEditFragment
	{

		public override void OnDetach ()
		{

			base.OnDetach ();
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_CONTACT_INFO);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardContactInfo, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			var btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};



			return view;
		}
	}
}

