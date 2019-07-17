using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public abstract class CardListVM : BaseCardListViewModel
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

        private bool _hasCards;
        public bool HasCards
        {
            get => _hasCards;
            set
            {
                _hasCards = value;
                OnPropertyChanged(nameof(HasCards));
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

            if (UserCards == null || UserCards.Count == 0)
            {
                return await LoadUserCards(cachedPath);
            }
            else
            {
                HasCards = true;
                IsEmpty = false;
            }

            return true;
        }

        public void SetFilteredList(ObservableRangeCollection<UserCard> subset)
        {
            FilteredUserCards.Clear();
            FilteredUserCards.AddRange(subset);
            OnPropertyChanged(nameof(FilteredUserCards));
        }

        public void DoSearch()
        {
            if (string.IsNullOrEmpty(SearchValue)) return;

            var subset = from uc in UserCards
                         where (uc.Card.Name?.Contains(SearchValue) ?? false) ||
                               (uc.Card.CompanyName?.Contains(SearchValue) ?? false) ||
                               (uc.Card.Email?.Contains(SearchValue) ?? false) ||
                               (uc.Card.Url?.Contains(SearchValue) ?? false) ||
                               (uc.Card.PhoneNumbers?.Any(pn => !string.IsNullOrEmpty(pn.Number) && pn.Number.Contains(SearchValue)) ?? false) ||
                               (uc.Card.Tags?.Any(t => !String.IsNullOrEmpty(t.Text) && t.Text.Contains(SearchValue)) ?? false)
                         select uc;

            var filter = new ObservableRangeCollection<UserCard>();
            filter.AddRange(subset);
            SetFilteredList(filter);
        }

        public async Task<bool> LoadUserCards(string cachedPath)
        {
            ShowFilter = false;
            LoadingProgress = 0;

            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            var status = new ProgressStatus { Count = 0 };

            try
            {
                ShowProgress = true;

                if (UserCards == null)
                {
                    UserCards = new ObservableRangeCollection<UserCard>();
                }

                UserCards.Clear();

                var cards = Serialization.GetCachedResult<List<UserCard>>(cachedPath);

                if (IsRefreshing || !cards.Any()) cards = await GetCards();

                if (IsRefreshing && cards != null)
                {
                    cards.ForEach(c => c.ExistsInMyBusidex = true);
                }

                //if (!IsRefreshing)
                //{
                //    UserCards.AddRange(cards);
                //    SetFilteredList(UserCards);
                //    IsRefreshing = false;
                //    HasCards = UserCards.Count > 0;
                //    IsEmpty = !HasCards;
                //    return true;
                //}

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
                IsRefreshing = false;
                ShowProgress = false;
                ShowFilter = HasCards;
            }

            return true;
        }

        public async virtual Task<List<UserCard>> GetCards() { return await Task.FromResult(new List<UserCard>()); }

        public virtual void SaveCardsToFile(string json) { }
    }
}
