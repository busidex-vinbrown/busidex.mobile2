using System;
using UIKit;
using System.IO;
using System.Linq;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.Threading.Tasks;
using GoogleAnalytics.iOS;
using CoreAnimation;
using CoreGraphics;

namespace Busidex.Presentation.iOS
{
	public partial class BaseController : UIViewController
	{

		protected LoadingOverlay Overlay;
		protected string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);



		public BaseController (IntPtr handle) : base (handle)
		{
			
		}

		public BaseController ()
		{

		}


		protected void ShowOverlay ()
		{
			Overlay = new CardLoadingOverlay (View.Bounds);
			Overlay.MessageText = "Loading Your Card";
			View.AddSubview (Overlay);
		}

		protected CALayer GetBorder (CGRect frame, CGColor color, float offset = 0f, float borderWidth = 1f)
		{
			var layer = new CALayer ();
			layer.Bounds = new CGRect (frame.X, frame.Y, frame.Width + offset, frame.Height + offset);
			layer.Frame = new CGRect (frame.X, frame.Y, frame.Width + offset, frame.Height + offset);
			layer.Position = new CGPoint ((frame.Width / 2f) + offset, (frame.Height / 2f) + offset);
			layer.ContentsGravity = CALayer.GravityResize;
			layer.BorderWidth = borderWidth;
			layer.BorderColor = color;
			layer.Name = "Border";

			return layer;
		}

		void handleQuickShare(QuickShareLink link){
			if (NavigationController != null) {
				((BaseNavigationController)NavigationController).GoToQuickShare (link);
				if (Application.Overlay != null) {
					Application.Overlay.Hide ();
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UISubscriptionService.OnQuickShareLoaded -= handleQuickShare;
			UISubscriptionService.OnQuickShareLoaded += handleQuickShare;
		}

		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetToolbarHidden (true, false);
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Send (GAIDictionaryBuilder.CreateScreenView ().Build ());

			base.ViewDidAppear (animated);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			var shouldAllowOtherOrientation = ShouldAllowLandscape (); // same here
			return shouldAllowOtherOrientation ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.Portrait; 

		}

		protected bool ShouldAllowLandscape ()
		{
			return false; // implement this to return true when u want it
		}

		protected virtual void StartSearch ()
		{
			InvokeOnMainThread (() => {
				Overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				//Overlay.RemoveFromSuperview ();
				View.Add (Overlay);
			});

		}

		protected void GoToCardEditMenu ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is CardMenuController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.cardMenuController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.cardMenuController, true);
			}	
		}

		protected void GoToTerms ()
		{
			if (NavigationController.ChildViewControllers.Count (c => c is TermsController) == 0) {
				NavigationController.PushViewController (BaseNavigationController.termsController, true);
			} else {
				NavigationController.PopToViewController (BaseNavigationController.termsController, true);
			}
		}

		protected void GoToSettings ()
		{
			if (NavigationController.ViewControllers.Any (c => c as SettingsController != null)) {
				NavigationController.PopToViewController (BaseNavigationController.settingsController, true);
			} else {
				NavigationController.PushViewController (BaseNavigationController.settingsController, true);
			}
		}

		protected void GoToCreateProfile ()
		{
			if (NavigationController.ViewControllers.Any (c => c as CreateProfileController != null)) {
				NavigationController.PopToViewController (BaseNavigationController.createProfileController, true);
			} else {
				NavigationController.PushViewController (BaseNavigationController.createProfileController, true);
			}
		}

		protected void GoToMain ()
		{
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (true, true);
			
				if (NavigationController.ViewControllers.Any (c => c as HomeController != null)) {
					NavigationController.PopToViewController (BaseNavigationController.homeController, true);
				} else {
					NavigationController.PushViewController (BaseNavigationController.homeController, true);
				}
			}
		}

		protected void ShareCard (UserCard seletcedCard)
		{
			try {
				
				BaseNavigationController.sharedCardController.SelectedCard = seletcedCard;

				if (NavigationController.ViewControllers.Any (c => c as SharedCardController != null)) {
					NavigationController.PopToViewController (BaseNavigationController.sharedCardController, true);
				} else {
					NavigationController.PushViewController (BaseNavigationController.sharedCardController, true);
				}

				string name = Resources.GA_LABEL_SHARE;
				if (BaseNavigationController.sharedCardController.SelectedCard != null && BaseNavigationController.sharedCardController.SelectedCard.Card != null) {
					name = string.IsNullOrEmpty (BaseNavigationController.sharedCardController.SelectedCard.Card.Name) ? BaseNavigationController.sharedCardController.SelectedCard.Card.CompanyName : BaseNavigationController.sharedCardController.SelectedCard.Card.Name;
				}

				AppDelegate.TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_LABEL_SHARE, name, 0);

			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		protected bool isProgressFinished (float processed, float total)
		{
			return processed.Equals (total);
		}

		protected virtual void ProcessCards (string data)
		{

		}

		protected void LoadCardsFromFile (string fullFilePath)
		{

			if (File.Exists (fullFilePath)) {
				var file = File.OpenText (fullFilePath);
				var fileJson = file.ReadToEnd ();
				file.Close ();
				InvokeInBackground (() => ProcessCards (fileJson));
			}
		}

		protected BusinessCardDimensions GetCardDimensions (string orientation)
		{

			/*
				Business cards have an aspect ratio of 1.75 (Canada and US).
			*/
			const float ASPECT_RATIO = 1.75f;
			const float hBase = 165f;
			const float vBase = 115f;

			float height;
			float width;
			float leftMargin;

			if (orientation == "H") {
				height = hBase;
				width = hBase * ASPECT_RATIO;
			} else {
				height = vBase * ASPECT_RATIO;
				width = vBase;
			}
			leftMargin = ((float)UIScreen.MainScreen.Bounds.Width - width) / 2f;

			return new BusinessCardDimensions (height, width, leftMargin);
		}

		protected struct BusinessCardDimensions
		{

			public BusinessCardDimensions (float h, float w, float m)
			{
				Height = h;
				Width = w;
				MarginLeft = m;
			}

			public float Height;
			public float Width;
			public float MarginLeft;
		}
	}
}

