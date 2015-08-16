
using Android.App;
using Android.Content;
using System.IO;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using Android.Views.InputMethods;
using Android.OS;
using System.Threading.Tasks;
using System;
using Android.Gms.Analytics;
using System.Collections.Generic;

namespace Busidex.Presentation.Android
{
	 
	[Activity (Label = "BaseActivity", ConfigurationChanges =  global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]			
	public class BaseActivity : Activity
	{
		protected BaseApplicationResource applicationResource;

		static Tracker _tracker;
		protected static Tracker GATracker { 
			get { 
				return _tracker; 
			} 
		}
		protected static ProgressDialog progressDialog;
		protected bool isLoggedIn;

		Handler progressBarHandler = new Handler();

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Portrait;
			_tracker = _tracker ?? GoogleAnalytics.GetInstance (this).NewTracker (Busidex.Mobile.Resources.GOOGLE_ANALYTICS_KEY_ANDROID);
			applicationResource = new BaseApplicationResource (this);
		}
		public override void OnBackPressed ()
		{
			base.OnBackPressed ();

			if (FragmentManager.BackStackEntryCount == 0) {
				LoadFragment (typeof(MainFragment));
			} else {
				var frag = FragmentManager.GetBackStackEntryAt (0);
				if (frag is MainFragment) {
					// no op
				} else {
					base.OnBackPressed ();

				}
			}
		}
		#region Loading
		protected virtual void ProcessFile(string data){

		}
		/*
		protected bool CheckBusidexFileCache(string fullFilePath){
			if(File.Exists(fullFilePath)){
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				file.Close ();
				return !string.IsNullOrEmpty (fileJson);
			}
			return false;
		}

		protected void LoadFromFile(string fullFilePath){

			if(File.Exists(fullFilePath)){
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				file.Close ();
				ProcessFile (fileJson);
			}
		}
		*/
		protected void RedirectToMainIfLoggedIn(){

			if(applicationResource.GetAuthCookie () == null){
				LoadFragment (typeof(StartUpFragment));
			}else{
				LoadFragment (typeof(MainFragment));
			}

		}

		public void LoadFragment(Type type){

			//			if (type != typeof(BaseFragment)) {
			//				return;
			//			}

			var fragment = (BaseFragment)FragmentManager.FindFragmentByTag (type.Name);
			using (var transaction = FragmentManager.BeginTransaction ()) {

				transaction.SetCustomAnimations (
					Resource.Animator.SlideAnimation, 
					Resource.Animator.SlideOutAnimation, 
					Resource.Animator.SlideAnimation, 
					Resource.Animator.SlideOutAnimation);

				FragmentManager.PopBackStack ();
				if (fragment != null) {
					transaction.Show (fragment);
				}
				else {
					fragment = getFragmentByType (type.Name);
					transaction.Add (Resource.Id.fragment_holder, fragment, fragment.uniqueId).Show(fragment);
						//.Add (fragment, fragment.uniqueId);
						//.Replace(Resource.Id.fragment_holder, fragment, fragment.uniqueId);
				}
				transaction.AddToBackStack (fragment.uniqueId).Commit ();
				//fragment.OnResume ();
			}
		}

		private BaseFragment getFragmentByType(string typeName){

			BaseFragment fragment;

			switch (typeName) {
			case "MainFragment":
				{
					fragment = new MainFragment ();
					break;
				}
			case "StartUpFragment":
				{
					fragment = new StartUpFragment ();
					break;
				}
			case "LoginFragment":
				{
					fragment = new LoginFragment ();
					break;
				}
			case "MyBusidexFragment":
				{
					fragment = new MyBusidexFragment ();
					break;
				}
			case "SearchFragment":
				{
					fragment = new SearchFragment ();
					break;
				}
			case "EventListFragment":
				{
					fragment = new EventListFragment ();
					break;
				}
			case "MyOrganizationsFragment":
				{
					fragment = new MyOrganizationsFragment ();
					break;
				}
			default:{
					fragment = new MainFragment ();
					break;
				}
			}
			return fragment;
		}

		protected BaseFragment getFragment(string tag){
			return (BaseFragment)FragmentManager.FindFragmentByTag (tag);		
		}

		protected void UnloadFragment(Type type){
			var fragment = getFragment (type.Name);
			if(fragment != null){
				using (var transaction = FragmentManager.BeginTransaction ()) {
					transaction.Hide (fragment);
				}
			}
		}


		#endregion


		/*
		#region Card Actions
		protected void Redirect(Intent intent){

			StartActivity(intent);
			OverridePendingTransition(Resource.Animation.SlideAnimation, Resource.Animation.SlideOutAnimation);

		}

		protected void ShowCard(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Details, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_DETAILS, 0);

			Redirect(intent);
		}

		protected void SendEmail(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Email, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_EMAIL, 0);

			StartActivity(intent);
		}

		protected void OpenBrowser(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Website, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_URL, 0);

			var browserIntent = Intent.CreateChooser(intent, "Open with");
			StartActivity (browserIntent);
		}

		protected void OpenMap(Intent intent){

			var userCard = GetUserCardFromIntent (intent);
			var token = applicationResource.GetAuthCookie ();
			ActivityController.SaveActivity ((long)EventSources.Map, userCard.CardId, token);

			TrackAnalyticsEvent (Busidex.Mobile.Resources.GA_CATEGORY_ACTIVITY, Busidex.Mobile.Resources.GA_MY_BUSIDEX_LABEL, Busidex.Mobile.Resources.GA_LABEL_MAP, 0);

			StartActivity (intent);
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

		#region Keyboard
		static UserCard GetUserCardFromIntent(Intent intent){

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		protected async Task<bool> DismissKeyboard(IBinder token){
			var imm = (InputMethodManager)GetSystemService(InputMethodService); 
			return imm.HideSoftInputFromWindow(token, 0);
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

		#region Preferences
		ISharedPreferences GetBusidexSharedPreferences(){
			return Application.Context.GetSharedPreferences("Busidex", FileCreationMode.Private); 
		}

		protected void SaveStringPreference(string key, string value){

			ISharedPreferences prefs = GetBusidexSharedPreferences ();
			ISharedPreferencesEditor editor = prefs.Edit();
			//editor.PutInt(("number_of_times_accessed", accessCount++);
			editor.PutString(key, value);
			editor.Apply();
		}

		protected string GetStringPreference(string key){

			ISharedPreferences prefs = GetBusidexSharedPreferences ();
			return prefs.GetString (key, string.Empty);
		}
		#endregion

		#region Progress Bar
		protected void ShowLoadingSpinner(string message = null, ProgressDialogStyle style = ProgressDialogStyle.Spinner, int max = 100){

			Context context = this;
			var loadingText = message ?? context.GetString (Resource.String.Global_OneMoment);

			if(progressDialog != null){
				progressDialog.Dismiss ();
				progressDialog.Dispose ();
				progressDialog = null;
			}

			if (progressDialog == null) {
				progressDialog = new ProgressDialog (this);
			}
			if (!IsFinishing) {
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
//			new Thread(new ThreadStart(delegate
//				{
//					RunOnUiThread(() => Toast.MakeText(this, messageText, ToastLength.Long).Show());
//				})
//			).Start();
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

		#region Alerts
		protected void ShowAlert(string title, string message){
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle(title);
			builder.SetMessage(message);
			builder.SetCancelable(false);
			builder.SetPositiveButton("OK", delegate { return; });
			builder.Show();
		}
		#endregion 
		*/
	}
}

