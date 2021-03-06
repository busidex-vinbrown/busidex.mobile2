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
using Android.Views;
using Android.Views.Animations;
using Android.Animation;
using Android.Preferences;
using System.Threading;

namespace Busidex.Presentation.Android
{
	public class MainFragment : BaseFragment, GestureDetector.IOnGestureListener, View.IOnTouchListener
	{


		public override bool OnTouch (View v, MotionEvent e)
		{
			_detector.OnTouchEvent (e);
			return true;
		}

		static bool animationsLoaded = false;


		bool eventsRefreshing;
		bool organizationsRefreshing;
		bool myBusidexRefreshing;
		public bool profileIsOpen;

		Button btnSearch;
		Button btnMyBusidex;
		Button btnMyOrganizations;
		Button btnEvents;
		ImageButton btnLogout;

		ImageView imgSearchIcon;
		ImageView imgBusidexIcon;
		ImageView imgOrgIcon;
		ImageView imgEventIcon;

		TextView txtNotificationCount;



		Dialog helpDialog;
		public bool interceptTouchEvents = true;

		#region Touch Events
		public override bool OnDown (MotionEvent e)
		{
			return interceptTouchEvents;
		}

		public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			const float SWIPE_THRESHOLD = 400;
			// detect swipe right
			if (e2.GetY () - e1.GetY () > SWIPE_THRESHOLD) {
				
				var profileFragment = ((SplashActivity)Activity).fragments[typeof(ProfileFragment).Name];
				((SplashActivity)Activity).LoadFragment (
					profileFragment,
					Resource.Animator.SlideUpAnimation, 
					Resource.Animator.SlideDownAnimation);
				
			}

			if (e1.GetY () - e2.GetY () > SWIPE_THRESHOLD) {
				Sync ().ContinueWith(r => {

				});
			}
			return true;
		}

		public override void OnLongPress (MotionEvent e)
		{

		}

		public override bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return false;
		}

		public override void OnShowPress (MotionEvent e)
		{

		}

		public override bool OnSingleTapUp (MotionEvent e)
		{
			return false;
		}
			

		#endregion

		ImageView btnSharedCardsNotification;

		//GestureDetector _detector;

		bool isRefreshing(){
			return organizationsRefreshing || myBusidexRefreshing || eventsRefreshing || profileIsOpen;
		}

		int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int) ((pixelValue) * Resources.DisplayMetrics.Density);
			return dp;
		}
			
		public override void OnResume ()
		{
			base.OnResume ();
			ThreadPool.QueueUserWorkItem (o => LoadEventList (true));
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			var mainView = inflater.Inflate (Resource.Layout.Main, container, false);

			//SetContentView (Resource.Layout.Main);

			//_detector = new GestureDetector(this);

			//LoadProfileFragment ();
			mainView.SetOnTouchListener( this );

			btnSearch = mainView.FindViewById<Button> (Resource.Id.btnSearch);
			btnMyBusidex = mainView.FindViewById<Button> (Resource.Id.btnMyBusidex);
			btnMyOrganizations = mainView.FindViewById<Button> (Resource.Id.btnMyOrganizations);
			btnEvents = mainView.FindViewById<Button> (Resource.Id.btnEvents);
			btnSharedCardsNotification = mainView.FindViewById<ImageView> (Resource.Id.btnSharedCardsNotification);

			imgSearchIcon = mainView.FindViewById<ImageView> (Resource.Id.imgSearchIcon);
			imgBusidexIcon = mainView.FindViewById<ImageView> (Resource.Id.imgBusidexIcon);
			imgOrgIcon = mainView.FindViewById<ImageView> (Resource.Id.imgOrgIcon);
			imgEventIcon = mainView.FindViewById<ImageView> (Resource.Id.imgEventIcon);

			txtNotificationCount = mainView.FindViewById<TextView> (Resource.Id.txtNotificationCount);

			#region Startup Animations

			if(!animationsLoaded){
				const int duration = 150;
				const float mStart = -400;
				const float mEnd = 40;

				imgSearchIcon.Visibility = imgBusidexIcon.Visibility = imgOrgIcon.Visibility = imgEventIcon.Visibility = ViewStates.Invisible;
				btnSearch.Visibility = btnMyBusidex.Visibility = btnMyOrganizations.Visibility = btnEvents.Visibility = ViewStates.Invisible;

				ValueAnimator animator1 = ValueAnimator.OfFloat(ConvertPixelsToDp(mStart), ConvertPixelsToDp(mEnd));
				animator1.SetDuration(duration);
				animator1.Update += (sender, e) => {
					imgSearchIcon.Visibility = ViewStates.Visible;
					float newLeft = (float)e.Animation.AnimatedValue;
					var layoutParams = (RelativeLayout.LayoutParams)imgSearchIcon.LayoutParameters;
					layoutParams.LeftMargin = (int)newLeft;
					imgSearchIcon.LayoutParameters = layoutParams;
				};

				ValueAnimator animator2 = ValueAnimator.OfFloat(ConvertPixelsToDp(mStart), ConvertPixelsToDp(mEnd));
				animator2.SetDuration(duration);
				animator2.Update += (sender, e) => {
					float newLeft = (float)e.Animation.AnimatedValue;
					var layoutParams = (RelativeLayout.LayoutParams)imgBusidexIcon.LayoutParameters;
					layoutParams.LeftMargin = (int)newLeft;
					imgBusidexIcon.LayoutParameters = layoutParams;
				};

				ValueAnimator animator3 = ValueAnimator.OfFloat(ConvertPixelsToDp(mStart), ConvertPixelsToDp(mEnd));
				animator3.SetDuration(duration);
				animator3.Update += (sender, e) => {
					float newLeft = (float)e.Animation.AnimatedValue;
					var layoutParams = (RelativeLayout.LayoutParams)imgOrgIcon.LayoutParameters;
					layoutParams.LeftMargin = (int)newLeft;
					imgOrgIcon.LayoutParameters = layoutParams;
				};

				ValueAnimator animator4 = ValueAnimator.OfFloat(ConvertPixelsToDp(mStart), ConvertPixelsToDp(mEnd));
				animator4.SetDuration(duration);
				animator4.Update += (sender, e) => {
					float newLeft = (float)e.Animation.AnimatedValue;
					var layoutParams = (RelativeLayout.LayoutParams)imgEventIcon.LayoutParameters;
					layoutParams.LeftMargin = (int)newLeft;
					imgEventIcon.LayoutParameters = layoutParams;
				};

				animator1.Start();

				animator1.AnimationEnd += (sender, e) => {
					imgBusidexIcon.Visibility = ViewStates.Visible;
					btnSearch.Visibility = ViewStates.Visible;
					animator2.Start ();
				};
				animator2.AnimationEnd += (s, ee) => {
					imgOrgIcon.Visibility = ViewStates.Visible;
					btnMyBusidex.Visibility = ViewStates.Visible;
					animator3.Start ();
				};
				animator3.AnimationEnd += (s, ee) => {
					imgEventIcon.Visibility = ViewStates.Visible;
					btnMyOrganizations.Visibility = ViewStates.Visible;
					animator4.Start ();
				};
				animator4.AnimationEnd += (s, ee) => {
					btnEvents.Visibility = ViewStates.Visible;
				};
				animationsLoaded = true;
			}
			#endregion

			btnLogout = mainView.FindViewById<ImageButton> (Resource.Id.btnLogout);

			btnSharedCardsNotification.Click -= GoToSharedCards;
			btnSharedCardsNotification.Click += GoToSharedCards;

			btnSearch.Click += delegate {
				if(!interceptTouchEvents) return;
				GoToSearch();
			};

			imgSearchIcon.Click += delegate {
				if(!interceptTouchEvents) return;
				GoToSearch();
			};

			btnMyBusidex.Touch += async delegate(object sender, View.TouchEventArgs e) {
				if(!interceptTouchEvents) return;
				if(e.Event.Action == MotionEventActions.Up){
					GoToMyBusidex();
				}
			};

			imgBusidexIcon.Click += async delegate {
				if(!interceptTouchEvents) return;
				GoToMyBusidex();
			};

			btnMyOrganizations.Click += async delegate {
				if(!interceptTouchEvents) return;
				GoToMyOrganizations();
			};

			imgOrgIcon.Click += async delegate {
				if(!interceptTouchEvents) return;
				GoToMyOrganizations();
			};

			imgEventIcon.Click += async delegate {
				if(!interceptTouchEvents) return;
				GoToEventList();
			};

			btnEvents.Click += async delegate {
				if(!interceptTouchEvents) return;
				GoToEventList();
			};

			btnLogout.Click += delegate {
				if(!interceptTouchEvents) return;
				Logout();
			};




			return mainView;
		}

		void CheckPreferences(){

			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity); 
			bool popupSeen = prefs.GetBoolean (Busidex.Mobile.Resources.PREFERENCE_FIRST_USE_POPUP_SEEN, false);
			if(!popupSeen){

				var inflater = (LayoutInflater)Activity.GetSystemService(Context.LayoutInflaterService);
				View popupContent = inflater.Inflate (Resource.Layout.StartPopup, null);

				helpDialog = new Dialog (Activity);
				helpDialog.AddContentView (popupContent,new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
				helpDialog.SetCancelable (false);

				Button btnOk = popupContent.FindViewById<Button> (Resource.Id.btnOk);
				btnOk.Click += (sender, e) => {
					//ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this); 
					ISharedPreferencesEditor editor = prefs.Edit();
					editor.PutBoolean (Busidex.Mobile.Resources.PREFERENCE_FIRST_USE_POPUP_SEEN, true);
					editor.Apply ();
					helpDialog.Dismiss ();
				};

				helpDialog.Show ();
			}
		}

		void SetNotificationUI(){
			var notificationCount = GetNotifications();
			txtNotificationCount.Text = notificationCount.ToString();
			btnSharedCardsNotification.Visibility = txtNotificationCount.Visibility = notificationCount > 0 ? ViewStates.Visible : ViewStates.Gone;
		}

		void Logout(){

			ShowAlert (
				Activity.GetString (Resource.String.Global_Logout_Title),
				GetString (Resource.String.Global_Logout_Message), 
				GetString (Resource.String.Global_ButtonText_Logout),
				new EventHandler<DialogClickEventArgs> ((o, e) => {

					var dialog = o as AlertDialog;
					Button btnClicked = dialog.GetButton(e.Which);
					if (btnClicked.Text == GetString (Resource.String.Global_ButtonText_Logout)) {
						applicationResource.RemoveAuthCookie ();
						Utils.RemoveCacheFiles ();

						Redirect (new StartUpFragment ());
					}
				}));
		}

		async Task<bool> Sync(){

			if(isRefreshing()){
				return false;
			}

			var notificationCount = GetNotifications();
			if(notificationCount > 0){
				txtNotificationCount.Text = notificationCount.ToString();

				btnSharedCardsNotification.Visibility = ViewStates.Visible;
			}

			const bool FORCE = true;
			await LoadMyBusidexAsync(FORCE).ContinueWith( async r=>{
				await LoadMyOrganizationsAsync(FORCE).ContinueWith( async r2 => {
					await LoadEventList (FORCE);
				});
			});

			return true;
		}

		void GoToSearch(){
			Redirect(((SplashActivity)Activity).fragments[typeof(SearchFragment).Name]);
		}


		void GoToMyBusidex(){
			Redirect(((SplashActivity)Activity).fragments[typeof(MyBusidexFragment).Name]);
		}

		void GoToSharedCards(object sender, EventArgs args){
			Redirect(((SplashActivity)Activity).fragments[typeof(SharedCardsFragment).Name]);
		}

		void GoToEventList(){
			Redirect(((SplashActivity)Activity).fragments[typeof(EventListFragment).Name]);
		}

		int GetNotifications(){

			var ctrl = new SharedCardController ();
			var cookie = applicationResource.GetAuthCookie ();
			var sharedCardsResponse = ":404";//ctrl.GetSharedCards (cookie);
			if(sharedCardsResponse.Contains(":404")){
				return 0;
			}

			var sharedCards = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsResponse);

			Utils.SaveResponse (sharedCardsResponse, Busidex.Mobile.Resources.SHARED_CARDS_FILE);

			foreach (SharedCard card in sharedCards.SharedCards) {
				var fileName = card.Card.FrontFileName;
				var fImagePath = Busidex.Mobile.Resources.CARD_PATH + fileName;
				if (!File.Exists (Busidex.Mobile.Resources.DocumentsPath + "/" + fileName)) {
					Utils.DownloadImage (fImagePath, Busidex.Mobile.Resources.DocumentsPath, fileName).ContinueWith (t => {

					});
				}
			}

			return sharedCards != null ? sharedCards.SharedCards.Count : 0;
		}



		async Task<bool> LoadMyBusidexAsync(bool force = false){

			myBusidexRefreshing = true;

			var cookie = applicationResource.GetAuthCookie ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);

			if (File.Exists (fullFilePath) && CheckBusidexFileCache(fullFilePath) && applicationResource.CheckRefreshDate(Busidex.Mobile.Resources.BUSIDEX_REFRESH_COOKIE_NAME) && !force) {
				myBusidexRefreshing = false;
				GoToMyBusidex ();
			} else {
				if (cookie != null) {

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
													myBusidexRefreshing = false;
													if(!force){
														GoToMyBusidex ();
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
												myBusidexRefreshing = false;
												if(!force){
													GoToMyBusidex ();
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
								myBusidexRefreshing = false;
								if(!force){
									GoToMyBusidex ();
								}
							});
						}else{
							myBusidexRefreshing = false;
						}
					});
				}else{
					myBusidexRefreshing = false;
				}

			}
			return true;
		}

		async Task<bool> LoadEventList(bool force = false){

			eventsRefreshing = true;
			var cookie = applicationResource.GetAuthCookie ();

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.EVENT_LIST_FILE);
			if (File.Exists (fullFilePath) && applicationResource.CheckRefreshDate (Busidex.Mobile.Resources.EVENT_LIST_REFRESH_COOKIE_NAME) && !force) {
				eventsRefreshing = false;
				GoToEventList ();
			} else {
				try {
					var controller = new SearchController ();
					await controller.GetEventTags (cookie).ContinueWith(r => {
						if (!string.IsNullOrEmpty (r.Result)) {

							Activity.RunOnUiThread (() => {
								Utils.SaveResponse (r.Result, Busidex.Mobile.Resources.EVENT_LIST_FILE);

								applicationResource.SetRefreshCookie(Busidex.Mobile.Resources.EVENT_LIST_REFRESH_COOKIE_NAME);
								eventsRefreshing = false;
								if(!force){
									GoToEventList ();
								}
							});
						}
					});

				} catch (Exception ignore) {

				}
			}

			return true;
		}
	}
}