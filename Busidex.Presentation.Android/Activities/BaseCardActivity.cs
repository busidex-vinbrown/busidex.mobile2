
using Android.App;
using Android.OS;
using Busidex.Mobile.Models;
using System.IO;
using Busidex.Mobile;
using Android.Net;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "BaseCardActivity")]			
	public class BaseCardActivity : BaseActivity
	{
		protected UserCard UserCard { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var data = Intent.GetStringExtra ("Card");
			if (!string.IsNullOrEmpty (data)) {
				UserCard = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data);

				var frontFileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, UserCard.Card.FrontFileName);
				var frontUri = Uri.Parse (frontFileName);

				if (File.Exists (frontFileName)) {
					OnImageDownloadCompleted(frontUri);
				}else{

					//ShowLoadingSpinner ();

					Utils.DownloadImage (Busidex.Mobile.Resources.CARD_PATH + UserCard.Card.FrontFileName, Busidex.Mobile.Resources.DocumentsPath, UserCard.Card.FrontFileName).ContinueWith (t => {
						RunOnUiThread (() => {
							//HideLoadingSpinner ();
							OnImageDownloadCompleted (frontUri);
						});
					});
				}
			}
		}

		protected virtual void OnImageDownloadCompleted(Uri uri){

		}

		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			if(UserCard != null){
				Window.SetTitle(UserCard.Card.Name);
			}
		}
	}
}

