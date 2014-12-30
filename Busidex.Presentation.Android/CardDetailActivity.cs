
//using System;

using Android.App;
using Android.Net;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using Android.Content;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Busidex")]			
	public class CardDetailActivity : BaseActivity
	{
		public UserCard Card { get; set; }
		string FrontFileName{ get; set; }
		string BackFileName{ get; set; }
		enum CardViewState{
			Loading = 1,
			Front = 2,
			Back = 3
		}
		ImageButton btnCard { get; set; }

		CardViewState ViewState {get;set;}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var data = Intent.GetStringExtra ("Card");
			Card = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data);

			SetContentView (Resource.Layout.Card);
			btnCard = FindViewById<ImageButton> (Resource.Id.imgCard);
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

		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			if(Card != null){
				this.Window.SetTitle(Card.Card.Name);
			}
		}
	}
}

