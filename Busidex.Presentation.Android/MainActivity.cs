
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.IO;
using Busidex.Mobile.Models;
using System;
using System.Collections.Generic;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Busidex", Icon = "@drawable/icon")]
	public class MainActivity : BaseActivity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Main);

			var btnSearch = FindViewById<Button> (Resource.Id.btnSearch);
			var btnMyBusidex = FindViewById<Button> (Resource.Id.btnMyBusidex);
			var btnMyOrganizations = FindViewById<Button> (Resource.Id.btnMyOrganizations);

			btnSearch.Click += delegate {
				Redirect(new Intent(this, typeof(SearchActivity)));
			};

			btnMyBusidex.Click += delegate {
				LoadMyBusidexAsync();
			};

			btnMyOrganizations.Click += delegate {
				Redirect(new Intent(this, typeof(MyOrganizationsActivity)));
			};
		}


		void GoToMyBusidex(){
			Redirect(new Intent(this, typeof(MyBusidexActivity)));
		}

		bool CheckRefreshCookie(){
			return true;
		}

		void SetRefreshCookie(){

		}

		async Task<bool> LoadMyBusidexAsync(bool force = false){
			var account = GetAuthCookie ();
			var cookies = account.Cookies.GetCookies(new Uri(Busidex.Mobile.Resources.COOKIE_URI));
			var cookie = cookies [Busidex.Mobile.Resources.AUTHENTICATION_COOKIE_NAME];

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshCookie() && !force) {
				GoToMyBusidex ();
			} else {
				if (cookie != null) {

//					var overlay = new MyBusidexLoadingOverlay (View.Bounds);
//					overlay.MessageText = Resources.GetString(Resource.String.Global_LoadingCards);
//
//					View.AddSubview (overlay);

					var ctrl = new Busidex.Mobile.MyBusidexController ();
					await ctrl.GetMyBusidex (cookie.Value).ContinueWith(async r => {

						if (!string.IsNullOrEmpty (r.Result)) {
							MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);

						    Busidex.Mobile.Utils.SaveResponse(r.Result, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
							SetRefreshCookie();

							var cards = new List<UserCard> ();
							var idx = 0;
//							InvokeOnMainThread (() =>{
//								overlay.TotalItems = myBusidexResponse.MyBusidex.Busidex.Count();
//								overlay.UpdateProgress (idx);
//							});

							foreach (var item in myBusidexResponse.MyBusidex.Busidex) {
								if (item.Card != null) {

									var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
									var bImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + item.Card.BackFileName;
									var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
									var bName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

									cards.Add (item);

									if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fName) || force) {
										await Busidex.Mobile.Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
											//InvokeOnMainThread ( () => overlay.UpdateProgress (idx));
										});
									} else{
										//InvokeOnMainThread (() => overlay.UpdateProgress (idx));
									}

									if ((!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + bName) || force) && item.Card.BackFileId.ToString () != Busidex.Mobile.Resources.EMPTY_CARD_ID) {
										await Busidex.Mobile.Utils.DownloadImage (bImageUrl, Busidex.Mobile.Resources.DocumentsPath, bName).ContinueWith (t => {
										});
									}
									idx++;
								}
							}

							RunOnUiThread (() => {
								//overlay.Hide();
								GoToMyBusidex ();
							});

						}
					});
				}

			}
			return true;
		}
	}
}


