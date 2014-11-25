using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Busidex.Mobile.Models;
using System.Drawing;

namespace Busidex.Presentation.iOS
{
	partial class CardViewController : UIViewController
	{
		private readonly string documentsPath;
		public UserCard UserCard{ get; set; }
		private string FrontFileName{ get; set; }
		private string BackFileName{ get; set; }
		private bool ShowingFrontImage = true;
		const string EMPTY_CARD_ID = "b66ff0ee-e67a-4bbc-af3b-920cd0de56c6";

		public CardViewController (IntPtr handle) : base (handle)
		{
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		}

		private void ToggleImage(){

			ShowingFrontImage = !ShowingFrontImage;

			var fileName = System.IO.Path.Combine (documentsPath, ShowingFrontImage ? UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType : UserCard.Card.BackFileId + "." + UserCard.Card.BackType);
			if (File.Exists (fileName)) {

				btnCard.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal);

			}
		}

		public void LoadCard(){

			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}

			if (UserCard != null && UserCard.Card != null) {
				FrontFileName = System.IO.Path.Combine (documentsPath, UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType);
				if (File.Exists (FrontFileName)) {

					btnCard.SetBackgroundImage (UIImage.FromFile (FrontFileName), UIControlState.Normal);

					ShowingFrontImage = true;
				}
			}
			//			var width = UIScreen.MainScreen.Bounds.Width;
			//			var height = UIScreen.MainScreen.Bounds.Height;
			//			var frame = new RectangleF (10f, 10f, width, height);
			//			btnCard.Frame = frame;
			//
			//			btnCard.SetNeedsLayout ();
		}

		public override void DidRotate(UIInterfaceOrientation orientation){

			base.DidRotate (orientation);
		}

		public override void ViewWillAppear (bool animated)
		{

			base.ViewWillAppear (animated);

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
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


