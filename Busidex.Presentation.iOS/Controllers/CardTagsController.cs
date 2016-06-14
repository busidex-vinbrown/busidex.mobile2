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

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Card Tags");
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (SelectedCard == null || SelectedCard.Tags == null) {
				return;
			}

			loadTags ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			btnSave.TouchUpInside += delegate {

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
						SelectedCard.Tags.Add (new Mobile.Models.Tag {
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

				UISubscriptionService.SaveCardInfo (new Mobile.Models.CardDetailModel (SelectedCard));

			};
		}
	}
}


