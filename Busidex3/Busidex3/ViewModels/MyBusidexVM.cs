using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;

namespace Busidex3.ViewModels
{
    public delegate void OnMyBusidexLoadedEventHandler (ObservableRangeCollection<UserCard> cards);
    public delegate void OnMyBusidexUpdatedEventHandler (ProgressStatus status);
    public delegate void OnNotesUpdatedEventHandler ();

    public class MyBusidexVM :  BaseViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
        public event OnMyBusidexUpdatedEventHandler OnMyBusidexUpdated;
        public static event OnNotesUpdatedEventHandler OnNotesUpdated;

        private readonly MyBusidexHttpService _myBusidexHttpService = new MyBusidexHttpService();
        private readonly ActivityHttpService _activityHttpService = new ActivityHttpService();
        private readonly NotesHttpService _notesHttpService = new NotesHttpService();
        private decimal _loadingProgress = 0;

        private ObservableRangeCollection<UserCard> _filteredUserCards = new ObservableRangeCollection<UserCard>();
        public ObservableRangeCollection<UserCard> FilteredUserCards
        {
            get => _filteredUserCards;
            set
            {
                _filteredUserCards = value;
                OnPropertyChanged(nameof(FilteredUserCards));
            }
        } 

        private ObservableRangeCollection<UserCard> _userCards = new ObservableRangeCollection<UserCard>();
        public ObservableRangeCollection<UserCard> UserCards
        {
            get => _userCards;
            set
            {
                _userCards = value;
                OnPropertyChanged(nameof(UserCards));
            }
        }

        public Card OwnedCard => null;

        private bool _isRefreshing;
        public bool IsRefreshing { 
            get => _isRefreshing;
            set {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private int totalCards { get;set; }

        public void SetFilteredList(ObservableRangeCollection<UserCard> subset)
        {
            FilteredUserCards.Clear();
            FilteredUserCards.AddRange(subset);
        }

        public decimal LoadingProgress
        {
            get => _loadingProgress;
            set
            {
                if (_loadingProgress.Equals(value)) return;

                _loadingProgress = value;
                OnPropertyChanged(nameof(LoadingProgress));
            }
        }
    
        

        public override async Task<bool> Init()
        {
            IsRefreshing = true;

            UserCards = Serialization.LoadData<ObservableRangeCollection<UserCard>> (Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));
            if (UserCards == null || UserCards.Count == 0) {
                return await LoadUserCards ();
            }
            SetFilteredList(UserCards);

            //OnMyBusidexLoaded?.Invoke(UserCards);
            IsRefreshing = false;
            return await Task.FromResult(true);
        }

        private decimal getLoadingProgress(decimal progress)
        {
            return Math.Round(totalCards == 0 ? 0 : (progress / totalCards) * 100, 1);                      
        }

        public async Task<bool> LoadUserCards()
        {
            IsRefreshing = true;
            LoadingProgress = 0;

            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            var cards = new List<UserCard>();
            var status = new ProgressStatus {Count = 0};

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

                status.Total = totalCards = cards.Count;
                
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
                                status.Count++;
                                await Task.Factory.StartNew(() => { LoadingProgress = getLoadingProgress(status.Count); });
                      
                                await DownloadImage(fImageUrl, Resources.DocumentsPath, fName).ConfigureAwait(false);
                                
                                //OnMyBusidexUpdated?.Invoke(status);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        else
                        {
                            status.Count++;
                            await Task.Factory.StartNew(() => { LoadingProgress = getLoadingProgress(status.Count); });
                            //OnMyBusidexUpdated?.Invoke(status);
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

                SetFilteredList(UserCards);

                // Fire event handler
                //OnMyBusidexLoaded?.Invoke(UserCards);

            }
            catch (Exception ex)
            {
                //Xamarin.Insights.Report(new Exception("Error Loading My Busidex", ex));

                try
                {
                    UserCards =
                        Serialization.LoadData<ObservableRangeCollection<UserCard>>(Path.Combine(Resources.DocumentsPath,
                            Resources.MY_BUSIDEX_FILE));
                }
                catch (Exception innerEx)
                {
                    //Xamarin.Insights.Report(new Exception("Error Loading My Busidex From File", innerEx));
                }

                //OnMyBusidexLoaded?.Invoke(UserCards);
            }
            finally
            {
                semaphore.Release();
                IsRefreshing = false;
            }

            return true;
        }

        private ObservableRangeCollection<UserCard> SortUserCards ()
        {
            var list = new ObservableRangeCollection<UserCard> ();

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
				//Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
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
				//Xamarin.Insights.Report (ex, Xamarin.Insights.Severity.Error);
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
				//Xamarin.Insights.Report (ex);
			    return false;
			}
			return true;
		}        
    }
}
