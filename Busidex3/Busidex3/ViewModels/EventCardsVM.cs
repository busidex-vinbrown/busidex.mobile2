using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Microsoft.AppCenter.Crashes;

namespace Busidex3.ViewModels
{
    public delegate void OnEventListLoadedEventHandler (List<EventTag> tags);
    public delegate void OnEventCardsLoadedEventHandler (EventTag tag, List<UserCard> cards);
    public delegate void OnEventCardsUpdatedEventHandler (ProgressStatus status);

    public class EventCardsVM : CardListVM
    {
        public event OnEventListLoadedEventHandler OnEventListLoaded;
        public Dictionary<string, OnEventCardsLoadedEventHandler> EventCardsLoadedEventTable;
        public event OnEventCardsUpdatedEventHandler OnEventCardsUpdated;

        public List<EventTag> EventList { get; set; }
        public Dictionary<string, List<UserCard>> EventCards { get; set; }

        private readonly SearchHttpService _searchHttpService;

        private string EventCardsFile { get; set; }

        public EventCardsVM(EventTag e)
        {
            _searchHttpService = new SearchHttpService();
            EventTag = e;
            EventCardsFile = string.Format(StringResources.EVENT_CARDS_FILE, EventTag.EventTagId);
        }

        private EventTag _eventTag;
        public EventTag EventTag
        {
            get => _eventTag;
            set
            {
                _eventTag = value;
                OnPropertyChanged(nameof(EventTag));
            }
        }
            
        public override async Task<bool> Init()
        {
            var loaded = false;

            EventList = Serialization.LoadData<List<EventTag>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.EVENT_LIST_FILE));
            if (EventList == null || EventList.Count == 0)
            {
                EventList = new List<EventTag>();
                loaded = await LoadUserCards();
            }

            OnEventListLoaded?.Invoke(EventList);

            return await Task.FromResult(loaded);
        }

        public override void SaveCardsToFile(string json)
        {
            Serialization.SaveResponse(json, EventCardsFile);
        }

        public override async Task<List<UserCard>> GetCards()
        {
            var result = await _searchHttpService.SearchBySystemTag(EventTag.Text);
            var list = new List<UserCard>();
            result.SearchModel.Results.ForEach(c =>
            {
                list.Add(new UserCard(c)
                {
                    ExistsInMyBusidex = c.ExistsInMyBusidex,
                    Card = c,
                    CardId = c.CardId
                });
            });
            return list;
        }
    }
}
