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
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Card);
			btnCard = FindViewById<ImageButton> (Resource.Id.imgCardDetail);
			btnCard.SetScaleType (ImageView.ScaleType.FitXy);

			btnCard.Click += delegate {
				ToggleImage();
			};

			ViewState = CardViewState.Loading;

			ToggleImage();
		}
			
		void ToggleImage(){

			var frontFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Card.Card.FrontFileName);
			var backFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Card.Card.BackFileName);
			var frontUri = Uri.Parse (frontFileName);
			var backUri = Uri.Parse (backFileName);

			switch(ViewState){
			case CardViewState.Loading:{

					if (File.Exists (frontFileName)) {
						btnCard.SetImageURI (frontUri);
					}else{

						//ShowOverlay ();

						Busidex.Mobile.Utils.DownloadImage (Busidex.Mobile.Resources.CARD_PATH + Card.Card.FrontFileName, Busidex.Mobile.Resources.DocumentsPath, Card.Card.FrontFileName).ContinueWith (t => {
							RunOnUiThread (() => {
								btnCard.SetImageURI (frontUri);
								//Overlay.Hide();
							});
						});
					}
					ViewState = CardViewState.Front;
					break;
				}
			case CardViewState.Front:{

					if (Card.Card.BackFileId.ToString().Equals (Busidex.Mobile.Resources.EMPTY_CARD_ID)) {
						Redirect(new Intent(this, typeof(MyBusidexActivity)));
					} else {
						if (File.Exists (backFileName)) {
							btnCard.SetImageURI (backUri);
						} else {
							//ShowOverlay ();
							Busidex.Mobile.Utils.DownloadImage (Busidex.Mobile.Resources.CARD_PATH + Card.Card.BackFileName, Busidex.Mobile.Resources.DocumentsPath, Card.Card.BackFileName).ContinueWith (t => {
								RunOnUiThread (() => {
									btnCard.SetImageURI (backUri);
									//Overlay.Hide();
								});
							});
						}
					}
					ViewState = CardViewState.Back;
					break;
				}
			case CardViewState.Back:{
					Redirect(new Intent(this, typeof(MyBusidexActivity)));
					break;
				}
			}
		}
	}
}

