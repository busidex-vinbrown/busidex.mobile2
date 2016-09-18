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

		protected const string CARD_UPDATED_LOSE_WARNING = "You have unsaved changes, Continue and lose all changes or Go Back to save those changes?";
		protected const string CARD_UPDATED_SAVE_WARNING = "You have unsaved changes, continue and save those changes?";
		LoadingOverlay overlay;
		public bool CardInfoChanged { get; set; }

		public BaseCardEditController (IntPtr handle) : base (handle)
		{
		}

		protected virtual void CancelChanges(){
			
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
				Application.ShowAlert ("Unsaved Changes", CARD_UPDATED_LOSE_WARNING, new string [] {
					"Continue",
					"Go Back"
				}).ContinueWith (async button => {
					if (await button == 0) {
						InvokeOnMainThread (() => {
							CancelChanges ();
							GoToCardEditMenu ();
						});
					}
				});
			}else{
				GoToCardEditMenu ();
			}

		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
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


