using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class CardSearchInfoFragment : BaseCardEditFragment
	{
		EditText txtCompanyName;
		EditText txtYourName;
		EditText txtTitle;


		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_SEARCH_INFO);
			}
		}

		public override void OnDetach ()
		{
			txtCompanyName = null;
			txtTitle = null;
			txtYourName = null;

			base.OnDetach ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardSearchInfo, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			txtCompanyName = view.FindViewById<EditText> (Resource.Id.txtCompanyName);
			txtYourName = view.FindViewById<EditText> (Resource.Id.txtYourName);
			txtTitle = view.FindViewById<EditText> (Resource.Id.txtTitle);

			txtCompanyName.Text = SelectedCard.CompanyName;
			txtYourName.Text = SelectedCard.Name;
			txtTitle.Text = SelectedCard.Title;

			var btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				txtCompanyName.ClearFocus ();
				imm.HideSoftInputFromWindow (txtCompanyName.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtYourName.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTitle.WindowToken, 0);
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			var btnSave = view.FindViewById<Button> (Resource.Id.btnSave);
			btnSave.Click += delegate {

				imm.HideSoftInputFromWindow (txtCompanyName.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtYourName.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTitle.WindowToken, 0);

				SelectedCard.CompanyName = txtCompanyName.Text;
				SelectedCard.Name = txtYourName.Text;
				SelectedCard.Title = txtTitle.Text;

				UISubscriptionService.SaveCardInfo (new Mobile.Models.CardDetailModel (SelectedCard));
			};

			txtCompanyName.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtCompanyName.ClearFocus ();
				imm.HideSoftInputFromWindow (txtCompanyName.WindowToken, 0);
			};
			txtYourName.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtYourName.ClearFocus ();
				imm.HideSoftInputFromWindow (txtYourName.WindowToken, 0);
			};
			txtTitle.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTitle.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTitle.WindowToken, 0);
			};

			txtCompanyName.FocusChange += delegate {
				btnSave.Visibility = txtCompanyName.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtYourName.FocusChange += delegate {
				btnSave.Visibility = txtYourName.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtTitle.FocusChange += delegate {
				btnSave.Visibility = txtTitle.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};

			return view;
		}
	}
}

