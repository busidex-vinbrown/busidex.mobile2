
using System;
using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using CoreGraphics;

namespace Busidex.Presentation.iOS
{
	public partial class BaseCardViewController : UIBarButtonItemWithImageViewController
	{
		public BaseCardViewController (IntPtr handle) : base (handle)
		{
		}

		protected UIImageView cardImagePortrait;
		protected UIImageView cardImageLandscape;
		public UserCard SelectedCard;

		protected string FrontFileName{ get; set; }
		protected string BackFileName{ get; set; }

		protected enum CardViewState{
			Loading = 1,
			Front = 2,
			Back = 3
		}

		protected CardViewState ViewState {get;set;}

		protected void ToggleImage(){

			switch(ViewState){
			case CardViewState.Loading:{
					NavigationController.SetNavigationBarHidden (true, false);

					var frontFileName = Path.Combine (documentsPath, SelectedCard.Card.FrontFileId + "." + SelectedCard.Card.FrontType);
					if (File.Exists (frontFileName)) {
						if(SelectedCard.Card.FrontOrientation == "H"){
							cardImageLandscape.Image = new UIImage(UIImage.FromFile (frontFileName).CGImage, 1, UIImageOrientation.Right);
						}else{
							cardImagePortrait.Image = UIImage.FromFile (frontFileName);
						}

					}else{

						ShowOverlay ();

						Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.Card.FrontFileName, documentsPath, SelectedCard.Card.FrontFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								if(SelectedCard.Card.FrontOrientation == "H"){
									cardImageLandscape.Image = new UIImage(UIImage.FromFile (frontFileName).CGImage, 1, UIImageOrientation.Right);
								}else{
									cardImagePortrait.Image = UIImage.FromFile (frontFileName);
								}
								Overlay.Hide();
							});
						});
					}
					ViewState = CardViewState.Front;

					cardImageLandscape.Hidden = SelectedCard.Card.FrontOrientation == "V";
					cardImagePortrait.Hidden = SelectedCard.Card.FrontOrientation == "H";

					break;
				}
			case CardViewState.Front:{
					NavigationController.SetNavigationBarHidden (true, false);
					var backFileName = Path.Combine (documentsPath, SelectedCard.Card.BackFileId + "." + SelectedCard.Card.BackType);
					if (SelectedCard.Card.BackFileId.ToString().Equals (Resources.EMPTY_CARD_ID) ||
						SelectedCard.Card.BackFileId.ToString().Equals (Resources.NULL_CARD_ID)) {
						HideCardDetail ();
						break;
					}  

					if (File.Exists (backFileName)) {
						if(SelectedCard.Card.BackOrientation == "H"){
							cardImageLandscape.Image = new UIImage(UIImage.FromFile (backFileName).CGImage, 1, UIImageOrientation.Right);
						}else{
							cardImagePortrait.Image = UIImage.FromFile (backFileName);
						}
					} else {
						ShowOverlay ();
						Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.Card.BackFileName, documentsPath, SelectedCard.Card.BackFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								if(SelectedCard.Card.BackOrientation == "H"){
									cardImageLandscape.Image = new UIImage(UIImage.FromFile (backFileName).CGImage, 1, UIImageOrientation.Right);
								}else{
									cardImagePortrait.Image = UIImage.FromFile (backFileName);
								}
								Overlay.Hide();
							});
						});
					}

					ViewState = CardViewState.Back;
					cardImageLandscape.Hidden = SelectedCard.Card.BackOrientation == "V";
					cardImagePortrait.Hidden = SelectedCard.Card.BackOrientation == "H";

					break;
				}
			case CardViewState.Back:{
					HideCardDetail ();
					break;
				}
			}
		}

		protected void HideCardDetail(){
			NavigationController.SetNavigationBarHidden (false, false);
			NavigationController.PopViewController (true);
		}

		protected void GoToCard(){
			ViewState = CardViewState.Loading;

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_DETAILS, SelectedCard.Card.Name, 0);

			ToggleImage ();
		}

		protected void ShowCardActions(UserCard card){
			
			buttonPanelController.SelectedCard = card;

			if (buttonPanelController != null) {
				NavigationController.PushViewController (buttonPanelController, true);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			cardImagePortrait = new UIImageView ();
			cardImageLandscape = new UIImageView ();
			var togglerP = new UIButton ();
			var togglerL = new UIButton ();

			cardImagePortrait.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height);
			cardImageLandscape.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height);

			cardImagePortrait.UserInteractionEnabled = cardImageLandscape.UserInteractionEnabled = true;


			togglerP.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height);
			togglerL.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height);

			togglerP.TouchUpInside += delegate {
				ToggleImage();
			};
			togglerL.TouchUpInside += delegate {
				ToggleImage();
			};
			cardImagePortrait.AddSubview (togglerP);
			cardImageLandscape.AddSubview (togglerL);

			cardImageLandscape.ContentMode = UIViewContentMode.ScaleToFill;

			View.AddSubview (cardImagePortrait);
			View.AddSubview (cardImageLandscape);

			cardImagePortrait.Hidden = cardImageLandscape.Hidden = true;
		}
	}
}

