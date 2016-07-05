using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class CardVisibilityFragment : BaseCardEditFragment, RadioGroup.IOnCheckedChangeListener
	{

		RadioButton rdoPublic;
		RadioButton rdoSemiPrivate;
		RadioButton rdoPrivate;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardVisibility, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			rdoPublic = view.FindViewById<RadioButton> (Resource.Id.rdoPublic);
			rdoSemiPrivate = view.FindViewById<RadioButton> (Resource.Id.rdoSemiPrivate);
			rdoPrivate = view.FindViewById<RadioButton> (Resource.Id.rdoPrivate);

			var btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			var btnSave = view.FindViewById<Button> (Resource.Id.btnSave);
			btnSave.Click += delegate {
				UISubscriptionService.SaveCardVisibility (SelectedCard.Visibility);
			};

			hideProgress ();

			var radioGroup = view.FindViewById<RadioGroup> (Resource.Id.rdoGroupVisibility);
			radioGroup.SetOnCheckedChangeListener (this);

			switch (SelectedCard.Visibility) {
			case (byte)CardVisibility.Public: {
					radioGroup.Check (rdoPublic.Id);
					break;
				}
			case (byte)CardVisibility.SemiPublic: {
					radioGroup.Check (rdoSemiPrivate.Id);
					break;
				}
			case (byte)CardVisibility.Private: {
					radioGroup.Check (rdoPrivate.Id);
					break;
				}
			}

			return view;
		}

		public void OnCheckedChanged (RadioGroup group, int checkedId)
		{
			var checkedRadioButton = group.FindViewById<RadioButton> (checkedId);
			var val = byte.Parse (checkedRadioButton.Tag.ToString ());
			SelectedCard.Visibility = val;
		}
	}
}

