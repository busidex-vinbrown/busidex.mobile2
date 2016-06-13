using System;
using Busidex.Mobile;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	public partial class SearchInfoController : BaseCardEditController
	{

		public SearchInfoController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Search Info");

			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			//var frame = lblInstructions.Frame;
			//lblInstructions.Frame = new CoreGraphics.CGRect (frame.X, 95f, frame.Width, frame.Height);

			if (SelectedCard != null) {
				txtCompanyName.Text = SelectedCard.CompanyName;
				txtName.Text = SelectedCard.Name;
				txtTitle.Text = SelectedCard.Title;
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnSave.TouchUpInside += delegate {
				if (txtCompanyName.Text != SelectedCard.CompanyName ||
					txtName.Text != SelectedCard.Name ||
					txtTitle.Text != SelectedCard.Title) {

					SelectedCard.CompanyName = txtCompanyName.Text;
					SelectedCard.Name = txtName.Text;
					SelectedCard.Title = txtTitle.Text;

					UISubscriptionService.SaveCardInfo (new Mobile.Models.CardDetailModel (SelectedCard));
				}
			};
		}
	}
}


