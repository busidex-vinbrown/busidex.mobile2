using System;

using UIKit;
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

//			txtCompanyName = vwFields.Subviews.GetValue (1) as UITextField;
//			txtName = vwFields.Subviews.GetValue (3) as UITextField;
//			txtTitle = vwFields.Subviews.GetValue (5) as UITextField;

			if (SelectedCard != null) {
				txtCompanyName.Text = SelectedCard.CompanyName;
				txtName.Text = SelectedCard.Name;
				txtTitle.Text = SelectedCard.Title;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			var frame = lblInstructions.Frame;
			lblInstructions.Frame = new CoreGraphics.CGRect (frame.X, 95f, frame.Width, frame.Height);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var btnSave = vwFields.Subviews.GetValue (6) as UIButton;
			btnSave.TouchUpInside += delegate {

				SaveCard ();
			};
		}

		public override void SaveCard ()
		{
			if (txtCompanyName.Text != SelectedCard.CompanyName ||
			    txtName.Text != SelectedCard.Name ||
			    txtTitle.Text != SelectedCard.Title) {

				base.SaveCard ();
			}
		}
	}
}


