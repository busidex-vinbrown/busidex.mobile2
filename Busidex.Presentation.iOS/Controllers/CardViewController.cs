using System;
using UIKit;
using System.IO;
using Busidex.Mobile.Models;
using Busidex.Mobile;

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

		void ToggleImage(){
		
			switch(ViewState){
			case CardViewState.Loading:{
					var frontFileName = Path.Combine (documentsPath, UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType);
					if (File.Exists (frontFileName)) {
						btnCard.SetBackgroundImage (UIImage.FromFile (frontFileName), UIControlState.Normal);
					}else{
						Busidex.Mobile.Utils.DownloadImage (Resources.CARD_PATH + UserCard.Card.FrontFileName, documentsPath, UserCard.Card.FrontFileName).ContinueWith (t => {
							InvokeOnMainThread (() => btnCard.SetBackgroundImage (UIImage.FromFile (frontFileName), UIControlState.Normal));
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
							Busidex.Mobile.Utils.DownloadImage (Resources.CARD_PATH + UserCard.Card.BackFileName, documentsPath, UserCard.Card.BackFileName).ContinueWith (t => {
								InvokeOnMainThread (() => btnCard.SetBackgroundImage (UIImage.FromFile (backFileName), UIControlState.Normal));
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
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

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

			}

		}
	}
}


