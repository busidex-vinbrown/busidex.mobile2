
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

		protected UITableView TableView;

		protected UIImageView cardImagePortrait;
		protected UIImageView cardImageLandscape;
		protected UserCard selectedCard;

		protected string FrontFileName{ get; set; }
		protected string BackFileName{ get; set; }

		protected enum CardViewState{
			Loading = 1,
			Front = 2,
			Back = 3
		}

		protected CardViewState ViewState {get;set;}

		protected void ShowOverlay(){
			Overlay = new CardLoadingOverlay (View.Bounds);
			Overlay.MessageText = "Loading Your Card";
			View.AddSubview (Overlay);
		}

		protected void ToggleImage(){

			switch(ViewState){
			case CardViewState.Loading:{
					var frontFileName = Path.Combine (documentsPath, selectedCard.Card.FrontFileId + "." + selectedCard.Card.FrontType);
					if (File.Exists (frontFileName)) {
						if(selectedCard.Card.FrontOrientation == "H"){
							cardImageLandscape.Image = new UIImage(UIImage.FromFile (frontFileName).CGImage, 1, UIImageOrientation.Right);
						}else{
							cardImagePortrait.Image = UIImage.FromFile (frontFileName);
						}

					}else{

						ShowOverlay ();

						Utils.DownloadImage (Resources.CARD_PATH + selectedCard.Card.FrontFileName, documentsPath, selectedCard.Card.FrontFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								if(selectedCard.Card.FrontOrientation == "H"){
									cardImageLandscape.Image = new UIImage(UIImage.FromFile (frontFileName).CGImage, 1, UIImageOrientation.Right);
								}else{
									cardImagePortrait.Image = UIImage.FromFile (frontFileName);
								}
								Overlay.Hide();
							});
						});
					}
					ViewState = CardViewState.Front;

					TableView.Hidden = true;
					cardImageLandscape.Hidden = selectedCard.Card.FrontOrientation == "V";
					cardImagePortrait.Hidden = selectedCard.Card.FrontOrientation == "H";

					break;
				}
			case CardViewState.Front:{
					var backFileName = Path.Combine (documentsPath, selectedCard.Card.BackFileId + "." + selectedCard.Card.BackType);
					if (selectedCard.Card.BackFileId.ToString().Equals (Resources.EMPTY_CARD_ID)) {
						HideCardDetail ();
						break;
					}  

					if (File.Exists (backFileName)) {
						if(selectedCard.Card.FrontOrientation == "H"){
							cardImageLandscape.Image = new UIImage(UIImage.FromFile (backFileName).CGImage, 1, UIImageOrientation.Right);
						}else{
							cardImagePortrait.Image = UIImage.FromFile (backFileName);
						}
					} else {
						ShowOverlay ();
						Utils.DownloadImage (Resources.CARD_PATH + selectedCard.Card.BackFileName, documentsPath, selectedCard.Card.BackFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								if(selectedCard.Card.BackOrientation == "H"){
									cardImageLandscape.Image = new UIImage(UIImage.FromFile (backFileName).CGImage, 1, UIImageOrientation.Right);
								}else{
									cardImagePortrait.Image = UIImage.FromFile (backFileName);
								}
								Overlay.Hide();
							});
						});
					}


					ViewState = CardViewState.Back;
					TableView.Hidden = true;
					cardImageLandscape.Hidden = selectedCard.Card.FrontOrientation == "V";
					cardImagePortrait.Hidden = selectedCard.Card.FrontOrientation == "H";

					break;
				}
			case CardViewState.Back:{
					HideCardDetail ();
					break;
				}
			}
		}

		protected void HideCardDetail(){
			TableView.Hidden = false;
			cardImageLandscape.Hidden = cardImagePortrait.Hidden = true; 
			cardImageLandscape.Image = cardImagePortrait.Image = null;
		}

		protected void GoToCard(){
			ViewState = CardViewState.Loading;
			selectedCard = ((BaseTableSource)TableView.Source).SelectedCard;

			AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_DETAILS, selectedCard.Card.Name, 0);

			ToggleImage ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			cardImagePortrait = new UIImageView ();
			cardImageLandscape = new UIImageView ();
			var togglerP = new UIButton ();
			var togglerL = new UIButton ();

			var navBarHeight = NavigationController.NavigationBar.Frame.Size.Height;

			cardImagePortrait.Frame = new CGRect (0, navBarHeight, View.Frame.Width, View.Frame.Height - navBarHeight);
			cardImageLandscape.Frame = new CGRect (0, navBarHeight, View.Frame.Width, View.Frame.Height - navBarHeight);

			cardImagePortrait.UserInteractionEnabled = cardImageLandscape.UserInteractionEnabled = true;


			togglerP.Frame = new CGRect (0, navBarHeight, View.Frame.Width, View.Frame.Height - navBarHeight);
			togglerL.Frame = new CGRect (0, navBarHeight, View.Frame.Width, View.Frame.Height - navBarHeight);

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

