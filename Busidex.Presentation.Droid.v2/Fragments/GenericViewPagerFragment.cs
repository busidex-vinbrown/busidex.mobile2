using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using System.IO;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using Android.Views.InputMethods;
using Android.Gms.Analytics;

namespace Busidex.Presentation.Droid.v2
{
	public class GenericViewPagerFragment : Android.Support.V4.App.Fragment
	{
		Func<LayoutInflater, ViewGroup, Bundle, View> _view;

		protected GestureDetector _detector;

		public GenericViewPagerFragment (Func<LayoutInflater, ViewGroup, Bundle, View> view)
		{
			_view = view;

		}

		public GenericViewPagerFragment ()
		{
			
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (_view == null) {
				return base.OnCreateView (inflater, container, savedInstanceState);
			} else {
				base.OnCreateView (inflater, container, savedInstanceState);
				return _view (inflater, container, savedInstanceState);
			}
		}

		#region Alerts

		protected void ShowAlert (string title, string message, string buttonText, EventHandler<DialogClickEventArgs> callback)
		{
			var builder = new AlertDialog.Builder (Activity);
			builder.SetTitle (title);
			builder.SetMessage (message);
			builder.SetNegativeButton (Activity.GetString (Resource.String.Global_ButtonText_Cancel), new EventHandler<DialogClickEventArgs> ((o, e) => {
				return;
			}));
			builder.SetCancelable (true);
			builder.SetPositiveButton (buttonText, callback);
			builder.Show ();
		}

		#endregion


		#region Keyboard

		static UserCard GetUserCardFromIntent (Intent intent)
		{

			var data = intent.GetStringExtra ("Card");
			return !string.IsNullOrEmpty (data) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserCard> (data) : null;
		}

		protected void DismissKeyboard (IBinder token, Activity context)
		{
			var imm = (InputMethodManager)context.GetSystemService ("input_method"); 
			imm.HideSoftInputFromWindow (token, HideSoftInputFlags.None);
		}

		#endregion

		/*
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

		static Tracker _tracker;
		protected static Tracker GATracker { 
			get { 
				return _tracker; 
			} 
		}
		#endregion
		*/

	}
}