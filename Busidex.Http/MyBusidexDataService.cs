using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Models.Dto;
using Busidex.Resources.String;
using Newtonsoft.Json;

namespace Busidex.Http
{
    public delegate void CardAddedHandler();
    public delegate void CardRemovedHandler();
    public delegate void MyBusidexLoadedHandler();

    public class MyBusidexDataService
    {
        private static List<UserCard> _myBusidex;

        private static readonly MyBusidexHttpService _myBusidexHttpService = new MyBusidexHttpService();
        private static readonly ActivityHttpService _activityHttpService = new ActivityHttpService();

        public static event CardAddedHandler OnCardAdded;
        public static event CardRemovedHandler OnCardRemoved;
        public static event MyBusidexLoadedHandler OnMyBusidexLoaded;

        public static List<UserCard> GetMyBusidex()
        {
            return _myBusidex;
        }

        public static bool Load()
        {
            _myBusidex = Serialization.LoadData<List<UserCard>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));

            OnMyBusidexLoaded?.Invoke();

            return true;
        }

        public static async Task<bool> Refresh()
        {
            var myBusidexHttpService = new MyBusidexHttpService();
            var result = await myBusidexHttpService.GetMyBusidex();
            var json = JsonConvert.SerializeObject(result.MyBusidex.Busidex);
            Serialization.SaveResponse(json, StringResources.MY_BUSIDEX_FILE);
            Serialization.SetDataRefreshDate(RefreshItem.MyBusidex);

            Load();

            return true;
        }

        public static async Task<bool> AddToMyBusidex(UserCard selectedCard)
        {
            if (_myBusidex.Any(b => b.CardId == selectedCard.CardId)) return false;

            selectedCard.ExistsInMyBusidex = selectedCard.Card.ExistsInMyBusidex = true;

            _myBusidex.Add(selectedCard);
            var newList = new List<UserCard>();
            newList.AddRange(_myBusidex
                .OrderByDescending(c => c.Card != null && c.Card.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                .ThenBy(c => c.Card != null ? c.Card.Name : "")
                .ThenBy(c => c.Card != null ? c.Card.CompanyName : "")
                .ToList());
            var savedResult = JsonConvert.SerializeObject(newList);

            Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);

            var cardId = selectedCard.CardId;
            await _myBusidexHttpService.AddToMyBusidex(cardId);

            OnCardAdded?.Invoke();

            //App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardAdded, selectedCard.Card.Name ?? selectedCard.Card.CompanyName);

            await _activityHttpService.SaveActivity((long)EventSources.Add, cardId);

            return true;
        }

        public static void Update(UserCard selectedCard)
        {
            var myBusidex = Serialization.LoadData<List<UserCard>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));
            var existingCard = myBusidex?.SingleOrDefault(b => b.CardId == selectedCard.CardId);

            if (existingCard != null)
            {
                var idx = myBusidex.IndexOf(existingCard);
                myBusidex[idx] = selectedCard;
                var file = JsonConvert.SerializeObject(myBusidex);
                Serialization.SaveResponse(file, Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));
            }

            Serialization.SaveResponse(
                JsonConvert.SerializeObject(selectedCard.Card),
                Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE)
            );

        }

        public static async Task<bool> RemoveFromMyBusidex(long cardId)
        {
            if (_myBusidex.All(b => b.CardId != cardId)) return false;

            var selectedCard = _myBusidex.Single(c => c.Card.CardId == cardId);

            _myBusidex.RemoveAll(b => b.CardId == cardId);
            var savedResult = JsonConvert.SerializeObject(_myBusidex);

            Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);

            await _myBusidexHttpService.RemoveFromMyBusidex(cardId);

            OnCardRemoved?.Invoke();

            //App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardRemoved,
            //    selectedCard.Card.Name ?? selectedCard.Card.CompanyName);

            return true;
        }
    }
}
