using System;

using UIKit;
using System.IO;
using Busidex.Mobile;
using CoreAnimation;
using Foundation;
using CoreGraphics;
using GoogleAnalytics.iOS;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Drawing;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	public partial class CardImageController : BaseCardEditController
	{
		//BusinessCardDimensions frontDimensions;
		//BusinessCardDimensions backDimensions;
		enum DisplayMode
		{
			Front = 0,
			Back = 1
		}

		enum CamaraViewMode
		{
			TakingPicture = 1,
			ReviewingPicture = 2,
			Done = 3
		}

		bool frontImageChanged;
		bool backImageChanged;

		MobileCardImage.DisplayMode SelectedDisplayMode { get; set; }

		const float HEIGHT_RATIO = .583f;
		const int HORIZONTAL_WIDTH = 300;
		const int HORIZONTAL_HEIGHT = 176;
		const int VERTICAL_WIDTH = 176;
		const int VERTICAL_HEIGHT = 300;
		const string ORIENTATION_HORIZONTAL = "H";
		const string ORIENTATION_VERTICAL = "V";

		string SelectedOrientation;

		MobileCardImage CardModel { get; set; }

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
			var loaded = SelectedCard != null && SelectedCard.FrontFileId.Equals (UISubscriptionService.OwnedCard.FrontFileId);

			base.ViewWillAppear (animated);

			setImageSelectionUI (false);

			SelectedCard = UISubscriptionService.OwnedCard;
			SelectedOrientation = SelectedOrientation ?? SelectedCard.FrontOrientation;

			CardModel = new MobileCardImage {
				Orientation = SelectedOrientation,
				Side = SelectedDisplayMode,
				EncodedCardImage = string.Empty
			};

			if (!loaded) {
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
		}

		static UIImage cropImage (UIImage srcImage, RectangleF rect)
		{
			using (CGImage cr = srcImage.CGImage.WithImageInRect (rect)) {
				UIImage cropped = UIImage.FromImage (cr);
				return cropped;
			}
		}


		void setImage (MediaFile picture)
		{

			if (picture != null) {

				var img = UIImage.FromFile (picture.Path);

				InvokeOnMainThread (() => {

					//var data = UIImage.FromFile (picture.Path).AsJPEG (.4f);
					//var img = UIImage.LoadFromData (data);
					float x, y, w, h;
					x = 50;
					y = 350;
					w = 3200;
					h = 1800;

					//					if (txtH.Text == "") {
					//						txtX.Text = x.ToString ();
					//						txtY.Text = y.ToString ();
					//						txtW.Text = w.ToString ();
					//						txtH.Text = h.ToString ();
					//					} else {
					//						x = float.Parse (txtX.Text);
					//						y = float.Parse (txtY.Text);
					//						w = float.Parse (txtW.Text);
					//						h = float.Parse (txtH.Text);
					//					}
					var rect = new RectangleF (x, y, w, h);

					var croppedImage = cropImage (img, rect);// img.CGImage.WithImageInRect (rect);

					img = SelectedOrientation == "H"
						? new UIImage (croppedImage.CGImage).Scale (new CGSize (HORIZONTAL_WIDTH * 2, HORIZONTAL_HEIGHT * 2))
						: new UIImage (croppedImage.CGImage).Scale (new CGSize (HORIZONTAL_HEIGHT * 2, HORIZONTAL_WIDTH * 2));

					if (SelectedOrientation == "V") {
						img = new UIImage (img.CGImage, 1f, UIImageOrientation.Right);
					}

					CardModel.EncodedCardImage = img.AsJPEG (0.5f).GetBase64EncodedData (NSDataBase64EncodingOptions.None).ToString ();

					btnCardImage.SetBackgroundImage (img, UIControlState.Normal);

					frontImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Front;
					backImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Back;

				});
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			txtH.Hidden = txtW.Hidden = txtX.Hidden = txtY.Hidden = true;

			btnCardImage.Layer.BorderWidth = 1;
			btnCardImage.Layer.CornerRadius = 1;
			btnCardImage.Layer.BorderColor = UIColor.Gray.CGColor;

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
				var options = new StoreCameraMediaOptions {
					Directory = "Sample",
					Name = "test.jpg"
				};

				var file = await CrossMedia.Current.TakePhotoAsync (options);
				setImage (file);
			};

			btnCardImage.TouchUpInside += (sender, args) => {

				if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
					ShowAlert ("No Camara Available", "There is no camara available right now", "Ok");
					return;
				}

				setImageSelectionUI (true);
			};

			lblTitle.Frame = new CGRect (lblTitle.Frame.Left, lblTitle.Frame.Top, UIScreen.MainScreen.Bounds.Width, lblTitle.Frame.Height);
			btnBack.TouchUpInside += delegate {
				toggle (MobileCardImage.DisplayMode.Back);
			};
			btnFront.TouchUpInside += delegate {
				toggle (MobileCardImage.DisplayMode.Front);
			};
			btnRotate.TouchUpInside += delegate {
				var orientation = SelectedOrientation == ORIENTATION_VERTICAL ? ORIENTATION_HORIZONTAL : ORIENTATION_VERTICAL;
				rotate (orientation);
			};

			btnSave.TouchUpInside += delegate {

				var newFileName = Guid.NewGuid ();
				if (frontImageChanged) {
					CardModel.FrontFileId = newFileName;
				}
				if (backImageChanged) {
					CardModel.BackFileId = newFileName;
				}
				UISubscriptionService.SaveCardImage (CardModel);
			};
		}

		void rotate (string orientation)
		{
			const int SLIDE_DISTANCE = HORIZONTAL_WIDTH / 4 - 15;

			const int bSlideDistanceY = 40;

			var vFrame = new CGRect (btnCardImage.Frame.Left + SLIDE_DISTANCE, btnCardImage.Frame.Top - bSlideDistanceY, VERTICAL_WIDTH, VERTICAL_HEIGHT);
			var hFrame = new CGRect (btnCardImage.Frame.Left - SLIDE_DISTANCE, btnCardImage.Frame.Top + bSlideDistanceY, HORIZONTAL_WIDTH, HORIZONTAL_HEIGHT + 25f);

			var rotationAnimation = new CABasicAnimation ();
			rotationAnimation.KeyPath = "transform.rotation.z";
			rotationAnimation.To = new NSNumber (Math.PI * 2);
			rotationAnimation.Duration = 1;
			rotationAnimation.Cumulative = true;
			rotationAnimation.RepeatCount = .5f;

			UIView.Animate (.5, 0, UIViewAnimationOptions.CurveEaseInOut,
				() => {
					if (SelectedOrientation == ORIENTATION_HORIZONTAL) {
						var buttonFrame = new CGRect (btnRotate.Frame.X + SLIDE_DISTANCE, btnRotate.Frame.Y - bSlideDistanceY, btnRotate.Frame.Width, btnRotate.Frame.Height);
						btnRotate.Frame = buttonFrame;
						btnCardImage.Frame = vFrame;
						btnRotate.Layer.AddAnimation (rotationAnimation, "rotationAnimation");

					} else {
						var buttonFrame = new CGRect (btnRotate.Frame.X - SLIDE_DISTANCE, btnRotate.Frame.Y + bSlideDistanceY, btnRotate.Frame.Width, btnRotate.Frame.Height);
						btnRotate.Frame = buttonFrame;
						btnCardImage.Frame = hFrame;
						btnRotate.Layer.AddAnimation (rotationAnimation, "rotationAnimation");
					}
				},
				() => {
				});

			SelectedOrientation = orientation;
			CardModel.Orientation = SelectedOrientation;
		}

		void toggle (MobileCardImage.DisplayMode mode)
		{
			SelectedDisplayMode = mode;

			CardModel.Side = mode;

			string fileName;
			if (mode == MobileCardImage.DisplayMode.Front) {
				btnFront.SetTitleColor (UIColor.White, UIControlState.Normal);
				btnFront.BackgroundColor = UIColor.Blue;

				btnBack.SetTitleColor (UIColor.Blue, UIControlState.Normal);
				btnBack.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

				fileName = Path.Combine (documentsPath, SelectedCard.FrontFileId + "." + SelectedCard.FrontType);
				if (!File.Exists (fileName)) {
					Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.FrontFileName, documentsPath, SelectedCard.FrontFileName).ContinueWith (t => {
						InvokeOnMainThread (() => {
							setDisplay (fileName);
							Overlay.Hide ();
						});
					});
				}
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
					if (!File.Exists (fileName)) {
						Utils.DownloadImage (Resources.CARD_PATH + SelectedCard.BackFileName, documentsPath, SelectedCard.BackFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								setDisplay (fileName);
								Overlay.Hide ();
							});
						});
					}
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


