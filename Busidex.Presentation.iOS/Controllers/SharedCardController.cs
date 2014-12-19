
using System;

using Foundation;
using UIKit;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;

namespace Busidex.Presentation.iOS
{
	public partial class SharedCardController : BaseController
	{
		public UserCard UserCard{ get; set; }
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }

		public SharedCardController (IntPtr handle) : base (handle)
		{
		}

		void LoadCard(){

			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
			}

			if (UserCard != null && UserCard.Card != null) {

				var fullFilePath = Path.Combine (documentsPath, Resources.MY_BUSIDEX_FILE);
				UserCard userCard = null;
				if (File.Exists (fullFilePath)) {
					using (var myBusidexFile = File.OpenText (fullFilePath)) {
						var myBusidexJson = myBusidexFile.ReadToEnd ();
						MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
						foreach(var uc in myBusidexResponse.MyBusidex.Busidex){
							if(uc.Card.CardId == UserCard.Card.CardId){
								userCard = uc;
								break;
							}
						}
					}
				}

				if (userCard != null) {
					FrontFileName = Path.Combine (documentsPath, Resources.THUMBNAIL_FILE_NAME_PREFIX + userCard.Card.FrontFileName);
					if (File.Exists (FrontFileName)) {
						imgCard.Image = UIImage.FromFile (FrontFileName);
					}
				}
			}
			imgCardShared.Hidden = true;
		}

		public void ShareCard(){

			if(string.IsNullOrEmpty(txtEmail.Text)){
				return;
			}

			lblError.Hidden = true;

			var cookie = GetAuthCookie ();

			var controller = new Busidex.Mobile.SharedCardController ();
			var response = controller.ShareCard (UserCard.Card, txtEmail.Text, cookie.Value);

			if( !string.IsNullOrEmpty(response) && response.Contains("true")){
				imgCardShared.Hidden = false;
			}else{
				lblError.Hidden = false;
				imgCardShared.Hidden = true;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				const bool HIDDEN = false;
				NavigationController.SetNavigationBarHidden (HIDDEN, true);

				var imgFrame = new CoreGraphics.CGRect (UIScreen.MainScreen.Bounds.Width * .70f, 5f, 100f, 25f);
				var shareImage = UIButton.FromType (UIButtonType.System);
				shareImage.Frame = imgFrame;
				shareImage.Font = UIFont.FromName ("Helvetica", 17f);

				shareImage.SetTitle ("Share", UIControlState.Normal);
				shareImage.TouchUpInside += ((s, e) => ShareCard ());
				var shareButton = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
				shareButton.CustomView = shareImage;

				NavigationItem.SetRightBarButtonItem(
					shareButton, true);
			}

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			lblError.Hidden = true;
			imgCardShared.Hidden = true;

			LoadCard ();

		}
	}
}

