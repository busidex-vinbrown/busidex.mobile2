using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Drawing;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.iOS
{
	partial class NotesController : BaseController
	{
		private readonly string documentsPath;
		public UserCard UserCard{ get; set; }
		private string FrontFileName{ get; set; }
		private string BackFileName{ get; set; }

		private UIView activeview;             // Controller that activated the keyboard
		private nfloat scroll_amount = 0.0f;    // amount to scroll 
		private nfloat bottom = 0.0f;           // bottom point
		private float offset = 10.0f;          // extra offset
		private bool moveViewUp = false;           // which direction are we moving



		public NotesController (IntPtr handle) : base (handle)
		{
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		private void LoadCard(){

			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}

			if (UserCard != null && UserCard.Card != null) {

				var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
				UserCard userCard = null;
				if (File.Exists (fullFilePath)) {
					using (var myBusidexFile = File.OpenText (fullFilePath)) {
						var myBusidexJson = myBusidexFile.ReadToEnd ();
						MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
						foreach(var uc in myBusidexResponse.MyBusidex.Busidex){
							if(uc.Card.CardId == UserCard.Card.CardId){
								userCard = uc;
								break;
							}
						}
					}
				}

				if (userCard != null) {
					FrontFileName = System.IO.Path.Combine (documentsPath, userCard.Card.FrontFileId + "." + userCard.Card.FrontType);
					if (File.Exists (FrontFileName)) {
						imgCard.Image = UIImage.FromFile (FrontFileName);
					}
					txtNotes.Text = userCard.Notes;
				}
			}
			imgSaved.Hidden = true;
		}

		public override void DidRotate(UIInterfaceOrientation orientation){
			base.DidRotate (orientation);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public void KBToolbarButtonDoneHandler(object sender, EventArgs e)
		{
			txtNotes.ResignFirstResponder();
		}

		private void SaveNotes(){

			var cookie = GetAuthCookie ();
			string token = string.Empty;

			if (cookie != null) {
				token = cookie.Value;
			}

			var controller = new Busidex.Mobile.NotesController ();
			var response = controller.SaveNotes (UserCard.UserCardId, txtNotes.Text.Trim (), token);
			var result = response.Result;
			if(!string.IsNullOrEmpty(result)){

				SaveNotesResponse obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveNotesResponse> (result);
				if(obj.Success){

					UpdateLocalCardNotes (UserCard.UserCardId, txtNotes.Text);

					// need to sync the notes with the local user card
					imgSaved.Hidden = false;
				}
			}
		}

		private void UpdateLocalCardNotes(long userCardId, string notes){

			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			// we only need to update the file if they've gotten their busidex. If they haven't, the new card will
			// come along with all the others
			var file = string.Empty;
			if (File.Exists (fullFilePath)) {
				using (var myBusidexFile = File.OpenText (fullFilePath)) {
					var myBusidexJson = myBusidexFile.ReadToEnd ();
					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
					foreach(var uc in myBusidexResponse.MyBusidex.Busidex){
						if(uc.UserCardId == UserCard.UserCardId){
							uc.Notes = txtNotes.Text.Trim ();
							break;
						}
					}
					file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);
					Application.MyBusidex = myBusidexResponse.MyBusidex.Busidex;

				}

				File.WriteAllText (fullFilePath, file);
			}

		}

		public override void AwakeFromNib ()
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			try{
				LoadCard ();
			}catch(Exception ex){

			}
			UIToolbar keyboardDoneButtonToolbar = new UIToolbar(RectangleF.Empty)
			{
				BarStyle = UIBarStyle.Black,
				Translucent = true,
				UserInteractionEnabled = true,
				TintColor = null
			};
			keyboardDoneButtonToolbar.SizeToFit();

			// NOTE Need 2 spacer elements here and not sure why...
			UIBarButtonItem btnKeyboardDone = new UIBarButtonItem(UIBarButtonSystemItem.Done, this.KBToolbarButtonDoneHandler);
			keyboardDoneButtonToolbar.SetItems(new []
				{
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null),
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null),
					btnKeyboardDone
				}, true);

			txtNotes.InputAccessoryView = keyboardDoneButtonToolbar;

			// Keyboard popup
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification,KeyBoardUpNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification,KeyBoardDownNotification);

			btnSaveNotes.TouchUpInside += delegate {
				SaveNotes();
			};

			txtNotes.Changed += delegate {
				imgSaved.Hidden = true;
			};

		}

		private void KeyBoardDownNotification(NSNotification notification)
		{
			if(moveViewUp){ScrollTheView(false);}
		}

		private void ScrollTheView(bool move)
		{

			// scroll the view up or down
			UIView.BeginAnimations (string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration (0.3);

			var frame = View.Frame;

			if (move) {
				frame.Y -= scroll_amount;
			} else {
				frame.Y += scroll_amount;
				scroll_amount = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}

		private void KeyBoardUpNotification(NSNotification notification)
		//private void KeyBoardUpNotification()
		{
			// get the keyboard size
			//RectangleF r = UIKeyboard.BoundsFromNotification (notification);

			// Find what opened the keyboard
			foreach (UIView view in this.View.Subviews) {
				if (view.IsFirstResponder) {
					activeview = view;
					break;
				}
			}

			// Bottom of the controller = initial position + height + offset  
			if (activeview != null) {
				bottom = (activeview.Frame.Y + activeview.Frame.Height + offset);
			}else{
				bottom = 320;
			}
			// Calculate how far we need to scroll
			//scroll_amount = (r.Height - (View.Frame.Size.Height - bottom)) ;
			scroll_amount = (260 - (View.Frame.Size.Height - bottom)) ;
			// Perform the scrolling
			if (scroll_amount > 0) {
				moveViewUp = true;
				ScrollTheView (moveViewUp);
			} else {
				moveViewUp = false;
			}

		}
	}
}
