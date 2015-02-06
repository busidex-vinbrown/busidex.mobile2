using System;
using UIKit;
using System.IO;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	partial class CardViewController : BaseController
	{
		public UserCard UserCard{ get; set; }
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }
		enum CardViewState{
			Loading = 1,
			Front = 2,
			Back = 3
		}

		CardViewState ViewState {get;set;}

		public CardViewController (IntPtr handle) : base (handle)
		{

		}

		void ShowOverlay(){
			Overlay = new CardLoadingOverlay (View.Bounds);
			Overlay.MessageText = "Loading Your Card";
			View.AddSubview (Overlay);
		}

		void ToggleImage(){
		
			switch(ViewState){
			case CardViewState.Loading:{
					var frontFileName = Path.Combine (documentsPath, UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType);
					if (File.Exists (frontFileName)) {
						btnCard.SetBackgroundImage (UIImage.FromFile (frontFileName), UIControlState.Normal);
					}else{

						ShowOverlay ();

						Busidex.Mobile.Utils.DownloadImage (Resources.CARD_PATH + UserCard.Card.FrontFileName, documentsPath, UserCard.Card.FrontFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								btnCard.SetBackgroundImage (UIImage.FromFile (frontFileName), UIControlState.Normal);
								Overlay.Hide();
							});
						});
					}
					ViewState = CardViewState.Front;
					break;
				}
			case CardViewState.Front:{
					var backFileName = Path.Combine (documentsPath, UserCard.Card.BackFileId + "." + UserCard.Card.BackType);
					if (UserCard.Card.BackFileId.ToString().Equals (Resources.EMPTY_CARD_ID)) {
						NavigationController.PopViewController (true);
					} else {
						if (File.Exists (backFileName)) {
							btnCard.SetBackgroundImage (UIImage.FromFile (backFileName), UIControlState.Normal);
						} else {
							ShowOverlay ();
							Busidex.Mobile.Utils.DownloadImage (Resources.CARD_PATH + UserCard.Card.BackFileName, documentsPath, UserCard.Card.BackFileName).ContinueWith (t => {
								InvokeOnMainThread (() => {
									btnCard.SetBackgroundImage (UIImage.FromFile (backFileName), UIControlState.Normal);
									Overlay.Hide();
								});
							});
						}
					}
					ViewState = CardViewState.Back;
					break;
				}
			case CardViewState.Back:{
					NavigationController.PopViewController(true);
					break;
				}
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			if (UserCard != null && UserCard.Card != null && !string.IsNullOrEmpty (UserCard.Card.Name)) {
				GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Card Detail - " + UserCard.Card.Name);
			}

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var userToken = GetAuthCookie ();
			try{

				if(NavigationController != null){
					NavigationController.SetNavigationBarHidden(true, true);
				}

				ViewState = CardViewState.Loading;

				ToggleImage();

				btnCard.TouchUpInside += delegate {
					ToggleImage();
				};

			}catch(Exception ex){
				LoggingController.LogError (ex, userToken != null ? userToken.Value : string.Empty);
			}

		}
	}
}


