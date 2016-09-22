using System;
using UIKit;
using GoogleAnalytics.iOS;
using Busidex.Mobile.Models;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class VisibilityController : BaseCardEditController
	{
		const string DESCRIPTION_PUBLIC = "With this option your card will be completely searchable by your name, company, title, email, and any tag you have associated with your card. Anyone with a Busidex Account can search for your card.";
		const string DESCRIPTION_SEMI_PUBLIC = "Searching\nYour card will not be searchable by anyone except by those that have a Busidex account with whom your card has been shared.\n\nSharing\nIf you share with someone that does not have a Busidex account they will need to open an account to view your card in their Busidex page. Once you have shared your card, you give those with whom you shared your card the authorization to then share your card with whomever they wish. In other words, you authorize that your card can be shared with anyone by anyone that has your card.";
		const string DESCRIPTION_PRIVATE = "With this option your card can only be shared by you. Even those that have your card cannot share it. You are the only person that can give your card to others.";

		public VisibilityController (IntPtr handle) : base (handle)
		{
		}

		protected override void CardUpdated ()
		{
			base.CardUpdated ();

			InvokeOnMainThread (() => {
				switch (UnsavedData.Visibility) {
				case (int)CardVisibility.Public: {
						setVisibilityUI (CardVisibility.Public);
						break;
					}
				case (int)CardVisibility.SemiPublic: {
						setVisibilityUI (CardVisibility.SemiPublic);
						break;
					}
				case (int)CardVisibility.Private: {
						setVisibilityUI (CardVisibility.Private);
						break;
					}
				}
			});
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			UISubscriptionService.OnCardInfoSaved += CardUpdated;

			if(UnsavedData != null){
				switch (UnsavedData.Visibility) {
				case (int)CardVisibility.Public: {
						setVisibilityUI (CardVisibility.Public);
						break;
					}
				case (int)CardVisibility.SemiPublic: {
						setVisibilityUI (CardVisibility.SemiPublic);
						break;
					}
				case (int)CardVisibility.Private: {
						setVisibilityUI (CardVisibility.Private);
						break;
					}
				}
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Card Visibility");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnPublic.TouchUpInside += delegate {
				CardInfoChanged = true;
				setVisibilityUI (CardVisibility.Public);
			};

			btnSemiPublic.TouchUpInside += delegate {
				CardInfoChanged = true;
				setVisibilityUI (CardVisibility.SemiPublic);
			};

			btnPrivate.TouchUpInside += delegate {
				CardInfoChanged = true;
				setVisibilityUI (CardVisibility.Private);
			};
		}

		public override void SaveCard ()
		{
			if (!CardInfoChanged) {
				return;
			}

			UISubscriptionService.SaveCardVisibility (UnsavedData.Visibility);

			base.SaveCard ();
		}

		void setVisibilityUI (CardVisibility visibility)
		{
			const float DEFAULT_FONT_SIZE = 15F;

			switch (visibility) {
			case CardVisibility.Public: {
					setButtonState (btnPublic, true);
					setButtonState (btnSemiPublic, false);
					setButtonState (btnPrivate, false);
					txtDescription.Text = DESCRIPTION_PUBLIC;
					lblPublic.Font = UIFont.BoldSystemFontOfSize (DEFAULT_FONT_SIZE);
					lblSemiPublic.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					lblPrivate.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					break;
				}
			case CardVisibility.SemiPublic: {
					setButtonState (btnPublic, false);
					setButtonState (btnSemiPublic, true);
					setButtonState (btnPrivate, false);
					txtDescription.Text = DESCRIPTION_SEMI_PUBLIC;
					lblPublic.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					lblSemiPublic.Font = UIFont.BoldSystemFontOfSize (DEFAULT_FONT_SIZE);
					lblPrivate.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					break;
				}
			case CardVisibility.Private: {
					setButtonState (btnPublic, false);
					setButtonState (btnSemiPublic, false);
					setButtonState (btnPrivate, true);
					txtDescription.Text = DESCRIPTION_PRIVATE;
					lblPublic.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					lblSemiPublic.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					lblPrivate.Font = UIFont.BoldSystemFontOfSize (DEFAULT_FONT_SIZE);
					break;
				}
			}

			UnsavedData.Visibility = (byte)visibility;
		}

		void setButtonState (UIButton button, bool state)
		{
			button.SetImage (state ? UIImage.FromFile ("radio_on.png") : UIImage.FromFile ("radio_off.png"), UIControlState.Normal);
		}
	}
}
