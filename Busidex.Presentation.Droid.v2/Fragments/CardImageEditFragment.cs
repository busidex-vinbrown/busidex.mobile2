using System;
using System.Drawing;
using System.IO;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
//using Android.Views.Animations;
using Android.Widget;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class CardImageEditFragment : BaseCardEditFragment, RadioGroup.IOnCheckedChangeListener
	{
		ImageButton btnBack;
		Button btnCardFront;
		Button btnCardBack;
		ImageButton btnCardImageHor;
		ImageButton btnCardImageVer;
		RelativeLayout imageWrapperHor;
		RelativeLayout imageWrapperVer;

		string SelectedFrontOrientation;
		string SelectedBackOrientation;
		CardEditMode SelectedCardEditMode;

		const int PIC_CROP = 2;
		const string ORIENTATION_HORIZONTAL = "H";
		const string ORIENTATION_VERTICAL = "V";

		enum CardEditMode
		{
			Front,
			Back
		}

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

		public override void OnResume ()
		{
			base.OnResume ();

			if (IsVisible) {
				BaseApplicationResource.TrackScreenView (Mobile.Resources.GA_SCREEN_CARD_IMAGE);
			}
		}

		void cropImage (Android.Net.Uri srcImage, RectangleF rect)
		{
			var cropIntent = new Intent ("com.android.camera.action.CROP");
			//indicate image type and Uri
			cropIntent.SetData (srcImage);
			cropIntent.SetType ("image/*");
			//set crop properties
			cropIntent.PutExtra ("crop", "true");
			//indicate aspect of desired crop
			cropIntent.PutExtra ("aspectX", 1);
			cropIntent.PutExtra ("aspectY", 1);
			//indicate output X and Y
			cropIntent.PutExtra ("outputX", 256);
			cropIntent.PutExtra ("outputY", 256);
			//retrieve data on return
			cropIntent.PutExtra ("return-data", true);
			//start the activity - we handle returning in onActivityResult
			StartActivityForResult (cropIntent, PIC_CROP);
		}

		void setDisplay (CardEditMode mode, string fileName)
		{
			var uri = !string.IsNullOrEmpty (fileName) ? Android.Net.Uri.Parse (fileName) : null;

			btnCardImageHor.SetImageURI (uri);
			btnCardImageVer.SetImageURI (uri);

			setOrientation (mode == CardEditMode.Front ? SelectedCard.FrontOrientation : SelectedCard.BackOrientation);
			SelectedCardEditMode = mode;
		}

		void loadCardImage (CardEditMode mode)
		{
			try {

				if (!SelectedCard.FrontFileId.ToString ().Equals (Mobile.Resources.EMPTY_CARD_ID) &&
					!SelectedCard.FrontFileId.ToString ().Equals (Mobile.Resources.NULL_CARD_ID)) {
					var fileName = mode == CardEditMode.Front
													   ? SelectedCard.FrontFileId + "." + SelectedCard.FrontType
													   : SelectedCard.BackFileId + "." + SelectedCard.BackType;

					var fullFileName = Path.Combine (Mobile.Resources.DocumentsPath, fileName);
					if (File.Exists (fullFileName)) {
						setDisplay (mode, fullFileName);
					} else {

						//ShowOverlay ();

						Utils.DownloadImage (Mobile.Resources.CARD_PATH + fileName, Mobile.Resources.DocumentsPath, fileName).ContinueWith (t => {
							Activity.RunOnUiThread (() => {
								setDisplay (mode, fileName);
								//Overlay.Hide ();
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

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.CardImageEdit, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			updateCover = view.FindViewById<RelativeLayout> (Resource.Id.updateCover);
			updateCover.Visibility = ViewStates.Gone;

			imageWrapperHor = view.FindViewById<RelativeLayout> (Resource.Id.imageWrapperHor);
			imageWrapperVer = view.FindViewById<RelativeLayout> (Resource.Id.imageWrapperVer);
			btnCardImageHor = view.FindViewById<ImageButton> (Resource.Id.btnCardImageHor);
			btnCardImageVer = view.FindViewById<ImageButton> (Resource.Id.btnCardImageVer);
			btnCardFront = view.FindViewById<Button> (Resource.Id.btnCardFront);
			btnCardBack = view.FindViewById<Button> (Resource.Id.btnCardBack);

			btnCardFront.Click += delegate {
				toggleSide (CardEditMode.Front);
			};

			btnCardBack.Click += delegate {
				toggleSide (CardEditMode.Back);
			};

			btnBack = view.FindViewById<ImageButton> (Resource.Id.btnBack);
			btnBack.Click += delegate {
				((MainActivity)Activity).LoadFragment (new CardMenuFragment ());
			};

			SelectedFrontOrientation = SelectedCard.FrontOrientation;
			SelectedBackOrientation = SelectedCard.BackOrientation;

			var radioGroup = view.FindViewById<RadioGroup> (Resource.Id.rdoOrientation);
			radioGroup.SetOnCheckedChangeListener (this);

			loadCardImage (CardEditMode.Front);

			return view;
		}

		void toggleOrientation(){

			var orientation = string.Empty;

			if (SelectedCardEditMode == CardEditMode.Front) {
				SelectedFrontOrientation = (SelectedFrontOrientation == "H") ? "V" : "H";
				orientation = SelectedFrontOrientation;
			} else {
				SelectedBackOrientation = (SelectedBackOrientation == "H") ? "V" : "H";
				orientation = SelectedBackOrientation;
			}
			setOrientation (orientation);
		}

		void setOrientation(string orientation){

			imageWrapperHor.Visibility = orientation == "H" ? ViewStates.Visible : ViewStates.Gone;
			imageWrapperVer.Visibility = orientation == "H" ? ViewStates.Gone : ViewStates.Visible;

			//var rotateHorizontal = AnimationUtils.LoadAnimation (Activity, Resource.Animation.ResizeHorizontal);
			//var rotateVertical = AnimationUtils.LoadAnimation (Activity, Resource.Animation.ResizeVertical);

			//var h = orientation == "H" ? (int)((300) * Resources.DisplayMetrics.Density) : (int)((480) * Resources.DisplayMetrics.Density);
			//var w = orientation == "H" ? (int)((480) * Resources.DisplayMetrics.Density) : (int)((300) * Resources.DisplayMetrics.Density);

			//var layoutParams = new RelativeLayout.LayoutParams (w, h); //Width, Height


			//var rotate = orientation == "H"
			//	? rotateHorizontal
			//	: rotateVertical;
			
			//imageWrapperHor.StartAnimation (rotate);
		}

		void toggleSide (CardEditMode mode)
		{
			switch (mode) {
			case CardEditMode.Front: {
					btnCardFront.SetBackgroundResource (Resource.Color.buttonFontColor);
					btnCardFront.SetTextColor (Resources.GetColor (Resource.Color.buttonWhite));
					btnCardBack.SetBackgroundResource (Resource.Color.buttonWhite);
					btnCardBack.SetTextColor (Resources.GetColor (Resource.Color.buttonFontColor));
					break;
				}
			case CardEditMode.Back: {
					btnCardFront.SetBackgroundResource (Resource.Color.buttonWhite);
					btnCardFront.SetTextColor (Resources.GetColor (Resource.Color.buttonFontColor));
					btnCardBack.SetBackgroundResource (Resource.Color.buttonFontColor);
					btnCardBack.SetTextColor (Resources.GetColor (Resource.Color.buttonWhite));
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

