using System;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Threading;

namespace Busidex.Mobile
{
	public delegate void OnMyBusidexLoadedEventHandler(List<UserCard> cards);
	public delegate void OnMyOrganizationsLoadedEventHandler(List<Organization> organizations);
	public delegate void OnEventListLoadedEventHandler(List<EventTag> tags);
	public delegate void OnEventCardsLoadedEventHandler(EventTag tag, List<UserCard> cards);
	public delegate void OnBusidexUserLoadedEventHandler(BusidexUser user);
	public delegate void OnNotificationsLoadedEventHandler(List<SharedCard> notifications);

	public static class UISubscriptionService
	{

		public static event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
		public static event OnMyOrganizationsLoadedEventHandler OnMyOrganizationsLoaded;
		public static event OnEventListLoadedEventHandler OnEventListLoaded;
		public static event OnEventCardsLoadedEventHandler OnEventCardsLoaded;
		public static event OnBusidexUserLoadedEventHandler OnBusidexUserLoaded;
		public static event OnNotificationsLoadedEventHandler OnNotificationsLoaded;

		public static string AuthToken { get; set; }

		static UISubscriptionService(){

			EventCards = EventCards ?? new Dictionary<string, List<UserCard>> ();
			OrganizationMembers = OrganizationMembers ?? new Dictionary<long, List<Card>> ();
			OrganizationReferrals = OrganizationReferrals ?? new Dictionary<long, List<UserCard>> ();

			myBusidexController = new MyBusidexController ();
			organizationController = new OrganizationController ();
			searchController = new SearchController ();
			UserCards = new List<UserCard> ();
			EventList = new List<EventTag> ();
			OrganizationList = new List<Organization> ();
			Notifications = new List<SharedCard> ();

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? loadUser();
		}

		static readonly MyBusidexController myBusidexController;
		static readonly OrganizationController organizationController;
		static readonly SearchController searchController;

		public static List<UserCard> UserCards { get; set; }
		public static List<EventTag> EventList { get; set; }
		public static Dictionary<string, List<UserCard>> EventCards { get; set; }
		public static List<Organization> OrganizationList { get; set; }
		public static Dictionary<long, List<Card>> OrganizationMembers { get; set; }
		public static Dictionary<long, List<UserCard>> OrganizationReferrals { get; set; }
		public static BusidexUser CurrentUser { get; set; }
		public static List<SharedCard> Notifications { get; set; }

		static T loadData<T>(string path) where T : new(){
			return loadDataFromFile<T>(path);
		}

		static T loadDataFromFile<T>(string path) where T: new(){

			try{
				var jsonData = loadFromFile (path);
				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T> (jsonData);
				if(result == null){
					result = new T();
				}
				return result;
			}catch(Exception e){
				return new T();
			}
		}

		public static async void Init(){

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? loadUser();
			CurrentUser = CurrentUser ?? new BusidexUser ();

			UserCards = loadData<List<UserCard>>(Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));
			if(OnMyBusidexLoaded != null){
				OnMyBusidexLoaded(UserCards);
			}

			Notifications = loadData<List<SharedCard>>(Path.Combine (Resources.DocumentsPath, Resources.SHARED_CARDS_FILE));
			if(OnNotificationsLoaded != null){
				OnNotificationsLoaded(Notifications);
			}

			EventList = loadData<List<EventTag>>(Path.Combine (Resources.DocumentsPath, Resources.EVENT_LIST_FILE));
			if(OnEventListLoaded != null){
				OnEventListLoaded(EventList);
			}

			OrganizationList = loadData<List<Organization>>(Path.Combine (Resources.DocumentsPath, Resources.MY_ORGANIZATIONS_FILE));
			if(OnMyOrganizationsLoaded != null){
				OnMyOrganizationsLoaded(OrganizationList);
			}


		}

		public static void Clear(){

			UserCards.Clear ();
			if(OnMyBusidexLoaded != null){
				OnMyBusidexLoaded(UserCards);
			}

			OrganizationList.Clear ();OrganizationList.Clear ();
			if(OnMyOrganizationsLoaded != null){
				OnMyOrganizationsLoaded(OrganizationList);
			}

			EventList.Clear ();
			if(OnEventListLoaded != null){
				OnEventListLoaded(EventList);
			}
			CurrentUser = null;
		}

		public static void LoadUser(){
			loadUser();
		}

		public static void LoadUserCards(){
			loadUserCards ().ContinueWith(r=>{
				if(OnMyBusidexLoaded != null){
					OnMyBusidexLoaded(UserCards);
				}
			});	 	
		}

		public static void LoadOrganizations(){
			loadOrganizations ().ContinueWith(r=>{
				if(OnMyOrganizationsLoaded != null){
					OnMyOrganizationsLoaded(OrganizationList);
				}
			});	 	
		}

		public static async void LoadNotifications(){

			Notifications = loadData<List<SharedCard>>(Path.Combine (Resources.DocumentsPath, Resources.SHARED_CARDS_FILE));

			await loadNotifications ().ContinueWith(r=>{
				if(OnNotificationsLoaded != null){
					OnNotificationsLoaded(Notifications);
				}
			});	 	
		}

		public static async void Sync(){

			loadUser ();

			await loadUserCards ().ContinueWith(r=>{
				if(OnMyBusidexLoaded != null){
					OnMyBusidexLoaded(UserCards);
				}
			});	
			await loadOrganizations ().ContinueWith(r=>{
				if(OnMyOrganizationsLoaded != null){
					OnMyOrganizationsLoaded(OrganizationList);
				}
			});
			await loadEventList ().ContinueWith(r=>{
				if(OnEventListLoaded != null){
					OnEventListLoaded(EventList);
				}
			});
			await loadNotifications ().ContinueWith(r=>{
				if(OnNotificationsLoaded != null){
					OnNotificationsLoaded(Notifications);
				}
			});
		}

		static string loadFromFile(string fullFilePath){

			string fileJson = string.Empty;
			if(File.Exists(fullFilePath)){
				using (var file = File.OpenText (fullFilePath)) {
					fileJson = file.ReadToEnd ();
					file.Close ();
				}
			}
			return fileJson;
		}

		public static void loadEventCards(EventTag tag){
			searchController.SearchBySystemTag(tag.Text, AuthToken).ContinueWith(t => {
				Utils.SaveResponse(t.Result, string.Format("{0}.json", tag));

				var eventSearchResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (t.Result);

				var cards = new List<UserCard> ();

				foreach (var card in eventSearchResponse.SearchModel.Results.Where(c => c.OwnerId.HasValue).ToList()) {
					if (card != null) {

						var userCard = new UserCard (card);

						userCard.ExistsInMyBusidex = card.ExistsInMyBusidex;
						userCard.Card = card;
						userCard.CardId = card.CardId;

						cards.Add (userCard);

						var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
						var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
						var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
						var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
						if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
							Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName);
						}
						if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
							Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
						}
					}
				}
				EventCards.Add(tag.Text, cards);

				var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(EventList);

				Utils.SaveResponse (savedResult, string.Format(Resources.EVENT_CARDS_FILE, tag));

				if(OnEventCardsLoaded != null){
					OnEventCardsLoaded(tag, EventCards[tag.Text]);
				}
			});
		}

		static async Task<bool> loadEventList(){

			EventList.Clear ();

			await searchController.GetEventTags (AuthToken).ContinueWith(r => {
				if (!string.IsNullOrEmpty (r.Result)) {

					var eventListResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (r.Result);
					EventList = eventListResponse.Model;

					foreach(var tag in EventList){
						loadEventCards(tag);
					}

					var savedEvents = Newtonsoft.Json.JsonConvert.SerializeObject(EventList);

					Utils.SaveResponse (savedEvents, Resources.EVENT_LIST_FILE);
				}
			});
			return true;
		}

		static async Task<bool> loadOrganizations(){

			OrganizationList.Clear ();

			await organizationController.GetMyOrganizations (AuthToken).ContinueWith (r => {

				OrganizationResponse myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (r.Result);

				foreach (Organization org in myOrganizationsResponse.Model) {

					OrganizationList.Add(org);

					var fileName = org.LogoFileName + "." + org.LogoType;
					var fImagePath = Resources.CARD_PATH + fileName;
					if (!File.Exists (Resources.DocumentsPath + "/" + fileName)) {
						Utils.DownloadImage (fImagePath, Resources.DocumentsPath, fileName);
					} 
					// load organization members
					organizationController.GetOrganizationMembers(AuthToken, org.OrganizationId).ContinueWith(async cards =>{

						OrgMemberResponse orgMemberResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (cards.Result);
						Utils.SaveResponse(cards.Result, Resources.ORGANIZATION_MEMBERS_FILE + org.OrganizationId);

						var idx = 0;
						OrganizationMembers = new Dictionary<long, List<Card>>();
						OrganizationMembers.Add(org.OrganizationId, orgMemberResponse.Model);

						foreach(var card in orgMemberResponse.Model){

							var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
							var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
							var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
							var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
							if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
								await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName);
							}
							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
								await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
							}
							idx++;
						}
					});

					organizationController.GetOrganizationReferrals(AuthToken, org.OrganizationId).ContinueWith(async cards =>{

						var orgReferralResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (cards.Result);
						Utils.SaveResponse(cards.Result, Resources.ORGANIZATION_REFERRALS_FILE + org.OrganizationId);

						var idx = 0;

						OrganizationReferrals = new Dictionary<long, List<UserCard>>();
						OrganizationReferrals.Add(org.OrganizationId, orgReferralResponse.Model);

						foreach(var card in orgReferralResponse.Model){

							var fImageUrl = Resources.THUMBNAIL_PATH + card.Card.FrontFileName;
							var bImageUrl = Resources.THUMBNAIL_PATH + card.Card.BackFileName;
							var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName;
							var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.BackFileName;
							if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
								await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName);
							}
							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.Card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
								await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
							}
							idx++;
						}
					});
				}

				var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(OrganizationList);

				Utils.SaveResponse(savedResult, Resources.MY_ORGANIZATIONS_FILE);

			});
			return true;
		}

		static async Task<bool> loadUserCards(){

			var cards = new List<UserCard> ();

			await myBusidexController.GetMyBusidex (AuthToken).ContinueWith (r => {

				var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);
				myBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);			

				int idx = 0;
				foreach (var item in myBusidexResponse.MyBusidex.Busidex) {
					if (item.Card != null) {

						var fImageUrl = Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
						var bImageUrl = Resources.THUMBNAIL_PATH + item.Card.BackFileName;
						var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
						var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

						cards.Add (item);

						if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
							Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (t => {
								idx++;
							});
						} else{
							idx++;
						}

						if ((!File.Exists (Resources.DocumentsPath + "/" + bName)) && item.Card.BackFileId.ToString () != Resources.EMPTY_CARD_ID) {
							Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
						}
					}
				}

				UserCards.Clear();
				UserCards.AddRange(cards);

				var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(UserCards);

				Utils.SaveResponse (savedResult, Resources.MY_BUSIDEX_FILE);

			});
			return true;
		}

		static async Task<bool> loadNotifications(){
			var ctrl = new SharedCardController ();
			var sharedCardsResponse = ctrl.GetSharedCards (AuthToken);
			if(sharedCardsResponse.Contains(":404")){
				return false;
			}

			var sharedCards = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsResponse);

			Utils.SaveResponse (sharedCardsResponse, Resources.SHARED_CARDS_FILE);

			foreach (SharedCard card in sharedCards.SharedCards) {
				var fileName = card.Card.FrontFileName;
				var fImagePath = Resources.CARD_PATH + fileName;
				if (!File.Exists (Resources.DocumentsPath + "/" + fileName)) {
					Utils.DownloadImage (fImagePath, Resources.DocumentsPath, fileName);
				}
			}

			Notifications = sharedCards.SharedCards;

			return true;
		}

		static public void AddCardToMyBusidex(UserCard userCard){

			var fullFilePath = Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
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
					Utils.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

					if(UserCards.FirstOrDefault(c=> c.CardId == userCard.CardId) == null){
						UserCards.Add (userCard);
					}
				}

				myBusidexController.AddToMyBusidex (userCard.Card.CardId, AuthToken);

				//TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_MY_BUSIDEX_LABEL, Resources.GA_LABEL_ADD, 0);

				ActivityController.SaveActivity ((long)EventSources.Add, userCard.CardId, AuthToken);
			}
		}

		public static void RemoveCardFromMyBusidex(UserCard userCard){

			var fullFilePath = Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);

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
					Utils.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

					UserCards.RemoveAll (uc => uc.CardId == userCard.CardId);
				}
				//TrackAnalyticsEvent (Resources.GA_CATEGORY_ACTIVITY, Resources.GA_MY_BUSIDEX_LABEL, Resources.GA_LABEL_REMOVED, 0);

				myBusidexController.RemoveFromMyBusidex (userCard.Card.CardId, AuthToken);
			}
		}

		public static void SaveSharedCard(SharedCard sharedCard){

			// Accept/Decline the card
			var ctrl = new SharedCardController ();
			var cardId = new long? (sharedCard.Card.CardId);

			ctrl.UpdateSharedCards (
				sharedCard.Accepted.GetValueOrDefault() ?  cardId: null, 
				sharedCard.Declined.GetValueOrDefault() ? cardId : null, 
				AuthToken);

			// if the card was accepted, update local copy of MyBusidex
			if(sharedCard.Accepted.GetValueOrDefault()){

				var card = GetCardFromSharedCard (AuthToken, sharedCard.CardId);

				if (card != null) {
					var userCard = new UserCard {
						Card = card,
						CardId = card.CardId
					};
					if (UserCards.All (uc => uc.CardId != card.CardId)) {
						UserCards.Add (userCard);
						if(OnMyBusidexLoaded != null){
							OnMyBusidexLoaded (UserCards);
						}
					}
				}
			}

			// update local copy of Shared Cards
			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.SHARED_CARDS_FILE);
			if(File.Exists(fullFilePath)){
				string sharedCardsJson;
				using (var sharedCardsFile = File.OpenText (fullFilePath)) {
					sharedCardsJson = sharedCardsFile.ReadToEnd ();
					sharedCardsFile.Close ();
				}
				var sharedCardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsJson);
				sharedCardResponse.SharedCards.RemoveAll (c => c.Card.CardId == sharedCard.Card.CardId);

				sharedCardsJson = Newtonsoft.Json.JsonConvert.SerializeObject (sharedCardResponse);

				Utils.SaveResponse (sharedCardsJson, Busidex.Mobile.Resources.SHARED_CARDS_FILE);
			}
		}

		static Card GetCardFromSharedCard(string token, long cardId){

			try{
				var cardData = CardController.GetCardById(token, cardId);
				if(!string.IsNullOrEmpty(cardData)){
					var response = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse>(cardData);
					return new Card(response.Model);
				}
			}catch(Exception ex){
				return null;
			}
			return null;
		}

		static BusidexUser loadUser(){

			var accountJSON = AccountController.GetAccount (AuthToken);
			Utils.SaveResponse (accountJSON, Resources.BUSIDEX_USER_FILE);

			CurrentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJSON);

			if(OnBusidexUserLoaded != null){
				OnBusidexUserLoaded (CurrentUser);
			}

			return CurrentUser;
		}
	}
}

