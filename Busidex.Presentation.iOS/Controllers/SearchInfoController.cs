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

			if (UnsavedData != null) {
				txtCompanyName.Text = UnsavedData.CompanyName;
				txtName.Text = UnsavedData.Name;
				txtTitle.Text = UnsavedData.Title;
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			txtCompanyName.EditingDidBegin += (sender, e) => {
				CardInfoChanged = true;
			};

			txtName.EditingDidBegin += (sender, e) => {
				CardInfoChanged = true;
			};

			txtTitle.EditingDidBegin += (sender, e) => {
				CardInfoChanged = true;
			};
		}

		public override void SaveCard ()
		{
			if (txtCompanyName.Text != UnsavedData.CompanyName ||
					txtName.Text != UnsavedData.Name ||
					txtTitle.Text != UnsavedData.Title) {

				UnsavedData.CompanyName = txtCompanyName.Text;
				UnsavedData.Name = txtName.Text;
				UnsavedData.Title = txtTitle.Text;

				UISubscriptionService.SaveCardInfo (new Mobile.Models.CardDetailModel (UnsavedData));
			}

			base.SaveCard ();
		}
	}
}
