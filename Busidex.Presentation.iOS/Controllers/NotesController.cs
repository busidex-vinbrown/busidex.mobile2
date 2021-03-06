﻿using System;
using Foundation;
using UIKit;
using System.IO;
using System.Drawing;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Linq;
using Google.Analytics;
using BaseController = Busidex.Presentation.iOS.Controllers.BaseController;

namespace Busidex.Presentation.iOS
{
	partial class NotesController : BaseController
	{
		public UserCard SelectedCard{ get; set; }
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }

		UIView activeview;             // Controller that activated the keyboard
		nfloat scroll_amount = 0.0f;    // amount to scroll 
		nfloat bottom = 0.0f;           // bottom point
		const float OFFSET = 10.0f;          // extra offset
		bool moveViewUp;           // which direction are we moving

		public NotesController (IntPtr handle) : base (handle)
		{
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		void LoadCard(){

			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			if (SelectedCard != null && SelectedCard.Card != null) {

				if(SelectedCard.UserCardId == 0){
					var mbCard = UISubscriptionService.UserCards.SingleOrDefault (uc => uc.CardId == SelectedCard.CardId);
					if(mbCard != null){
						SelectedCard.UserCardId = mbCard.UserCardId;
					}
				}
				BusinessCardDimensions dimensions = GetCardDimensions (SelectedCard.Card.FrontOrientation);
				imgCard.Frame = new CoreGraphics.CGRect (dimensions.MarginLeft, 75f, dimensions.Width, dimensions.Height);

				if (SelectedCard != null) {
					FrontFileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName);
					if (File.Exists (FrontFileName)) {
						imgCard.Image = UIImage.FromFile (FrontFileName);
					}
					txtNotes.Text = SelectedCard.Notes;
				}
			}
			imgSaved.Hidden = true;
		}

		public void KBToolbarButtonDoneHandler(object sender, EventArgs e)
		{
			txtNotes.ResignFirstResponder();
		}

		void SaveNotes(){
			UISubscriptionService.SaveNotes (SelectedCard.UserCardId, txtNotes.Text.Trim ());
		}

		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Notes");

			base.ViewDidAppear (animated);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			try{
				LoadCard ();
			}catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var keyboardDoneButtonToolbar = new UIToolbar(RectangleF.Empty)
			{
				BarStyle = UIBarStyle.Black,
				Translucent = true,
				UserInteractionEnabled = true,
				TintColor = null
			};
			keyboardDoneButtonToolbar.SizeToFit();

			// NOTE Need 2 spacer elements here and not sure why...
			var btnKeyboardDone = new UIBarButtonItem (UIBarButtonSystemItem.Done, KBToolbarButtonDoneHandler);
			keyboardDoneButtonToolbar.SetItems(new []
				{
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null),
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null),
					btnKeyboardDone
				}, true);

			txtNotes.InputAccessoryView = keyboardDoneButtonToolbar;

			// Keyboard popup
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification,KeyBoardUpNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification,KeyBoardDownNotification);

			btnSaveNotes.TouchUpInside += delegate {
				SaveNotes();
			};

			txtNotes.Changed += delegate {
				imgSaved.Hidden = true;
			};

			OnNotesUpdatedEventHandler callback = () => InvokeOnMainThread (() => {				
				imgSaved.Hidden = false;
			});

			UISubscriptionService.OnNotesUpdated += callback;
		}

		void KeyBoardDownNotification(NSNotification notification)
		{
			if(moveViewUp){ScrollTheView(false);}
		}

		void ScrollTheView(bool move)
		{

			// scroll the view up or down
			UIView.BeginAnimations (string.Empty, IntPtr.Zero);
			UIView.SetAnimationDuration (0.3);

			var frame = View.Frame;

			if (move) {
				frame.Y -= scroll_amount;
			} else {
				frame.Y += scroll_amount;
				scroll_amount = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}

		void KeyBoardUpNotification(NSNotification notification)
		{
			// Find what opened the keyboard
			foreach (UIView view in View.Subviews) {
				if (view.IsFirstResponder) {
					activeview = view;
					break;
				}
			}

			// Bottom of the controller = initial position + height + offset  
			bottom = activeview != null ? (activeview.Frame.Y + activeview.Frame.Height + OFFSET) : 320;
			// Calculate how far we need to scroll
			//scroll_amount = (r.Height - (View.Frame.Size.Height - bottom)) ;
			scroll_amount = (260 - (View.Frame.Size.Height - bottom)) ;
			// Perform the scrolling
			if (scroll_amount > 0) {
				moveViewUp = true;
				ScrollTheView (moveViewUp);
			} else {
				moveViewUp = false;
			}
		}
	}
}
