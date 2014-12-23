using System;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using Busidex.Mobile;
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

		const string NO_CARDS = "There are no {0} in this organization";

		public MemberMode OrganizationMemberMode{ get; set;}

		public OrgMembersController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
				SetNavBarOrgImage ();
			}
		}

		void SetNavBarOrgImage(){
			var fileName = Path.Combine (documentsPath, OrganizationLogo);

			var imageFile = fileName;
			if (File.Exists (imageFile)) {
				var data = NSData.FromFile (imageFile);
				if (data != null) {
					imgLogo.Image = new UIImage (data);
				}
			}
		}

		void SetFilter(string filter){
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

		void ResetFilter(){

			txtSearch.Text = string.Empty;
			TableSource src = ConfigureTableSourceEventHandlers(Cards);
			src.NoCardsMessage = string.Format(NO_CARDS, (OrganizationMemberMode == MemberMode.Members ? "members" : "referrals"));
			src.IsFiltering = false;
			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		void ConfigureSearchBar(){

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

		void GoToCard(){

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

			var cardController = board.InstantiateViewController ("CardViewController") as CardViewController;

			cardController.UserCard = ((TableSource)tblMembers.Source).SelectedCard;

			if (cardController != null) {
				NavigationController.PushViewController (cardController, true);
			}
		}

		void ShowPhoneNumbers(){

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
			var phoneViewController = board.InstantiateViewController ("PhoneViewController") as PhoneViewController;
			phoneViewController.UserCard = ((TableSource)tblMembers.Source).SelectedCard;

			if (phoneViewController != null) {
				NavigationController.PushViewController (phoneViewController, true);
			}
		}

		void EditNotes(){

			UIStoryboard board = UIStoryboard.FromName ("MainStoryboard_iPhone", null);

			var notesController = board.InstantiateViewController ("NotesController") as NotesController;
			notesController.UserCard = ((TableSource)tblMembers.Source).SelectedCard;

			if (notesController != null) {
				NavigationController.PushViewController (notesController, true);
			}
		}

		TableSource ConfigureTableSourceEventHandlers(List<UserCard> data){
			var src = new OrgMemberTableSource (data);
			src.ShowNotes = OrganizationMemberMode == MemberMode.Referrals;
			src.ShowNoCardMessage = !data.Any ();
			src.NoCardsMessage = "No members have been loaded for this organization";
			src.CardSelected += delegate {
				GoToCard();
			};
			src.EditingNotes += delegate {
				EditNotes();
			};	
			src.SendingEmail += delegate(string email) {
				var _mailController = new MFMailComposeViewController ();
				_mailController.SetToRecipients (new []{email});
				_mailController.Finished += ( s, args) => args.Controller.DismissViewController (true, null);
				PresentViewController (_mailController, true, null);
			};

			src.ViewWebsite += url => UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url.Replace ("http://", "")));

			src.CardAddedToMyBusidex += AddCardToMyBusidexCache;

			src.CallingPhoneNumber += delegate {
				ShowPhoneNumbers();
			};

			src.SharingCard += delegate {
				ShareCard (((TableSource)tblMembers.Source).SelectedCard);
			};

			return src;
		}

		void LoadMembers(List<UserCard> cards){
			var src = ConfigureTableSourceEventHandlers(cards); 
			Cards = new List<UserCard> ();
			Cards.AddRange (cards);

			tblMembers.Source = src;
			tblMembers.ReloadData ();
			tblMembers.AllowsSelection = true;
			tblMembers.SetNeedsDisplay ();
		}

		async Task<int> ProcessMembers(string response){
			OrgMemberResponse Members = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (response);
			var cards = new List<UserCard> ();
			float total = Members.Model.Count;
			float processed = 0;

			if (!Members.Model.Any ()) {
				LoadMembers (cards);
			} else {
				foreach (var item in Members.Model) {
					if (item != null) {

						string frontFileId = item.FrontFileId.ToString ();
						string frontType = item.FrontType;

						var imagePath = Resources.THUMBNAIL_PATH + frontFileId + "." + frontType;
						var fName = frontFileId + "." + frontType;

						var userCard = new UserCard ();

						userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
						userCard.Card = item;
						userCard.CardId = item.CardId;

						cards.Add (userCard);

						if (!File.Exists (Path.Combine (documentsPath, frontFileId + "." + frontType))) {
							await Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {
								if (isProgressFinished(++processed, total)) {
									InvokeOnMainThread (() => LoadMembers (cards));
								} 
							});
						} else {
							if (isProgressFinished(++processed, total)) {
								LoadMembers (cards);
							}
						}
					}
				}
			}
			return 1;
		}

		List<UserCard> getCardsFromModel(string data){

			if(OrganizationMemberMode == MemberMode.Members){
				var response = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (data);
				var cards = new List<UserCard> ();

				foreach(var card in response.Model){
					var userCard = new UserCard ();
					userCard.ExistsInMyBusidex = card.ExistsInMyBusidex;
					userCard.Card = card;
					userCard.CardId = card.CardId;

					cards.Add (userCard);
				}
				return cards;
			}else{
				var response = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (data);
				return response.Model;
			}
		}

		protected override void ProcessCards(string data){

			var cards = getCardsFromModel (data);

			if (tblMembers.Source == null) {
				var src = ConfigureTableSourceEventHandlers(cards);
				src.NoCardsMessage = NO_CARDS;
				tblMembers.Source = src;
			}
			tblMembers.AllowsSelection = true;
		}

		void GetMembers(){

			LoadCardsFromFile (OrganizationMemberMode == MemberMode.Members
				? Resources.ORGANIZATION_MEMBERS_FILE + OrganizationId
				: Resources.ORGANIZATION_REFERRALS_FILE + OrganizationId
			);
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tblMembers.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

			LoadCardsFromFile (OrganizationMemberMode == MemberMode.Members
				? documentsPath + "/"+ Resources.ORGANIZATION_MEMBERS_FILE + OrganizationId
				: documentsPath + "/" + Resources.ORGANIZATION_REFERRALS_FILE + OrganizationId
			);

			ConfigureSearchBar ();

		}
	}
}
