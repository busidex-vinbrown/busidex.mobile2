
using Android.App;
using Android.Content;
using Android.OS;
using System;
using Android.Gms.Analytics;
using System.Collections.Generic;
using System.IO;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Threading.Tasks;
using Android.Views.InputMethods;
using Android.Views;

namespace Busidex.Presentation.Android
{
	public class BaseFragment : Fragment, GestureDetector.IOnGestureListener, View.IOnTouchListener
	{
		#region IOnTouchListener 
		public bool OnTouch (View v, MotionEvent e)
		{
			return false;
		}
		#endregion

		#region IOnGestureListener
		public bool OnDown (MotionEvent e)
		{
			return false;
		}

		public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			return false;
		}

		public void OnLongPress (MotionEvent e)
		{
			//throw new NotImplementedException ();

		}

		public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return false;
		}

		public void OnShowPress (MotionEvent e)
		{
			
		}

		public bool OnSingleTapUp (MotionEvent e)
		{
			return false;
		}
		#endregion

		protected static ProgressDialog progressDialog;
		static Tracker _tracker;
		protected static Tracker GATracker { 
			get { 
				return _tracker; 
			} 
		}
			
		protected BusidexFragmentManager manager { get; set;}

		public BaseFragment(){
			uniqueId = this.GetType().Name;	
			if(Activity != null){
				manager = new BusidexFragmentManager (Activity);
			}

		}

		public string uniqueId{ get; protected set; }

		protected BaseApplicationResource applicationResource;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			_tracker = _tracker ?? GoogleAnalytics.GetInstance (Activity).NewTracker (Busidex.Mobile.Resources.GOOGLE_ANALYTICS_KEY_ANDROID);
			applicationResource = new BaseApplicationResource (Activity);
		}

		#region Navigation
		protected void RedirectToMainIfLoggedIn(){

			var cookie = applicationResource.GetAuthCookie ();
			if(cookie != null){
				Redirect(typeof(MainFragment));
			}
		}
			
		protected void Redirect(Type fragmentType){

			((BaseActivity)Activity).LoadFragment (fragmentType);
		}
		#endregion

		#region Loading
		protected virtual async Task<bool> ProcessFile(string data){
			return await new TaskFactory ().StartNew (() => {
				return true;
			});
		}

		protected bool CheckBusidexFileCache(string fullFilePath){
			if(File.Exists(fullFilePath)){
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				file.Close ();
				return !string.IsNullOrEmpty (fileJson);
			}
			return false;
		}

		protected async Task<bool> LoadFromFile(string fullFilePath){

			if(File.Exists(fullFilePath)){
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				file.Close ();
				await ProcessFile (fileJson);
			}
			return true;
		}
		#endregion

		#region Card Actions

		protected void ShowCard(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Details, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_DETAILS, 0);

			//Redirect(intent);
		}

		protected void SendEmail(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Email, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_EMAIL, 0);

			Activity.StartActivity(intent);
		}

		protected void OpenBrowser(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Website, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_URL, 0);

			var browserIntent = Intent.CreateChooser(intent, "Open with");
			Activity.StartActivity (browserIntent);
		}

		protected void OpenMap(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Map, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MAP, 0);

			Activity.StartActivity (intent);
		}

		protected void AddCardToMyBusidex(Intent intent){

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			var userCard = GetUserCardFromIntent (intent);
			if (userCard!= null) {
				userCard.Card.ExistsInMyBusidex = true;
				string file;
				string myBusidexJson;
				if (File.Exists (fullFilePath)) {
					using (var myBusidexFile = File.OpenText (fullFilePath)) {
						myBusidexJson = myBusidexFile.ReadToEnd ();
					}

					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
					myBusidexResponse.MyBusidex.Busidex.Add (userCard);
					file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);
					Utils.SaveResponse (file, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
				}
				var token = applicationResource.GetAuthCookie ();

				var controller = new MyBusidexController ();
				controller.AddToMyBusidex (userCard.Card.CardId, token);

				TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_ADD, 0);

				ActivityController.SaveActivity ((long)EventSources.Add, userCard.CardId, token);
			}
		}

		protected void RemoveCardFromMyBusidex(Intent intent){

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
			var userCard = GetUserCardFromIntent (intent);

			if (userCard!= null) {

				string file;
				string myBusidexJson;
				if (File.Exists (fullFilePath)) {
					using (var myBusidexFile = File.OpenText (fullFilePath)) {
						myBusidexJson = myBusidexFile.ReadToEnd ();
					}
					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
					myBusidexResponse.MyBusidex.Busidex.RemoveAll (uc => uc.CardId == userCard.CardId);
					file = Newtonsoft.Json.JsonConvert.SerializeObject (myBusidexResponse);
					Utils.SaveResponse (file, Busidex.Mobile.Resources.MY_BUSIDEX_FILE);
				}
				var token = applicationResource.GetAuthCookie ();

				TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_REMOVED, 0);

				var controller = new MyBusidexController ();
				controller.RemoveFromMyBusidex (userCard.Card.CardId, token);
			}
		}
		#endregion

		#region Google Analytics
		protected static void TrackAnalyticsEvent(string category, string label, string action, int value){

			var build = new HitBuilders.EventBuilder ()
				.SetCategory (category)
				.SetLabel (label)	
				.SetAction (action)
				.SetValue (value) 
				.Build ();
			var build2 = new Dictionary<string,string>();
			foreach (var key in build.Keys)
			{
				build2.Add (key, build [key]);
			}
			GATracker.Send (build2);
		}

		protected static void TrackException(Exception ex){
			try{
				var build = new HitBuilders.ExceptionBuilder ()
					.SetDescription (ex.Message)
					.SetFatal (false) // This is useful for uncaught exceptions
					.Build();
				var build2 = new Dictionary<string,string>();
				foreach (var key in build.Keys)
				{
					build2.Add (key, build [key]);
				}
				GATracker.Send(build2);
			}catch{

			}
		}
		#endregion

		#region Keyboard
		static UserCard GetUserCardFromIntent(Intent intent){

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		protected void DismissKeyboard(IBinder token, Activity context){
			var imm = (InputMethodManager)context.GetSystemService(Activity.InputMethodService); 
			imm.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
		}
		#endregion


		#region Progress Bar
		Handler progressBarHandler = new Handler();

		protected void ShowLoadingSpinner(string message = null, ProgressDialogStyle style = ProgressDialogStyle.Spinner, int max = 100){

			Context context = Activity;
			var loadingText = message ?? context.GetString (Resource.String.Global_OneMoment);

			if(progressDialog != null){
				progressDialog.Dismiss ();
				progressDialog.Dispose ();
				progressDialog = null;
			}

			if (progressDialog == null) {
				progressDialog = new ProgressDialog (Activity);
			}
			if (!Activity.IsFinishing) {
				progressDialog.Max = max;
				progressDialog.SetProgressStyle (style);
				progressDialog.SetMessage (loadingText);
				progressDialog.Progress = 0;
				progressDialog.SetCanceledOnTouchOutside (false);

				try{
					progressDialog.Show ();
				}
				catch(Exception ex){
					TrackException (ex);
				}
			}
		
		}
		protected void UpdateLoadingSpinner(decimal current, decimal total){
			if (progressDialog != null) {
				var progress = total == 0 ? 0 : (Math.Round (current / total, 2)) * 100;
				progressBarHandler.Post (() => {
					progressDialog.Progress = (int)progress;
				});
			}
		}

		protected void HideLoadingSpinner(){
			if (progressDialog != null) {
				progressDialog.Dismiss();
			}
		}
		#endregion
	}
}

