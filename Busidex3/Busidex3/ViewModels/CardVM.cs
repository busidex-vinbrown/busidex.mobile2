using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;

namespace Busidex3.ViewModels
{
    public delegate void OnCardInfoUpdatingHandler ();
    public delegate void OnCardInfoSavedHandler ();

    public class CardVM : BaseViewModel
    {
        public static event OnCardInfoUpdatingHandler OnCardInfoUpdating;
        public static event OnCardInfoSavedHandler OnCardInfoSaved;

        private readonly CardHttpService _cardHttpService = new CardHttpService();

        public Card SelectedCard { get; }

        public CardVM(Card card)
        {
            SelectedCard = card;
        }

        public async Task<bool> SaveCardImage(MobileCardImage card)
        {
            OnCardInfoUpdating?.Invoke();

            var result = await _cardHttpService.UpdateCardImage(card);

            await LoadOwnedCard();

            return result;
        }

        public async Task<bool> SaveCardVisibility(byte visibility)
        {
            OnCardInfoUpdating?.Invoke();

            var result = await _cardHttpService.UpdateCardVisibility(visibility);
            await LoadOwnedCard();

            return result;
        }

        public async Task<bool> SaveCardInfo(CardDetailModel card)
        {
            OnCardInfoUpdating?.Invoke();

            var result = await _cardHttpService.UpdateCardContactInfo(card);
            await LoadOwnedCard();

            return result;
        }

        private async Task<Card> LoadOwnedCard ()
        {
            try {

                var myCardResponse = await _cardHttpService.GetMyCard ();
                if(myCardResponse == null){
                    OnCardInfoSaved?.Invoke ();
                    return null;
                }

                var card = myCardResponse.Success && myCardResponse.Model != null
                    ? new Card(myCardResponse.Model)
                    : null;

                Serialization.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (card), Resources.OWNED_CARD_FILE);

                var myBusidex = Serialization.LoadData<List<UserCard>> (Path.Combine (Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE));

                if(myBusidex != null){
                    foreach(var uc in myBusidex){
                        if (card == null || uc.Card == null || uc.Card.CardId != card.CardId) continue;
                        
                        card.ExistsInMyBusidex = true;
                        uc.Card = new Card (card);
                        break;
                    }
                    
                    var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidex);

                    Serialization.SaveResponse(savedResult, Resources.MY_BUSIDEX_FILE);
                }

                OnCardInfoSaved?.Invoke();

                return await Task.FromResult(card);

            } catch (Exception ex) {
                //Xamarin.Insights.Report (ex);
            }
            return null;
        }
    }
}
