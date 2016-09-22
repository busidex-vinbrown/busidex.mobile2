using System;
using GoogleAnalytics.iOS;
using System.Linq;
using Busidex.Mobile;
using System.Collections.Generic;

namespace Busidex.Presentation.iOS
{
	public partial class CardTagsController : BaseCardEditController
	{

		public CardTagsController (IntPtr handle) : base (handle)
		{
		}

		void loadTags ()
		{
			var userTags = UnsavedData.Tags.Where (t => t.TagType == 1).ToList ();
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

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Card Tags");
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (UnsavedData == null || UnsavedData.Tags == null) {
				return;
			}

			loadTags ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			txtTag1.AllEditingEvents += (sender, e) => {
				CardInfoChanged = (UnsavedData.Tags.Count == 0 && !string.IsNullOrEmpty(txtTag1.Text)) || (UnsavedData.Tags.Count >= 1 && txtTag1.Text != UnsavedData.Tags [0].Text);
			};

			txtTag2.AllEditingEvents += (sender, e) => {
				CardInfoChanged = (UnsavedData.Tags.Count <= 1 && !string.IsNullOrEmpty (txtTag2.Text)) || (UnsavedData.Tags.Count >= 2 && txtTag2.Text != UnsavedData.Tags [1].Text);
			};

			txtTag3.AllEditingEvents += (sender, e) => {
				CardInfoChanged = (UnsavedData.Tags.Count <= 2 && !string.IsNullOrEmpty (txtTag3.Text)) || (UnsavedData.Tags.Count >= 3 && txtTag3.Text != UnsavedData.Tags [2].Text);
			};

			txtTag4.AllEditingEvents += (sender, e) => {
				CardInfoChanged = (UnsavedData.Tags.Count <= 3 && !string.IsNullOrEmpty (txtTag4.Text)) || (UnsavedData.Tags.Count >= 4 && txtTag4.Text != UnsavedData.Tags [3].Text);
			};

			txtTag5.AllEditingEvents += (sender, e) => {
				CardInfoChanged = (UnsavedData.Tags.Count <= 4 && !string.IsNullOrEmpty (txtTag5.Text)) || (UnsavedData.Tags.Count >= 5 && txtTag5.Text != UnsavedData.Tags [4].Text);
			};

			txtTag6.AllEditingEvents += (sender, e) => {
				CardInfoChanged = (UnsavedData.Tags.Count <= 5 && !string.IsNullOrEmpty (txtTag6.Text)) || (UnsavedData.Tags.Count >= 6 && txtTag6.Text != UnsavedData.Tags [5].Text);
			};

			txtTag7.AllEditingEvents += (sender, e) => {
				CardInfoChanged = (UnsavedData.Tags.Count <= 6 && !string.IsNullOrEmpty (txtTag7.Text)) || (UnsavedData.Tags.Count >= 7 && txtTag7.Text != UnsavedData.Tags [6].Text);
			};
		}

		public override void SaveCard ()
		{
			if(!CardInfoChanged){
				return;
			}

			var tags = new List<string> ();
			tags.Add (txtTag1.Text);
			tags.Add (txtTag2.Text);
			tags.Add (txtTag3.Text);
			tags.Add (txtTag4.Text);
			tags.Add (txtTag5.Text);
			tags.Add (txtTag6.Text);
			tags.Add (txtTag7.Text);
			tags.RemoveAll (t => string.IsNullOrEmpty (t.Trim()));

			// Add new tags
			foreach (var tag in tags) {
				if (UnsavedData.Tags.FirstOrDefault (t => string.Equals (t.Text, tag, StringComparison.InvariantCultureIgnoreCase)) == null) {
					UnsavedData.Tags.Add (new Mobile.Models.Tag {
						Text = tag,
						TagTypeId = 1,
						Deleted = false
					});
				}
			}

			// Clear tags that have been removed
			var existingTags = new List<string> ();
			existingTags.AddRange (UnsavedData.Tags.Select (t => t.Text.ToLower ()).Distinct ());
			foreach (var tag in existingTags) {
				if (tags.FirstOrDefault (t => string.Equals (t, tag, StringComparison.InvariantCultureIgnoreCase)) == null) {
					UnsavedData.Tags.RemoveAll (t => t.Text.ToLower () == tag);
				}
			}

			UISubscriptionService.SaveCardInfo (new Mobile.Models.CardDetailModel (UnsavedData));

			base.SaveCard ();
		}
	}
}


