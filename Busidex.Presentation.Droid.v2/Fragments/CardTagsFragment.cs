
using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class CardTagsFragment : BaseCardEditFragment
	{
		EditText txtTag1;
		EditText txtTag2;
		EditText txtTag3;
		EditText txtTag4;
		EditText txtTag5;
		EditText txtTag6;
		EditText txtTag7;
		TextView txtDescription;

		public override void OnDetach ()
		{

			txtTag1 = null;
			txtTag2 = null;
			txtTag3 = null;
			txtTag4 = null;
			txtTag5 = null;
			txtTag6 = null;
			txtTag7 = null;

			base.OnDetach ();
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_TAGS);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardTags, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			txtDescription = view.FindViewById<TextView> (Resource.Id.txtDescription);

			var btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			txtTag1 = view.FindViewById<EditText> (Resource.Id.txtTag1);
			txtTag2 = view.FindViewById<EditText> (Resource.Id.txtTag2);
			txtTag3 = view.FindViewById<EditText> (Resource.Id.txtTag3);
			txtTag4 = view.FindViewById<EditText> (Resource.Id.txtTag4);
			txtTag5 = view.FindViewById<EditText> (Resource.Id.txtTag5);
			txtTag6 = view.FindViewById<EditText> (Resource.Id.txtTag6);
			txtTag7 = view.FindViewById<EditText> (Resource.Id.txtTag7);

			var btnSave = view.FindViewById<Button> (Resource.Id.btnSave);
			btnSave.Click += delegate {

				imm.HideSoftInputFromWindow (txtTag1.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTag2.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTag3.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTag4.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTag5.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTag6.WindowToken, 0);
				imm.HideSoftInputFromWindow (txtTag7.WindowToken, 0);

				var tags = new List<string> ();
				tags.Add (txtTag1.Text);
				tags.Add (txtTag2.Text);
				tags.Add (txtTag3.Text);
				tags.Add (txtTag4.Text);
				tags.Add (txtTag5.Text);
				tags.Add (txtTag6.Text);
				tags.Add (txtTag7.Text);
				tags.RemoveAll (t => string.IsNullOrEmpty (t));

				// Add new tags
				foreach (var tag in tags) {
					if (SelectedCard.Tags.FirstOrDefault (t => string.Equals (t.Text, tag, StringComparison.InvariantCultureIgnoreCase)) == null) {
						SelectedCard.Tags.Add (new Tag {
							Text = tag,
							TagTypeId = 1,
							Deleted = false
						});
					}
				}

				// Clear tags that have been removed
				var existingTags = new List<string> ();
				existingTags.AddRange (SelectedCard.Tags.Select (t => t.Text.ToLower ()).Distinct ());
				foreach (var tag in existingTags) {
					if (tags.FirstOrDefault (t => string.Equals (t, tag, StringComparison.InvariantCultureIgnoreCase)) == null) {
						SelectedCard.Tags.RemoveAll (t => t.Text.ToLower () == tag);
					}
				}

				UISubscriptionService.SaveCardInfo (new CardDetailModel (SelectedCard));
			};

			initTagsUI ();

			txtTag1.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTag1.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTag1.WindowToken, 0);
			};
			txtTag2.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTag2.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTag2.WindowToken, 0);
			};
			txtTag3.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTag3.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTag3.WindowToken, 0);
			};
			txtTag4.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTag4.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTag4.WindowToken, 0);
			};
			txtTag5.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTag5.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTag5.WindowToken, 0);
			};
			txtTag6.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTag6.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTag6.WindowToken, 0);
			};
			txtTag7.EditorAction += (object sender, TextView.EditorActionEventArgs e) => {
				txtTag7.ClearFocus ();
				imm.HideSoftInputFromWindow (txtTag7.WindowToken, 0);
			};

			txtTag1.FocusChange += delegate {
				btnSave.Visibility = txtDescription.Visibility = txtTag1.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtTag2.FocusChange += delegate {
				btnSave.Visibility = txtDescription.Visibility = txtTag2.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtTag3.FocusChange += delegate {
				btnSave.Visibility = txtDescription.Visibility = txtTag3.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtTag4.FocusChange += delegate {
				btnSave.Visibility = txtDescription.Visibility = txtTag4.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtTag5.FocusChange += delegate {
				btnSave.Visibility = txtDescription.Visibility = txtTag5.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtTag6.FocusChange += delegate {
				btnSave.Visibility = txtDescription.Visibility = txtTag6.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};
			txtTag7.FocusChange += delegate {
				btnSave.Visibility = txtDescription.Visibility = txtTag7.HasFocus ? ViewStates.Gone : ViewStates.Visible;
			};

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			UISubscriptionService.OnCardInfoSaved += CardUpdated;

			return view;
		}

		protected override void CardUpdated ()
		{
			base.CardUpdated ();

			Activity.RunOnUiThread (() => {
				initTagsUI ();
			});
		}

		void initTagsUI ()
		{
			var userTags = SelectedCard.Tags.Where (t => t.TagType == 1).ToList ();
			txtTag1.Text = txtTag2.Text = txtTag3.Text = txtTag4.Text = txtTag5.Text = txtTag6.Text = txtTag7.Text = string.Empty;
			for (var i = 0; i < userTags.Count; i++) {

				switch (i) {
				case 0: {
						txtTag1.Text = userTags [i].Text;
						break;
					}
				case 1: {
						txtTag2.Text = userTags [i].Text;
						break;
					}
				case 2: {
						txtTag3.Text = userTags [i].Text;
						break;
					}
				case 3: {
						txtTag4.Text = userTags [i].Text;
						break;
					}
				case 4: {
						txtTag5.Text = userTags [i].Text;
						break;
					}
				case 5: {
						txtTag6.Text = userTags [i].Text;
						break;
					}
				case 6: {
						txtTag7.Text = userTags [i].Text;
						break;
					}
				}
			}
		}
	}
}

