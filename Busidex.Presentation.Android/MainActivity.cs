
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

		public override void OnBackPressed ()
		{
			// noop
			return;
		}

		void GoToMyBusidex(){
			Redirect(new Intent(this, typeof(MyBusidexActivity)));
		}

		async Task<bool> LoadMyBusidexAsync(bool force = false){
			var cookie = GetAuthCookie ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);

			if (File.Exists (fullFilePath) && CheckBusidexFileCache(fullFilePath) && CheckRefreshDate() && !force) {
				GoToMyBusidex ();
			} else {
				if (cookie != null) {

					ShowLoadingSpinner(Resources.GetString (Resource.String.Global_OneMoment));

					var ctrl = new Busidex.Mobile.MyBusidexController ();
					await ctrl.GetMyBusidex (cookie).ContinueWith(async r => {

						if (!string.IsNullOrEmpty (r.Result)) {
							MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);

							RunOnUiThread (() => {
								HideLoadingSpinner();

								ShowLoadingSpinner (
									Resources.GetString (Resource.String.Global_LoadingCards), 
									ProgressDialogStyle.Horizontal, 
									myBusidexResponse.MyBusidex.Busidex.Count);
							});

						    
							SetRefreshCookie();

							var cards = new List<UserCard> ();
							var idx = 0;
							var total = myBusidexResponse.MyBusidex.Busidex.Count;

							RunOnUiThread (() => UpdateLoadingSpinner (idx, total));

							foreach (var item in myBusidexResponse.MyBusidex.Busidex) {
								if (item.Card != null) {

									var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
									var bImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + item.Card.BackFileName;
									var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
									var bName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

									cards.Add (item);

									if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fName) || force) {
										await Busidex.Mobile.Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
											RunOnUiThread (() => {
												idx++;
												if(idx == total){
													HideLoadingSpinner();
													GoToMyBusidex ();
												}else{
													UpdateLoadingSpinner (idx, total);
												}
											});
										});
									} else{
										RunOnUiThread (() => {
											idx++;
											if(idx == total){
												HideLoadingSpinner();
												GoToMyBusidex ();
											}else{
												UpdateLoadingSpinner (idx, total);
											}
										});
									}

									if ((!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + bName) || force) && item.Card.BackFileId.ToString () != Busidex.Mobile.Resources.EMPTY_CARD_ID) {
										await Busidex.Mobile.Utils.DownloadImage (bImageUrl, Busidex.Mobile.Resources.DocumentsPath, bName).ContinueWith (t => {
										});
									}

								}
							}

							RunOnUiThread (() => {
								Busidex.Mobile.Utils.SaveResponse(r.Result, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
								HideLoadingSpinner();
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


