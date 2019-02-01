using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Busidex3.Analytics;
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

        private readonly CardHttpService _cardHttpService = new CardHttpService();
        private readonly MyBusidexHttpService _myBusidexHttpService = new MyBusidexHttpService();
        private readonly NotesHttpService _notesHttpService = new NotesHttpService();
        private readonly ActivityHttpService _activityHttpService = new ActivityHttpService();
        private readonly ObservableRangeCollection<UserCard> _myBusidex;

        public UserCard SelectedCard { get; }
         
        public List<PhoneNumberVM> PhoneNumbers { get; set; }

        public bool ShowAddButton => !SelectedCard.ExistsInMyBusidex;
        public bool ShowRemoveButton => SelectedCard.ExistsInMyBusidex;

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                _isSaving = value;
                OnPropertyChanged(nameof(IsSaving));
            }
        }

        public CardVM(ref UserCard uc, ref ObservableRangeCollection<UserCard> myBusidex, UserCardDisplay.DisplaySetting setting = UserCardDisplay.DisplaySetting.Detail)
        {
            SelectedCard = uc;
            SelectedCard.SetDisplay(
                setting, 
                UserCardDisplay.CardSide.Front, 
                StringResources.THUMBNAIL_FILE_NAME_PREFIX + SelectedCard.Card.FrontFileName
                );
            PhoneNumbers = uc.Card.PhoneNumbers.Select(p => new PhoneNumberVM(p)).ToList();
            _myBusidex = myBusidex;

            Task.Factory.StartNew(async () => await App.LoadOwnedCard());        
        }
       
        #region UserCard Actions 
        public ICommand SendSMS
        {
            get { return new Command(async (number) =>
            {
                Device.OpenUri(new Uri($"sms:{number.ToString()}"));
                await _activityHttpService.SaveActivity ((long)EventSources.Text, SelectedCard.CardId);
                App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.SMSSent, number.ToString());
            }); }
        }

        public ICommand DialPhoneNumber
        {
            get { return new Command(async (number) =>
            {
                Device.OpenUri(new Uri($"tel:{number.ToString()}"));
                await _activityHttpService.SaveActivity ((long)EventSources.Call, SelectedCard.CardId);
                App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.PhoneDialed, number.ToString());
            }); }
        }

        public async void LaunchMapApp() {
            // Windows Phone doesn't like ampersands in the names and the normal URI escaping doesn't help
            var address = SelectedCard.Card.Addresses.FirstOrDefault()?.ToString() 
                          ?? string.Empty;

            string request;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                {
                    request = $"geo:0,0?q={address}";
                    break;
                }
                case Device.iOS:
                {
                    address = Uri.EscapeUriString(address);
                    request = $"http://maps.apple.com/maps?q={address}";
                    break;
                }
                case Device.UWP:
                {
                    request = $"bingmaps:?cp={address}";
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Device.OpenUri(new Uri(request));

            await _activityHttpService.SaveActivity ((long)EventSources.Map, SelectedCard.CardId);
            App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.MapViewed, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);
        }

        public async void LaunchEmail()
        {
            Device.OpenUri(new Uri($"mailto:{SelectedCard.Card.Email}"));

            await _activityHttpService.SaveActivity ((long)EventSources.Email, SelectedCard.CardId);
            App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.EmailSent, SelectedCard.Card.Email);
        }

        public async void LaunchBrowser()
        {
            string url;
            if (string.IsNullOrEmpty(SelectedCard.Card.Url)) return;

            url = !SelectedCard.Card.Url.StartsWith ("http", StringComparison.Ordinal) 
                ? "http://" + SelectedCard.Card.Url 
                : SelectedCard.Card.Url;
            Device.OpenUri(new Uri(url));

            await _activityHttpService.SaveActivity ((long)EventSources.Website, SelectedCard.CardId);
            App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.WebPageViewed, SelectedCard.Card.Url);
        }

        public async void RemoveFromMyBusidex()
        {
            if (_myBusidex.All(b => b.CardId != SelectedCard.CardId)) return;

            _myBusidex.RemoveAll(b => b.CardId == SelectedCard.CardId);
            var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(_myBusidex);

            Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);

            var cardId = SelectedCard.CardId;
            SelectedCard.ExistsInMyBusidex = false;
            
            OnPropertyChanged(nameof(ShowAddButton));
            OnPropertyChanged(nameof(ShowRemoveButton));

            await _myBusidexHttpService.RemoveFromMyBusidex(cardId);

            App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardRemoved,
                SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);
        }

        public async void AddToMyBusidex()
        {
            if (_myBusidex.Any(b => b.CardId == SelectedCard.CardId)) return;

            SelectedCard.ExistsInMyBusidex = true;

            _myBusidex.Add(SelectedCard);
            var newList = new ObservableRangeCollection<UserCard>(); 
            newList.AddRange(_myBusidex
                .OrderByDescending(c => c.Card != null && c.Card.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                .ThenBy(c => c.Card != null ? c.Card.Name : "")
                .ThenBy(c => c.Card != null ? c.Card.CompanyName : "")
                .ToList());
            var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(newList);
                
            Serialization.SaveResponse(savedResult, StringResources.MY_BUSIDEX_FILE);

            var cardId = SelectedCard.CardId;
            await _myBusidexHttpService.AddToMyBusidex(cardId);

            OnPropertyChanged(nameof(ShowAddButton));
            OnPropertyChanged(nameof(ShowRemoveButton));

            App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.CardAdded, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);

            await _activityHttpService.SaveActivity ((long)EventSources.Add, cardId);
        }

        #endregion

        #region Card Saving
        public async Task<bool> SaveCardImage(MobileCardImage card)
        {
            OnCardInfoUpdating?.Invoke();

            var result = await _cardHttpService.UpdateCardImage(card);

            await App.LoadOwnedCard();

            App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.CardImageUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);

            SaveToFile();

            return result;
        }

        public async Task<bool> SaveCardVisibility(byte visibility)
        {
            OnCardInfoUpdating?.Invoke();

            var result = await _cardHttpService.UpdateCardVisibility(visibility);
            await App.LoadOwnedCard();

            App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.CardVisibilityUpdated, SelectedCard.Card.Visibility.ToString());

            SaveToFile();

            return result;
        }

        public async Task SaveNotes(string notes)
        {
            IsSaving = true;
            await _notesHttpService.SaveNotes(SelectedCard.UserCardId, notes);

            App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.UserCardNotesUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);

            SaveToFile();

            IsSaving = false;
        }

        public async Task<bool> SaveCardInfo(CardDetailModel card)
        {
            OnCardInfoUpdating?.Invoke();

            var result = await _cardHttpService.UpdateCardContactInfo(card);

            SaveToFile();

            await App.LoadOwnedCard();

            App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.ContactInfoUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);

            return result;
        }

        private void SaveToFile()
        {
            var myBusidex = Serialization.LoadData<ObservableRangeCollection<UserCard>> (Path.Combine (Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));
            var existingCard = myBusidex.SingleOrDefault(b => b.CardId == SelectedCard.CardId);
            
            if (existingCard == null) return;

            var idx = myBusidex.IndexOf(existingCard);
            myBusidex[idx] = SelectedCard;
            var file = Newtonsoft.Json.JsonConvert.SerializeObject (myBusidex);
            Serialization.SaveResponse (file, StringResources.MY_BUSIDEX_FILE);
        }
        #endregion
    }
}
