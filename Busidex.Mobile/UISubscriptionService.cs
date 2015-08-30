using System;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Busidex.Mobile
{
	public delegate void OnMyBusidexLoadedEventHandler();
	public delegate void OnMyOrganizationsLoadedEventHandler();
	public delegate void OnEventListLoadedEventHandler();

	public class UISubscriptionService
	{

		public event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
		public event OnMyOrganizationsLoadedEventHandler OnMyOrganizationsLoaded;
		public event OnEventListLoadedEventHandler OnEventListLoaded;

		public UISubscriptionService(){
			
			EventCards = EventCards ?? new Dictionary<string, List<UserCard>> ();
			OrganizationMembers = OrganizationMembers ?? new Dictionary<long, List<Card>> ();
			OrganizationReferrals = OrganizationReferrals ?? new Dictionary<long, List<UserCard>> ();

			myBusidexController = new MyBusidexController ();
			organizationController = new OrganizationController ();
			searchController = new SearchController ();

			UserCards = loadDataFromFile<List<UserCard>>(Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE)) ?? new List<UserCard>();
			EventList = loadDataFromFile<List<EventTag>>(Path.Combine (Resources.DocumentsPath, Resources.EVENT_LIST_FILE)) ?? new List<EventTag> ();
			OrganizationList = loadDataFromFile<List<Organization>>(Path.Combine (Resources.DocumentsPath, Resources.MY_ORGANIZATIONS_FILE)) ?? new List<Organization> ();
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

		T loadDataFromFile<T>(string path){

			try{
				var jsonData = loadFromFile (path);
				return Newtonsoft.Json.JsonConvert.DeserializeObject<T> (jsonData);
			}catch(Exception e){
				return default(T);
			}
		}

		public async void reset(string token){

			await loadUserCards (token).ContinueWith(r=>{
				if(OnMyBusidexLoaded != null){
					OnMyBusidexLoaded();
				}
			});	
			await loadOrganizations (token).ContinueWith(r=>{
				if(OnMyOrganizationsLoaded != null){
					OnMyOrganizationsLoaded();
				}
			});
			await loadEventList (token).ContinueWith(r=>{
				if(OnEventListLoaded != null){
					OnEventListLoaded();
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

		async Task<bool> loadEventList(string userToken){

			EventList.Clear ();

			await searchController.GetEventTags (userToken).ContinueWith(r => {
				if (!string.IsNullOrEmpty (r.Result)) {

					Utils.SaveResponse (r.Result, Resources.EVENT_LIST_FILE);

					var eventListResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventListResponse> (r.Result);
					EventList = eventListResponse.Model;

				}
			});
			return true;
		}

		async Task<bool> loadOrganizations(string userToken){

			OrganizationList.Clear ();

			await organizationController.GetMyOrganizations (userToken).ContinueWith (r => {

				Utils.SaveResponse(r.Result, Resources.MY_ORGANIZATIONS_FILE);

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
					organizationController.GetOrganizationMembers(userToken, org.OrganizationId).ContinueWith(async cards =>{

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

					organizationController.GetOrganizationReferrals(userToken, org.OrganizationId).ContinueWith(async cards =>{

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
			});
			return true;
		}

		async Task<bool> loadUserCards(string userToken){

			var cards = new List<UserCard> ();

			await myBusidexController.GetMyBusidex (userToken).ContinueWith (r => {

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

				var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(UserCards);

				Utils.SaveResponse (savedResult, Resources.MY_BUSIDEX_FILE);

				UserCards.Clear();
				UserCards.AddRange(cards);

			});
			return true;
		}
	}
}

