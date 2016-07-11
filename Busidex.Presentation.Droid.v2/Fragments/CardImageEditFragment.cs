using Android.OS;
using Android.Views;
using Android.Widget;

namespace Busidex.Presentation.Droid.v2
{
	public class CardImageEditFragment : BaseCardEditFragment
	{
		ImageButton btnBack;
		Button btnCardFront;
		Button btnCardBack;

		enum CardEditMode
		{
			Front,
			Back
		}

		public override void OnDetach ()
		{
			btnBack = null;
			btnCardFront = null;
			btnCardFront = null;
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_CARD_IMAGE);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardImageEdit, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			updateCover = view.FindViewById<RelativeLayout> (Resource.Id.updateCover);
			updateCover.Visibility = ViewStates.Gone;

			btnCardFront = view.FindViewById<Button> (Resource.Id.btnCardFront);
			btnCardBack = view.FindViewById<Button> (Resource.Id.btnCardBack);

			btnCardFront.Click += delegate {
				toggle (CardEditMode.Front);
			};

			btnCardBack.Click += delegate {
				toggle (CardEditMode.Back);
			};

			btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			return view;
		}

		void toggle (CardEditMode mode)
		{
			switch (mode) {
			case CardEditMode.Front: {
					btnCardFront.SetBackgroundResource (Resource.Color.buttonFontColor);
					btnCardFront.SetTextColor (Resources.GetColor (Resource.Color.buttonWhite));
					btnCardBack.SetBackgroundResource (Resource.Color.buttonWhite);
					btnCardBack.SetTextColor (Resources.GetColor (Resource.Color.buttonFontColor));
					break;
				}
			case CardEditMode.Back: {
					btnCardFront.SetBackgroundResource (Resource.Color.buttonWhite);
					btnCardFront.SetTextColor (Resources.GetColor (Resource.Color.buttonFontColor));
					btnCardBack.SetBackgroundResource (Resource.Color.buttonFontColor);
					btnCardBack.SetTextColor (Resources.GetColor (Resource.Color.buttonWhite));
					break;
				}
			}
		}
	}
}

