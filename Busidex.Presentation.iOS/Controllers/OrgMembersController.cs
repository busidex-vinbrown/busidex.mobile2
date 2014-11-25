using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using Busidex.Mobile;
using Newtonsoft.Json;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using System.IO;
using System.Linq;
using MessageUI;

namespace Busidex.Presentation.iOS
{
	partial class OrgMembersController : BaseController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		public long OrganizationId{ get; set; }
		public string OrganizationName{ get; set; }
		public string OrganizationLogo{ get; set; }

		public enum MemberMode{
			Members = 1,
			Referrals = 2
		}
		List<UserCard> FilterResults;
		List<UserCard> Cards;

		private const string NO_CARDS = "There are no {0} in this organization";

		public MemberMode OrganizationMemberMode{ get; set;}

		public OrgMembersController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
				SetNavBarOrgImage ();
			}
		}

		private void SetNavBarOrgImage(){
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var fileName = System.IO.Path.Combine (documentsPath, OrganizationLogo);

			var imageFile = fileName;
			if (File.Exists (imageFile)) {
				var data = NSData.FromFile (imageFile);
				if (data != null) {
					this.imgLogo.Image = new UIImage (data);
				}
			}

		}
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		protected override void StartSearch(){

			base.StartSearch ();
		}

		protected override void DoSearch(){

			base.DoSearch ();
		}

		private void SetFilter(string filter){
			FilterResults = new List<UserCard> ();
			string loweredFilter = filter.ToLowerInvariant ();

			FilterResults.AddRange (
				Cards.Where (c => 
					(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.ToLowerInvariant().Contains (loweredFilter)) ||
					(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.ToLowerInvariant().Contains (loweredFilter)) ||
					(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (loweredFilter)))
				));

			TableSource src = ConfigureTableSourceEventHandlers(FilterResults);
			src.IsFiltering = true;
			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		private void ResetFilter(){

			txtSearch.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers(Cards);
			src.NoCardsMessage = string.Format(NO_CARDS, (OrganizationMemberMode == MemberMode.Members ? "members" : "referrals"));
			src.IsFiltering = false;
			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		private void ConfigureSearchBar(){

			txtSearch.Placeholder = "Filter";
			txtSearch.BarStyle = UIBarStyle.Default;
			txtSearch.ShowsCancelButton = true;

			txtSearch.SearchButtonClicked += delegate {
				SetFilter(txtSearch.Text);
				txtSearch.ResignFirstResponder();
			};
			txtSearch.CancelButtonClicked += delegate {
				ResetFilter();
				txtSearch.ResignFirstResponder();
			};
		}

		private void GoToCard(){

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

			var cardController = board.InstantiateViewController ("CardViewController") as CardViewController;

			cardController.UserCard = ((TableSource)this.tblMembers.Source).SelectedCard;

			if (cardController != null) {
				this.NavigationController.PushViewController (cardController, true);
			}
		}

		private void ShowPhoneNumbers(){

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var phoneViewController = board.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)this.tblMembers.Source).SelectedCard;

			if (phoneViewController != null) {
				this.NavigationController.PushViewController (phoneViewController, true);
			}
		}

		private void EditNotes(){

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

			var notesController = board.InstantiateViewController ("NotesController") as NotesController;
			notesController.UserCard = ((TableSource)this.tblMembers.Source).SelectedCard;

			if (notesController != null) {
				this.NavigationController.PushViewController (notesController, true);
			}
		}

		private TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new OrgMemberTableSource (data);
			src.ShowNotes = OrganizationMemberMode == MemberMode.Referrals;
			src.ShowNoCardMessage = data.Count() == 0;
			src.NoCardsMessage = "No members have been loaded for this organization";
			src.CardSelected += delegate {
				GoToCard();
			};
			src.EditingNotes += delegate {
				EditNotes();
			};	
			src.SendingEmail += delegate(string email) {
				MFMailComposeViewController _mailController = new MFMailComposeViewController ();
				_mailController.SetToRecipients (new string[]{email});
				_mailController.Finished += ( object s, MFComposeResultEventArgs args) => {
					args.Controller.DismissViewController (true, null);
				};
				this.PresentViewController (_mailController, true, null);
			};

			src.ViewWebsite += delegate(string url) {
				UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url.Replace("http://", "")));
			};

			src.CardAddedToMyBusidex += new CardAddedToMyBusidexHandler (AddCardToMyBusidex);

			src.CallingPhoneNumber += delegate {
				ShowPhoneNumbers();
			};
			return src;
		}

		private void LoadMembers(List<UserCard> cards){
			var src = ConfigureTableSourceEventHandlers(cards); 
			Cards = new List<UserCard> ();
			Cards.AddRange (cards);

			this.tblMembers.Source = src;
			this.tblMembers.ReloadData ();
			this.tblMembers.AllowsSelection = true;
			this.tblMembers.SetNeedsDisplay ();
		}

		private async Task<int> ProcessMembers(string response){
			OrgMemberResponse Members = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (response);
			List<UserCard> cards = new List<UserCard> ();
			float total = Members.Model.Count;
			float processed = 0;

			if (Members.Model.Count () == 0) {
				LoadMembers (new List<UserCard> ());
			} else {
				foreach (var item in Members.Model) {
					if (item != null) {

						string frontFileId = item.FrontFileId.ToString();
						string frontType = item.FrontType;

						var imagePath = Busidex.Mobile.Utils.CARD_PATH + frontFileId + "." + frontType;
						var fName = frontFileId + "." + frontType;

						var userCard =  new UserCard ();

						userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
						userCard.Card = item;
						userCard.CardId = item.CardId;

						cards.Add (userCard);

						if (!File.Exists (System.IO.Path.Combine (documentsPath, frontFileId + "." + frontType))) {
							await Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {

								if (++processed == total) {

									this.InvokeOnMainThread (() => {
										LoadMembers (cards);
									});
								} 
							});
						} else {

							if (++processed == total) {
								LoadMembers (cards);
							}
						}
					}
				}
			}
			return 1;
		}

		private int ProcessReferrals(string response){
			OrgReferralResponse Members = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (response);

			List<UserCard> cards = new List<UserCard> ();
			float total = Members.Model.Count;
			float processed = 0;

			if (Members.Model.Count () == 0) {
				LoadMembers (new List<UserCard> ());
			} else {

				foreach (var item in Members.Model) {
					if (item != null) {

						string frontFileId = item.Card.FrontFileId.ToString();
						string frontType = item.Card.FrontType;

						var imagePath = Busidex.Mobile.Utils.CARD_PATH + frontFileId + "." + frontType;
						var fName = frontFileId + "." + frontType;

						item.ExistsInMyBusidex = item.Card.ExistsInMyBusidex;

						cards.Add (item);

						if (!File.Exists (System.IO.Path.Combine (documentsPath, frontFileId + "." + frontType))) {
							Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {

								if (++processed == total) {

									this.InvokeOnMainThread (() => {
										LoadMembers (cards);
									});
								} 
							});
						} else {

							if (++processed == total) {
								LoadMembers (cards);
							}
						}
					}
				}
			}
			return 1;
		}

		private async Task<int> GetMembers(){

			var cookie = GetAuthCookie ();
			string token = string.Empty;

			if (cookie != null) {
				token = cookie.Value;
			}

			var ctrl = new Busidex.Mobile.OrganizationController ();
			string response = OrganizationMemberMode == MemberMode.Members 
				? await ctrl.GetOrganizationMembers(token, OrganizationId)
				: await ctrl.GetOrganizationReferrals(token, OrganizationId);

			if (!string.IsNullOrEmpty (response)) {

				// The items coming back from GetOrganizationMembers is a list of Cards
				// The items coming back from GetOrganizationReferras is a list of UserCards (these have notes)
				if(OrganizationMemberMode == MemberMode.Members){
					ProcessMembers (response);
				}else{
					ProcessReferrals (response);
				}
			}
			return 1;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.tblMembers.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

			GetMembers ();
			ConfigureSearchBar ();

		}
	}
}
