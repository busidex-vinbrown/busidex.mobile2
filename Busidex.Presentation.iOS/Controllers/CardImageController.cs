using System;

using UIKit;
using System.IO;
using Busidex.Mobile;
using System.Threading.Tasks;
using CoreAnimation;
using Foundation;
using CoreGraphics;

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

		const int HORIZONTAL_WIDTH = 300;
		const int HORIZONTAL_HEIGHT = 150;
		const int VERTICAL_WIDTH = 150;
		const int VERTICAL_HEIGHT = 300;
		const string ORIENTATION_HORIZONTAL = "H";
		const string ORIENTATION_VERTICAL = "V";

		DisplayMode SelectedDisplayMode;
		string SelectedOrientation;

		public CardImageController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SelectedCard = UISubscriptionService.OwnedCard;

			SelectedDisplayMode = DisplayMode.Front;
			SelectedOrientation = SelectedCard.FrontOrientation;

			int height = SelectedCard.FrontOrientation == ORIENTATION_HORIZONTAL ? HORIZONTAL_HEIGHT : VERTICAL_HEIGHT;
			int width = SelectedCard.FrontOrientation == ORIENTATION_HORIZONTAL ? HORIZONTAL_WIDTH : VERTICAL_WIDTH;

			var frame = new CGRect (btnCardImage.Frame.Left, btnCardImage.Frame.Top, width, height + 20f);

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

					var backFileName = Path.Combine (documentsPath, SelectedCard.BackFileId + "." + SelectedCard.BackType);
					if (!File.Exists (backFileName)) {
						Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.BackFileName, documentsPath, SelectedCard.BackFileName).ContinueWith (t => {
							
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
			btnRotate.TouchUpInside += delegate {
				var orientation = SelectedOrientation == ORIENTATION_VERTICAL ? ORIENTATION_HORIZONTAL : ORIENTATION_VERTICAL;
				rotate (orientation);
			};
		}

		void rotate (string orientation)
		{
			const int SLIDE_DISTANCE = HORIZONTAL_WIDTH / 4;

			var border = btnCardImage.Layer.Sublayers [1];
			const int bSlideDistanceX = (SLIDE_DISTANCE / 4) - 15;
			const int bSlideDistanceY = 32;

			var vFrame = new CoreGraphics.CGRect (btnCardImage.Frame.Left + SLIDE_DISTANCE, btnCardImage.Frame.Top - bSlideDistanceY, VERTICAL_WIDTH, VERTICAL_HEIGHT);
			var hFrame = new CoreGraphics.CGRect (btnCardImage.Frame.Left - SLIDE_DISTANCE, btnCardImage.Frame.Top + bSlideDistanceY, HORIZONTAL_WIDTH, HORIZONTAL_HEIGHT + 20f);

			var bvFrame = new CoreGraphics.CGRect (border.Frame.Left + bSlideDistanceX, border.Frame.Top - (bSlideDistanceY / 2), VERTICAL_WIDTH, VERTICAL_HEIGHT);
			var bhFrame = new CoreGraphics.CGRect (border.Frame.Left - bSlideDistanceX, border.Frame.Top + (bSlideDistanceY / 2), HORIZONTAL_WIDTH, HORIZONTAL_HEIGHT + 20f);

			CABasicAnimation rotationAnimation = new CABasicAnimation ();
			rotationAnimation.KeyPath = "transform.rotation.z";
			rotationAnimation.To = new NSNumber (Math.PI * 2);
			rotationAnimation.Duration = 1;
			rotationAnimation.Cumulative = true;
			rotationAnimation.RepeatCount = .5f;

			UIView.Animate (.5, 0, UIViewAnimationOptions.CurveEaseInOut,
				() => {
					if (SelectedOrientation == ORIENTATION_HORIZONTAL) {
						var buttonFrame = new CoreGraphics.CGRect (btnRotate.Frame.X + SLIDE_DISTANCE, btnRotate.Frame.Y - bSlideDistanceY - 15, btnRotate.Frame.Width, btnRotate.Frame.Height);
						btnRotate.Frame = buttonFrame;
						btnCardImage.Layer.Sublayers [1].Frame = bvFrame;
						btnCardImage.Frame = vFrame;
						btnRotate.Layer.AddAnimation (rotationAnimation, "rotationAnimation");

					} else {
						var buttonFrame = new CoreGraphics.CGRect (btnRotate.Frame.X - SLIDE_DISTANCE, btnRotate.Frame.Y + bSlideDistanceY + 15, btnRotate.Frame.Width, btnRotate.Frame.Height);
						btnRotate.Frame = buttonFrame;
						btnCardImage.Layer.Sublayers [1].Frame = bhFrame;
						btnCardImage.Frame = hFrame;
						btnRotate.Layer.AddAnimation (rotationAnimation, "rotationAnimation");
					}
				},
				() => {
				});

			SelectedOrientation = orientation;
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

			SelectedDisplayMode = mode;

			var fast = .3f;
			UIView.Animate (fast, 0, UIViewAnimationOptions.CurveEaseInOut,
				() => {
					btnRotate.Hidden = true;
					btnCardImage.Transform = CGAffineTransform.MakeScale (0.01f, 1.1f);

				},
				() => {
					UIView.Animate (fast, 0, UIViewAnimationOptions.CurveEaseInOut,
						() => {
							btnCardImage.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal);
							btnCardImage.Transform = CGAffineTransform.MakeScale (1.0f, 1.0f);
						},
						() => {
							btnRotate.Hidden = false;
						});
				});
			
		}

		void setDisplay (string fileName)
		{
			if (string.IsNullOrEmpty (fileName)) {
				const string DEFALUT_PHOTO_IMAGE = "default_photo.png";
				btnCardImage.SetImage (UIImage.FromFile (DEFALUT_PHOTO_IMAGE), UIControlState.Normal);
			} else {
				btnCardImage.SetImage (null, UIControlState.Normal);
				if (SelectedCard.FrontOrientation == ORIENTATION_HORIZONTAL) {
					btnCardImage.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal);
				} else {
					btnCardImage.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal);
				}
			}
		}
	}
}


