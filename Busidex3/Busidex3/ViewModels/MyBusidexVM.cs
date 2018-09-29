using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;

namespace Busidex3.ViewModels
{
    public delegate void OnMyBusidexLoadedEventHandler (List<UserCard> cards);
    public delegate void OnMyBusidexUpdatedEventHandler (ProgressStatus status);
    public delegate void OnNotesUpdatedEventHandler ();

    public class MyBusidexVM : BaseViewModel
    {
        public event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
        public event OnMyBusidexUpdatedEventHandler OnMyBusidexUpdated;
        public static event OnNotesUpdatedEventHandler OnNotesUpdated;

        private readonly MyBusidexHttpService _myBusidexHttpService = new MyBusidexHttpService();
        private readonly ActivityHttpService _activityHttpService = new ActivityHttpService();
        private readonly NotesHttpService _notesHttpService = new NotesHttpService();

        public List<UserCard> UserCards { get; private set; }
        public Card OwnedCard => null;

        public override async Task<bool> Init()
        {
            UserCards = Serialization.LoadData<List<UserCard>> (Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));
            if (UserCards == null || UserCards.Count == 0) {
                UserCards = new List<UserCard> ();
                return await LoadUserCards ();
            }

            OnMyBusidexLoaded?.Invoke(UserCards);
            return await Task.FromResult(true);
        }

        private async Task<bool> LoadUserCards()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            var cards = new List<UserCard>();
            var status = new ProgressStatus();

            try
            {
                var result = await _myBusidexHttpService.GetMyBusidex();

                if (result == null)
                {
                    var fullFileName = Path.Combine(Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
                    cards.AddRange(Serialization.GetCachedResult<List<UserCard>>(fullFileName));
                }
                else
                {
                    cards.AddRange(result.MyBusidex.Busidex);
                    cards.ForEach(c => c.ExistsInMyBusidex = true);
                }

                status.Total = cards.Count;

                foreach (var item in cards)
                {
                    if (item.Card != null)
                    {

                        var fImageUrl = Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
                        var bImageUrl = Resources.THUMBNAIL_PATH + item.Card.BackFileName;
                        var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
                        var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

                        if (!File.Exists(Resources.DocumentsPath + "/" + fName))
                        {
                            try
                            {
                                await DownloadImage(fImageUrl, Resources.DocumentsPath, fName).ContinueWith(t =>
                                {
                                    status.Count++;
                                    OnMyBusidexUpdated?.Invoke(status);
                                });
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        else
                        {
                            status.Count++;
                            OnMyBusidexUpdated?.Invoke(status);
                        }

                        if ((!File.Exists(Resources.DocumentsPath + "/" + bName)) &&
                            item.Card.BackFileId.ToString() != Resources.EMPTY_CARD_ID)
                        {
                            try
                            {
                                await DownloadImage(bImageUrl, Resources.DocumentsPath, bName);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }

                UserCards.Clear();

                // If the user has a card, make sure it's always at the top of the list
                if (OwnedCard != null)
                {
                    var ownersCard = cards.FirstOrDefault(uc => uc.Card.CardId == OwnedCard.CardId &&
                                                                (!string.IsNullOrEmpty(OwnedCard.Name) ||
                                                                 !string.IsNullOrEmpty(OwnedCard.CompanyName) ||
                                                                 OwnedCard.FrontFileId.HasValue));
                    if (ownersCard != null)
                    {
                        UserCards.Add(ownersCard);
                    }

                    UserCards.AddRange(cards.Distinct(new UserCardEqualityComparer())
                        .Where(c => c.Card.CardId != OwnedCard.CardId));
                }
                else
                {
                    UserCards.AddRange(cards.Distinct(new UserCardEqualityComparer()));
                }

                var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(UserCards);

                Serialization.SaveResponse(savedResult, Resources.MY_BUSIDEX_FILE);

                // Fire event handler
                OnMyBusidexLoaded?.Invoke(UserCards);

            }
            catch (Exception ex)
            {
                Xamarin.Insights.Report(new Exception("Error Loading My Busidex", ex));

                try
                {
                    UserCards =
                        Serialization.LoadData<List<UserCard>>(Path.Combine(Resources.DocumentsPath,
                            Resources.MY_BUSIDEX_FILE));
                }
                catch (Exception innerEx)
                {
                    Xamarin.Insights.Report(new Exception("Error Loading My Busidex From File", innerEx));
                }

                OnMyBusidexLoaded?.Invoke(UserCards);
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }

        private List<UserCard> SortUserCards ()
        {
            var list = new List<UserCard> ();

            UserCards.RemoveAll (c => c.Card == null);

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

        public async Task<bool> AddCardToMyBusidex (UserCard userCard)
		{
			try {
			    if (userCard == null) return false;

			    userCard.Card.ExistsInMyBusidex = true;

			    if (!UserCards.Any (c => c.CardId.Equals (userCard.CardId))) {
			        UserCards.Add (userCard);
			    }

			    await _myBusidexHttpService.AddToMyBusidex (userCard.Card.CardId);

			    UserCards = SortUserCards ();

			    var file = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);
			    Serialization.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

			    return await _activityHttpService.SaveActivity ((long)EventSources.Add, userCard.CardId);

			} catch (Exception ex) {
				Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
			    return false;
			}
		}

		public async Task<bool> RemoveCardFromMyBusidex (UserCard userCard)
		{
			try {
			    if (userCard == null) return false;

			    UserCards.RemoveAll (uc => uc.CardId == userCard.CardId);

			    var file = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);
			    Serialization.SaveResponse (file, Resources.MY_BUSIDEX_FILE);

			    return await _myBusidexHttpService.RemoveFromMyBusidex (userCard.Card.CardId);
			} catch (Exception ex) {
				Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
			    return false;
			}
		}

		public bool ExistsInMyBusidex (UserCard card)
		{
			return UserCards.Any (uc => uc.CardId == card.CardId);
		}

		public async Task<bool> SaveNotes (long userCardId, string notes)
		{
			try
			{
			    var result = await _notesHttpService.SaveNotes(userCardId, notes);
			    if (result != null && result.Success)
			    {
			        UserCards.Single(uc =>uc.UserCardId == userCardId).Notes = notes;
			            
			        Serialization.SaveResponse(Newtonsoft.Json.JsonConvert.SerializeObject(UserCards),
			            Resources.MY_BUSIDEX_FILE);
			    }

                OnNotesUpdated?.Invoke();

            } catch (Exception ex) {
				Xamarin.Insights.Report (ex);
			    return false;
			}
			return true;
		}
    }
}
