﻿using System;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace Busidex.Mobile
{
	#region Delegates
	public delegate void OnMyBusidexLoadedEventHandler(List<UserCard> cards);
	public delegate void OnMyBusidexUpdatedEventHandler(ProgressStatus status);
	public delegate void OnMyOrganizationsLoadedEventHandler(List<Organization> organizations);
	public delegate void OnMyOrganizationsUpdatedEventHandler(ProgressStatus status);
	public delegate void OnMyOrganizationMembersUpdatedEventHandler(ProgressStatus status);
	public delegate void OnMyOrganizationMembersLoadedEventHandler(List<Card> cards);
	public delegate void OnMyOrganizationReferralsUpdatedEventHandler(ProgressStatus status);
	public delegate void OnMyOrganizationReferralsLoadedEventHandler(List<UserCard> cards);
	public delegate void OnEventListLoadedEventHandler(List<EventTag> tags);
	public delegate void OnEventListUpdatedEventHandler(ProgressStatus status);
	public delegate void OnEventCardsLoadedEventHandler(EventTag tag, List<UserCard> cards);
	public delegate void OnEventCardsUpdatedEventHandler(ProgressStatus status);
	public delegate void OnBusidexUserLoadedEventHandler(BusidexUser user);
	public delegate void OnNotificationsLoadedEventHandler(List<SharedCard> notifications);
	#endregion

	public static class UISubscriptionService
	{
		#region Events
		public static event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
		public static event OnMyBusidexUpdatedEventHandler OnMyBusidexUpdated;
		public static event OnMyOrganizationsLoadedEventHandler OnMyOrganizationsLoaded;
		public static event OnMyOrganizationsUpdatedEventHandler OnMyOrganizationsUpdated;
		public static event OnMyOrganizationMembersUpdatedEventHandler OnMyOrganizationMembersUpdated;
		public static event OnMyOrganizationMembersLoadedEventHandler OnMyOrganizationMembersLoaded;
		public static event OnMyOrganizationReferralsUpdatedEventHandler OnMyOrganizationReferralsUpdated;
		public static event OnMyOrganizationReferralsLoadedEventHandler OnMyOrganizationReferralsLoaded;
		public static event OnEventListLoadedEventHandler OnEventListLoaded;
		public static event OnEventListUpdatedEventHandler OnEventListUpdated;
		public static event OnEventCardsLoadedEventHandler OnEventCardsLoaded;
		public static event OnBusidexUserLoadedEventHandler OnBusidexUserLoaded;
		public static event OnNotificationsLoadedEventHandler OnNotificationsLoaded;
		#endregion

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
		public static QuickShareLink AppQuickShareLink { get; set; }

		public static string AuthToken { get; set; }

		static readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new ConcurrentDictionary<string, SemaphoreSlim>();

		public static bool OrganizationsLoaded { get; private set; }
		public static Dictionary<long, bool> OrganizationMembersLoaded { get; private set; }
		public static Dictionary<long, bool> OrganizationReferralsLoaded { get; private set; }
		public static bool EventListLoaded { get; private set; }
		public static Dictionary<string, bool> EventCardsLoaded { get; private set; }
		public static bool MyBusidexLoaded { get; private set; }

		static bool MyBusidexLoading;
		static bool EventListLoading;
		static Dictionary<string, bool> EventCardsLoading = new Dictionary<string, bool>();
		static Dictionary<long, bool> OrganizationMembersLoading = new Dictionary<long, bool>();
		static Dictionary<long, bool> OrganizationReferralsLoading = new Dictionary<long, bool>();
		static bool OrganizationsLoading;

		static UISubscriptionService(){

			EventCards = EventCards ?? new Dictionary<string, List<UserCard>> ();
			OrganizationMembers = OrganizationMembers ?? new Dictionary<long, List<Card>> ();
			OrganizationReferrals = OrganizationReferrals ?? new Dictionary<long, List<UserCard>> ();
			UserCards = new List<UserCard> ();
			EventList = new List<EventTag> ();
			OrganizationList = new List<Organization> ();
			Notifications = new List<SharedCard> ();

			EventCardsLoaded = new Dictionary<string, bool> ();
			OrganizationMembersLoaded = new Dictionary<long, bool> ();
			OrganizationReferralsLoaded = new Dictionary<long, bool> ();

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? loadUser();

			myBusidexController = new MyBusidexController ();
			organizationController = new OrganizationController ();
			searchController = new SearchController ();
		}

		#region Initialization / Startup
		static T loadData<T>(string path) where T : new(){
			return loadDataFromFile<T>(path);
		}

		static T loadDataFromFile<T>(string path) where T: new(){

			try{
				var jsonData = loadFromFile (path);
				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T> (jsonData);
				return result;
			}catch(Exception ex){
				Xamarin.Insights.Report (ex);
				return new T();
			}
		}

		public static async void Init(){

			OrganizationsLoaded = false;
			MyBusidexLoaded = false;
			EventListLoaded = false;

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? loadUser();
			CurrentUser = CurrentUser ?? new BusidexUser ();

			UserCards = loadData<List<UserCard>>(Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));
			if(UserCards == null || 0.Equals (UserCards.Count)){
				UserCards = new List<UserCard> ();
				await loadUserCards ().ContinueWith( r => {
					
				});
			} else{
				MyBusidexLoaded = true;
				if(MyBusidexLoaded && OnMyBusidexLoaded != null){
					OnMyBusidexLoaded(UserCards);
				}
			}

			Notifications = loadData<List<SharedCard>>(Path.Combine (Resources.DocumentsPath, Resources.SHARED_CARDS_FILE));
			if(Notifications == null || Notifications.Count == 0){
				Notifications = new List<SharedCard> ();
			}
			await loadNotifications ();

			if(OnNotificationsLoaded != null){
				OnNotificationsLoaded(Notifications);
			}

			EventList = loadData<List<EventTag>>(Path.Combine (Resources.DocumentsPath, Resources.EVENT_LIST_FILE));
			if(EventList == null || EventList.Count == 0){
				EventList = new List<EventTag> ();
				await loadEventList ().ContinueWith( r => {
					EventListLoaded = true;
				});
			}else{
				EventListLoaded = true;
				if(EventListLoaded && OnEventListLoaded != null){
					OnEventListLoaded(EventList);
				}
			}

			OrganizationList = loadData<List<Organization>>(Path.Combine (Resources.DocumentsPath, Resources.MY_ORGANIZATIONS_FILE));
			if(OrganizationList == null || OrganizationList.Count == 0){
				OrganizationList = new List<Organization> ();
				await loadOrganizations ().ContinueWith( r => {
					OrganizationsLoaded = true;
				});
			}else{
				OrganizationsLoaded = true;
				if(OrganizationsLoaded && OnMyOrganizationsLoaded != null){
					OnMyOrganizationsLoaded(OrganizationList);
				}
			}
		}
		#endregion

		#region Public Methods

		public static void Clear(){

			OrganizationsLoaded = false;
			MyBusidexLoaded = false;
			EventListLoaded = false;

			UserCards.Clear ();
			OrganizationList.Clear ();
			EventList.Clear ();
			CurrentUser = null;

			AuthToken = string.Empty;
		}

		public static async void Sync(){

			OrganizationsLoaded = false;
			MyBusidexLoaded = false;
			EventListLoaded = false;

			loadUser ();

			await loadUserCards ();	
			await loadOrganizations ();
			await loadEventList ();
			await loadNotifications ();
		}

		public static void LoadUser(){
			loadUser();
		}

		public async static void LoadUserCards(){
			await loadUserCards (); 	
		}

		public static void LoadOrganizations(){
			loadOrganizations ().ContinueWith(r=>{
				
			});	 	
		}

		public static void LoadEventList(){
			loadEventList ().ContinueWith (r => {
				
			});
		}

		public static void LoadEventCards(EventTag tag){
			loadEventCards (tag).ContinueWith (r => {

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

		public static void AddCardToMyBusidex(UserCard userCard){

			try{
				var fullFilePath = Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
				if (userCard!= null) {
					userCard.Card.ExistsInMyBusidex = true;
					string file;
					string myBusidexJson;
					if (File.Exists (fullFilePath)) {
						using (var myBusidexFile = File.OpenText (fullFilePath)) {
							myBusidexJson = myBusidexFile.ReadToEnd ();
						}

						var myBusidex = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserCard>> (myBusidexJson);
						myBusidex.Add (userCard);
						file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidex);
						Utils.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

						if(UserCards.FirstOrDefault(c=> c.CardId == userCard.CardId) == null){
							UserCards.Add (userCard);
						}
					}

					myBusidexController.AddToMyBusidex (userCard.Card.CardId, AuthToken);

					UserCards = sortUserCards();

					ActivityController.SaveActivity ((long)EventSources.Add, userCard.CardId, AuthToken);
				}
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
			}
		}

		public static void RemoveCardFromMyBusidex(UserCard userCard){

			try{
				var fullFilePath = Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);

				if (userCard!= null) {

					string file;
					string myBusidexJson;
					if (File.Exists (fullFilePath)) {
						using (var myBusidexFile = File.OpenText (fullFilePath)) {
							myBusidexJson = myBusidexFile.ReadToEnd ();
						}
						var myBusidex = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserCard>> (myBusidexJson);
						myBusidex.RemoveAll (uc => uc.CardId == userCard.CardId);
						file = Newtonsoft.Json.JsonConvert.SerializeObject (myBusidex);
						Utils.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

						UserCards.RemoveAll (uc => uc.CardId == userCard.CardId);
					}
					myBusidexController.RemoveFromMyBusidex (userCard.Card.CardId, AuthToken);
				}
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
			}
		}

		public static bool ExistsInMyBusidex(UserCard card){
			return UserCards.Any (uc => uc.CardId == card.CardId);
		}

		public static void SaveNotes(long userCardId, string notes){

			try{
				var controller = new NotesController ();
				controller.SaveNotes (userCardId, notes, AuthToken).ContinueWith (response => {
					var result = response.Result;
					if(!string.IsNullOrEmpty(result)){

						SaveNotesResponse obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveNotesResponse> (result);
						if(obj.Success){

							var card = UserCards.SingleOrDefault(uc => uc.UserCardId == userCardId );
							if(card != null){
								UserCards.ForEach(uc => {
									if (uc.UserCardId == userCardId) {
										uc.Notes = notes;
									}
								});
								Utils.SaveResponse(Newtonsoft.Json.JsonConvert.SerializeObject(UserCards), Resources.MY_BUSIDEX_FILE);
							}
						}
					}
				});
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
		}

		public static void ChangeEmail(string email){
			try{
				CurrentUser.Email = email;
				Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject(CurrentUser), Resources.BUSIDEX_USER_FILE);
				if(OnBusidexUserLoaded != null){
					OnBusidexUserLoaded (CurrentUser);
				}
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
		}

		public static void SaveSharedCard(SharedCard sharedCard){

			try{
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
							UserCards = sortUserCards();
							if(OnMyBusidexLoaded != null){
								OnMyBusidexLoaded (UserCards);
							}
						}
					}
				}

				// update local copy of Shared Cards
				Notifications.RemoveAll (c => c.Card.CardId == sharedCard.Card.CardId);
				Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject(Notifications), Resources.SHARED_CARDS_FILE);
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
		}
		#endregion

		#region Load From API
		public static async Task<bool> loadEventCards(EventTag tag){

			if(EventCardsLoading.ContainsKey(tag.Text) && EventCardsLoading[tag.Text]){
				return false;
			}

			if(!EventCardsLoading.ContainsKey(tag.Text)){
				EventCardsLoading.Add (tag.Text, true);
			}else{
				EventCardsLoading [tag.Text] = true;
			}
			if(!EventCardsLoaded.ContainsKey(tag.Text)){
				EventCardsLoaded.Add (tag.Text, false);
			}else{
				EventCardsLoaded [tag.Text] = false;
			}

			var fileName = string.Format(Resources.EVENT_CARDS_FILE, tag);
			var semaphore = locks.GetOrAdd(fileName, new SemaphoreSlim(1, 1));
			await semaphore.WaitAsync();

			try{
				await searchController.SearchBySystemTag(tag.Text, AuthToken).ContinueWith(async t => {
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
								try{
									await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName);
								}catch(Exception){

								}
							}
							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
								try{
									await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
								}catch(Exception){

								}
							}
						}
					}
					if(!EventCards.ContainsKey(tag.Text)){
						EventCards.Add(tag.Text, cards);
					}
					var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(EventList);


					Utils.SaveResponse (savedResult, fileName);

					EventCardsLoading [tag.Text] = false;
					EventCardsLoaded [tag.Text] = true;

					if(OnEventCardsLoaded != null){
						OnEventCardsLoaded(tag, EventCards[tag.Text]);
					}
				});
			}
			catch(Exception ex){
				EventCardsLoading [tag.Text] = false;
				Xamarin.Insights.Report (ex);
			}
			finally{
				semaphore.Release ();
			}
			return true;
		}

		static async Task<bool> loadEventList(){

			if(EventListLoading){
				return false;
			}

			EventListLoading = true;

			var semaphore = locks.GetOrAdd(Resources.EVENT_LIST_FILE, new SemaphoreSlim(1, 1));
			await semaphore.WaitAsync();

			try{
				
				await searchController.GetEventTags (AuthToken).ContinueWith(async r => {

					EventList.Clear ();
					if(r.Result == null || string.IsNullOrEmpty(r.Result)){
						var fullFileName = Path.Combine(Resources.DocumentsPath, Resources.EVENT_LIST_FILE);
						EventList.AddRange(Utils.GetCachedResult<List<EventTag>>(fullFileName));
					}else{
						var eventListResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (r.Result);
						EventList.AddRange(eventListResponse.Model);
					}

					var savedEvents = Newtonsoft.Json.JsonConvert.SerializeObject(EventList);

					Utils.SaveResponse (savedEvents, Resources.EVENT_LIST_FILE);

					if(OnEventListLoaded != null){
						EventListLoading = false;
						EventListLoaded = true;
						OnEventListLoaded(EventList);
					}

					foreach(var tag in EventList){
						await loadEventCards(tag);
					}
				});
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
			finally{
				semaphore.Release ();
			}
			return true;
		}

		static async Task<bool> loadOrganizations(){

			if(OrganizationsLoading){
				return false;
			}

			OrganizationsLoading = true;

			var semaphore = locks.GetOrAdd(Resources.MY_ORGANIZATIONS_FILE, new SemaphoreSlim(1, 1));
			await semaphore.WaitAsync();

			try{

				await organizationController.GetMyOrganizations (AuthToken).ContinueWith (async r => {
					if(r.Result != null){
						OrganizationResponse myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (r.Result);

						if(myOrganizationsResponse != null && myOrganizationsResponse.Model != null){

							// Buid the Organization List
							OrganizationList.Clear ();
							OrganizationList.AddRange(myOrganizationsResponse.Model);

							var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(OrganizationList);
							Utils.SaveResponse(savedResult, Resources.MY_ORGANIZATIONS_FILE);

							var status = new ProgressStatus();
							status.Total = myOrganizationsResponse.Model.Count;

							// Get Organization members and referals
							foreach (Organization org in myOrganizationsResponse.Model) {

								status.Count++;
								if(OnMyOrganizationsUpdated != null){
									OnMyOrganizationsUpdated(status);
								}

								var fileName = org.LogoFileName + "." + org.LogoType;
								var fImagePath = Resources.CARD_PATH + fileName;
								if (!File.Exists (Resources.DocumentsPath + "/" + fileName)) {
									try{
										await Utils.DownloadImage (fImagePath, Resources.DocumentsPath, fileName);
									}catch(Exception){
										
									}
								} 
								// load organization members and referrals
								await LoadOrganizationMembers(org.OrganizationId);
								await LoadOrganizationReferrals(org.OrganizationId);
							}
						}

						OrganizationsLoaded = true;
						OrganizationsLoading = false;
						if(OnMyOrganizationsLoaded != null){
							OnMyOrganizationsLoaded(OrganizationList);
						}
					}
				});
			}
			catch(Exception ex){
				OrganizationsLoaded = true;
				OrganizationsLoading = false;
				Xamarin.Insights.Report (ex);
			}
			finally{
				semaphore.Release ();
			}
			return true;
		}

		public static async Task<bool> LoadOrganizationReferrals(long organizationId){
		
			if(OrganizationReferralsLoading.ContainsKey(organizationId) && OrganizationReferralsLoading[organizationId]){
				return false;
			}

			if(!OrganizationReferralsLoading.ContainsKey(organizationId)){
				OrganizationReferralsLoading.Add (organizationId, true);
			}else{
				OrganizationReferralsLoading[organizationId] = true;
			}
			if(!OrganizationReferralsLoading.ContainsKey(organizationId)){
				OrganizationReferralsLoading.Add (organizationId, false);
			}else{
				OrganizationReferralsLoading [organizationId] = false;
			}

			var fileName = string.Format(Resources.ORGANIZATION_REFERRALS_FILE, organizationId);
			var semaphore = locks.GetOrAdd(fileName, new SemaphoreSlim(1, 1));
			await semaphore.WaitAsync();

			try{
				await organizationController.GetOrganizationReferrals(AuthToken, organizationId).ContinueWith(async cards =>{

					var orgReferralResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (cards.Result);
					Utils.SaveResponse(Newtonsoft.Json.JsonConvert.SerializeObject(orgReferralResponse.Model), string.Format(Resources.ORGANIZATION_REFERRALS_FILE, organizationId));

					OrganizationReferrals = OrganizationReferrals ?? new Dictionary<long, List<UserCard>>();
					if(!OrganizationReferrals.ContainsKey(organizationId)){
						OrganizationReferrals.Add(organizationId, orgReferralResponse.Model);
					}else{
						OrganizationReferrals[organizationId] = orgReferralResponse.Model;
					}

					var status = new ProgressStatus();
					status.Total = OrganizationReferrals[organizationId].Count;

					foreach(var card in orgReferralResponse.Model){

						var fImageUrl = Resources.THUMBNAIL_PATH + card.Card.FrontFileName;
						var bImageUrl = Resources.THUMBNAIL_PATH + card.Card.BackFileName;
						var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName;
						var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.BackFileName;
						if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
							try{
								await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith( r => {
									status.Count++;
								});
							}catch(Exception){

							}
						}else{
							status.Count++;
						}

						if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.Card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID) {
							try{
								await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
							}catch(Exception){

							}
						}
						if(OnMyOrganizationReferralsUpdated != null){
							OnMyOrganizationReferralsUpdated(status);
						}
					}

					OrganizationReferralsLoading[organizationId] = false;
					OrganizationReferralsLoaded[organizationId] = true;

					if(OnMyOrganizationReferralsLoaded != null){
						OnMyOrganizationReferralsLoaded(OrganizationReferrals[organizationId] );
					}
				});
			}catch(Exception ex){
				OrganizationReferralsLoading[organizationId] = false;
				OrganizationReferralsLoaded[organizationId] = true;

				Xamarin.Insights.Report (new Exception("Error Loading Organization Referrals", ex));
			}
			finally{
				semaphore.Release ();	
			}
			return true;
		}

		public static async Task<bool> LoadOrganizationMembers(long organizationId){

			if(OrganizationMembersLoading.ContainsKey(organizationId) && OrganizationMembersLoading[organizationId]){
				return false;
			}

			if(!OrganizationMembersLoading.ContainsKey(organizationId)){
				OrganizationMembersLoading.Add (organizationId, true);
			}else{
				OrganizationMembersLoading[organizationId] = true;
			}
			if(!OrganizationMembersLoaded.ContainsKey(organizationId)){
				OrganizationMembersLoaded.Add (organizationId, false);
			}else{
				OrganizationMembersLoaded [organizationId] = false;
			}

			var fileName = string.Format(Resources.ORGANIZATION_MEMBERS_FILE, organizationId);
			var semaphore = locks.GetOrAdd(fileName, new SemaphoreSlim(1, 1));
			await semaphore.WaitAsync();

			try {
				await organizationController.GetOrganizationMembers (AuthToken, organizationId).ContinueWith (async cards => {

					var orgMemberResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (cards.Result);

					Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (orgMemberResponse.Model), string.Format (Resources.ORGANIZATION_MEMBERS_FILE, organizationId));

					OrganizationMembers = OrganizationMembers ?? new Dictionary<long, List<Card>> ();
					if (!OrganizationMembers.ContainsKey (organizationId)) {
						OrganizationMembers.Add (organizationId, orgMemberResponse.Model);
					} else {
						OrganizationMembers [organizationId] = orgMemberResponse.Model;
					}

					var status = new ProgressStatus();
					status.Total = OrganizationMembers[organizationId].Count;

					foreach (var card in orgMemberResponse.Model) {

						var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
						var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
						var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
						var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
						if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
							try {
								await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith(r => {
									status.Count++;
								});
							} catch (Exception) {

							}
						}else{
							status.Count++;
						}

						if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString ().ToLowerInvariant () != Resources.EMPTY_CARD_ID) {
							try {
								await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
							} catch (Exception) {

							}
						}

						if(OnMyOrganizationMembersUpdated != null){
							OnMyOrganizationMembersUpdated(status);
						}
					}

					OrganizationMembersLoading[organizationId] = false;
					OrganizationMembersLoaded[organizationId] = true;

					if(OnMyOrganizationMembersLoaded != null){
						OnMyOrganizationMembersLoaded(OrganizationMembers [organizationId]);
					}
				});
			} catch (Exception ex) {
				OrganizationMembersLoading[organizationId] = false;
				OrganizationMembersLoaded[organizationId] = true;

				Xamarin.Insights.Report (new Exception("Error Loading Organization Members", ex));
			}
			finally{
				semaphore.Release ();
			}

			return true;
		}

		static async Task<bool> loadUserCards(){

			if(MyBusidexLoading){
				return false;
			}

			MyBusidexLoading = true;

			var semaphore = locks.GetOrAdd(Resources.MY_BUSIDEX_FILE, new SemaphoreSlim(1, 1));
			await semaphore.WaitAsync();


			var cards = new List<UserCard> ();
			var status = new ProgressStatus();

			try{
				await myBusidexController.GetMyBusidex (AuthToken).ContinueWith (async r => {

					if(r.Result == null || string.IsNullOrEmpty(r.Result)){
						var fullFileName = Path.Combine(Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
						cards.AddRange(Utils.GetCachedResult<List<UserCard>>(fullFileName));
					}else{
						var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);
						cards.AddRange(myBusidexResponse.MyBusidex.Busidex);
						cards.ForEach (c => c.ExistsInMyBusidex = true);	
					}

					status.Total = cards.Count;

					foreach (var item in cards) {
						if (item.Card != null) {

							var fImageUrl = Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
							var bImageUrl = Resources.THUMBNAIL_PATH + item.Card.BackFileName;
							var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
							var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

							if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
								try{
									await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (t => {
										status.Count++;
										if(OnMyBusidexUpdated != null){
											OnMyBusidexUpdated(status);
										}
									});
								}catch(Exception){

								}
							} else{
								status.Count++;
								if(OnMyBusidexUpdated != null){
									OnMyBusidexUpdated(status);
								}
							}

							if ((!File.Exists (Resources.DocumentsPath + "/" + bName)) && item.Card.BackFileId.ToString () != Resources.EMPTY_CARD_ID) {
								try{
									await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
								}catch(Exception){

								}
							}

						}
					}

					UserCards.Clear();
					UserCards.AddRange(cards);

					var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(UserCards);

					Utils.SaveResponse (savedResult, Resources.MY_BUSIDEX_FILE);

					// Fire event handler
					if(OnMyBusidexLoaded != null){
						MyBusidexLoading = false;
						MyBusidexLoaded = true;
						OnMyBusidexLoaded(UserCards);
					}

				});
			}catch(Exception ex){
				MyBusidexLoading = false;
				MyBusidexLoaded = true;
				Xamarin.Insights.Report (new Exception("Error Loading My Busidex", ex));
			}
			finally{
				semaphore.Release ();
			}
			return true;
		}

		static async Task<bool> loadNotifications(){
			try{
				var ctrl = new SharedCardController ();
				var sharedCardsResponse = ctrl.GetSharedCards (AuthToken);
				if(sharedCardsResponse.Contains(":404") || string.IsNullOrEmpty(sharedCardsResponse)){
					return false;
				}

				var sharedCards = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsResponse);

				if (sharedCards.Success) {
					foreach (SharedCard card in sharedCards.SharedCards) {
						var fileName = card.Card.FrontFileName;
						var fImagePath = Resources.CARD_PATH + fileName;
						if (!File.Exists (Resources.DocumentsPath + "/" + fileName)) {
							try{
								await Utils.DownloadImage (fImagePath, Resources.DocumentsPath, fileName);
							}catch(Exception){

							}
						}
					}

					Notifications = sharedCards.SharedCards;

					Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (Notifications), Resources.SHARED_CARDS_FILE);

				}else{
					Notifications = new List<SharedCard> ();
				}

				if(OnNotificationsLoaded != null){
					OnNotificationsLoaded(Notifications);
				}

			}
			catch(Exception ex){
				Xamarin.Insights.Report (new Exception("Error Loading Notifications", ex));
			}
			return true;
		}
		#endregion

		#region Private Methods
		static List<UserCard> sortUserCards(){
			return UserCards.OrderByDescending (c => c.Card != null && c.Card.OwnerId.GetValueOrDefault () > 0 ? 1 : 0)
				.ThenBy (c => c.Card != null ? c.Card.Name : "")
				.ThenBy (c => c.Card != null ? c.Card.CompanyName : "")
				.ToList ();
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

		static Card GetCardFromSharedCard(string token, long cardId){

			try{
				var cardData = CardController.GetCardById(token, cardId);
				if(!string.IsNullOrEmpty(cardData)){
					var response = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse>(cardData);
					return new Card(response.Model);
				}
			}catch(Exception ex){
				Xamarin.Insights.Report (ex);
				return null;
			}
			return null;
		}

		static BusidexUser loadUser(){

			try{
				if(!string.IsNullOrEmpty(AuthToken)){
					var accountJSON = AccountController.GetAccount (AuthToken);
					Utils.SaveResponse (accountJSON, Resources.BUSIDEX_USER_FILE);

					if(!string.IsNullOrEmpty(accountJSON)){
						CurrentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJSON);

						if(OnBusidexUserLoaded != null){
							OnBusidexUserLoaded (CurrentUser);
						}
					}
				}
				return CurrentUser;
			}
			catch(Exception ex){
				Xamarin.Insights.Report (ex);
			}
			return null;
		}
		#endregion
	}
}