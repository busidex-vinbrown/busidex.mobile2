﻿using System;
using Busidex.Mobile;
using Google.Analytics;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class SearchInfoController : BaseCardEditController
	{
		public SearchInfoController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Search Info");

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

			txtCompanyName.ResignFirstResponder ();
			txtName.ResignFirstResponder ();
			txtTitle.ResignFirstResponder ();

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			txtCompanyName.AllEditingEvents += (sender, e) => {
				CardInfoChanged = CardInfoChanged || txtCompanyName.Text != (UnsavedData.CompanyName ?? string.Empty);
			};

			txtName.AllEditingEvents += (sender, e) => {
				CardInfoChanged = CardInfoChanged || txtName.Text != (UnsavedData.Name ?? string.Empty);
			};

			txtTitle.AllEditingEvents += (sender, e) => {
				CardInfoChanged = CardInfoChanged || txtTitle.Text != (UnsavedData.Title ?? string.Empty);
			};
		}

		public override void SaveCard ()
		{
			if (!CardInfoChanged) {
				return;
			}

			UnsavedData.CompanyName = txtCompanyName.Text;
			UnsavedData.Name = txtName.Text;
			UnsavedData.Title = txtTitle.Text;

			UISubscriptionService.SaveCardInfo (new Mobile.Models.CardDetailModel (UnsavedData));

			base.SaveCard ();
		}
	}
}
