using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public abstract class CardListVM : BaseCardListViewModel
    {
        private List<UserCard> _filteredUserCards = new List<UserCard>();
        public List<UserCard> FilteredUserCards
        {
            get => _filteredUserCards;
            set
            {
                _filteredUserCards = value;
                OnPropertyChanged(nameof(FilteredUserCards));
            }
        }

        public ImageSource BackgroundImage
        {
            get
            {
                return ImageSource.FromResource("Busidex3.Resources.cards_back2.png",
                    typeof(SearchVM).Assembly);
            }
        }

        private bool _showFilter;
        public bool ShowFilter
        {
            get => _showFilter;
            set
            {
                _showFilter = value;
                OnPropertyChanged(nameof(ShowFilter));
            }
        }

        private bool _showProgress;
        public bool ShowProgress
        {
            get => _showProgress;
            set
            {
                _showProgress = value;
                OnPropertyChanged(nameof(ShowProgress));
            }
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        public override async Task<bool> Init(string cachedPath)
        {
            ShowFilter = true;
            IsEmpty = HasCards = false;

            if (UserCards == null || UserCards.Count == 0)
            {
                return await LoadUserCards(cachedPath);
            }

            SetFilteredList(UserCards);
            HasCards = UserCards.Count > 0;
            IsEmpty = !HasCards;

            return true;
        }

        public void LoadFromCache(string cachedPath)
        {
            UserCards = Serialization.GetCachedResult<List<UserCard>>(cachedPath) ?? new List<UserCard>();
            SetFilteredList(UserCards);
        }

        public void SetFilteredList(List<UserCard> subset)
        {
            FilteredUserCards = new List<UserCard>(subset);
        }

        public void DoSearch()
        {
            if (string.IsNullOrEmpty(SearchValue)) return;

            var subset = (from uc in UserCards
                         where (uc.Card.Name?.Contains(SearchValue) ?? false) ||
                               (uc.Card.CompanyName?.Contains(SearchValue) ?? false) ||
                               (uc.Card.Email?.Contains(SearchValue) ?? false) ||
                               (uc.Card.Url?.Contains(SearchValue) ?? false) ||
                               (uc.Card.PhoneNumbers?.Any(pn => !string.IsNullOrEmpty(pn.Number) && pn.Number.Contains(SearchValue)) ?? false) ||
                               (uc.Card.Tags?.Any(t => !String.IsNullOrEmpty(t.Text) && t.Text.Contains(SearchValue)) ?? false)
                         select uc).ToList();

            SetFilteredList(subset);
        }

        public async Task<bool> LoadUserCards(string cachedPath, bool showProgress = true)
        {
            ShowFilter = false;
            LoadingProgress = 0;
            IsEmpty = HasCards = false;

            using (var semaphore = new SemaphoreSlim(1, 1))
            {
                await semaphore.WaitAsync();

                var status = new ProgressStatus {Count = 0};

                try
                {
                    ShowProgress = showProgress;

                    if (UserCards == null)
                    {
                        UserCards = new List<UserCard>();
                    }

                    UserCards.Clear();

                    var cards = Serialization.GetCachedResult<List<UserCard>>(cachedPath) ?? new List<UserCard>();

                    if (IsRefreshing || !cards.Any()) cards = await GetCards() ?? new List<UserCard>();

                    if (IsRefreshing)
                    {
                        cards.ForEach(c => c.ExistsInMyBusidex = true);
                    }

                    status.Total = TotalCards = cards.Count;

                    await DownloadImages(cards, status);

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

                    SaveCardsToFile(savedResult);

                    HasCards = UserCards.Count > 0;
                    IsEmpty = !HasCards;

                    await Task.Factory.StartNew(() => { SetFilteredList(UserCards); });
                    
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally
                {
                    semaphore.Release();
                    IsRefreshing = ShowProgress = false;
                    ShowFilter = HasCards;
                }
            }

            return true;
        }

        public async virtual Task<List<UserCard>> GetCards() { return await Task.FromResult(new List<UserCard>()); }

        public virtual void SaveCardsToFile(string json) { }
    }
}
