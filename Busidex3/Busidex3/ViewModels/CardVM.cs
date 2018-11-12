using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public delegate void OnCardInfoUpdatingHandler ();
    public delegate void OnCardInfoSavedHandler ();

    public class CardVM : BaseViewModel
    {
        public static event OnCardInfoUpdatingHandler OnCardInfoUpdating;
        public static event OnCardInfoSavedHandler OnCardInfoSaved;

        private readonly CardHttpService _cardHttpService = new CardHttpService();

        public UserCard SelectedCard { get; }

        public CardVM(UserCard uc)
        {
            SelectedCard = uc;
        }

        public void LaunchMapApp() {
            // Windows Phone doesn't like ampersands in the names and the normal URI escaping doesn't help
            var addr = SelectedCard.Card.Addresses.FirstOrDefault()?.ToString();
            var request = string.Empty;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                {
                    request = $"geo:0,0?q={addr}";
                    break;
                }
                case Device.iOS:
                {
                    addr = Uri.EscapeUriString(addr);
                    request = $"http://maps.apple.com/maps?q={addr}";
                    break;
                }
                case Device.UWP:
                {
                    request = $"bingmaps:?cp={addr}";
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Device.OpenUri(new Uri(request));
        }

        public void LaunchEmail()
        {
            Device.OpenUri(new Uri($"mailto:{SelectedCard.Card.Email}"));
        }

        public void LaunchBrowser()
        {
            var url = string.Empty;
            if (string.IsNullOrEmpty(SelectedCard.Card.Url)) return;

            url = !SelectedCard.Card.Url.StartsWith ("http", StringComparison.Ordinal) 
                ? "http://" + SelectedCard.Card.Url 
                : SelectedCard.Card.Url;
            Device.OpenUri(new Uri(url));
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

                Serialization.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (card), StringResources.OWNED_CARD_FILE);

                var myBusidex = Serialization.LoadData<List<UserCard>> (Path.Combine (StringResources.DocumentsPath, StringResources.MY_BUSIDEX_FILE));

                if(myBusidex != null){
                    foreach(var uc in myBusidex){
                        if (card == null || uc.Card == null || uc.Card.CardId != card.CardId) continue;
                        
                        card.ExistsInMyBusidex = true;
                        uc.Card = new Card (card);
                        break;
                    }
                    
                    var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidex);

                    Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);
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
