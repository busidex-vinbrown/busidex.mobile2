// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using Xamarin.InAppPurchase;
using System.Drawing;
using CoreGraphics;

namespace InAppPurchaseTest
{
	public partial class FeaturesController : UIViewController
	{
		#region Private Variables
		private InAppPurchaseManager _purchaseManager;
		private UIStoryboard _Storyboard;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets the purchase manager.
		/// </summary>
		/// <value>The purchase manager.</value>
		public InAppPurchaseManager PurchaseManager {
			get { return _purchaseManager; }
		}

		/// <summary>
		/// Gets the storyboard.
		/// </summary>
		/// <value>The storyboard.</value>
		public UIStoryboard Storyboard {
			get { return _Storyboard; }
		}
		#endregion 

		#region Constructors
		public FeaturesController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Updates the coin count.
		/// </summary>
		private void UpdateCoinCount() {

			// Display the total of all coins
			if (TotalCoins !=null)
				TotalCoins.Text = String.Format ("{0} Coins", PurchaseManager.ProductQuantityAvailable ("gold.coins"));
		}

		/// <summary>
		/// Moves the master containing view out of the way of the keyboard.
		/// </summary>
		/// <param name="y">The y coordinate.</param>
		private void MoveView(float y) {

			// Move view out of the way of the keyboard
			SpecialView.Frame = new CGRect (SpecialView.Frame.Left, y, SpecialView.Frame.Width, SpecialView.Frame.Height);
		}
		#endregion

		#region Public Override Methods
		/// <summary>
		/// Views the will appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Update the count before displaying
			UpdateCoinCount ();
		
		}

		/// <summary>
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Wireup amount to consume
			AmountToConsume.ShouldBeginEditing= delegate(UITextField field){
				//Placeholder
				UIView.BeginAnimations("keyboard");
				UIView.SetAnimationDuration(0.3f);
				MoveView(-170);
				UIView.CommitAnimations();
				return true;
			};
			AmountToConsume.ShouldReturn = delegate (UITextField field){
				field.ResignFirstResponder ();
				//this.View.EndEditing(true);
				UIView.BeginAnimations("keyboard");
				UIView.SetAnimationDuration(0.3f);
				MoveView(0);
				UIView.CommitAnimations();
				//field.Text holds the current value


				return true;
			};
			AmountToConsume.ShouldEndEditing= delegate (UITextField field){
				field.ResignFirstResponder();
				//this.View.EndEditing(true);
				UIView.BeginAnimations("keyboard");
				UIView.SetAnimationDuration(0.3f);
				MoveView(0);
				UIView.CommitAnimations();
				//field.Text holds the current value

				return true;
			};

			// Stop editing if the user touches the view
			ViewTouched.AddTarget (async delegate() {
				// Close the keyboard if displayed
				AmountToConsume.EndEditing(true);
			});


			// Wireup the consume button
			ConsumeButton.TouchUpInside += (object sender, EventArgs e) => {
				// Close the keyboard if displayed
				AmountToConsume.EndEditing(true);

				// Trap all errors
				try {
					// Verify
					int amount = int.Parse(AmountToConsume.Text);
					int available =PurchaseManager.ProductQuantityAvailable ("gold.coins");

					// Valid amount?
					if (amount <1 || amount > available) {
						//Display Alert Dialog Box
						using(var alert = new UIAlertView("Xamarin.InAppPurchase", "Not a valid amount to consume. The amount to consume must be at least one and less than the available amount.", null, "OK", null))
						{
							alert.Show();	
						}

						// Abort
						return ;
					}

					// Ask the purchase manager to consume the amount
					PurchaseManager.ConsumeProductQuantity("gold.coins", amount);
				}
				catch {
					//Display Alert Dialog Box
					using(var alert = new UIAlertView("Xamarin.InAppPurchase", "Not a valid amount to consume.", null, "OK", null))
					{
						alert.Show();	
					}

				}
			};
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="InAppPurchaseTest.FeaturesController"/> disables automatic
		/// keyboard dismissal.
		/// </summary>
		/// <value><c>true</c> if disables automatic keyboard dismissal; otherwise, <c>false</c>.</value>
		public override bool DisablesAutomaticKeyboardDismissal {
			get {
				// Force this view controller to allow the keyboard to be dismissed.
				return false;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Attachs to purchase manager.
		/// </summary>
		/// <param name="purchaseManager">Purchase manager.</param>
		public void AttachToPurchaseManager(UIStoryboard Storyboard, InAppPurchaseManager purchaseManager) {

			// Save connection
			_Storyboard = Storyboard;
			_purchaseManager = purchaseManager;

			// Respond to events
			_purchaseManager.InAppProductPurchased += (StoreKit.SKPaymentTransaction transaction, InAppProduct product) => {
				// Update the display
				UpdateCoinCount();
			};

			_purchaseManager.InAppPurchaseProductQuantityConsumed += (identifier) => {
				// Update the display
				UpdateCoinCount();
			};

			// Display initial data
			UpdateCoinCount ();
		}
		#endregion
	}
}
