﻿using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using GoogleAnalytics.iOS;
using System.Threading.Tasks;
using CoreGraphics;

namespace Busidex.Presentation.iOS
{
	partial class LoginController : BaseController
	{
		LoadingOverlay loadingOverlay;

		public LoginController (IntPtr handle) : base (handle)
		{
		}

		public string DataObject {
			get;
			set;
		}

		static long UserId{ get; set; }

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.Title = "Login";
				NavigationController.SetNavigationBarHidden (false, true);
				NavigationController.SetToolbarHidden (true, false);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Login");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var cookie = GetAuthCookie ();
			long userId;
			if (cookie != null) {
				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
				if (userId > 0) {
					UserId = userId;
				} 
			} 

			btnLogin.TouchUpInside += (o, s) => DoLogin ();
		}

		bool loggingIn;

		void spinImage(){
			UIView.AnimateNotify (.5, 0, UIViewAnimationOptions.Autoreverse | UIViewAnimationOptions.CurveEaseOut, () =>
				{
					imgLogo.Transform = CGAffineTransform.MakeScale(0.01f, 1.1f);
				}, (finished) => {
					if(loggingIn){
						spinImage();
					}else{
						imgLogo.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
					}
				});
		}

		async Task<bool> DoLogin(){
			try {
				lblLoginResult.Text = string.Empty;

				string username = txtUserName.Text;
				string password = txtPassword.Text;

				// dismiss the keyboard
				txtPassword.ResignFirstResponder();
				txtUserName.ResignFirstResponder();

				loggingIn = true;

				spinImage();
				var loginController = new Busidex.Mobile.LoginController();
				await loginController.DoLogin (username, password).ContinueWith(async response => {
					string result = await response;
					if (!string.IsNullOrEmpty (result) && !result.Contains ("404")) {
						var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (result);

						UserId = loginResponse != null ? loginResponse.UserId : 0;

						if (UserId > 0) {

							SetAuthCookie (UserId);

							var user = NSUserDefaults.StandardUserDefaults;

							user.SetString (username, Busidex.Mobile.Resources.USER_SETTING_USERNAME);
							user.SetString (password, Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
							user.SetString (username, Busidex.Mobile.Resources.USER_SETTING_EMAIL);
							user.SetBool (true, Busidex.Mobile.Resources.USER_SETTING_AUTOSYNC);
							user.Synchronize ();

							loggingIn = false;

							InvokeOnMainThread (GoToHome);

							return true;
						}
					}
					InvokeOnMainThread(()=> {
						loggingIn = false;
						lblLoginResult.Text = "Login Failed";
						lblLoginResult.TextColor = UIColor.Red;
					});
					return true;
				});


			} catch (Exception ignore) {
				await ShowAlert ("Login Error", "There was a problem logging in.", new []{ "Ok" });
				loggingIn = false;
			} 
			return true;
		}

		void GoToHome ()
		{

			var dataViewController = Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

			if (dataViewController != null) {
				NavigationController.PushViewController (dataViewController, true);

			}
		}
	}
}
