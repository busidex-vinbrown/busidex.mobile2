using System;

using UIKit;
using System.IO;
using Busidex.Mobile;
using CoreAnimation;
using Foundation;
using CoreGraphics;
using System.Threading.Tasks;
using AVFoundation;
using GoogleAnalytics.iOS;
using Plugin.Media.Abstractions;
using Plugin.Media;

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

		enum CamaraViewMode
		{
			TakingPicture = 1,
			ReviewingPicture = 2,
			Done = 3
		}

		DisplayMode SelectedDisplayMode { get; set; }

		const int HORIZONTAL_WIDTH = 300;
		const int HORIZONTAL_HEIGHT = 150;
		const int VERTICAL_WIDTH = 150;
		const int VERTICAL_HEIGHT = 300;
		const string ORIENTATION_HORIZONTAL = "H";
		const string ORIENTATION_VERTICAL = "V";

		const float DEFAULT_HOR_LEFT = 10f;
		const float DEFAULT_HOR_TOP = 220f;
		const float DEFAULT_VERT_LEFT = 10f;
		const float DEFAULT_VERT_TOP = 220f;

		string SelectedOrientation;



		bool IsTakingPicture { get; set; }

		public CardImageController (IntPtr handle) : base (handle)
		{
		}

		#region Manual Camara Functionality

		/* 
	    AVCaptureSession captureSession;
		AVCaptureDeviceInput captureDeviceInput;
		AVCaptureStillImageOutput stillImageOutput;
		AVCaptureVideoPreviewLayer videoPreviewLayer;
		static async Task AuthorizeCameraUse ()
		{
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);

			if (authorizationStatus != AVAuthorizationStatus.Authorized) {
				await AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
			}
		}

		static void ConfigureCameraForDevice (AVCaptureDevice device)
		{
			NSError error;
			if (device.IsFocusModeSupported (AVCaptureFocusMode.ContinuousAutoFocus)) {
				device.LockForConfiguration (out error);
				device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				device.UnlockForConfiguration ();
			} else if (device.IsExposureModeSupported (AVCaptureExposureMode.ContinuousAutoExposure)) {
				device.LockForConfiguration (out error);
				device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				device.UnlockForConfiguration ();
			} else if (device.IsWhiteBalanceModeSupported (AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance)) {
				device.LockForConfiguration (out error);
				device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				device.UnlockForConfiguration ();
			}
		}

		public void SetupLiveCameraStream ()
		{
			captureSession = new AVCaptureSession ();

			var viewLayer = btnCardImage.Layer;
			//var frame = new CGRect (0, 0, HORIZONTAL_WIDTH, HORIZONTAL_HEIGHT + 25f);

			videoPreviewLayer = new AVCaptureVideoPreviewLayer (captureSession) {
				Frame = View.Frame,
				VideoGravity = AVLayerVideoGravity.ResizeAspectFill
			};

			if (btnCardImage.Layer.Sublayers.Length > 2) {
				btnCardImage.Layer.Sublayers [2].RemoveFromSuperLayer ();
			}
			View.Layer.InsertSublayer (videoPreviewLayer, 2);

			//btnCardImage.Layer.Sublayers [2].ContentsRect = frame;

			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			if (captureDevice != null) {
				ConfigureCameraForDevice (captureDevice);
				captureDeviceInput = AVCaptureDeviceInput.FromDevice (captureDevice);
				captureSession.AddInput (captureDeviceInput);

				var dictionary = new NSMutableDictionary ();
				dictionary [AVVideo.CodecKey] = new NSNumber ((int)AVVideoCodec.JPEG);
				stillImageOutput = new AVCaptureStillImageOutput () {
					OutputSettings = new NSDictionary ()
				};

				captureSession.AddOutput (stillImageOutput);
				captureSession.StartRunning ();
			}
		}
		*/

		#endregion

		CGRect getDefaultFrame ()
		{
			var frontOrientation = SelectedCard.FrontOrientation;
			int height = frontOrientation == ORIENTATION_HORIZONTAL ? HORIZONTAL_HEIGHT : VERTICAL_HEIGHT;
			int width = frontOrientation == ORIENTATION_HORIZONTAL ? HORIZONTAL_WIDTH : VERTICAL_WIDTH;
			float left = frontOrientation == ORIENTATION_HORIZONTAL ? DEFAULT_HOR_LEFT : DEFAULT_VERT_LEFT;
			float top = frontOrientation == ORIENTATION_HORIZONTAL ? DEFAULT_HOR_TOP : DEFAULT_VERT_TOP;

			var frame = new CGRect (left, top, width, height + 25f);
			return frame;
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Card Image");

			base.ViewDidAppear (animated);
		}

		void setImageSelectionUI (bool visible)
		{
			imgButtonFrame.Hidden = btnCancelImage.Hidden = btnTakeImage.Hidden = btnSelectImage.Hidden = !visible;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);


			setImageSelectionUI (false);

			SelectedCard = UISubscriptionService.OwnedCard;

			SelectedOrientation = SelectedCard.FrontOrientation;

			var frame = getDefaultFrame ();

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

		void setImage (MediaFile picture)
		{
			if (picture != null) {
				InvokeOnMainThread (() => btnCardImage.SetBackgroundImage (UIImage.FromFile (picture.Path), UIControlState.Normal));
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnCancelImage.TouchUpInside += delegate {
				setImageSelectionUI (false);
			};

			btnSelectImage.TouchUpInside += async (sender, e) => {
				var file = await CrossMedia.Current.PickPhotoAsync ();	
				if (file != null) {
					setImage (file);
				}
			};

			btnTakeImage.TouchUpInside += async (sender, e) => {
				var file = await CrossMedia.Current.TakePhotoAsync (new Plugin.Media.Abstractions.StoreCameraMediaOptions {
					Directory = "Sample",
					Name = "test.jpg"
				});
				setImage (file);
			};

			btnCardImage.TouchUpInside += (sender, args) => {

				if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
					ShowAlert ("No Camara Available", "There is no camara available right now", "Ok");
					return;
				}

				setImageSelectionUI (true);

//				var isAvailable = CrossMedia.Current.IsCameraAvailable;
//				var defaultName = Guid.NewGuid ().ToString ();
//				if (isAvailable) {
//					int button = await ShowAlert ("Card Image", "Choose an image source", new [] {
//						"Select from Gallery",
//						"Take a picture now"
//					});
//
//					var picture = button == 0
//							? await CrossMedia.Current.PickPhotoAsync ()
//							: await CrossMedia.Current.TakePhotoAsync (new StoreCameraMediaOptions {
//						Name = defaultName + ".jpg",
//						SaveToAlbum = true
//					});
//
//					setImage (picture);
//
//				} else {
//					ShowAlert ("No Camara Available", "There is no camara available right now", "Ok");
//				}
			};
//			AuthorizeCameraUse ().ContinueWith (r => {
//				InvokeOnMainThread (() => {
//					btnCardImage.TouchUpInside += delegate {
//						if (!IsTakingPicture) {
//							SetupLiveCameraStream ();	
//							setCamaraMode (CamaraViewMode.TakingPicture);
//						}
//					};
//				});
//			});

			lblTitle.Frame = new CGRect (lblTitle.Frame.Left, lblTitle.Frame.Top, UIScreen.MainScreen.Bounds.Width, lblTitle.Frame.Height);
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
//			btnAcceptImage.Layer.CornerRadius = 2;
//			btnAcceptImage.Layer.BorderWidth = 1;
//
//			btnCancelImage.Layer.CornerRadius = 2;
//			btnCancelImage.Layer.BorderWidth = 1;

//			btnCancelImage.TouchUpInside += delegate {
//				discardImage ();
//				setCamaraMode (CamaraViewMode.Done);
//			};
//
//			btnAcceptImage.TouchUpInside += delegate {
//				setCamaraMode (CamaraViewMode.Done);
//			};
//
//			btnSetImage.TouchUpInside += delegate {
//				setCamaraMode (CamaraViewMode.ReviewingPicture);
//				//setImage ();
//			};
		}

		/*
		async void setImage ()
		{
			//var videoConnection = stillImageOutput.ConnectionFromMediaType (AVMediaType.Video);
			//var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync (videoConnection);

			//var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer);
			//var jpegAsByteArray = jpegImageAsNsData.ToArray ();

			var img = UIImage.LoadFromData (jpegImageAsNsData);
			var img2 = UIImage.FromImage (img.CGImage, 1.0f, UIImageOrientation.Up);
			//var img3 = img2.CreateResizableImage (new UIEdgeInsets (1, 5, 1f, 5f), UIImageResizingMode.Stretch);
			btnCardImage.SetBackgroundImage (img2.Crop (20, 60, 420, 280), UIControlState.Normal);

			btnSetImage.Hidden = true;

			btnAcceptImage.Hidden = btnCancelImage.Hidden = false;

			if (View.Layer.Sublayers.Length > 2) {
				View.Layer.Sublayers [2].RemoveFromSuperLayer ();
			}
		}

		void discardImage ()
		{
			var fileName = string.Empty;
			if (SelectedDisplayMode == DisplayMode.Front) {
				fileName = Path.Combine (documentsPath, SelectedCard.FrontFileId + "." + SelectedCard.FrontType);
			} else {
				fileName = Path.Combine (documentsPath, SelectedCard.BackFileId + "." + SelectedCard.BackType);
			}
			setDisplay (fileName);
		}

		void setCamaraMode (CamaraViewMode mode)
		{

			switch (mode) {
			case CamaraViewMode.TakingPicture:
				{
					NavigationController.SetNavigationBarHidden (true, true);
					lblTitle.Hidden = btnRotate.Hidden = btnSave.Hidden = true;
					imgGuideView.Hidden = btnSetImage.Hidden = false;
					btnAcceptImage.Hidden = btnCancelImage.Hidden = true;
					btnBack.Enabled = btnFront.Enabled = btnCardImage.Enabled = false;
					IsTakingPicture = true;
					break;
				}
			case CamaraViewMode.ReviewingPicture:
				{
					NavigationController.SetNavigationBarHidden (true, true);
					lblTitle.Hidden = btnRotate.Hidden = btnSave.Hidden = true;
					imgGuideView.Hidden = btnSetImage.Hidden = true;
					btnBack.Enabled = btnFront.Enabled = btnCardImage.Enabled = false;
					if (btnCardImage.Layer.Sublayers.Length > 2) {
						btnCardImage.Layer.Sublayers [2].RemoveFromSuperLayer ();
					}
					IsTakingPicture = true;
					break;
				}
			case CamaraViewMode.Done:
				{
					NavigationController.SetNavigationBarHidden (false, true);
					if (btnCardImage.Layer.Sublayers.Length > 2) {
						btnCardImage.Layer.Sublayers [2].RemoveFromSuperLayer ();
					}
					lblTitle.Hidden = btnRotate.Hidden = btnSave.Hidden = false;
					imgGuideView.Hidden = btnSetImage.Hidden = true;
					btnAcceptImage.Hidden = btnCancelImage.Hidden = true;
					btnBack.Enabled = btnFront.Enabled = btnCardImage.Enabled = true;
					IsTakingPicture = false;
					break;
				}
			}
		}
		*/

		void rotate (string orientation)
		{
			const int SLIDE_DISTANCE = HORIZONTAL_WIDTH / 4;

			var border = btnCardImage.Layer.Sublayers [1];
			const int bSlideDistanceX = (SLIDE_DISTANCE / 4) - 15;
			const int bSlideDistanceY = 32;

			var vFrame = new CGRect (btnCardImage.Frame.Left + SLIDE_DISTANCE, btnCardImage.Frame.Top - bSlideDistanceY, VERTICAL_WIDTH, VERTICAL_HEIGHT);
			var hFrame = new CGRect (btnCardImage.Frame.Left - SLIDE_DISTANCE, btnCardImage.Frame.Top + bSlideDistanceY, HORIZONTAL_WIDTH, HORIZONTAL_HEIGHT + 25f);

			var bvFrame = new CGRect (border.Frame.Left + bSlideDistanceX, border.Frame.Top - (bSlideDistanceY / 2), VERTICAL_WIDTH, VERTICAL_HEIGHT);
			var bhFrame = new CGRect (border.Frame.Left - bSlideDistanceX, border.Frame.Top + (bSlideDistanceY / 2), HORIZONTAL_WIDTH, HORIZONTAL_HEIGHT + 25f);

			var rotationAnimation = new CABasicAnimation ();
			rotationAnimation.KeyPath = "transform.rotation.z";
			rotationAnimation.To = new NSNumber (Math.PI * 2);
			rotationAnimation.Duration = 1;
			rotationAnimation.Cumulative = true;
			rotationAnimation.RepeatCount = .5f;

			UIView.Animate (.5, 0, UIViewAnimationOptions.CurveEaseInOut,
				() => {
					if (SelectedOrientation == ORIENTATION_HORIZONTAL) {
						var buttonFrame = new CGRect (btnRotate.Frame.X + SLIDE_DISTANCE, btnRotate.Frame.Y - bSlideDistanceY - 15, btnRotate.Frame.Width, btnRotate.Frame.Height);
						btnRotate.Frame = buttonFrame;
						btnCardImage.Layer.Sublayers [1].Frame = bvFrame;
						btnCardImage.Frame = vFrame;
						btnRotate.Layer.AddAnimation (rotationAnimation, "rotationAnimation");

					} else {
						var buttonFrame = new CGRect (btnRotate.Frame.X - SLIDE_DISTANCE, btnRotate.Frame.Y + bSlideDistanceY + 15, btnRotate.Frame.Width, btnRotate.Frame.Height);
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
			SelectedDisplayMode = mode;

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

			const float ANIMATION_SPPED_FAST = .3f;
			UIView.Animate (ANIMATION_SPPED_FAST, 0, UIViewAnimationOptions.CurveEaseInOut,
				() => {
					btnRotate.Hidden = true;
					btnCardImage.Transform = CGAffineTransform.MakeScale (0.01f, 1.1f);

				},
				() => {
					UIView.Animate (ANIMATION_SPPED_FAST, 0, UIViewAnimationOptions.CurveEaseInOut,
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


