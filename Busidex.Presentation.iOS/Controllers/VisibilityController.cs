﻿using System;

using UIKit;
using GoogleAnalytics.iOS;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	public partial class VisibilityController : BaseCardEditController
	{
		const string DESCRIPTION_PUBLIC = "With this option your card will be completely searchable by your name, company, title, email, and any tag you have associated with your card. Anyone with a Busidex Account can search for your card.";
		const string DESCRIPTION_SEMI_PUBLIC = "Searching\nYour card will not be searchable by anyone except by those that have a Busidex account with whom your card has been shared.\n\nSharing\nIf you share with someone that does not have a Busidex account they will need to open an account to view your card in their Busidex page. Once you have shared your card, you give those with whom you shared your card the authorization to then share your card with whomever they wish. In other words, you authorize that your card can be shared with anyone by anyone that has your card.";
		const string DESCRIPTION_PRIVATE = "With this option your card can only be shared by you. Even those that have your card cannot share it. You are the only person that can give your card to others.";

		enum Visibility
		{
			Public = 0,
			SemiPublic = 1,
			Private = 2
		}

		public VisibilityController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			switch (SelectedCard.Visibility) {
			case (int)Visibility.Public:
				{
					setVisibilityUI (Visibility.Public); 
					break;
				}
			case (int)Visibility.SemiPublic:
				{
					setVisibilityUI (Visibility.SemiPublic); 
					break;
				}
			case (int)Visibility.Private:
				{
					setVisibilityUI (Visibility.Private); 
					break;
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
				setVisibilityUI (Visibility.Public);
			};

			btnSemiPublic.TouchUpInside += delegate {
				setVisibilityUI (Visibility.SemiPublic);
			};

			btnPrivate.TouchUpInside += delegate {
				setVisibilityUI (Visibility.Private);
			};
		}

		void setVisibilityUI (Visibility visibility)
		{
			const float DEFAULT_FONT_SIZE = 15F;

			switch (visibility) {
			case Visibility.Public:
				{
					setButtonState (btnPublic, true);
					setButtonState (btnSemiPublic, false);
					setButtonState (btnPrivate, false);
					txtDescription.Text = DESCRIPTION_PUBLIC;
					lblPublic.Font = UIFont.BoldSystemFontOfSize (DEFAULT_FONT_SIZE);
					lblSemiPublic.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					lblPrivate.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					break;
				}
			case Visibility.SemiPublic:
				{
					setButtonState (btnPublic, false);
					setButtonState (btnSemiPublic, true);
					setButtonState (btnPrivate, false);
					txtDescription.Text = DESCRIPTION_SEMI_PUBLIC;
					lblPublic.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					lblSemiPublic.Font = UIFont.BoldSystemFontOfSize (DEFAULT_FONT_SIZE);
					lblPrivate.Font = UIFont.SystemFontOfSize (DEFAULT_FONT_SIZE);
					break;
				}
			case Visibility.Private:
				{
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
		}

		void setButtonState (UIButton button, bool state)
		{
			button.SetImage (state ? UIImage.FromFile ("radio_on.png") : UIImage.FromFile ("radio_off.png"), UIControlState.Normal);
		}
	}
}


