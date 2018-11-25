using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Microsoft.AppCenter.Crashes;

namespace Busidex3.ViewModels
{
    public class MyBusidexVM :  BaseCardListViewModel
    {
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

        private bool _showFilter;
        public bool ShowFilter { 
            get => _showFilter;
            set {
                _showFilter = value;
                OnPropertyChanged(nameof(ShowFilter));
            }
        }
       
        public void SetFilteredList(ObservableRangeCollection<UserCard> subset)
        {
            FilteredUserCards.Clear();
            FilteredUserCards.AddRange(subset);
            OnPropertyChanged(nameof(FilteredUserCards));
        }
        
        public override async Task<bool> Init()
        {
            UserCards = Serialization.LoadData<ObservableRangeCollection<UserCard>>(
                Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));
            
            if (UserCards == null || UserCards.Count == 0) {
                return await LoadUserCards ();
            }

            SetFilteredList(UserCards);

            return true;
        }

        public async Task<bool> LoadUserCards()
        {
            IsRefreshing = true;
            ShowFilter = false;
            LoadingProgress = 0;

            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            var cards = new List<UserCard>();
            var status = new ProgressStatus {Count = 0};

            try
            {
                var result = await _myBusidexHttpService.GetMyBusidex();
                
                if (result != null)
                {
                    cards.AddRange(result.MyBusidex.Busidex);
                    cards.ForEach(c => c.ExistsInMyBusidex = true);
                }

                status.Total = TotalCards = cards.Count;

                await DownloadImages(cards, status);

                if (UserCards == null)
                {
                    UserCards = new ObservableRangeCollection<UserCard>();
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

                Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);

                await Task.Factory.StartNew(() => { SetFilteredList(UserCards); });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                semaphore.Release();
                IsRefreshing = false;
                ShowFilter = true;
            }

            return true;
        }
       
    }
}
