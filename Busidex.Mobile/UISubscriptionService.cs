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

	public class UISubscriptionService
	{

		public event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
		public event OnMyOrganizationsLoadedEventHandler OnMyOrganizationsLoaded;
		public event OnEventListLoadedEventHandler OnEventListLoaded;
		public event OnEventCardsLoadedEventHandler OnEventCardsLoaded;
		public event OnBusidexUserLoadedEventHandler OnBusidexUserLoaded;

		public string AuthToken { get; set; }

		public UISubscriptionService(){
			
			EventCards = EventCards ?? new Dictionary<string, List<UserCard>> ();
			OrganizationMembers = OrganizationMembers ?? new Dictionary<long, List<Card>> ();
			OrganizationReferrals = OrganizationReferrals ?? new Dictionary<long, List<UserCard>> ();

			myBusidexController = new MyBusidexController ();
			organizationController = new OrganizationController ();
			searchController = new SearchController ();
			UserCards = new List<UserCard> ();
			EventList = new List<EventTag> ();
			OrganizationList = new List<Organization> ();

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? new BusidexUser ();
		}

		readonly MyBusidexController myBusidexController;
		readonly OrganizationController organizationController;
		readonly SearchController searchController;

		public List<UserCard> UserCards { get; set; }
		public List<EventTag> EventList { get; set; }
		public Dictionary<string, List<UserCard>> EventCards { get; set; }
		public List<Organization> OrganizationList { get; set; }
		public Dictionary<long, List<Card>> OrganizationMembers { get; set; }
		public Dictionary<long, List<UserCard>> OrganizationReferrals { get; set; }
		public BusidexUser CurrentUser { get; set; }

		T loadData<T>(string path) where T : new(){
			return loadDataFromFile<T>(path);
		}

		T loadDataFromFile<T>(string path) where T: new(){

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

		public void Init(){

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? new BusidexUser ();

			//ThreadPool.QueueUserWorkItem( o =>  {
				UserCards = loadData<List<UserCard>>(Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));
				if(OnMyBusidexLoaded != null){
					OnMyBusidexLoaded(UserCards);
				}
			//});
			//ThreadPool.QueueUserWorkItem (o => {
				EventList = loadData<List<EventTag>>(Path.Combine (Resources.DocumentsPath, Resources.EVENT_LIST_FILE));
				if(OnEventListLoaded != null){
					OnEventListLoaded(EventList);
				}
			//});
			//ThreadPool.QueueUserWorkItem (o => {
				OrganizationList = loadData<List<Organization>>(Path.Combine (Resources.DocumentsPath, Resources.MY_ORGANIZATIONS_FILE));
				if(OnMyOrganizationsLoaded != null){
					OnMyOrganizationsLoaded(OrganizationList);
				}
			//});
		}

		public void Clear(){

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

		public void LoadUser(){
			loadUser();
		}

		public void LoadUserCards(){
			loadUserCards ().ContinueWith(r=>{
				if(OnMyBusidexLoaded != null){
					OnMyBusidexLoaded(UserCards);
				}
			});	 	
		}

		public async void reset(){

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
		}

		string loadFromFile(string fullFilePath){

			string fileJson = string.Empty;
			if(File.Exists(fullFilePath)){
				using (var file = File.OpenText (fullFilePath)) {
					fileJson = file.ReadToEnd ();
					file.Close ();
				}
			}
			return fileJson;
		}

		public void loadEventCards(EventTag tag){
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

		async Task<bool> loadEventList(){

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

		async Task<bool> loadOrganizations(){

			OrganizationList.Clear ();

			await organizationController.GetMyOrganizations (AuthToken).ContinueWith (r => {

				OrganizationResponse myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (r.Result);

				foreach (Organization org in myOrganizationsResponse.Model) {

					OrganizationList.Add(org);

					var fileName = org.LogoFileName + "." + org.LogoType;
					var fImagePath = Resources.CARD_PATH + fileName;
					if (!File.Exists (Resources.DocumentsPath + "/" + fileName)) {
						Utils.DownloadImage (fImagePath, Resources.DocumentsPath, fileName).ContinueWith (t => {

						});
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
								await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (t => {
									
								});
							}
							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
								await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName).ContinueWith (t => {

								});
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
								await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (t => {
									
								});
							}
							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.Card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
								await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName).ContinueWith (t => {

								});
							}
							idx++;
						}
					});
				}

				var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(OrganizationList);

				Utils.SaveResponse(r.Result, Resources.MY_ORGANIZATIONS_FILE);

			});
			return true;
		}

		async Task<bool> loadUserCards(){

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
							Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName).ContinueWith (t => {
							});
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

		public void AddCardToMyBusidex(UserCard userCard){

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

		public void RemoveCardFromMyBusidex(UserCard userCard){

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

		void loadUser(){

			var accountJSON = AccountController.GetAccount (AuthToken);
			Utils.SaveResponse (accountJSON, Resources.BUSIDEX_USER_FILE);

			CurrentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJSON);

			if(OnBusidexUserLoaded != null){
				OnBusidexUserLoaded (CurrentUser);
			}
		}
	}
}

