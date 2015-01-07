﻿
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.IO;
using Busidex.Mobile.Models;
using System;
using System.Collections.Generic;
using Busidex.Mobile;
using System.Linq;

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
				LoadMyOrganizationsAsync();
			};
		}

		public override void OnBackPressed ()
		{
			// noop
			return;
		}

		void GoToMyOrganizations(){
			Redirect(new Intent(this, typeof(MyOrganizationsActivity)));
		}

		void GoToMyBusidex(){
			Redirect(new Intent(this, typeof(MyBusidexActivity)));
		}

		public async Task<bool> LoadMyOrganizationsAsync(bool force = false){

			var cookie = GetAuthCookie ();
			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_ORGANIZATIONS_FILE);
			if (File.Exists (fullFilePath) && CheckRefreshDate (Busidex.Mobile.Resources.ORGANIZATION_REFRESH_COOKIE_NAME) && !force) {
				GoToMyOrganizations ();
			} else {
				if (cookie != null) {

					ShowLoadingSpinner(Resources.GetString (Resource.String.Global_OneMoment));

					var controller = new OrganizationController ();
					await controller.GetMyOrganizations (cookie).ContinueWith (async response => {
						if (!string.IsNullOrEmpty (response.Result)) {

							OrganizationResponse myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (response.Result);

							Utils.SaveResponse(response.Result, Busidex.Mobile.Resources.MY_ORGANIZATIONS_FILE);
							SetRefreshCookie(Busidex.Mobile.Resources.ORGANIZATION_REFRESH_COOKIE_NAME);

							foreach (Organization org in myOrganizationsResponse.Model) {
								var fileName = org.LogoFileName + "." + org.LogoType;
								var fImagePath = Busidex.Mobile.Resources.CARD_PATH + fileName;
								if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fileName)) {
									await Utils.DownloadImage (fImagePath, Busidex.Mobile.Resources.DocumentsPath, fileName).ContinueWith (t => {

									});
								} 
								// load organization members
								await controller.GetOrganizationMembers(cookie, org.OrganizationId).ContinueWith(async cards =>{

									OrgMemberResponse orgMemberResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (cards.Result);
									Utils.SaveResponse(cards.Result, Busidex.Mobile.Resources.ORGANIZATION_MEMBERS_FILE + org.OrganizationId);

									var idx = 0;
									RunOnUiThread (() => UpdateLoadingSpinner (idx, myOrganizationsResponse.Model.Count ()));
									foreach(var card in orgMemberResponse.Model){

										var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.FrontFileName;
										var bImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.BackFileName;
										var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
										var bName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
										if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fName) || force) {
											await Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
												RunOnUiThread (() => UpdateLoadingSpinner (idx, myOrganizationsResponse.Model.Count ()));
											});
										}
										if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString().ToLowerInvariant() != Busidex.Mobile.Resources.EMPTY_CARD_ID) {
											await Utils.DownloadImage (bImageUrl, Busidex.Mobile.Resources.DocumentsPath, bName).ContinueWith (t => {

											});
										}
										idx++;
									}
								});

								await controller.GetOrganizationReferrals(cookie, org.OrganizationId).ContinueWith(async cards =>{

									var orgReferralResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (cards.Result);
									Utils.SaveResponse(cards.Result, Busidex.Mobile.Resources.ORGANIZATION_REFERRALS_FILE + org.OrganizationId);

									var idx = 0;
									RunOnUiThread (() => UpdateLoadingSpinner (idx, myOrganizationsResponse.Model.Count ()));

									foreach(var card in orgReferralResponse.Model){

										var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.Card.FrontFileName;
										var bImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + card.Card.BackFileName;
										var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName;
										var bName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.BackFileName;
										if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fName) || force) {
											await Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
												RunOnUiThread (() => UpdateLoadingSpinner (idx, myOrganizationsResponse.Model.Count ()));
											});
										}
										if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + bName) && card.Card.BackFileId.ToString().ToLowerInvariant() != Busidex.Mobile.Resources.EMPTY_CARD_ID) {
											await Utils.DownloadImage (bImageUrl, Busidex.Mobile.Resources.DocumentsPath, bName).ContinueWith (t => {

											});
										}
										idx++;
									}
								});
							}

							RunOnUiThread (() => {
								HideLoadingSpinner();
								GoToMyOrganizations ();
							});
						}
					});
				}
			}
			return true;
		}

		async Task<bool> LoadMyBusidexAsync(bool force = false){
			var cookie = GetAuthCookie ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);

			if (File.Exists (fullFilePath) && CheckBusidexFileCache(fullFilePath) && CheckRefreshDate(Busidex.Mobile.Resources.BUSIDEX_REFRESH_COOKIE_NAME) && !force) {
				GoToMyBusidex ();
			} else {
				if (cookie != null) {

					ShowLoadingSpinner(Resources.GetString (Resource.String.Global_OneMoment));

					var ctrl = new MyBusidexController ();
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

						    
							SetRefreshCookie(Busidex.Mobile.Resources.BUSIDEX_REFRESH_COOKIE_NAME);

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
										await Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
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
										await Utils.DownloadImage (bImageUrl, Busidex.Mobile.Resources.DocumentsPath, bName).ContinueWith (t => {
										});
									}

								}
							}

							RunOnUiThread (() => {
								Utils.SaveResponse(r.Result, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
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

