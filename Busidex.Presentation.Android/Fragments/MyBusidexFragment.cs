
using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using System.IO;
using System.Threading.Tasks;
using Busidex.Mobile;
using Android.App;
using System.Threading;

namespace Busidex.Presentation.Android
{
	public class MyBusidexFragment : BaseFragment
	{
		//List<UserCard> Cards { get; set; }
		static UserCardAdapter MyBusidexAdapter { get; set; }
		static SearchView txtFilter { get; set; }
		ListView lstCards;
		TextView lblNoCardsMessage;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_LIST, 0);
		}
			
		public override void OnResume ()
		{
			base.OnResume ();
			ThreadPool.QueueUserWorkItem( o => LoadMyBusidexAsync ());
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			var view = inflater.Inflate(Resource.Layout.MyBusidex, container, false);

			lstCards = view.FindViewById<ListView> (Resource.Id.lstCards);
			txtFilter = view.FindViewById<SearchView> (Resource.Id.txtFilter);
			lblNoCardsMessage = view.FindViewById<TextView> (Resource.Id.lblNoCardsMessage);

			return view;
		}

		static void DoFilter(string filter){
			if(string.IsNullOrEmpty(filter)){
				MyBusidexAdapter.Filter.InvokeFilter ("");
			}else{
				MyBusidexAdapter.Filter.InvokeFilter(filter);
			}
		}

		async Task<bool> LoadMyBusidexAsync(bool force = false){

			//myBusidexRefreshing = true;

			var cookie = applicationResource.GetAuthCookie ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);

			if (File.Exists (fullFilePath) && CheckBusidexFileCache(fullFilePath) && applicationResource.CheckRefreshDate(Busidex.Mobile.Resources.BUSIDEX_REFRESH_COOKIE_NAME) && !force) {
				//myBusidexRefreshing = false;
				if(UISubscriptionService.UserCards == null){
					Activity.RunOnUiThread (() => ShowLoadingSpinner (GetString (Resource.String.Global_LoadingCards)));

					await LoadFromFile (fullFilePath);
				}else{
					await ProcessFile(null);
				}

			} else {
				if (cookie != null) {

					Activity.RunOnUiThread (() => ShowLoadingSpinner (GetString (Resource.String.Global_LoadingCards)));

					UISubscriptionService.UserCards = null;

					Activity.RunOnUiThread (() => ShowLoadingSpinner (Resources.GetString (Resource.String.Global_OneMoment)));

					var ctrl = new MyBusidexController ();
					await ctrl.GetMyBusidex (cookie).ContinueWith(async r => {

						if (!string.IsNullOrEmpty (r.Result)) {
							MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);

							Activity.RunOnUiThread (() => {
								HideLoadingSpinner();

								ShowLoadingSpinner (
									Resources.GetString (Resource.String.Global_LoadingCards), 
									ProgressDialogStyle.Horizontal, 
									myBusidexResponse.MyBusidex.Busidex.Count);
							});


							applicationResource.SetRefreshCookie(Busidex.Mobile.Resources.BUSIDEX_REFRESH_COOKIE_NAME);

							var cards = new List<UserCard> ();
							var idx = 0;
							var total = myBusidexResponse.MyBusidex.Busidex.Count;

							Activity.RunOnUiThread (() => UpdateLoadingSpinner (idx, total));

							foreach (var item in myBusidexResponse.MyBusidex.Busidex) {
								if (item.Card != null) {

									var fImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
									var bImageUrl = Busidex.Mobile.Resources.THUMBNAIL_PATH + item.Card.BackFileName;
									var fName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
									var bName = Busidex.Mobile.Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

									cards.Add (item);

									if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fName) || force) {
										await Utils.DownloadImage (fImageUrl, Busidex.Mobile.Resources.DocumentsPath, fName).ContinueWith (t => {
											Activity.RunOnUiThread (() => {
												idx++;
												if(idx == total){
													HideLoadingSpinner();
													//myBusidexRefreshing = false;
													if(!force){
														//GoToMyBusidex ();
														//LoadFromFile (fullFilePath);
													}
												}else{
													UpdateLoadingSpinner (idx, total);
												}
											});
										});
									} else{
										Activity.RunOnUiThread (() => {
											idx++;
											if(idx == total){
												HideLoadingSpinner();
												//myBusidexRefreshing = false;
												if(!force){
													//GoToMyBusidex ();
													//LoadFromFile (fullFilePath);
												}
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

							Activity.RunOnUiThread (() => {
								Utils.SaveResponse(r.Result, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
								HideLoadingSpinner();
								//myBusidexRefreshing = false;
								if(!force){
									//GoToMyBusidex ();
									LoadFromFile (fullFilePath);
								}
							});
						}
					});
				}

			}
			return true;
		}

		protected override async Task<bool> ProcessFile(string data){

			if (UISubscriptionService.UserCards == null) {
				var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);
				myBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);

				UISubscriptionService.UserCards = myBusidexResponse.MyBusidex.Busidex;
			}

			if (UISubscriptionService.UserCards != null) {
				Activity.RunOnUiThread (() => {
					MyBusidexAdapter = new UserCardAdapter (Activity, Resource.Id.lstCards, UISubscriptionService.UserCards);

					MyBusidexAdapter.Redirect += ShowCard;
					MyBusidexAdapter.SendEmail += SendEmail;
					MyBusidexAdapter.OpenBrowser += OpenBrowser;
					MyBusidexAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
					MyBusidexAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
					MyBusidexAdapter.OpenMap += OpenMap;

					MyBusidexAdapter.ShowNotes = true;


					lblNoCardsMessage.Text = GetString (Resource.String.MyBusidex_NoCards);

					lblNoCardsMessage.Visibility = UISubscriptionService.UserCards.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
					txtFilter.Visibility = UISubscriptionService.UserCards.Count == 0 ? ViewStates.Gone : ViewStates.Visible;

					lstCards.Adapter = MyBusidexAdapter;

					txtFilter.QueryTextChange += delegate {
						DoFilter (txtFilter.Query);
					};

					txtFilter.Iconified = false;
					txtFilter.ClearFocus();

					lstCards.RequestFocus (FocusSearchDirection.Down);


					txtFilter.Touch += delegate {
						txtFilter.Focusable = true;
						txtFilter.RequestFocus ();
					};

					HideLoadingSpinner ();
					DismissKeyboard (txtFilter.WindowToken, Activity);
				});
			}

			return true;
		}
	}
}

