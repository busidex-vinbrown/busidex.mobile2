using System;

using UIKit;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public partial class CardImageController : BaseCardEditController
	{
		//BusinessCardDimensions frontDimensions;
		//BusinessCardDimensions backDimensions;
		enum DisplayMode
		{
			Front = 1,
			Back = 2
		}

		public CardImageController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SelectedCard = UISubscriptionService.OwnedCard;

			var frame = new CoreGraphics.CGRect (btnCardImage.Frame.Left, btnCardImage.Frame.Top, btnCardImage.Frame.Width, btnCardImage.Frame.Height);

			btnCardImage.Layer.AddSublayer (GetBorder (frame, UIColor.Gray.CGColor));

			try {
				if (!SelectedCard.FrontFileId.ToString ().Equals (Resources.EMPTY_CARD_ID) &&
				    !SelectedCard.FrontFileId.ToString ().Equals (Resources.NULL_CARD_ID)) {

					var frontFileName = Path.Combine (documentsPath, SelectedCard.FrontFileId + "." + SelectedCard.FrontType);
					if (File.Exists (frontFileName)) {
						setDisplay (frontFileName);
					} else {

						ShowOverlay ();

						Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.FrontFileName, documentsPath, SelectedCard.FrontFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								setDisplay (frontFileName);
								Overlay.Hide ();
							});
						});
					}
				} else {
					setDisplay (string.Empty);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			lblTitle.Frame = new CoreGraphics.CGRect (lblTitle.Frame.Left, lblTitle.Frame.Top, UIScreen.MainScreen.Bounds.Width, lblTitle.Frame.Height);
			btnBack.TouchUpInside += delegate {
				toggle (DisplayMode.Back);
			};
			btnFront.TouchUpInside += delegate {
				toggle (DisplayMode.Front);
			};
		}

		void toggle (DisplayMode mode)
		{
			string fileName;
			if (mode == DisplayMode.Front) {
				btnFront.SetTitleColor (UIColor.White, UIControlState.Normal);
				btnFront.BackgroundColor = UIColor.Blue;

				btnBack.SetTitleColor (UIColor.Blue, UIControlState.Normal);
				btnBack.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

				fileName = Path.Combine (documentsPath, SelectedCard.FrontFileId + "." + SelectedCard.FrontType);	
			} else {
				btnBack.SetTitleColor (UIColor.White, UIControlState.Normal);
				btnBack.BackgroundColor = UIColor.Blue;

				btnFront.SetTitleColor (UIColor.Blue, UIControlState.Normal);
				btnFront.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

				if (SelectedCard.BackFileId.ToString ().Equals (Resources.EMPTY_CARD_ID) ||
				    SelectedCard.BackFileId.ToString ().Equals (Resources.NULL_CARD_ID)) {
					fileName = string.Empty;
				} else {
					fileName = Path.Combine (documentsPath, SelectedCard.BackFileId + "." + SelectedCard.BackType);	
				}

			}
			setDisplay (fileName);
		}

		void setDisplay (string fileName)
		{
			if (string.IsNullOrEmpty (fileName)) {
				btnCardImage.SetImage (UIImage.FromFile ("default_photo.png"), UIControlState.Normal);
			} else {
				btnCardImage.SetImage (null, UIControlState.Normal);
				if (SelectedCard.FrontOrientation == "H") {
					btnCardImage.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal);
				} else {
					btnCardImage.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal);
				}
			}
		}
	}
}


