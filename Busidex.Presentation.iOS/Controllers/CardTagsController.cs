using System;

using GoogleAnalytics.iOS;
using System.Linq;

namespace Busidex.Presentation.iOS
{
	public partial class CardTagsController : BaseCardEditController
	{

		public CardTagsController (IntPtr handle) : base (handle)
		{
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

			var userTags = SelectedCard.Tags.Where (t => t.TagType == 1).ToList ();
			for (var i = 0; i < userTags.Count; i++) {

				switch (i) {
				case 0:
					{
						txtTag1.Text = userTags [i].Text;
						break;
					}
				case 1:
					{
						txtTag2.Text = userTags [i].Text;
						break;
					}
				case 2:
					{
						txtTag3.Text = userTags [i].Text;
						break;
					}
				case 3:
					{
						txtTag4.Text = userTags [i].Text;
						break;
					}
				case 4:
					{
						txtTag5.Text = userTags [i].Text;
						break;
					}
				case 5:
					{
						txtTag6.Text = userTags [i].Text;
						break;
					}
				case 6:
					{
						txtTag7.Text = userTags [i].Text;
						break;
					}
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
	}
}


