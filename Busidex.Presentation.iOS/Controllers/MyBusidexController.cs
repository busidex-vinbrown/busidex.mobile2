using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using MessageUI;

namespace Busidex.Presentation.iOS
{
	partial class MyBusidexController : BaseController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		List<UserCard> FilterResults;
		private const string NO_CARDS = "You Don't Have Any Cards In Your Collection. Search for some and add them!";
		MFMailComposeViewController _mailController;

		public MyBusidexController (IntPtr handle) : base (handle)
		{
		}

		private void SetFilter(string filter){
			FilterResults = new List<UserCard> ();
			string loweredFilter = filter.ToLowerInvariant ();

			FilterResults.AddRange (
				Application.MyBusidex.Where (c => 
					(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.ToLowerInvariant().Contains (loweredFilter)) ||
					(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter)))
				));

			TableSource src = ConfigureTableSourceEventHandlers(FilterResults);
			src.IsFiltering = true;
			TableView.Source = src;
			TableView.ReloadData ();
			TableView.AllowsSelection = true;
			TableView.SetNeedsDisplay ();
		}

		private void ResetFilter(){

			SearchBar.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers(Application.MyBusidex);
			src.NoCardsMessage = NO_CARDS;
			src.IsFiltering = false;
			TableView.Source = src;
			TableView.ReloadData ();
			TableView.AllowsSelection = true;
			TableView.SetNeedsDisplay ();
		}

		private TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new TableSource (data);
			src.ShowNotes = true;
			src.ShowNoCardMessage = true;
			src.CardSelected += delegate {
				GoToCard();
			};
			src.EditingNotes += delegate {
				EditNotes();
			};	
			src.SendingEmail += delegate(string email) {

				_mailController = new MFMailComposeViewController ();
				_mailController.SetToRecipients (new string[]{email});

				_mailController.Finished += ( object s, MFComposeResultEventArgs args) => {

					this.InvokeOnMainThread( ()=> {
						args.Controller.DismissViewController (true, null);
					});
				};
				this.PresentViewController (_mailController, true, null);
			};

			src.ViewWebsite += delegate(string url) {
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url.Replace("http://", "")));
			};

			src.CallingPhoneNumber += delegate {
				ShowPhoneNumbers();
			};
			return src;
		}

		private void ConfigureSearchBar(){

			SearchBar.Placeholder = "Filter";
			SearchBar.BarStyle = UIBarStyle.Default;
			SearchBar.ShowsCancelButton = true;

			SearchBar.SearchButtonClicked += delegate {
				SetFilter(SearchBar.Text);
				SearchBar.ResignFirstResponder();
			};
			SearchBar.CancelButtonClicked += delegate {
				ResetFilter();
				SearchBar.ResignFirstResponder();
			};
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
		}

		private void GoToCard(){
			var cardController = this.Storyboard.InstantiateViewController ("CardViewController") as CardViewController;
			cardController.UserCard = ((TableSource)this.TableView.Source).SelectedCard;

			if (cardController != null) {
				this.NavigationController.PushViewController (cardController, true);
			}
		}

		private void EditNotes(){

			var notesController = this.Storyboard.InstantiateViewController ("NotesController") as NotesController;
			notesController.UserCard = ((TableSource)this.TableView.Source).SelectedCard;

			if (notesController != null) {
				this.NavigationController.PushViewController (notesController, true);
			}
		}

		private void ShowPhoneNumbers(){
			var phoneViewController = this.Storyboard.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)this.TableView.Source).SelectedCard;

			if (phoneViewController != null) {
				this.NavigationController.PushViewController (phoneViewController, true);
			}
		}

		private void ProcessMyBusidex(string data){
			MyBusidexResponse MyBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);

			Application.MyBusidex = new List<UserCard> ();
			MyBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);
			Application.MyBusidex.AddRange (MyBusidexResponse.MyBusidex.Busidex.Where (c => c.Card != null));

			if (this.TableView.Source == null) {
				var src = ConfigureTableSourceEventHandlers(Application.MyBusidex);
				src.NoCardsMessage = NO_CARDS;
				this.TableView.Source = src;
			}
			this.TableView.AllowsSelection = true;
		}

		private void LoadMyBusidexFromFile(string fullFilePath){

			if(File.Exists(fullFilePath)){
				var myBusidexFile = File.OpenText (fullFilePath);
				var myBusidexJson = myBusidexFile.ReadToEnd ();
				ProcessMyBusidex (myBusidexJson);
			}
		}

		private void SaveMyBusidexResponse(string response){
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			File.WriteAllText (fullFilePath, response);
		}

		private void LoadMyBusidexAsync(){
			var cookie = GetAuthCookie ();

			if (cookie != null) {

				var ctrl = new Busidex.Mobile.MyBusidexController ();
				var response = ctrl.GetMyBusidex (cookie.Value);

				if(!string.IsNullOrEmpty(response.Result)){
					ProcessMyBusidex (response.Result);
					SaveMyBusidexResponse (response.Result);
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}
		}


		public void LoadMyBusidex(){
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath)) {
				LoadMyBusidexFromFile (fullFilePath);
			} else {
				LoadMyBusidexAsync ();
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureSearchBar ();


			this.TableView.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			LoadMyBusidex ();
		}
	}
}
