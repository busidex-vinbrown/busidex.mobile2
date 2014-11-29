using System;
using UIKit;
using System.IO;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	partial class CardViewController : BaseController
	{
		public UserCard UserCard{ get; set; }
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }
		bool ShowingFrontImage = true;


		public CardViewController (IntPtr handle) : base (handle)
		{

		}

		void ToggleImage(){

			ShowingFrontImage = !ShowingFrontImage;

			var fileName = Path.Combine (documentsPath, ShowingFrontImage ? UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType : UserCard.Card.BackFileId + "." + UserCard.Card.BackType);
			if (File.Exists (fileName)) {

				btnCard.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal);

			}
		}

		public void LoadCard(){

			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			if (UserCard != null && UserCard.Card != null) {
				FrontFileName = Path.Combine (documentsPath, UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType);
				if (File.Exists (FrontFileName)) {

					btnCard.SetBackgroundImage (UIImage.FromFile (FrontFileName), UIControlState.Normal);

					ShowingFrontImage = true;
				}
			}

		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			try{
				LoadCard ();

				btnCard.TouchUpInside += delegate {
					if (UserCard.Card.BackFileId.ToString () != Busidex.Mobile.Resources.EMPTY_CARD_ID) {
						ToggleImage ();
					}
				};
			}catch(Exception ex){

			}

		}
	}
}


