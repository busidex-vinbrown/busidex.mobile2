using System;
using System.IO;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
//using Android.Views.Animations;
using Android.Widget;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Android.Graphics;
using Android.Provider;

namespace Busidex.Presentation.Droid.v2
{
	public class CardImageEditFragment : BaseCardEditFragment, RadioGroup.IOnCheckedChangeListener
	{
		//keep track of camera capture intent

		//captured picture uri
		private Android.Net.Uri picUri;

		enum DisplayMode
		{
			Front = 0,
			Back = 1
		}

		MobileCardImage.DisplayMode SelectedDisplayMode { get; set; }

		ImageButton btnBack;
		Button btnCardFront;
		Button btnCardBack;
		Button btnSave;
		Button btnTakePicture;
		Button btnSelectPicture;
		Button btnCancelPicture;
		ImageButton btnCardImageHor;
		ImageButton btnCardImageVer;
		ImageView imgPlaceholderHor;
		ImageView imgPlaceholderVer;
		RelativeLayout imageWrapperHor;
		RelativeLayout imageWrapperVer;
		ProgressBar progressBarCardEdit;
		RelativeLayout imageSelectOptions;

		string SelectedFrontOrientation;
		string SelectedBackOrientation;

		MobileCardImage CardModel { get; set; }

		const int PICTURE_SELECT = 0;
		const int CAMERA_CAPTURE = 1;
		const int PIC_CROP = 2;
		const int RESULT_OK = -1;
		const string ORIENTATION_HORIZONTAL = "H";
		const string ORIENTATION_VERTICAL = "V";

		public override void OnDetach ()
		{
			btnBack = null;
			btnCardFront = null;
			btnCardFront = null;
			imageWrapperHor = null;
			imageWrapperVer = null;

			if (btnCardImageHor != null) {
				var bd = (BitmapDrawable)btnCardImageHor.Drawable;
				if (bd != null) {
					bd.Bitmap.Recycle ();
				}
				btnCardImageHor.SetImageURI (null);
			}
			if (btnCardImageVer != null) {
				var bd = (BitmapDrawable)btnCardImageVer.Drawable;
				if (bd != null) {
					bd.Bitmap.Recycle ();
				}
				btnCardImageVer.SetImageURI (null);
			}
			btnCardFront = null;
			base.OnDetach ();
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			loadCardImage (MobileCardImage.DisplayMode.Front);
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_CARD_IMAGE);
			}
		}

		void setDisplay (MobileCardImage.DisplayMode mode, string fileName)
		{
			var uri = !string.IsNullOrEmpty (fileName) ? Android.Net.Uri.Parse (fileName) : null;

			imgPlaceholderHor.Visibility = ViewStates.Gone;
			imgPlaceholderVer.Visibility = ViewStates.Gone;
			btnCardImageHor.Visibility = ViewStates.Visible;
			btnCardImageVer.Visibility = ViewStates.Visible;

			var bm = Drawable.CreateFromPath (uri.Path);
			btnCardImageHor.SetImageDrawable (bm);
			btnCardImageVer.SetImageDrawable (bm);
			btnCardImageHor.Invalidate ();
			btnCardImageVer.Invalidate ();
			setOrientation (mode == MobileCardImage.DisplayMode.Front ? SelectedCard.FrontOrientation : SelectedCard.BackOrientation);

			SelectedDisplayMode = mode;

			CardModel.Orientation = SelectedDisplayMode == MobileCardImage.DisplayMode.Back ? SelectedBackOrientation : SelectedFrontOrientation;
			CardModel.Side = SelectedDisplayMode;

			hideProgress ();
		}

		async void loadCardImage (MobileCardImage.DisplayMode mode)
		{
			try {

				showProgress ();

				if (!SelectedCard.FrontFileId.ToString ().Equals (Mobile.Resources.EMPTY_CARD_ID) &&
					!SelectedCard.FrontFileId.ToString ().Equals (Mobile.Resources.NULL_CARD_ID)) {
					var fileName = mode == MobileCardImage.DisplayMode.Front
													   ? SelectedCard.FrontFileId + "." + SelectedCard.FrontType
													   : SelectedCard.BackFileId + "." + SelectedCard.BackType;

					var fullFileName = System.IO.Path.Combine (Mobile.Resources.DocumentsPath, fileName);
					if (File.Exists (fullFileName)) {
						setDisplay (mode, fullFileName);
					} else {

						await Utils.DownloadImage (Mobile.Resources.CARD_PATH + fileName, Mobile.Resources.DocumentsPath, fileName).ContinueWith (t => {
							Activity.RunOnUiThread (() => {
								loadCardImage (mode);
							});
						});
					}
				} else {
					setDisplay (mode, string.Empty);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		private void cropImage ()
		{
			// assume front
			double aspectX = 1.5;
			double aspectY = 1;

			var cropIntent = new Intent ("com.android.camera.action.CROP");
			//indicate image type and Uri
			cropIntent.SetDataAndType (picUri, "image/*");
			//set crop properties
			cropIntent.PutExtra ("crop", "true");
			//indicate aspect of desired crop
			cropIntent.PutExtra ("aspectX", aspectX);
			cropIntent.PutExtra ("aspectY", aspectY);
			//indicate output X and Y
			cropIntent.PutExtra ("outputX", 512);
			cropIntent.PutExtra ("outputY", 512);
			//retrieve data on return
			cropIntent.PutExtra ("return-data", true);
			//start the activity - we handle returning in onActivityResult
			StartActivityForResult (cropIntent, PIC_CROP);
		}

		public override void OnActivityResult (int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			var selectedOrientation = SelectedDisplayMode == MobileCardImage.DisplayMode.Front ? SelectedFrontOrientation : SelectedBackOrientation;

			Bitmap thePic;
			if (resultCode == RESULT_OK) {
				switch(requestCode){
					case PICTURE_SELECT:{
						picUri = data.Data;
						cropImage ();
						break;
					}
					case CAMERA_CAPTURE: {
						picUri = data.Data;
						cropImage ();
						break;
					}
					case PIC_CROP:{
						if (data != null) {
							Bundle extras = data.Extras;
							//get the cropped bitmap
							thePic = (Bitmap)extras.GetParcelable ("data");

							btnCardImageHor.SetImageBitmap (thePic);
							btnCardImageVer.SetImageBitmap (thePic);

							byte [] b;
							using (var stream = new MemoryStream ()) {
								thePic.Compress (Bitmap.CompressFormat.Png, 0, stream);
								b = stream.ToArray ();

								var s = Convert.ToBase64String (b);

								if (SelectedDisplayMode == MobileCardImage.DisplayMode.Front) {
									CardModel.FrontFileId = Guid.NewGuid ();
								} else {
									CardModel.BackFileId = Guid.NewGuid ();
								}
								CardModel.EncodedCardImage = s;
								CardModel.Side = SelectedDisplayMode;
								CardModel.Orientation = selectedOrientation;
							}
						}
						break;
					}
				}
				setCardImageOptionsDisplay (ViewStates.Gone);

				GC.Collect ();
			} 
		}

		void setCardImageOptionsDisplay(ViewStates visibility){
			imageSelectOptions.Visibility = visibility;
		}

		void takePicture(){
			imageSelectOptions.Visibility = ViewStates.Gone;
			var captureIntent = new Intent (MediaStore.ActionImageCapture);
			//we will handle the returned data in onActivityResult
			StartActivityForResult (captureIntent, CAMERA_CAPTURE);
		}

		void selectPicture(){
			var imageIntent = new Intent ();
			imageIntent.SetType ("image/*");
			imageIntent.SetAction (Intent.ActionGetContent);
			StartActivityForResult (
				Intent.CreateChooser (imageIntent, "Select photo"), 0);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardImageEdit, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			imageWrapperHor = view.FindViewById<RelativeLayout> (Resource.Id.imageWrapperHor);
			imageWrapperVer = view.FindViewById<RelativeLayout> (Resource.Id.imageWrapperVer);
			btnCardImageHor = view.FindViewById<ImageButton> (Resource.Id.btnCardImageHor);
			btnCardImageVer = view.FindViewById<ImageButton> (Resource.Id.btnCardImageVer);
			imgPlaceholderHor = view.FindViewById<ImageView> (Resource.Id.imgPlaceholderHor);
			imgPlaceholderVer = view.FindViewById<ImageView> (Resource.Id.imgPlaceholderVer);
			btnCardFront = view.FindViewById<Button> (Resource.Id.btnCardFront);
			btnCardBack = view.FindViewById<Button> (Resource.Id.btnCardBack);
			btnSave = view.FindViewById<Button> (Resource.Id.btnSave);
			btnTakePicture = view.FindViewById<Button> (Resource.Id.btnTakePicture);
			btnSelectPicture = view.FindViewById<Button> (Resource.Id.btnSelectPicture);
			btnCancelPicture = view.FindViewById<Button> (Resource.Id.btnCancelPicture);

			imageSelectOptions = view.FindViewById<RelativeLayout> (Resource.Id.imageSelectOptions);

			SelectedDisplayMode = MobileCardImage.DisplayMode.Front;
			SelectedBackOrientation = SelectedCard.FrontOrientation;

			CardModel = new MobileCardImage {
				Orientation = SelectedCard.FrontOrientation,
				Side = SelectedDisplayMode,
				EncodedCardImage = string.Empty
			};

			imageSelectOptions.Visibility = ViewStates.Gone;

			btnCardImageHor.Click += delegate {
				setCardImageOptionsDisplay (ViewStates.Visible);	
			}; 

			btnCardImageVer.Click += delegate {
				setCardImageOptionsDisplay (ViewStates.Visible);
			};

			btnCancelPicture.Click += delegate {
				setCardImageOptionsDisplay (ViewStates.Gone);
			};

			btnTakePicture.Click += delegate {
				takePicture ();
			};

			btnSelectPicture.Click += delegate {
				selectPicture ();
			};

			btnCardFront.Click += delegate {
				toggleSide (MobileCardImage.DisplayMode.Front);
			};

			btnCardBack.Click += delegate {
				toggleSide (MobileCardImage.DisplayMode.Back);
			};

			btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			btnSave.Click += delegate {
				UISubscriptionService.SaveCardImage (CardModel);
			};

			SelectedFrontOrientation = SelectedCard.FrontOrientation;
			SelectedBackOrientation = SelectedCard.BackOrientation;

			var radioGroup = view.FindViewById<RadioGroup> (Resource.Id.rdoOrientation);
			radioGroup.SetOnCheckedChangeListener (this);

			return view;
		}

		void setOrientation(string orientation){

			if (SelectedDisplayMode == MobileCardImage.DisplayMode.Front) {
				SelectedFrontOrientation = orientation;
			} else {
				SelectedBackOrientation = orientation;
			}

			imageWrapperHor.Visibility = orientation == "H" ? ViewStates.Visible : ViewStates.Gone;
			imageWrapperVer.Visibility = orientation == "H" ? ViewStates.Gone : ViewStates.Visible;
		}

		void toggleSide (MobileCardImage.DisplayMode mode)
		{
			switch (mode) {
			case MobileCardImage.DisplayMode.Front: {
					btnCardFront.SetBackgroundResource (Resource.Color.buttonFontColor);
					btnCardFront.SetTextColor (Resources.GetColor (Resource.Color.buttonWhite));
					btnCardBack.SetBackgroundResource (Resource.Color.buttonWhite);
					btnCardBack.SetTextColor (Resources.GetColor (Resource.Color.buttonFontColor));
					SelectedDisplayMode = MobileCardImage.DisplayMode.Front;

					break;
				}
			case MobileCardImage.DisplayMode.Back: {
					btnCardFront.SetBackgroundResource (Resource.Color.buttonWhite);
					btnCardFront.SetTextColor (Resources.GetColor (Resource.Color.buttonFontColor));
					btnCardBack.SetBackgroundResource (Resource.Color.buttonFontColor);
					btnCardBack.SetTextColor (Resources.GetColor (Resource.Color.buttonWhite));
					SelectedDisplayMode = MobileCardImage.DisplayMode.Back;

					break;
				}
			}
			loadCardImage (mode);
		}

		public void OnCheckedChanged (RadioGroup group, int checkedId)
		{
			var checkedRadioButton = group.FindViewById<RadioButton> (checkedId);
			var val = checkedRadioButton.Tag.ToString ();
			setOrientation (val);
		}
	}
}

