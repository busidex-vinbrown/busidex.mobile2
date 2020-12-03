using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Busidex3.ViewModels
{
    public class AdminVM : BaseViewModel
    {
        private List<UnownedCard> _unownedCards = new List<UnownedCard>();
        public List<UnownedCard> UnownedCards
        {
            get => _unownedCards;
            set
            {
                _unownedCards = value;
                OnPropertyChanged(nameof(UnownedCards));
            }
        }

        private List<UnownedCard> _filteredCards = new List<UnownedCard>();
        public List<UnownedCard> FilteredCards
        {
            get => _filteredCards;
            set
            {
                _filteredCards = value;
                OnPropertyChanged(nameof(FilteredCards));
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

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private decimal _loadingProgress;
        public decimal LoadingProgress
        {
            get => _loadingProgress;
            set
            {
                _loadingProgress = value;
                OnPropertyChanged(nameof(LoadingProgress));
            }
        }

        public int TotalCards { get; set; }

        public void SetFilteredList(List<UnownedCard> subset)
        {
            FilteredCards = new List<UnownedCard>(subset);
            OnPropertyChanged(nameof(FilteredCards));
        }

        public void DoSearch()
        {
            if (string.IsNullOrEmpty(SearchValue)) return;

            var subset = from uc in UnownedCards
                         where (uc.Name?.Contains(SearchValue) ?? false) ||
                               (uc.CompanyName?.Contains(SearchValue) ?? false) ||
                               (uc.Email?.Contains(SearchValue) ?? false) ||
                               (uc.Url?.Contains(SearchValue) ?? false) ||
                               (uc.PhoneNumbers?.Any(pn => !string.IsNullOrEmpty(pn.Number) && pn.Number.Contains(SearchValue)) ?? false) ||
                               (uc.Tags?.Any(t => !String.IsNullOrEmpty(t.Text) && t.Text.Contains(SearchValue)) ?? false)
                         select uc;

            var filter = new List<UnownedCard>();
            filter.AddRange(subset);
            SetFilteredList(filter);
        }


        public async Task<bool> LoadUnownedCards()
        {
            ShowFilter = false;
            LoadingProgress = 0;

            string cachedPath = Path.Combine(Serialization.LocalStorageFolder, StringResources.UNOWNED_CARDS_FILE);

            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            var status = new ProgressStatus { Count = 0 };

            try
            {
                ShowProgress = true;

                if (UnownedCards == null)
                {
                    UnownedCards = new List<UnownedCard>();
                }

                UnownedCards.Clear();

                var cards = Serialization.GetCachedResult<List<UnownedCard>>(cachedPath) ?? new List<UnownedCard>();

                if (IsRefreshing || !cards.Any()) cards = await GetCards();

                if (IsRefreshing && cards != null)
                {
                    cards.ForEach(c => c.ExistsInMyBusidex = true);
                }

                status.Total = TotalCards = cards.Count;

                await DownloadImages(cards, status);

                UnownedCards.AddRange(cards.Distinct(new UnownedCardEqualityComparer()));
                

                var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(UnownedCards);

                SaveCardsToFile(savedResult);

                await Task.Factory.StartNew(() => { SetFilteredList(UnownedCards); });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                semaphore.Release();
                IsRefreshing = ShowProgress = false;
                ShowFilter = true;
            }

            return true;
        }

        private void SaveCardsToFile(string json)
        {
            Serialization.SaveResponse(json, StringResources.UNOWNED_CARDS_FILE);
        }

        private async Task<List<UnownedCard>> GetCards()
        {
            var _adminHttpService = new AdminHttpService();
            var list = await _adminHttpService.GetUnownedCards();
            
            return list.Cards;
        }

        private async Task DownloadImages(List<UnownedCard> cards, ProgressStatus status)
        {
            var storagePath = Serialization.LocalStorageFolder;

            foreach (var item in cards)
            {
                if (item == null) continue;

                var fImageUrl = StringResources.THUMBNAIL_PATH + item.FrontFileName;
                var bImageUrl = StringResources.THUMBNAIL_PATH + item.BackFileName;
                var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + item.FrontFileName;
                var bName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + item.BackFileName;

                if (!File.Exists(storagePath + "/" + fName))
                {
                    try
                    {
                        status.Count++;
                        await Task.Factory.StartNew(() => { LoadingProgress = GetLoadingProgress(status.Count); });

                        if (item.FrontFileId != Guid.Empty && !string.IsNullOrEmpty(item.FrontFileName) && item.FrontFileName != StringResources.EMPTY_CARD_ID)
                        {
                            await App.DownloadImage(fImageUrl, storagePath, fName).ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else
                {
                    status.Count++;
                    await Task.Factory.StartNew(() => { LoadingProgress = GetLoadingProgress(status.Count); });
                }

                if ((File.Exists(storagePath + "/" + bName)) ||
                    item.BackFileId == null ||
                    item.BackFileId == Guid.Empty ||
                    item.BackFileId.ToString() == StringResources.EMPTY_CARD_ID) continue;

                try
                {
                    await App.DownloadImage(bImageUrl, storagePath, bName);
                }
                catch
                {
                    // ignored
                }

            }
        }

        private string _progressMessage;
        public string ProgressMessage
        {
            get => _progressMessage;
            set
            {
                _progressMessage = value;
                OnPropertyChanged(nameof(ProgressMessage));
            }
        }

        private decimal GetLoadingProgress(decimal progress)
        {
            var loadProgress = Math.Round(TotalCards == 0 ? 0 : (progress / TotalCards) * 100, 1);
            ProgressMessage = $"Loading {progress} of {TotalCards}";

            return loadProgress;
        }
    }
}
