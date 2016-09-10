using System;

using Busidex.Mobile.Models;
using Busidex.Mobile;
using UIKit;

namespace Busidex.Presentation.iOS
{
	public partial class BaseCardEditController : BaseController
	{
		protected Card SelectedCard { get; set; }
		protected Card UnsavedData { get; set; }

		LoadingOverlay overlay;
		public bool CardInfoChanged { get; set; }

		public BaseCardEditController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SelectedCard = UISubscriptionService.OwnedCard;
			UnsavedData = new Card (SelectedCard);

			CardInfoChanged = false;

			UISubscriptionService.OnCardInfoUpdating -= CardUpdating;
			UISubscriptionService.OnCardInfoUpdating += CardUpdating;

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			UISubscriptionService.OnCardInfoSaved += CardUpdated;

			if (SelectedCard.CardId <= 0) {
				UISubscriptionService.LoadOwnedCard ();
			}

			if (overlay == null) {
				overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			}

			NavigationItem.SetHidesBackButton (true, false); 

			NavigationItem.SetRightBarButtonItem (
					new UIBarButtonItem (UIBarButtonSystemItem.Save, (sender, args) => SaveCard ())
					, true);

			NavigationItem.SetLeftBarButtonItem (
					new UIBarButtonItem (UIBarButtonSystemItem.Done, (sender, args) => GoBack ())
					, true);
		}

		void GoBack(){

			if (CardInfoChanged) {
				Application.ShowAlert ("Unsaved Changes", "You have unsaved changes, continue and lose those changes?", new string [] {
					"Ok",
					"Cancel"
				}).ContinueWith (async button => {
					if (await button == 0) {
						InvokeOnMainThread (GoToCardEditMenu);
					}
				});
			}else{
				GoToCardEditMenu ();
			}

		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			//CardUpdated ();
			//UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			//UISubscriptionService.OnCardInfoUpdating -= CardUpdating;
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

		protected virtual void CardUpdated ()
		{
			SelectedCard = UISubscriptionService.OwnedCard;

			UnsavedData = new Card (SelectedCard);

			CardInfoChanged = false;

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


