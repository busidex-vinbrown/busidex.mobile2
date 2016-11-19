// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using Xamarin.InAppPurchase;

namespace InAppPurchaseTest
{
	public partial class ProductContent : UIViewController
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the product being displayed on this content sheet
		/// </summary>
		/// <value>The product.</value>
		public InAppProduct product { get; set; }

		/// <summary>
		/// Gets or sets the In App Purchase Manager
		/// </summary>
		/// <value>The manager.</value>
		public InAppPurchaseManager manager { get; set; }
		#endregion

		#region Constructors
		public ProductContent (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Loads the given file into the webview
		/// </summary>
		/// <param name="filename">Filename.</param>
		public void LoadFile(string filename){

			//Displays the given local file
			WebView.LoadRequest(new NSUrlRequest(NSUrl.FromFilename(filename)));
		}
		#endregion

		#region Override Methods
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Display information
			TitleBar.Title = product.Title;
			ContentVersion.Text = string.Format("Version {0}",product.Receipt.DownloadableContentVersion);

			// Wireup back button
			BackButton.Clicked += (object sender, EventArgs e) => {
				// Close this window and return to the master view
				DismissViewController(true,null);
			};

			// Take action based on the current state
			if (manager.SimulateiTunesAppStore) {
				// Running in the simulation mode, simulate content
				HeaderImage.Image = UIImage.FromFile ("Images/DefaultHeader.png");
				LoadFile ("Content/DefaultText.html");
			} else {
				string filename;

				// Display header
				filename = System.IO.Path.Combine (product.ContentPath, "Header.png");
				if (!System.IO.File.Exists (filename)) {
					// Switch to default
					filename = "Images/DefaultHeader.png";
				}

				// Display live content
				HeaderImage.Image = UIImage.FromFile (filename);
				LoadFile (System.IO.Path.Combine(product.ContentPath, "Document.html"));
			}
		}
		#endregion

	}
}
