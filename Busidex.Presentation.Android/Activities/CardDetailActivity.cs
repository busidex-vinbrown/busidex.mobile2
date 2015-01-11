using System.IO;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Busidex")]			
	public class CardDetailActivity : BaseCardActivity
	{
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }

		ImageButton btnCard { get; set; }
		CardViewState ViewState {get;set;}
		enum CardViewState{
			Loading = 1,
			Front = 2,
			Back = 3
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Card);

			base.OnCreate (savedInstanceState);

			btnCard = FindViewById<ImageButton> (Resource.Id.imgCardDetail);
			btnCard.SetScaleType (ImageView.ScaleType.FitXy);

			btnCard.Click += delegate {
				ToggleImage();
			};

			ViewState = CardViewState.Loading;

			ToggleImage();
		}

		protected override void OnImageDownloadCompleted (Uri uri){
			btnCard = FindViewById<ImageButton> (Resource.Id.imgCardDetail);
			btnCard.SetImageURI (uri);
			HideLoadingSpinner();
		}

		void ToggleImage(){

			var frontFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, UserCard.Card.FrontFileName);
			var backFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, UserCard.Card.BackFileName);
			var frontUri = Uri.Parse (frontFileName);
			var backUri = Uri.Parse (backFileName);

			switch(ViewState){
			case CardViewState.Loading:{

					if (File.Exists (frontFileName)) {
						OnImageDownloadCompleted (frontUri);
					}else{

						ShowLoadingSpinner ();

						var imagePath = Busidex.Mobile.Resources.CARD_PATH + UserCard.Card.FrontFileName;

						Busidex.Mobile.Utils.DownloadImage (imagePath, Busidex.Mobile.Resources.DocumentsPath, UserCard.Card.FrontFileName).ContinueWith (t => {
							RunOnUiThread (() => {

								OnImageDownloadCompleted(frontUri);
							});
						});
					}
					ViewState = CardViewState.Front;
					break;
				}
			case CardViewState.Front:{

					if (UserCard.Card.BackFileId.ToString().Equals (Busidex.Mobile.Resources.EMPTY_CARD_ID)) {
						Finish ();
					} else {
						if (File.Exists (backFileName)) {
							OnImageDownloadCompleted(backUri);
						} else {
							ShowLoadingSpinner ();

							var imagePath = Busidex.Mobile.Resources.CARD_PATH + UserCard.Card.BackFileName;

							Busidex.Mobile.Utils.DownloadImage (imagePath, Busidex.Mobile.Resources.DocumentsPath, UserCard.Card.BackFileName).ContinueWith (t => {
								RunOnUiThread (() => {
									HideLoadingSpinner();
									OnImageDownloadCompleted(backUri);
								});
							});
						}
					}
					ViewState = CardViewState.Back;
					break;
				}
			case CardViewState.Back:{
					//Redirect(new Intent(this, typeof(MyBusidexActivity)));
					Finish ();
					break;
				}
			}
		}
	}
}

