using System;
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
	public delegate void OnMyBusidexLoadedEventHandler (List<UserCard> cards);
	public delegate void OnMyBusidexUpdatedEventHandler (ProgressStatus status);
	public delegate void OnMyOrganizationsLoadedEventHandler (List<Organization> organizations);
	public delegate void OnMyOrganizationsUpdatedEventHandler (ProgressStatus status);
	public delegate void OnMyOrganizationMembersUpdatedEventHandler (ProgressStatus status);
	public delegate void OnMyOrganizationMembersLoadedEventHandler (List<Card> cards);
	public delegate void OnMyOrganizationReferralsUpdatedEventHandler (ProgressStatus status);
	public delegate void OnMyOrganizationReferralsLoadedEventHandler (List<UserCard> cards);
	public delegate void OnEventListLoadedEventHandler (List<EventTag> tags);
	public delegate void OnEventListUpdatedEventHandler (ProgressStatus status);
	public delegate void OnEventCardsLoadedEventHandler (EventTag tag, List<UserCard> cards);
	public delegate void OnEventCardsUpdatedEventHandler (ProgressStatus status);
	public delegate void OnBusidexUserLoadedEventHandler (BusidexUser user);
	public delegate void OnNotificationsLoadedEventHandler (List<SharedCard> notifications = null);
	public delegate void OnNotificationCountUpdatedEventHandler (int count);
	public delegate void OnNotesUpdatedEventHandler ();
	public delegate void OnCardInfoUpdatingHandler ();
	public delegate void OnCardInfoSavedHandler ();
	public delegate void OnQuickShareLoadedHandler (QuickShareLink link);

	#endregion

	public static class UISubscriptionService
	{
		#region Events

		public static event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
		public static event OnMyBusidexUpdatedEventHandler OnMyBusidexUpdated;
		public static event OnMyOrganizationsLoadedEventHandler OnMyOrganizationsLoaded;
		public static event OnMyOrganizationsUpdatedEventHandler OnMyOrganizationsUpdated;
		public static event OnMyOrganizationMembersUpdatedEventHandler OnMyOrganizationMembersUpdated;
		public static event OnMyOrganizationReferralsUpdatedEventHandler OnMyOrganizationReferralsUpdated;
		public static event OnEventListLoadedEventHandler OnEventListLoaded;
		public static event OnEventListUpdatedEventHandler OnEventListUpdated;
		public static event OnCardInfoUpdatingHandler OnCardInfoUpdating;
		public static event OnCardInfoSavedHandler OnCardInfoSaved;

		public static Dictionary<string, OnEventCardsLoadedEventHandler> EventCardsLoadedEventTable;
		public static Dictionary<long, OnMyOrganizationMembersLoadedEventHandler> OrganizationMembersLoadedEventTable;
		public static Dictionary<long, OnMyOrganizationReferralsLoadedEventHandler> OrganizationReferralsLoadedEventTable;

		public static event OnEventCardsUpdatedEventHandler OnEventCardsUpdated;
		public static event OnBusidexUserLoadedEventHandler OnBusidexUserLoaded;
		public static event OnNotificationsLoadedEventHandler OnNotificationsLoaded;
		public static event OnNotificationCountUpdatedEventHandler OnNotificationCountUpdated;
		public static event OnNotesUpdatedEventHandler OnNotesUpdated;

		public static event OnQuickShareLoadedHandler OnQuickShareLoaded;
		#endregion

		static readonly MyBusidexController myBusidexController;
		static readonly OrganizationController organizationController;
		static readonly SearchController searchController;
		//static readonly CardController cardController;

		#region Public Properties
		public static Card OwnedCard { get; set; }

		public static List<UserCard> UserCards { get; set; }

		public static List<EventTag> EventList { get; set; }

		public static Dictionary<string, List<UserCard>> EventCards { get; set; }

		public static List<Organization> OrganizationList { get; set; }

		public static Dictionary<long, List<Card>> OrganizationMembers { get; set; }

		public static Dictionary<long, List<UserCard>> OrganizationReferrals { get; set; }

		public static BusidexUser CurrentUser { get; set; }

		public static List<SharedCard> Notifications { get; set; }

		static QuickShareLink _quickShareLink;
		public static QuickShareLink AppQuickShareLink { 
			get{
				return _quickShareLink;
			}
			set {
				_quickShareLink = value;	
				if(OnQuickShareLoaded != null){
					OnQuickShareLoaded (_quickShareLink);
				}
			} 
		}

		public static string AuthToken { get; set; }

		static readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new ConcurrentDictionary<string, SemaphoreSlim> ();

		public static bool OrganizationsLoaded { get; private set; }

		public static Dictionary<long, bool> OrganizationMembersLoaded { get; private set; }

		public static Dictionary<long, bool> OrganizationReferralsLoaded { get; private set; }

		public static bool EventListLoaded { get; private set; }

		public static Dictionary<string, bool> EventCardsLoaded { get; private set; }

		public static bool MyBusidexLoaded { get; private set; }

		#endregion

		static bool MyBusidexLoading;
		static bool EventListLoading;
		static Dictionary<string, bool> EventCardsLoading;
		static bool OrganizationsLoading;
		static Dictionary<long, bool> OrganizationMembersLoading;
		static Dictionary<long, bool> OrganizationReferralsLoading;


		static UISubscriptionService ()
		{

			myBusidexController = new MyBusidexController ();
			organizationController = new OrganizationController ();
			searchController = new SearchController ();
			//cardController = new CardController ();

			InitDataStructures ();

			ResetFlags ();

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? loadUser ();
		}



		#region Initialization / Startup

		static T loadData<T> (string path) where T : new()
		{
			return loadDataFromFile<T> (path);
		}

		static T loadDataFromFile<T> (string path) where T : new()
		{

			try {
				var jsonData = loadFromFile (path);
				if(string.IsNullOrEmpty(jsonData)){
					return default(T);
				}
				var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T> (jsonData);
				return result;
			} catch (Exception ex) {
				Xamarin.Insights.Report (new Exception("Error loading jsonData from " + path, ex));
				return default(T);
			}
		}

		public static async void Init ()
		{

			InitDataStructures ();

			ResetFlags ();

			CurrentUser = loadDataFromFile<BusidexUser> (Path.Combine (Resources.DocumentsPath, Resources.BUSIDEX_USER_FILE)) ?? loadUser ();
			CurrentUser = CurrentUser ?? new BusidexUser ();

			OwnedCard = loadDataFromFile<Card> (Path.Combine (Resources.DocumentsPath, Resources.OWNED_CARD_FILE)) ?? await loadOwnedCard ();
			await loadOwnedCard ().ContinueWith (result => {
				if (result.IsFaulted) {
					OwnedCard = null;//OwnedCard ;?? new Card ();
				}
			});

			UserCards = loadData<List<UserCard>> (Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));
			if (UserCards == null || UserCards.Count == 0) {
				UserCards = new List<UserCard> ();
				await loadUserCards ();
			} else {
				MyBusidexLoaded = true;
				MyBusidexLoading = false;
				if (OnMyBusidexLoaded != null) {
					OnMyBusidexLoaded (UserCards);
				}
			}

			Notifications = loadData<List<SharedCard>> (Path.Combine (Resources.DocumentsPath, Resources.SHARED_CARDS_FILE));
			if (Notifications == null || Notifications.Count == 0) {
				Notifications = new List<SharedCard> ();
			}
			await loadNotifications ().ContinueWith (r => {
				if (OnNotificationsLoaded != null) {
					OnNotificationsLoaded (Notifications);
				}
			});

			EventList = loadData<List<EventTag>> (Path.Combine (Resources.DocumentsPath, Resources.EVENT_LIST_FILE));
			if (EventList == null || EventList.Count == 0) {
				EventList = new List<EventTag> ();
				await loadEventList ();
			} else {
				EventListLoaded = true;
				EventListLoading = false;
				if (OnEventListLoaded != null) {
					OnEventListLoaded (EventList);
				}

				EventCardsLoadedEventTable = new Dictionary<string, OnEventCardsLoadedEventHandler> ();

				foreach (var ev in EventList) {
					if (!EventCardsLoadedEventTable.ContainsKey (ev.Text)) {
						EventCardsLoadedEventTable.Add (ev.Text, null);
					}
					if (!EventCards.ContainsKey (ev.Text)) {
						EventCards.Add (ev.Text, new List<UserCard> ());
					}
					EventCards [ev.Text] = loadData<List<UserCard>> (Path.Combine (Resources.DocumentsPath, string.Format (Resources.EVENT_CARDS_FILE, ev.EventTagId)));
				}
			}

			OrganizationList = loadData<List<Organization>> (Path.Combine (Resources.DocumentsPath, Resources.MY_ORGANIZATIONS_FILE));
			if (OrganizationList == null || OrganizationList.Count == 0) {
				OrganizationList = new List<Organization> ();
				await loadOrganizations ();
			} else {
				OrganizationsLoaded = true;
				OrganizationsLoading = false;

				if (OrganizationsLoaded && OnMyOrganizationsLoaded != null) {
					OnMyOrganizationsLoaded (OrganizationList);
				}


				OrganizationMembersLoadedEventTable = new Dictionary<long, OnMyOrganizationMembersLoadedEventHandler> ();
				OrganizationReferralsLoadedEventTable = new Dictionary<long, OnMyOrganizationReferralsLoadedEventHandler> ();

				foreach (Organization org in OrganizationList) {
					if(!OrganizationMembersLoadedEventTable.ContainsKey(org.OrganizationId)){
						OrganizationMembersLoadedEventTable.Add (org.OrganizationId, null);	
					}
					if (!OrganizationReferralsLoadedEventTable.ContainsKey (org.OrganizationId)) { 
						OrganizationReferralsLoadedEventTable.Add (org.OrganizationId, null);
					}
				}

				foreach (var org in OrganizationList) {
					if (!OrganizationMembers.ContainsKey (org.OrganizationId)) {
						OrganizationMembers.Add (org.OrganizationId, new List<Card> ());
					}
					if (!OrganizationReferrals.ContainsKey (org.OrganizationId)) {
						OrganizationReferrals.Add (org.OrganizationId, new List<UserCard> ());
					}
					OrganizationMembers [org.OrganizationId] = loadData<List<Card>> (Path.Combine (Resources.DocumentsPath, string.Format (Resources.ORGANIZATION_MEMBERS_FILE, org.OrganizationId)));
					OrganizationReferrals [org.OrganizationId] = loadData<List<UserCard>> (Path.Combine (Resources.DocumentsPath, string.Format (Resources.ORGANIZATION_REFERRALS_FILE, org.OrganizationId)));
				}

			}
		}

		#endregion

		#region Public Methods
		public static async void LoadOwnedCard(){
			await loadOwnedCard (); 	
		}

		public static List<PhoneNumberType> GetPhoneNumberTypes ()
		{
			return new List<PhoneNumberType> {
				new PhoneNumberType {
					PhoneNumberTypeId = 1,
					Name = "Business"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 2,
					Name = "Home"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 3,
					Name = "Mobile"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 4,
					Name = "Fax"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 5,
					Name = "Toll Free"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 6,
					Name = "eFax"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 7,
					Name = "Other"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 8,
					Name = "Direct"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 9,
					Name = "Voice Mail"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 10,
					Name = "Business 2"
				}
			};
		}

		public static List<State> GetStates ()
		{
			var states = new List<State> ();
			states.Add (new State {
				StateCodeId = 1,
				Code = "AL",
				Name = "Alabama"
			});
			states.Add (new State {
				StateCodeId = 2,
				Code = "AK",
				Name = "Alaska"
			});
			states.Add (new State {
				StateCodeId = 3,
				Code = "AZ",
				Name = "Arizona"
			});
			states.Add (new State {
				StateCodeId = 4,
				Code = "AR",
				Name = "Arkansas"
			});
			states.Add (new State {
				StateCodeId = 5,
				Code = "CA",
				Name = "California"
			});
			states.Add (new State {
				StateCodeId = 6,
				Code = "CO",
				Name = "Colorado"
			});
			states.Add (new State {
				StateCodeId = 7,
				Code = "CT",
				Name = "Connecticut"
			});
			states.Add (new State {
				StateCodeId = 8,
				Code = "DE",
				Name = "Delaware"
			});
			states.Add (new State {
				StateCodeId = 9,
				Code = "DC",
				Name = "District Of Columbia"
			});
			states.Add (new State {
				StateCodeId = 10,
				Code = "FL",
				Name = "Florida"
			});
			states.Add (new State {
				StateCodeId = 11,
				Code = "GA",
				Name = "Georgia"
			});
			states.Add (new State {
				StateCodeId = 12,
				Code = "HI",
				Name = "Hawaii"
			});
			states.Add (new State {
				StateCodeId = 13,
				Code = "ID",
				Name = "Idaho"
			});
			states.Add (new State {
				StateCodeId = 14,
				Code = "IL",
				Name = "Illinois"
			});
			states.Add (new State {
				StateCodeId = 15,
				Code = "IN",
				Name = "Indiana"
			});
			states.Add (new State {
				StateCodeId = 16,
				Code = "IA",
				Name = "Iowa"
			});
			states.Add (new State {
				StateCodeId = 17,
				Code = "KS",
				Name = "Kansas"
			});
			states.Add (new State {
				StateCodeId = 18,
				Code = "KY",
				Name = "Kentucky"
			});
			states.Add (new State {
				StateCodeId = 19,
				Code = "LA",
				Name = "Louisiana"
			});
			states.Add (new State {
				StateCodeId = 20,
				Code = "ME",
				Name = "Maine"
			});
			states.Add (new State {
				StateCodeId = 21,
				Code = "MD",
				Name = "Maryland"
			});
			states.Add (new State {
				StateCodeId = 22,
				Code = "MA",
				Name = "Massachusetts"
			});
			states.Add (new State {
				StateCodeId = 23,
				Code = "MI",
				Name = "Michigan"
			});
			states.Add (new State {
				StateCodeId = 24,
				Code = "MN",
				Name = "Minnesota"
			});
			states.Add (new State {
				StateCodeId = 25,
				Code = "MS",
				Name = "Mississippi"
			});
			states.Add (new State {
				StateCodeId = 26,
				Code = "MO",
				Name = "Missouri"
			});
			states.Add (new State {
				StateCodeId = 27,
				Code = "MT",
				Name = "Montana"
			});
			states.Add (new State {
				StateCodeId = 28,
				Code = "NE",
				Name = "Nebraska"
			});
			states.Add (new State {
				StateCodeId = 29,
				Code = "NV",
				Name = "Nevada"
			});
			states.Add (new State {
				StateCodeId = 30,
				Code = "NH",
				Name = "New Hampshire"
			});
			states.Add (new State {
				StateCodeId = 31,
				Code = "NJ",
				Name = "New Jersey"
			});
			states.Add (new State {
				StateCodeId = 32,
				Code = "NM",
				Name = "New Mexico"
			});
			states.Add (new State {
				StateCodeId = 33,
				Code = "NY",
				Name = "New York"
			});
			states.Add (new State {
				StateCodeId = 34,
				Code = "NC",
				Name = "North Carolina"
			});
			states.Add (new State {
				StateCodeId = 35,
				Code = "ND",
				Name = "North Dakota"
			});
			states.Add (new State {
				StateCodeId = 36,
				Code = "OH",
				Name = "Ohio"
			});
			states.Add (new State {
				StateCodeId = 37,
				Code = "OK",
				Name = "Oklahoma"
			});
			states.Add (new State {
				StateCodeId = 38,
				Code = "OR",
				Name = "Oregon"
			});
			states.Add (new State {
				StateCodeId = 39,
				Code = "PA",
				Name = "Pennsylvania"
			});
			states.Add (new State {
				StateCodeId = 40,
				Code = "RI",
				Name = "Rhode Island"
			});
			states.Add (new State {
				StateCodeId = 41,
				Code = "SC",
				Name = "South Carolina"
			});
			states.Add (new State {
				StateCodeId = 42,
				Code = "SD",
				Name = "South Dakota"
			});
			states.Add (new State {
				StateCodeId = 43,
				Code = "TN",
				Name = "Tennessee"
			});
			states.Add (new State {
				StateCodeId = 44,
				Code = "TX",
				Name = "Texas"
			});
			states.Add (new State {
				StateCodeId = 45,
				Code = "UT",
				Name = "Utah"
			});
			states.Add (new State {
				StateCodeId = 46,
				Code = "VT",
				Name = "Vermont"
			});
			states.Add (new State {
				StateCodeId = 47,
				Code = "VA",
				Name = "Virginia"
			});
			states.Add (new State {
				StateCodeId = 48,
				Code = "WA",
				Name = "Washington"
			});
			states.Add (new State {
				StateCodeId = 49,
				Code = "WV",
				Name = "West Virginia"
			});
			states.Add (new State {
				StateCodeId = 50,
				Code = "WI",
				Name = "Wisconsin"
			});
			states.Add (new State {
				StateCodeId = 51,
				Code = "WY",
				Name = "Wyoming"
			});
			return states;
		}

		public static void Clear ()
		{

			ResetFlags ();

			InitDataStructures ();

			CurrentUser = null;

			AuthToken = string.Empty;
		}

		public static async void Sync ()
		{

			OrganizationsLoaded = false;
			MyBusidexLoaded = false;
			EventListLoaded = false;

			loadUser ();

			await loadOwnedCard ();
			await loadUserCards ();
			await loadOrganizations ();
			await loadEventList ();
			await loadNotifications ();
		}

		public static void LoadUser ()
		{
			loadUser ();
		}

		public async static void LoadUserCards ()
		{
			await loadUserCards ();
		}

		public async static void LoadOrganizations ()
		{
			await loadOrganizations ();
		}

		public async static void LoadEventList ()
		{
			await loadEventList ();
		}

		public async static void LoadEventCards (EventTag tag)
		{
			await loadEventCards (tag);
		}

		public static async void LoadNotifications ()
		{

			Notifications = loadData<List<SharedCard>> (Path.Combine (Resources.DocumentsPath, Resources.SHARED_CARDS_FILE));

			await loadNotifications ().ContinueWith (r => {
				if (OnNotificationsLoaded != null) {
					OnNotificationsLoaded (Notifications);
				}
			});
		}

		public static async void AddCardToMyBusidex (UserCard userCard)
		{
			try {
				if (userCard != null) {
					userCard.Card.ExistsInMyBusidex = true;

					if (!UserCards.Any (c => c.CardId.Equals (userCard.CardId))) {
						UserCards.Add (userCard);
					}

					await myBusidexController.AddToMyBusidex (userCard.Card.CardId, AuthToken);

					UserCards = sortUserCards ();

					var file = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);
					Utils.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

					ActivityController.SaveActivity ((long)EventSources.Add, userCard.CardId, AuthToken);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
			}
		}

		public static void RemoveCardFromMyBusidex (UserCard userCard)
		{
			try {

				if (userCard != null) {

					UserCards.RemoveAll (uc => uc.CardId == userCard.CardId);

					var file = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);
					Utils.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

					myBusidexController.RemoveFromMyBusidex (userCard.Card.CardId, AuthToken);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
			}
		}

		public static bool ExistsInMyBusidex (UserCard card)
		{
			return UserCards.Any (uc => uc.CardId == card.CardId);
		}

		public async static void SaveNotes (long userCardId, string notes)
		{
			try {
				var controller = new NotesController ();
				await controller.SaveNotes (userCardId, notes, AuthToken).ContinueWith (response => {
					var result = response.Result;
					if (!string.IsNullOrEmpty (result)) {

						SaveNotesResponse obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveNotesResponse> (result);
						if (obj.Success) {

							var card = UserCards.SingleOrDefault (uc => uc.UserCardId == userCardId);
							if (card != null) {
								UserCards.ForEach (uc => {
									if (uc.UserCardId == userCardId) {
										uc.Notes = notes;
									}
								});
								Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (UserCards), Resources.MY_BUSIDEX_FILE);
							}
						}
					}
					if (OnNotesUpdated != null) {
						OnNotesUpdated ();
					}
				});
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		public static void ChangeEmail (string email)
		{
			try {
				CurrentUser.Email = email;
				Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (CurrentUser), Resources.BUSIDEX_USER_FILE);
				if (OnBusidexUserLoaded != null) {
					OnBusidexUserLoaded (CurrentUser);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		public static void SaveSharedCard (SharedCard sharedCard)
		{
			try {
				// Accept/Decline the card
				var ctrl = new SharedCardController ();
				var cardId = new long? (sharedCard.Card.CardId);

				ctrl.UpdateSharedCards (
					sharedCard.Accepted.GetValueOrDefault () ? cardId : null,
					sharedCard.Declined.GetValueOrDefault () ? cardId : null,
					AuthToken);

				// if the card was accepted, update local copy of MyBusidex
				if (sharedCard.Accepted.GetValueOrDefault ()) {

					var card = GetCardFromSharedCard (AuthToken, sharedCard.CardId);

					if (card != null) {
						var userCard = new UserCard {
							Card = card,
							CardId = card.CardId
						};
						if (UserCards.All (uc => uc.CardId != card.CardId)) {
							UserCards.Add (userCard);
							UserCards = sortUserCards ();
							if (OnMyBusidexLoaded != null) {
								OnMyBusidexLoaded (UserCards);
							}
						}
					}
				}

				// update local copy of Shared Cards
				Notifications.RemoveAll (c => c.Card.CardId == sharedCard.Card.CardId);
				Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (Notifications), Resources.SHARED_CARDS_FILE);

				// Notify subscribers
				if (OnNotificationCountUpdated != null) {
					OnNotificationCountUpdated (Notifications.Count);
				}

			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
		}

		#endregion

		#region Load From API

		public static async Task<bool> loadEventCards (EventTag tag)
		{

			if (EventCardsLoading.ContainsKey (tag.Text) && EventCardsLoading [tag.Text]) {
				return false;
			}

			if (!EventCardsLoading.ContainsKey (tag.Text)) {
				EventCardsLoading.Add (tag.Text, true);
			} else {
				EventCardsLoading [tag.Text] = true;
			}
			if (!EventCardsLoaded.ContainsKey (tag.Text)) {
				EventCardsLoaded.Add (tag.Text, false);
			} else {
				EventCardsLoaded [tag.Text] = false;
			}

			var fileName = string.Format (Resources.EVENT_CARDS_FILE, tag.EventTagId);
			var semaphore = locks.GetOrAdd (fileName, new SemaphoreSlim (1, 1));
			await semaphore.WaitAsync ();

			try {
				await searchController.SearchBySystemTag (tag.Text, AuthToken).ContinueWith (async t => {

					var eventSearchResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventSearchResponse> (t.Result);

					var cards = new List<UserCard> ();

					var status = new ProgressStatus ();
					status.Total = eventSearchResponse.SearchModel.Results.Count;

					if (!EventCards.ContainsKey (tag.Text)) {
						EventCards.Add (tag.Text, new List<UserCard> ());
					} else {
						EventCards [tag.Text] = new List<UserCard> ();
					}

					foreach (var card in eventSearchResponse.SearchModel.Results.Where (c => c.OwnerId.HasValue).ToList ()) {
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
								try {
									await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (r => {
										status.Count++;
										if (OnEventCardsUpdated != null) {
											OnEventCardsUpdated (status);
										}
									});
								} catch (Exception) {

								}
							} else {
								status.Count++;
								if (OnEventCardsUpdated != null) {
									OnEventCardsUpdated (status);
								}
							}
							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString ().ToLowerInvariant () != Resources.EMPTY_CARD_ID) {
								try {
									await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
								} catch (Exception) {

								}
							}

						}
					}

					EventCards [tag.Text] = cards;

					var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject (EventCards [tag.Text]);

					Utils.SaveResponse (savedResult, fileName);

					EventCardsLoading [tag.Text] = false;
					EventCardsLoaded [tag.Text] = true;

					if (EventCardsLoadedEventTable [tag.Text] != null) {
						EventCardsLoadedEventTable [tag.Text] (tag, EventCards [tag.Text]);
					}
				});
			} catch (Exception ex) {
				EventCardsLoading [tag.Text] = false;
				EventCardsLoaded [tag.Text] = true;
				if (EventCardsLoadedEventTable [tag.Text] != null && EventCards.ContainsKey (tag.Text)) {
					EventCardsLoadedEventTable [tag.Text] (tag, EventCards [tag.Text]);
				}
				Xamarin.Insights.Report (new Exception ("Error loading event cards", ex));
			} finally {
				semaphore.Release ();
			}
			return true;
		}

		static async Task<bool> loadEventList ()
		{

			if (EventListLoading) {
				return false;
			}

			EventListLoading = true;

			var semaphore = locks.GetOrAdd (Resources.EVENT_LIST_FILE, new SemaphoreSlim (1, 1));
			await semaphore.WaitAsync ();

			try {

				await searchController.GetEventTags (AuthToken).ContinueWith (async r => {

					EventList.Clear ();
					if (r.Result == null || string.IsNullOrEmpty (r.Result)) {
						var fullFileName = Path.Combine (Resources.DocumentsPath, Resources.EVENT_LIST_FILE);
						EventList.AddRange (Utils.GetCachedResult<List<EventTag>> (fullFileName));
					} else {
						var eventListResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (r.Result);
						EventList.AddRange (eventListResponse.Model);
					}
					EventCardsLoadedEventTable = new Dictionary<string, OnEventCardsLoadedEventHandler> ();
					foreach (var e in EventList) {
						if (!EventCardsLoadedEventTable.ContainsKey (e.Text)) {
							EventCardsLoadedEventTable.Add (e.Text, null);
						}
					}
					var savedEvents = Newtonsoft.Json.JsonConvert.SerializeObject (EventList);

					Utils.SaveResponse (savedEvents, Resources.EVENT_LIST_FILE);

					EventListLoading = false;
					EventListLoaded = true;

					if (OnEventListLoaded != null) {
						OnEventListLoaded (EventList);
					}
				});
			} catch (Exception ex) {

				EventListLoading = false;
				EventListLoaded = true;
				OnEventListLoaded (EventList);
				Xamarin.Insights.Report (new Exception ("Error loading event list", ex));

				try {
					if (EventList.Count == 0) {
						EventList = loadData<List<EventTag>> (Path.Combine (Resources.DocumentsPath, Resources.EVENT_LIST_FILE));
					}
				} catch (Exception innerEx) {
					Xamarin.Insights.Report (new Exception ("Error loading event list from file", innerEx));
				}
				if (OnEventListLoaded != null) {
					OnEventListLoaded (EventList);
				}
				return false;
			} finally {
				semaphore.Release ();
			}
			return true;
		}

		static async Task<bool> loadOrganizations ()
		{

			if (OrganizationsLoading) {
				return false;
			}

			OrganizationsLoading = true;

			var semaphore = locks.GetOrAdd (Resources.MY_ORGANIZATIONS_FILE, new SemaphoreSlim (1, 1));
			await semaphore.WaitAsync ();

			try {

				await organizationController.GetMyOrganizations (AuthToken).ContinueWith (async r => {
					if (r.Result != null) {
						OrganizationResponse myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (r.Result);

						if (myOrganizationsResponse != null && myOrganizationsResponse.Model != null) {

							// Buid the Organization List
							OrganizationList.Clear ();
							OrganizationList.AddRange (myOrganizationsResponse.Model);

							var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject (OrganizationList);
							Utils.SaveResponse (savedResult, Resources.MY_ORGANIZATIONS_FILE);

							var status = new ProgressStatus ();
							status.Total = myOrganizationsResponse.Model.Count;

							OrganizationMembersLoadedEventTable = new Dictionary<long, OnMyOrganizationMembersLoadedEventHandler> ();
							OrganizationReferralsLoadedEventTable = new Dictionary<long, OnMyOrganizationReferralsLoadedEventHandler> ();

							foreach (Organization org in myOrganizationsResponse.Model) {
								if(!OrganizationMembersLoadedEventTable.ContainsKey(org.OrganizationId)){
									OrganizationMembersLoadedEventTable.Add (org.OrganizationId, null);	
								}
								if (!OrganizationReferralsLoadedEventTable.ContainsKey (org.OrganizationId)) { 
									OrganizationReferralsLoadedEventTable.Add (org.OrganizationId, null);
								}
							}

							// Get Organization members and referals
							foreach (Organization org in myOrganizationsResponse.Model) {

								var fileName = org.LogoFileName + "." + org.LogoType;
								var fImagePath = Resources.CARD_PATH + fileName;
								if (!File.Exists (Resources.DocumentsPath + "/" + fileName)) {
									try {
										await Utils.DownloadImage (fImagePath, Resources.DocumentsPath, fileName).ContinueWith (result => {
											status.Count++;
											if (OnMyOrganizationsUpdated != null) {
												OnMyOrganizationsUpdated (status);
											}
										});
									} catch (Exception) {

									}
								} else {
									status.Count++;
									if (OnMyOrganizationsUpdated != null) {
										OnMyOrganizationsUpdated (status);
									}
								}

								// load organization members and referrals
								await LoadOrganizationMembers (org.OrganizationId);
								await LoadOrganizationReferrals (org.OrganizationId);
							}
						}

						OrganizationsLoaded = true;
						OrganizationsLoading = false;
						if (OnMyOrganizationsLoaded != null) {
							OnMyOrganizationsLoaded (OrganizationList);
						}
					}
				});
			} catch (Exception ex) {
				OrganizationsLoaded = true;
				OrganizationsLoading = false;
				Xamarin.Insights.Report (new Exception ("Error loading organization list", ex));

				if (OrganizationList.Count == 0) {
					try {
						OrganizationList = loadData<List<Organization>> (Path.Combine (Resources.DocumentsPath, Resources.MY_ORGANIZATIONS_FILE));
					} catch (Exception innerEx) {
						Xamarin.Insights.Report (new Exception ("Error loading organization list from file", innerEx));
					}
					if (OnMyOrganizationsLoaded != null) {
						OnMyOrganizationsLoaded (OrganizationList);
					}
				}
			} finally {
				semaphore.Release ();
			}
			return true;
		}

		public static async Task<bool> LoadOrganizationReferrals (long organizationId)
		{

			if (OrganizationReferralsLoading.ContainsKey (organizationId) && OrganizationReferralsLoading [organizationId]) {
				return false;
			}

			if (!OrganizationReferralsLoading.ContainsKey (organizationId)) {
				OrganizationReferralsLoading.Add (organizationId, true);
			} else {
				OrganizationReferralsLoading [organizationId] = true;
			}
			if (!OrganizationReferralsLoaded.ContainsKey (organizationId)) {
				OrganizationReferralsLoaded.Add (organizationId, false);
			} else {
				OrganizationReferralsLoaded [organizationId] = false;
			}

			var fileName = string.Format (Resources.ORGANIZATION_REFERRALS_FILE, organizationId);
			var semaphore = locks.GetOrAdd (fileName, new SemaphoreSlim (1, 1));
			await semaphore.WaitAsync ();

			try {
				await organizationController.GetOrganizationReferrals (AuthToken, organizationId).ContinueWith (async cards => {

					var orgReferralResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (cards.Result);
					if (orgReferralResponse != null) {
						Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (orgReferralResponse.Model), string.Format (Resources.ORGANIZATION_REFERRALS_FILE, organizationId));

						OrganizationReferrals = OrganizationReferrals ?? new Dictionary<long, List<UserCard>> ();
						if (!OrganizationReferrals.ContainsKey (organizationId)) {
							OrganizationReferrals.Add (organizationId, orgReferralResponse.Model.Distinct (new UserCardEqualityComparer ()).ToList ());
						} else {
							OrganizationReferrals [organizationId] = orgReferralResponse.Model.Distinct (new UserCardEqualityComparer ()).ToList ();
						}

						var status = new ProgressStatus ();
						status.Total = OrganizationReferrals [organizationId].Count;

						foreach (var card in orgReferralResponse.Model) {

							var fImageUrl = Resources.THUMBNAIL_PATH + card.Card.FrontFileName;
							var bImageUrl = Resources.THUMBNAIL_PATH + card.Card.BackFileName;
							var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName;
							var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.BackFileName;
							if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
								try {
									await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (r => {
										status.Count++;
									});
								} catch (Exception) {

								}
							} else {
								status.Count++;
							}

							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.Card.BackFileId.ToString ().ToLowerInvariant () != Resources.EMPTY_CARD_ID) {
								try {
									await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
								} catch (Exception) {

								}
							}
							if (OnMyOrganizationReferralsUpdated != null) {
								OnMyOrganizationReferralsUpdated (status);
							}
						}
					}

					OrganizationReferralsLoading [organizationId] = false;
					OrganizationReferralsLoaded [organizationId] = true;

					if (OrganizationReferralsLoadedEventTable.ContainsKey (organizationId) && OrganizationReferralsLoadedEventTable [organizationId] != null) {
						OrganizationReferralsLoadedEventTable [organizationId] (OrganizationReferrals [organizationId]);
					}
				});
			} catch (Exception ex) {
				OrganizationReferralsLoading [organizationId] = false;
				OrganizationReferralsLoaded [organizationId] = true;
				if (OrganizationReferralsLoadedEventTable != null && OrganizationReferralsLoadedEventTable.ContainsKey (organizationId) && OrganizationReferralsLoadedEventTable [organizationId] != null && OrganizationReferrals.ContainsKey (organizationId)) {
					OrganizationReferralsLoadedEventTable [organizationId] (OrganizationReferrals [organizationId]);
				}
				Xamarin.Insights.Report (new Exception ("Error Loading Organization Referrals", ex));
			} finally {
				semaphore.Release ();
			}
			return true;
		}

		public static async Task<bool> LoadOrganizationMembers (long organizationId)
		{

			if (OrganizationMembersLoading.ContainsKey (organizationId) && OrganizationMembersLoading [organizationId]) {
				return false;
			}

			if (!OrganizationMembersLoading.ContainsKey (organizationId)) {
				OrganizationMembersLoading.Add (organizationId, true);
			} else {
				OrganizationMembersLoading [organizationId] = true;
			}
			if (!OrganizationMembersLoaded.ContainsKey (organizationId)) {
				OrganizationMembersLoaded.Add (organizationId, false);
			} else {
				OrganizationMembersLoaded [organizationId] = false;
			}

			var fileName = string.Format (Resources.ORGANIZATION_MEMBERS_FILE, organizationId);
			var semaphore = locks.GetOrAdd (fileName, new SemaphoreSlim (1, 1));
			await semaphore.WaitAsync ();

			try {
				await organizationController.GetOrganizationMembers (AuthToken, organizationId).ContinueWith (async cards => {

					if (string.IsNullOrEmpty (cards.Result)) {
						OrganizationMembersLoading [organizationId] = false;
						OrganizationMembersLoaded [organizationId] = true;

						if (OrganizationMembersLoadedEventTable.ContainsKey (organizationId) && OrganizationMembersLoadedEventTable [organizationId] != null) {
							OrganizationMembersLoadedEventTable [organizationId] (OrganizationMembers [organizationId]);
						}
					} else {

						var orgMemberResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (cards.Result);

						Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (orgMemberResponse.Model), string.Format (Resources.ORGANIZATION_MEMBERS_FILE, organizationId));

						OrganizationMembers = OrganizationMembers ?? new Dictionary<long, List<Card>> ();
						if (!OrganizationMembers.ContainsKey (organizationId)) {
							OrganizationMembers.Add (organizationId, orgMemberResponse.Model);
						} else {
							OrganizationMembers [organizationId] = orgMemberResponse.Model;
						}

						var status = new ProgressStatus ();
						status.Total = OrganizationMembers [organizationId].Count;

						foreach (var card in orgMemberResponse.Model) {

							var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
							var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
							var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
							var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
							if (!File.Exists (Resources.DocumentsPath + "/" + fName)) {
								try {
									await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (r => {
										status.Count++;
									});
								} catch (Exception) {

								}
							} else {
								status.Count++;
							}

							if (!File.Exists (Resources.DocumentsPath + "/" + bName) && card.BackFileId.ToString ().ToLowerInvariant () != Resources.EMPTY_CARD_ID) {
								try {
									await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
								} catch (Exception) {

								}
							}

							if (OnMyOrganizationMembersUpdated != null) {
								OnMyOrganizationMembersUpdated (status);
							}
						}

						OrganizationMembersLoading [organizationId] = false;
						OrganizationMembersLoaded [organizationId] = true;

						if (OrganizationMembersLoadedEventTable.ContainsKey (organizationId) && OrganizationMembersLoadedEventTable [organizationId] != null) {
							OrganizationMembersLoadedEventTable [organizationId] (OrganizationMembers [organizationId]);
						}
					}
				});
			} catch (Exception ex) {
				OrganizationMembersLoading [organizationId] = false;
				OrganizationMembersLoaded [organizationId] = true;
				if (OrganizationMembersLoadedEventTable != null && OrganizationMembersLoadedEventTable.ContainsKey (organizationId) && OrganizationMembersLoadedEventTable [organizationId] != null) {
					OrganizationMembersLoadedEventTable [organizationId] (OrganizationMembers [organizationId]);
				}
				Xamarin.Insights.Report (new Exception ("Error Loading Organization Members", ex));
			} finally {
				semaphore.Release ();
			}

			return true;
		}

		static async Task<bool> loadUserCards ()
		{

			if (MyBusidexLoading) {
				return false;
			}

			MyBusidexLoading = true;

			var semaphore = locks.GetOrAdd (Resources.MY_BUSIDEX_FILE, new SemaphoreSlim (1, 1));
			await semaphore.WaitAsync ();


			var cards = new List<UserCard> ();
			var status = new ProgressStatus ();

			try {
				await myBusidexController.GetMyBusidex (AuthToken).ContinueWith (async r => {

					if (r.Result == null || string.IsNullOrEmpty (r.Result)) {
						var fullFileName = Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
						cards.AddRange (Utils.GetCachedResult<List<UserCard>> (fullFileName));
					} else {
						var myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (r.Result);
						cards.AddRange (myBusidexResponse.MyBusidex.Busidex);
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
								try {
									await Utils.DownloadImage (fImageUrl, Resources.DocumentsPath, fName).ContinueWith (t => {
										status.Count++;
										if (OnMyBusidexUpdated != null) {
											OnMyBusidexUpdated (status);
										}
									});
								} catch (Exception) {

								}
							} else {
								status.Count++;
								if (OnMyBusidexUpdated != null) {
									OnMyBusidexUpdated (status);
								}
							}

							if ((!File.Exists (Resources.DocumentsPath + "/" + bName)) && item.Card.BackFileId.ToString () != Resources.EMPTY_CARD_ID) {
								try {
									await Utils.DownloadImage (bImageUrl, Resources.DocumentsPath, bName);
								} catch (Exception) {

								}
							}

						}
					}

					UserCards.Clear ();

					// If the user has a card, make sure it's always at the top of the list
					if(OwnedCard != null){
						var ownersCard = cards.SingleOrDefault (uc => uc.Card.CardId == OwnedCard.CardId);
						if (ownersCard != null) {
							UserCards.Add (ownersCard);
							UserCards.AddRange (cards.Distinct (new UserCardEqualityComparer ()).Where (c => c.Card.CardId != OwnedCard.CardId));
						}

					}else{
						UserCards.AddRange (cards.Distinct (new UserCardEqualityComparer ()));
					}

					var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);

					Utils.SaveResponse (savedResult, Resources.MY_BUSIDEX_FILE);

					// Fire event handler
					MyBusidexLoading = false;
					MyBusidexLoaded = true;
					if (OnMyBusidexLoaded != null) {
						OnMyBusidexLoaded (UserCards);
					}

				});
			} catch (Exception ex) {
				MyBusidexLoading = false;
				MyBusidexLoaded = true;
				Xamarin.Insights.Report (new Exception ("Error Loading My Busidex", ex));

				try {
					UserCards = loadData<List<UserCard>> (Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));
				} catch (Exception innerEx) {
					Xamarin.Insights.Report (new Exception ("Error Loading My Busidex From File", innerEx));
				}
				if (OnMyBusidexLoaded != null) {
					OnMyBusidexLoaded (UserCards);
				}
			} finally {
				semaphore.Release ();
			}
			return true;
		}

		static async Task<bool> loadNotifications ()
		{
			try {
				var ctrl = new SharedCardController ();
				var sharedCardsResponse = ctrl.GetSharedCards (AuthToken);
				if (sharedCardsResponse.Contains (":404") || string.IsNullOrEmpty (sharedCardsResponse)) {
					return false;
				}

				var sharedCards = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCardResponse> (sharedCardsResponse);

				if (sharedCards.Success) {
					foreach (SharedCard card in sharedCards.SharedCards) {
						var fileName = card.Card.FrontFileName;
						var fImagePath = Resources.CARD_PATH + fileName;
						if (!File.Exists (Resources.DocumentsPath + "/" + fileName)) {
							try {
								await Utils.DownloadImage (fImagePath, Resources.DocumentsPath, fileName);
							} catch (Exception) {

							}
						}
					}

					Notifications = sharedCards.SharedCards;

					Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (Notifications), Resources.SHARED_CARDS_FILE);

				} else {
					Notifications = new List<SharedCard> ();
				}

				if (OnNotificationsLoaded != null) {
					OnNotificationsLoaded (Notifications);
				}

				if (OnNotificationCountUpdated != null) {
					OnNotificationCountUpdated (Notifications.Count);
				}

			} catch (Exception ex) {
				Xamarin.Insights.Report (new Exception ("Error Loading Notifications", ex));
			}
			return true;
		}

		#endregion

		#region Saving Card Data
		public static void SaveCardImage (MobileCardImage card)
		{
			if (OnCardInfoUpdating != null) {
				OnCardInfoUpdating ();
			}
			CardController.UpdateCardImage (card).ContinueWith (async result => {
				await loadOwnedCard ();
			});
		}

		public static void SaveCardVisibility (byte visibility)
		{
			if (OnCardInfoUpdating != null) {
				OnCardInfoUpdating ();
			}
			CardController.UpdateCardVisibility (visibility).ContinueWith (async result => {
				await loadOwnedCard ();
			});
		}

		public static void SaveCardInfo (CardDetailModel card)
		{
			if (OnCardInfoUpdating != null) {
				OnCardInfoUpdating ();
			}
			CardController.UpdateCardContactInfo (card).ContinueWith (async result => {
				await loadOwnedCard ();
			});
		}

		#endregion

		#region Private Methods
		static void ResetFlags ()
		{
			OrganizationsLoaded = false;
			MyBusidexLoaded = false;
			EventListLoaded = false;

			MyBusidexLoading = false;
			OrganizationsLoading = false;
			EventListLoading = false;

			EventCardsLoaded = new Dictionary<string, bool> ();
			OrganizationMembersLoaded = new Dictionary<long, bool> ();
			OrganizationReferralsLoaded = new Dictionary<long, bool> ();

			EventCardsLoading = new Dictionary<string, bool> ();
			OrganizationMembersLoading = new Dictionary<long, bool> ();
			OrganizationReferralsLoading = new Dictionary<long, bool> ();
		}

		static void InitDataStructures ()
		{
			EventCards = new Dictionary<string, List<UserCard>> ();
			OrganizationMembers = new Dictionary<long, List<Card>> ();
			OrganizationReferrals = new Dictionary<long, List<UserCard>> ();
			UserCards = new List<UserCard> ();
			EventList = new List<EventTag> ();
			OrganizationList = new List<Organization> ();
			Notifications = new List<SharedCard> ();
			OwnedCard = new Card ();
		}

		static List<UserCard> sortUserCards ()
		{
			var list = new List<UserCard> ();
			if(OwnedCard != null){
				var ownersCard = UserCards.SingleOrDefault (uc => uc.Card.CardId == OwnedCard.CardId);
				if(ownersCard != null){
					list.Add (ownersCard);	
				}
				list.AddRange (
					UserCards.Where(c => c.Card.CardId != OwnedCard.CardId)
								.OrderByDescending (c => c.Card != null && c.Card.OwnerId.GetValueOrDefault () > 0 ? 1 : 0)
								.ThenBy (c => c.Card != null ? c.Card.Name : "")
								.ThenBy (c => c.Card != null ? c.Card.CompanyName : "")
							  	.ToList ()
						 );
			}else{
				list.AddRange (
						UserCards.OrderByDescending (c => c.Card != null && c.Card.OwnerId.GetValueOrDefault () > 0 ? 1 : 0)
						.ThenBy (c => c.Card != null ? c.Card.Name : "")
						.ThenBy (c => c.Card != null ? c.Card.CompanyName : "")
						.ToList ()
				);
			}
			
			return list;
		}

		static string loadFromFile (string fullFilePath)
		{

			string fileJson = string.Empty;
			if (File.Exists (fullFilePath)) {
				using (var file = File.OpenText (fullFilePath)) {
					fileJson = file.ReadToEnd ();
					file.Close ();
				}
			}
			return fileJson;
		}

		static Card GetCardFromSharedCard (string token, long cardId)
		{

			try {
				var cardData = CardController.GetCardById (token, cardId);
				if (!string.IsNullOrEmpty (cardData)) {
					var response = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (cardData);
					return new Card (response.Model);
				}
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
				return null;
			}
			return null;
		}

		static async Task<Card> loadOwnedCard ()
		{

			try {

				var cardJson = await CardController.GetMyCard ();
				if(string.IsNullOrEmpty(cardJson)){
					if (OnCardInfoSaved != null) {
						OnCardInfoSaved ();
					}
					return null;
				}

				var cardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CardDetailResponse> (cardJson);
				OwnedCard = cardResponse.Success ? new Card (cardResponse.Model) : null;
				Utils.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (OwnedCard), Resources.OWNED_CARD_FILE);

				if(UserCards != null){
					foreach(var uc in UserCards){
						if(uc.Card.CardId == OwnedCard.CardId){
							OwnedCard.ExistsInMyBusidex = true;
							uc.Card = new Card (OwnedCard);
							break;
						}
					}	
				}

				if (OnCardInfoSaved != null) {
					OnCardInfoSaved ();
				}

				return OwnedCard;
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
			return null;
		}

		static BusidexUser loadUser ()
		{

			try {
				if (!string.IsNullOrEmpty (AuthToken)) {
					var accountJSON = AccountController.GetAccount (AuthToken);
					Utils.SaveResponse (accountJSON, Resources.BUSIDEX_USER_FILE);

					if (!string.IsNullOrEmpty (accountJSON)) {
						CurrentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJSON);

						if (OnBusidexUserLoaded != null) {
							OnBusidexUserLoaded (CurrentUser);
						}
					}
				}
				return CurrentUser;
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			}
			return null;
		}

		#endregion
	}
}