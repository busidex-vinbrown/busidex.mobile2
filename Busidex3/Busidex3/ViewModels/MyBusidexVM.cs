﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Microsoft.AppCenter.Crashes;

namespace Busidex3.ViewModels
{
    public class MyBusidexVM :  BaseViewModel
    {

        private readonly MyBusidexHttpService _myBusidexHttpService = new MyBusidexHttpService();
        private readonly ActivityHttpService _activityHttpService = new ActivityHttpService();
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

        private int TotalCards { get;set; }

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

            UserCards = Serialization.LoadData<ObservableRangeCollection<UserCard>> (Path.Combine (StringResources.DocumentsPath, StringResources.MY_BUSIDEX_FILE));
            if (UserCards == null || UserCards.Count == 0) {
                return await LoadUserCards ();
            }
            SetFilteredList(UserCards);

            IsRefreshing = false;
            return await Task.FromResult(true);
        }

        private decimal GetLoadingProgress(decimal progress)
        {
            return Math.Round(TotalCards == 0 ? 0 : (progress / TotalCards) * 100, 1);                      
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
                    var fullFileName = Path.Combine(StringResources.DocumentsPath, StringResources.MY_BUSIDEX_FILE);
                    cards.AddRange(Serialization.GetCachedResult<List<UserCard>>(fullFileName));
                }
                else
                {
                    cards.AddRange(result.MyBusidex.Busidex);
                    cards.ForEach(c => c.ExistsInMyBusidex = true);
                }

                status.Total = TotalCards = cards.Count;
                
                foreach (var item in cards)
                {                
                    if (item.Card != null)
                    {
                        var fImageUrl = StringResources.THUMBNAIL_PATH + item.Card.FrontFileName;
                        var bImageUrl = StringResources.THUMBNAIL_PATH + item.Card.BackFileName;
                        var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
                        var bName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

                        if (!File.Exists(StringResources.DocumentsPath + "/" + fName))
                        {
                            try
                            {
                                status.Count++;
                                await Task.Factory.StartNew(() => { LoadingProgress = GetLoadingProgress(status.Count); });
                      
                                await DownloadImage(fImageUrl, StringResources.DocumentsPath, fName).ConfigureAwait(false);
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

                        if ((!File.Exists(StringResources.DocumentsPath + "/" + bName)) &&
                            item.Card.BackFileId.ToString() != StringResources.EMPTY_CARD_ID)
                        {
                            try
                            {
                                await DownloadImage(bImageUrl, StringResources.DocumentsPath, bName);
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

                Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);

                SetFilteredList(UserCards);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

                try
                {
                    UserCards =
                        Serialization.LoadData<ObservableRangeCollection<UserCard>>(Path.Combine(StringResources.DocumentsPath,
                            StringResources.MY_BUSIDEX_FILE));
                }
                catch (Exception innerEx)
                {
                    Crashes.TrackError(innerEx);
                }
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
			    Serialization.SaveResponse (file, StringResources.MY_BUSIDEX_FILE);

			    App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardAdded, userCard.Card.Name ?? userCard.Card.CompanyName);

			    return await _activityHttpService.SaveActivity ((long)EventSources.Add, userCard.CardId);

			} catch (Exception ex) {
			    Crashes.TrackError(ex);
			    return false;
			}
		}

		public async Task<bool> RemoveCardFromMyBusidex (UserCard userCard)
		{
			try {
			    if (userCard == null) return false;

			    UserCards.RemoveAll (uc => uc.CardId == userCard.CardId);

			    var file = Newtonsoft.Json.JsonConvert.SerializeObject (UserCards);
			    Serialization.SaveResponse (file, StringResources.MY_BUSIDEX_FILE);

			    App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardRemoved, userCard.Card.Name ?? userCard.Card.CompanyName);

			    return await _myBusidexHttpService.RemoveFromMyBusidex (userCard.Card.CardId);
			} catch (Exception ex) {
			    Crashes.TrackError(ex);
			    return false;
			}
		}

		public bool ExistsInMyBusidex (UserCard card)
		{
			return UserCards.Any (uc => uc.CardId == card.CardId);
		}        
    }
}
