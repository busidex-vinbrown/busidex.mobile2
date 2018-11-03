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
    public delegate void OnEventListLoadedEventHandler (List<EventTag> tags);
    //public delegate void OnEventListUpdatedEventHandler (ProgressStatus status);
    public delegate void OnEventCardsLoadedEventHandler (EventTag tag, List<UserCard> cards);
    public delegate void OnEventCardsUpdatedEventHandler (ProgressStatus status);
    
    public class EventsVM : BaseViewModel
    {
        public event OnEventListLoadedEventHandler OnEventListLoaded;
        //public event OnEventListUpdatedEventHandler OnEventListUpdated;
        public Dictionary<string, OnEventCardsLoadedEventHandler> EventCardsLoadedEventTable;
        public event OnEventCardsUpdatedEventHandler OnEventCardsUpdated;

        public List<EventTag> EventList { get; set; }
        public Dictionary<string, List<UserCard>> EventCards { get; set; }

        private readonly SearchHttpService _searchHttpService;

        public EventsVM()
        {
            _searchHttpService = new SearchHttpService();
        }

        public override async Task<bool> Init()
        {
            var loaded = false;

            EventList = Serialization.LoadData<List<EventTag>>(Path.Combine(Resources.DocumentsPath, Resources.EVENT_LIST_FILE));
            if (EventList == null || EventList.Count == 0)
            {
                EventList = new List<EventTag>();
                loaded = await LoadEventList();
            }

            OnEventListLoaded?.Invoke(EventList);

            EventCardsLoadedEventTable = new Dictionary<string, OnEventCardsLoadedEventHandler>();

            foreach (var ev in EventList)
            {
                if (!EventCardsLoadedEventTable.ContainsKey(ev.Text))
                {
                    EventCardsLoadedEventTable.Add(ev.Text, null);
                }

                if (!EventCards.ContainsKey(ev.Text))
                {
                    EventCards.Add(ev.Text, new List<UserCard>());
                }

                EventCards[ev.Text] = Serialization.LoadData<List<UserCard>>(Path.Combine(Resources.DocumentsPath,
                    string.Format(Resources.EVENT_CARDS_FILE, ev.EventTagId)));
                if (EventCards[ev.Text] == null)
                {
                    await LoadEventCards(ev);
                }

            }

            return await Task.FromResult(loaded);
        }

        private async Task<bool> LoadEventCards(EventTag tag)
        {

            var fileName = string.Format(Resources.EVENT_CARDS_FILE, tag.EventTagId);
            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            try
            {
                var result = await _searchHttpService.SearchBySystemTag(tag.Text);

                var cards = new List<UserCard>();

                var status = new ProgressStatus
                {
                    Total = result.SearchModel.Results.Count
                };

                if (!EventCards.ContainsKey(tag.Text))
                {
                    EventCards.Add(tag.Text, new List<UserCard>());
                }
                else
                {
                    EventCards[tag.Text] = new List<UserCard>();
                }

                foreach (var card in result.SearchModel.Results.Where(c => c.OwnerId.HasValue).ToList())
                {
                    if (card == null) continue;

                    var userCard = new UserCard(card)
                    {
                        ExistsInMyBusidex = card.ExistsInMyBusidex,
                        Card = card,
                        CardId = card.CardId
                    };

                    cards.Add(userCard);

                    var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
                    var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
                    var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
                    var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
                    if (!File.Exists(Resources.DocumentsPath + "/" + fName))
                    {
                        try
                        {
                            await DownloadImage(fImageUrl, Resources.DocumentsPath, fName).ContinueWith(r =>
                            {
                                status.Count++;
                                OnEventCardsUpdated?.Invoke(status);
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
                        OnEventCardsUpdated?.Invoke(status);
                    }

                    if (!File.Exists(Resources.DocumentsPath + "/" + bName) &&
                        card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID)
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

                EventCards[tag.Text] = cards;

                var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(EventCards[tag.Text]);

                Serialization.SaveResponse(savedResult, fileName);

                EventCardsLoadedEventTable[tag.Text]?.Invoke(tag, EventCards[tag.Text]);

            }
            catch (Exception ex)
            {
                if (EventCardsLoadedEventTable[tag.Text] != null && EventCards.ContainsKey(tag.Text))
                {
                    EventCardsLoadedEventTable[tag.Text](tag, EventCards[tag.Text]);
                }

                //Xamarin.Insights.Report(new Exception("Error loading event cards", ex));
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }

        private async Task<bool> LoadEventList()
        {

            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            try
            {
                var result = await _searchHttpService.GetEventTags();

                EventList.Clear();
                if (result == null)
                {
                    var fullFileName = Path.Combine(Resources.DocumentsPath, Resources.EVENT_LIST_FILE);
                    EventList.AddRange(Serialization.GetCachedResult<List<EventTag>>(fullFileName));
                }
                else
                {
                    EventList.AddRange(result.Model);
                }

                EventCardsLoadedEventTable = new Dictionary<string, OnEventCardsLoadedEventHandler>();
                foreach (var e in EventList)
                {
                    if (!EventCardsLoadedEventTable.ContainsKey(e.Text))
                    {
                        EventCardsLoadedEventTable.Add(e.Text, null);
                    }
                }

                var savedEvents = Newtonsoft.Json.JsonConvert.SerializeObject(EventList);

                Serialization.SaveResponse(savedEvents, Resources.EVENT_LIST_FILE);

                OnEventListLoaded?.Invoke(EventList);
            }
            catch (Exception ex)
            {

                //Xamarin.Insights.Report(new Exception("Error loading event list", ex));

                try
                {
                    if (EventList.Count == 0)
                    {
                        EventList = Serialization.LoadData<List<EventTag>>(Path.Combine(Resources.DocumentsPath,
                            Resources.EVENT_LIST_FILE));
                    }
                }
                catch (Exception innerEx)
                {
                    //Xamarin.Insights.Report(new Exception("Error loading event list from file", innerEx));
                }

                OnEventListLoaded?.Invoke(EventList);
                return false;
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }
    }
}
