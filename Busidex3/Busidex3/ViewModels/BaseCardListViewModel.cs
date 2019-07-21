using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;

namespace Busidex3.ViewModels
{
    public class BaseCardListViewModel : BaseViewModel
    {
        protected readonly MyBusidexHttpService _myBusidexHttpService = new MyBusidexHttpService();
        //private readonly ActivityHttpService _activityHttpService = new ActivityHttpService();

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

        public UserCard SelectedCard
        {
            get => null;
            set
            {
                SelectedCard = value;
                OnPropertyChanged(nameof(SelectedCard));
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing { 
            get => _isRefreshing;
            set {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
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

        protected int TotalCards { get; set; }


        public Card OwnedCard => null;

        protected decimal GetLoadingProgress(decimal progress)
        {
            var loadProgress = Math.Round(TotalCards == 0 ? 0 : (progress / TotalCards) * 100, 1);
            ProgressMessage = $"Loading {progress} of {TotalCards}";

            return loadProgress;
        }

        protected async Task DownloadImages(List<UserCard> cards, ProgressStatus status)
        {
            var storagePath = Serialization.LocalStorageFolder;

            foreach (var item in cards)
            {
                if (item.Card == null) continue;

                item.Card.Parent = item;

                var fImageUrl = StringResources.THUMBNAIL_PATH + item.Card.FrontFileName;
                var bImageUrl = StringResources.THUMBNAIL_PATH + item.Card.BackFileName;
                var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
                var bName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

                if (!File.Exists(storagePath + "/" + fName))
                {
                    try
                    {
                        status.Count++;
                        await Task.Factory.StartNew(() => { LoadingProgress = GetLoadingProgress(status.Count); });

                        if(item.Card.FrontFileId != Guid.Empty && !string.IsNullOrEmpty(item.Card.FrontFileName) && item.Card.FrontFileName != StringResources.EMPTY_CARD_ID)
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
                    item.Card.BackFileId == null ||
                    item.Card.BackFileId == Guid.Empty ||
                    item.Card.BackFileId.ToString() == StringResources.EMPTY_CARD_ID) continue;

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

        //public async Task<bool> AddCardToMyBusidex (UserCard userCard)
        //{
        //    try {
        //        if (userCard == null) return false;

        //        userCard.Card.ExistsInMyBusidex = true;

        //        if (!UserCards.Any (c => c.CardId.Equals (userCard.CardId))) {
        //            UserCards.Add (userCard);
        //        }

        //        await _myBusidexHttpService.AddToMyBusidex (userCard.Card.CardId);

        //        UserCards = SortUserCards ();

        //        var file = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);
        //        Serialization.SaveResponse (file, StringResources.MY_BUSIDEX_FILE);

        //        App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardAdded, userCard.Card.Name ?? userCard.Card.CompanyName);

        //        return await _activityHttpService.SaveActivity ((long)EventSources.Add, userCard.CardId);

        //    } catch (Exception ex) {
        //        Crashes.TrackError(ex);
        //        return false;
        //    }
        //}

        //public async Task<bool> RemoveCardFromMyBusidex (UserCard userCard)
        //{
        //    try {
        //        if (userCard == null) return false;

        //        UserCards.RemoveAll (uc => uc.CardId == userCard.CardId);

        //        var file = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);
        //        Serialization.SaveResponse (file, StringResources.MY_BUSIDEX_FILE);

        //        App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardRemoved, userCard.Card.Name ?? userCard.Card.CompanyName);

        //        return await _myBusidexHttpService.RemoveFromMyBusidex (userCard.Card.CardId);
        //    } catch (Exception ex) {
        //        Crashes.TrackError(ex);
        //        return false;
        //    }
        //}

        //private ObservableRangeCollection<UserCard> SortUserCards ()
        //{
        //    var list = new ObservableRangeCollection<UserCard> ();

        //    UserCards.RemoveAll (c => c.Card == null);

        //    if(OwnedCard != null){
        //        var ownersCard = UserCards.SingleOrDefault (uc => uc.Card.CardId == OwnedCard.CardId);
        //        if(ownersCard != null){
        //            list.Add (ownersCard);	
        //        }
        //        list.AddRange (
        //            UserCards.Where(c => c.Card.CardId != OwnedCard.CardId)
        //                .OrderByDescending (c => c.Card != null && c.Card.OwnerId.GetValueOrDefault () > 0 ? 1 : 0)
        //                .ThenBy (c => c.Card != null ? c.Card.Name : "")
        //                .ThenBy (c => c.Card != null ? c.Card.CompanyName : "")
        //                .ToList ()
        //        );
        //    }else{
        //        list.AddRange (
        //            UserCards.OrderByDescending (c => c.Card != null && c.Card.OwnerId.GetValueOrDefault () > 0 ? 1 : 0)
        //                .ThenBy (c => c.Card != null ? c.Card.Name : "")
        //                .ThenBy (c => c.Card != null ? c.Card.CompanyName : "")
        //                .ToList ()
        //        );
        //    }
			
        //    return list;
        //}
    }
}
