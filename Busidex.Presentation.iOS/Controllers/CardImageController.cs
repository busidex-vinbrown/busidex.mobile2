using System;
using System.Drawing;
using System.IO;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using CoreGraphics;
using Foundation;
using Google.Analytics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using UIKit;

namespace Busidex.Presentation.iOS.Controllers
{
	public partial class CardImageController : BaseCardEditController
	{
		enum DisplayMode
		{
			Front = 0,
			Back = 1
		}

		enum UIVisibility
		{
			Hidden = 0,
			Visible = 1
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
		struct TempCardInfo
		{
			public string FrontFileName { get; set; }
			public string BackFileName { get; set; }
			public Guid? FrontFileId { get; set; }
			public Guid? BackFileId { get; set; }
			public string FrontType { get; set; }
			public string BackType { get; set; }
			public string FrontOrientation { get; set; }
			public string BackOrientation { get; set; }
		};

		TempCardInfo tempCard = new TempCardInfo ();

		MobileCardImage CardModel { get; set; }

		public CardImageController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			Gai.SharedInstance.DefaultTracker.Set (GaiConstants.ScreenName, "Card Image");

			base.ViewDidAppear (animated);
		}

		protected override void CardUpdated ()
		{
			base.CardUpdated ();
			setTempCardInfo ();

			backImageChanged = frontImageChanged = false;

			InvokeOnMainThread (() => {
				//setDisplay (fileName);
				initUI (false);
			});
		}

		void setImageSelectionUI (UIVisibility visibility)
		{
			imgButtonFrame.Hidden =
				btnCancelImage.Hidden =
					btnTakeImage.Hidden =
						btnSelectImage.Hidden =
							btnReset.Hidden = visibility == UIVisibility.Hidden;
		}

		void setTempCardInfo ()
		{
			if (UnsavedData == null) {
				return;
			}

			tempCard.BackFileId = UnsavedData.BackFileId;
			tempCard.BackOrientation = UnsavedData.BackOrientation;
			tempCard.BackType = UnsavedData.BackType;
			tempCard.FrontFileId = UnsavedData.FrontFileId;
			tempCard.FrontType = UnsavedData.FrontType;
			tempCard.FrontOrientation = UnsavedData.FrontOrientation;
		}

		void restoreCardInfo ()
		{
			UnsavedData.BackFileId = tempCard.BackFileId;
			UnsavedData.BackOrientation = tempCard.BackOrientation;
			UnsavedData.BackType = tempCard.BackType;
			UnsavedData.FrontFileId = tempCard.FrontFileId;
			UnsavedData.FrontType = tempCard.FrontType;
			UnsavedData.FrontOrientation = tempCard.FrontOrientation;

			backImageChanged = frontImageChanged = false;
		}

		protected override void CancelChanges ()
		{
			restoreCardInfo ();
			setDisplay (getFileName ());
		}

		void initUI (bool loaded)
		{

			setTempCardInfo ();

			SelectedOrientation = SelectedOrientation ?? UnsavedData.FrontOrientation;
			SelectedOrientation = SelectedOrientation ?? "H";

			CardModel = new MobileCardImage {
				FrontFileId = UnsavedData.FrontFileId,
				BackFileId = UnsavedData.BackFileId,
				Orientation = SelectedOrientation,
				Side = SelectedDisplayMode,
				EncodedCardImage = string.Empty
			};

			setOrientation (SelectedOrientation);

			if (!loaded) {
				try {

					if (!UnsavedData.FrontFileId.ToString ().Equals (Resources.EMPTY_CARD_ID) &&
						!UnsavedData.FrontFileId.ToString ().Equals (Resources.NULL_CARD_ID)) {

						var frontFileName = Path.Combine (documentsPath, UnsavedData.FrontFileId + "." + UnsavedData.FrontType);
						if (File.Exists (frontFileName)) {
							if (SelectedDisplayMode == MobileCardImage.DisplayMode.Front) {
								setDisplay (frontFileName);
							}
						} else {

							ShowOverlay ();

							Utils.DownloadImage (Resources.CARD_PATH + UnsavedData.FrontFileName, documentsPath, UnsavedData.FrontFileName).ContinueWith (t => {
								InvokeOnMainThread (() => {
									setDisplay (frontFileName);
									if (Overlay != null) {
										Overlay.Hide ();
									}
								});
							});
						}

						var backFileName = Path.Combine (documentsPath, UnsavedData.BackFileId + "." + UnsavedData.BackType);

						if (!File.Exists (backFileName)) {
							Utils.DownloadImage (Resources.CARD_PATH + UnsavedData.BackFileName, documentsPath, UnsavedData.BackFileName).ContinueWith (t => {
								if (SelectedDisplayMode == MobileCardImage.DisplayMode.Back) {
									InvokeOnMainThread (() => {
										setDisplay (backFileName);
										if (Overlay != null) {
											Overlay.Hide ();
										}
									});
								}
							});
						} else {
							if (SelectedDisplayMode == MobileCardImage.DisplayMode.Back) {
								setDisplay (backFileName);
							}
						}

					} else {
						setDisplay (string.Empty);
					}
				} catch (Exception ex) {
					Xamarin.Insights.Report (ex);
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			var loaded = UnsavedData != null && UnsavedData.FrontFileId.Equals (UISubscriptionService.OwnedCard.FrontFileId);

			base.ViewWillAppear (animated);

			CrossMedia.Current.Initialize ();

			btnHCardImage.Hidden = btnVCardImage.Hidden = true;

			setImageSelectionUI (UIVisibility.Hidden);

			if (UISubscriptionService.OwnedCard != null) {
				initUI (loaded);
			} else {
				if (overlay == null) {
					overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				}

				overlay.MessageText = "Loading your card information...";
				View.Add (overlay);
			}
		}

		static UIImage cropImage (UIImage srcImage, RectangleF rect)
		{
			using (CGImage cr = srcImage.CGImage.WithImageInRect (rect)) {
				if(cr == null){
					return srcImage;
				}
				UIImage cropped = UIImage.FromImage (cr);
				return cropped;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
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

					btnHCardImage.SetImage (null, UIControlState.Normal);
					btnHCardImage.SetBackgroundImage (img, UIControlState.Normal);

					btnVCardImage.SetImage (null, UIControlState.Normal);
					btnVCardImage.SetBackgroundImage (img, UIControlState.Normal);

					frontImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Front;
					backImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Back;

					CardInfoChanged = CardInfoChanged || frontImageChanged || backImageChanged;

				});
			}
		}

		public override void SaveCard ()
		{
			if (!CardInfoChanged) {
				return;
			}

			UISubscriptionService.OnCardInfoSaved -= CardUpdated;
			UISubscriptionService.OnCardInfoSaved += CardUpdated;

			UISubscriptionService.SaveCardImage (CardModel);

			setTempCardInfo ();

			base.SaveCard ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//txtH.Hidden = txtW.Hidden = txtX.Hidden = txtY.Hidden = true;
			NavigationItem.SetRightBarButtonItem (
					new UIBarButtonItem (UIBarButtonSystemItem.Save, (sender, args) => SaveCard ())
					, true);

			btnHCardImage.Layer.BorderWidth = 1;
			btnHCardImage.Layer.CornerRadius = 1;
			btnHCardImage.Layer.BorderColor = UIColor.Gray.CGColor;

			btnVCardImage.Layer.BorderWidth = 1;
			btnVCardImage.Layer.CornerRadius = 1;
			btnVCardImage.Layer.BorderColor = UIColor.Gray.CGColor;

			btnCancelImage.TouchUpInside += delegate {
				setImageSelectionUI (UIVisibility.Hidden);
			};

			btnSelectImage.TouchUpInside += async (sender, e) => {
				var file = await CrossMedia.Current.PickPhotoAsync ();

				if (SelectedDisplayMode == MobileCardImage.DisplayMode.Front) {
					CardModel.FrontFileId = Guid.NewGuid ();
				} else {
					CardModel.BackFileId = Guid.NewGuid ();
				}

				if (file != null) {
					setImage (file);
					setImageSelectionUI (UIVisibility.Hidden);
				}
			};

			btnTakeImage.TouchUpInside += async (sender, e) => {
				//var options = new StoreCameraMediaOptions {
				//	Directory = "Sample",
				//	Name = "test.jpg",
				//	AllowCropping = true
				//};

				Func<object> func = () => {
					var imageView = new UIImageView (UIImage.FromBundle ("cropoverlay2.png"));
					imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

					var screen = UIScreen.MainScreen.Bounds;
					imageView.Frame = screen;

					return imageView;
				};

				var file = await CrossMedia.Current.TakePhotoAsync (new StoreCameraMediaOptions{
					AllowCropping = true,
					OverlayViewProvider = func
				});

				if (SelectedDisplayMode == MobileCardImage.DisplayMode.Front) {
					CardModel.FrontFileId = Guid.NewGuid ();
				} else {
					CardModel.BackFileId = Guid.NewGuid ();
				}

				if (file != null) {
					setImage (file);
					setImageSelectionUI (UIVisibility.Hidden);
				}
			};

			btnHCardImage.TouchUpInside += (sender, args) => {

				if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
					Application.ShowAlert ("No Camara Available", "There is no camara available right now", "Ok");
					return;
				}

				setImageSelectionUI (UIVisibility.Visible);
			};

			btnVCardImage.TouchUpInside += (sender, args) => {

				if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
					Application.ShowAlert ("No Camara Available", "There is no camara available right now", "Ok");
					return;
				}

				setImageSelectionUI (UIVisibility.Visible);
			};

			lblTitle.Frame = new CGRect (lblTitle.Frame.Left, lblTitle.Frame.Top, UIScreen.MainScreen.Bounds.Width, lblTitle.Frame.Height);
			btnBack.TouchUpInside += delegate {
				toggle (MobileCardImage.DisplayMode.Back);
			};
			btnFront.TouchUpInside += delegate {
				toggle (MobileCardImage.DisplayMode.Front);
			};

			btnHorizontal.TouchUpInside += delegate {
				if (SelectedOrientation == "H") {
					return;
				}
				frontImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Front;
				backImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Back;
				CardInfoChanged = CardInfoChanged || frontImageChanged || backImageChanged;
				setOrientation ("H");
			};

			btnVertical.TouchUpInside += delegate {
				if (SelectedOrientation == "V") {
					return;
				}
				frontImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Front;
				backImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Back;
				CardInfoChanged = CardInfoChanged || frontImageChanged || backImageChanged;
				setOrientation ("V");
			};

			btnReset.TouchUpInside += delegate {

				if (SelectedDisplayMode == MobileCardImage.DisplayMode.Front) {
					CardModel.FrontFileId = Guid.Empty;
				} else {
					CardModel.BackFileId = Guid.Empty;
				}
				CardModel.EncodedCardImage = Guid.Empty.ToString ();

				frontImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Front;
				backImageChanged = SelectedDisplayMode == MobileCardImage.DisplayMode.Back;

				setDisplay (string.Empty);
				setImageSelectionUI (UIVisibility.Hidden);
				CardInfoChanged = CardInfoChanged || frontImageChanged || backImageChanged;
			};
		}

		void setOrientation (string orientation)
		{

			SelectedOrientation = orientation;

			rotate (SelectedOrientation);

			rdoHorizontal.SetImage (orientation == "H" ? UIImage.FromFile ("radio_on.png") : UIImage.FromFile ("radio_off.png"), UIControlState.Normal);
			rdoVertical.SetImage (orientation == "V" ? UIImage.FromFile ("radio_on.png") : UIImage.FromFile ("radio_off.png"), UIControlState.Normal);
		}

		void rotate (string orientation)
		{
			btnHCardImage.Hidden = orientation == "V";
			btnVCardImage.Hidden = orientation == "H";

			SelectedOrientation = orientation;
			CardModel.Orientation = SelectedOrientation;
		}

		string getOrientation ()
		{
			var orientation = SelectedDisplayMode == MobileCardImage.DisplayMode.Front
																			? SelectedCard.FrontOrientation
																			: SelectedCard.BackOrientation;
			return orientation;
		}

		string getFileName ()
		{
			var fileName = SelectedDisplayMode == MobileCardImage.DisplayMode.Front
																	 ? Path.Combine (documentsPath, SelectedCard.FrontFileId + "." + SelectedCard.FrontType)
																	 : Path.Combine (documentsPath, SelectedCard.BackFileId + "." + SelectedCard.BackType);
			return fileName;
		}

		async void toggle (MobileCardImage.DisplayMode mode)
		{
			if (mode == SelectedDisplayMode) {
				return;
			}
			string fileName;

			if (frontImageChanged || backImageChanged) {
				var choice = await Application.ShowAlert ("Card Image Changed", CARD_UPDATED_SAVE_WARNING, new [] { "Save", "Cancel" });
				if (choice == 1) {
					restoreCardInfo ();
					// Use the Original
					fileName = getFileName ();

					var orientation = getOrientation ();

					InvokeOnMainThread (() => {
						rotate (orientation);
						setDisplay (fileName);
					});

					return;
				}

				SaveCard ();
				if (SelectedDisplayMode == MobileCardImage.DisplayMode.Front) {
					UnsavedData.FrontOrientation = SelectedOrientation;
				} else {
					UnsavedData.BackOrientation = SelectedOrientation;
				}
				return;
			}

			SelectedDisplayMode = mode;

			CardModel.Side = mode;

			setOrientation (mode == MobileCardImage.DisplayMode.Front ? UnsavedData.FrontOrientation : UnsavedData.BackOrientation);

			if (mode == MobileCardImage.DisplayMode.Front) {

				btnFront.SetTitleColor (UIColor.White, UIControlState.Normal);
				btnFront.BackgroundColor = UIColor.Blue;

				btnBack.SetTitleColor (UIColor.Blue, UIControlState.Normal);
				btnBack.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

				fileName = Path.Combine (documentsPath, UnsavedData.FrontFileId + "." + UnsavedData.FrontType);
				if (!File.Exists (fileName)) {
					await Utils.DownloadImage (Resources.CARD_PATH + UnsavedData.FrontFileName, documentsPath, UnsavedData.FrontFileName).ContinueWith (t => {
						InvokeOnMainThread (() => {
							setDisplay (fileName);
							if (Overlay != null) {
								Overlay.Hide ();
							}
						});
					});
				} else {
					setDisplay (fileName);
				}
			} else {

				btnBack.SetTitleColor (UIColor.White, UIControlState.Normal);
				btnBack.BackgroundColor = UIColor.Blue;

				btnFront.SetTitleColor (UIColor.Blue, UIControlState.Normal);
				btnFront.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

				if (UnsavedData.BackFileId.ToString ().Equals (Resources.EMPTY_CARD_ID) ||
					UnsavedData.BackFileId.ToString ().Equals (Resources.NULL_CARD_ID)) {
					fileName = string.Empty;
					setDisplay (fileName);
					return;
				} else {
					fileName = Path.Combine (documentsPath, UnsavedData.BackFileId + "." + UnsavedData.BackType);
					if (!File.Exists (fileName)) {
						await Utils.DownloadImage (Resources.CARD_PATH + UnsavedData.BackFileName, documentsPath, UnsavedData.BackFileName).ContinueWith (t => {
							InvokeOnMainThread (() => {
								setDisplay (fileName);
								if (Overlay != null) {
									Overlay.Hide ();
								}
							});
						});
					} else {
						setDisplay (fileName);
					}
				}
			}
		}

		void setDisplay (string fileName)
		{
			UIImage img = null;

			if (string.IsNullOrEmpty (fileName) || fileName.Contains (Guid.Empty.ToString ())) {
				const string DEFALUT_PHOTO_IMAGE = "default_photo.png";
				img = UIImage.FromFile (DEFALUT_PHOTO_IMAGE);
				btnHCardImage.SetImage (img, UIControlState.Normal);
				btnHCardImage.SetBackgroundImage (null, UIControlState.Normal);

				btnVCardImage.SetImage (img, UIControlState.Normal);
				btnVCardImage.SetBackgroundImage (null, UIControlState.Normal);
			} else {
				btnHCardImage.SetImage (null, UIControlState.Normal);
				btnVCardImage.SetImage (null, UIControlState.Normal);
				if (!File.Exists (fileName)) {
					fileName = SelectedDisplayMode == MobileCardImage.DisplayMode.Front ? UnsavedData.FrontFileName : UnsavedData.BackFileName;
					Utils.DownloadImage (Resources.CARD_PATH + fileName, documentsPath, fileName).ContinueWith (t => {
						InvokeOnMainThread (() => {
							setDisplay (fileName);
							if (Overlay != null) {
								Overlay.Hide ();
							}
						});
					});
				} else {
					img = UIImage.FromFile (fileName);
					btnHCardImage.SetBackgroundImage (img, UIControlState.Normal);
					btnVCardImage.SetBackgroundImage (img, UIControlState.Normal);
				}
			}
			if (img != null) {
				CardModel.EncodedCardImage = img.AsJPEG (0.5f).GetBase64EncodedData (NSDataBase64EncodingOptions.None).ToString ();
			}
		}
	}
}
