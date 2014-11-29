using System;
using UIKit;
using System.IO;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	partial class CardViewController : UIViewController
	{
		readonly string documentsPath;
		public UserCard UserCard{ get; set; }
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }
		bool ShowingFrontImage = true;
		const string EMPTY_CARD_ID = "b66ff0ee-e67a-4bbc-af3b-920cd0de56c6";

		public CardViewController (IntPtr handle) : base (handle)
		{
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

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
					if (UserCard.Card.BackFileId.ToString () != EMPTY_CARD_ID) {
						ToggleImage ();
					}
				};
			}catch(Exception ex){

			}

		}
	}
}


