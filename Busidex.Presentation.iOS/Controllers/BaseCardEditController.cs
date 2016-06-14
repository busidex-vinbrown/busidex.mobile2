using System;

using Busidex.Mobile.Models;
using Busidex.Mobile;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public partial class BaseCardEditController : BaseController
	{
		protected Card SelectedCard { get; set; }
		LoadingOverlay overlay;

		public BaseCardEditController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SelectedCard = UISubscriptionService.OwnedCard;

			UISubscriptionService.OnCardInfoUpdating -= CardUpdating;
			UISubscriptionService.OnCardInfoUpdating += CardUpdating;

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			UISubscriptionService.OnCardInfoSaved += CardUpdated;

			if (overlay == null) {
				overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			CardUpdated ();
			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			UISubscriptionService.OnCardInfoUpdating -= CardUpdating;
		}

		protected void CardUpdating ()
		{
			InvokeOnMainThread (() => {
				if (overlay == null) {
					overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				}

				overlay.MessageText = "Saving your card information...";
				View.Add (overlay);
			});
		}

		protected void CardUpdated ()
		{
			SelectedCard = UISubscriptionService.OwnedCard;

			InvokeOnMainThread (() => {
				if (overlay != null) {
					overlay.Hide ();
				}
			});
		}

		public virtual void SaveCard ()
		{

		}
	}
}


