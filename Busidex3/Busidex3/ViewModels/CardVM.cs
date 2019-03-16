using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private ObservableRangeCollection<PhoneNumberVM> _phoneNumbers;
        public ObservableRangeCollection<PhoneNumberVM> PhoneNumbers { get => _phoneNumbers;
            set
            {
                _phoneNumbers = value;
                OnPropertyChanged(nameof(PhoneNumbers));
            }
        }

        private ObservableRangeCollection<PhoneNumberVM> _deletedPhoneNumbers;

        private ImageSource _addPhoneImage { get; set; }
        public ImageSource AddPhoneImage { get => _addPhoneImage;
            set
            {
                _addPhoneImage = value;
                OnPropertyChanged(nameof(AddPhoneImage));
            }
        }

        public bool ShowAddButton => !SelectedCard.ExistsInMyBusidex;
        public bool ShowRemoveButton => SelectedCard.ExistsInMyBusidex;

        private bool _allowSave;
        public bool AllowSave
        {
            get => _allowSave;
            set
            {
                _allowSave = value;
                OnPropertyChanged(nameof(AllowSave));
            }
        }

        private CardVisibility _visibility { get; set; }
        public CardVisibility Visibility { get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        private ImageSource _emailImage { get; set; }
        public ImageSource EmailImage { get => _emailImage;
            set
            {
                _emailImage = value;
                OnPropertyChanged(nameof(EmailImage));
            }
        }

        private ImageSource _urlImage { get; set; }
        public ImageSource UrlImage { get => _urlImage;
            set
            {
                _urlImage = value;
                OnPropertyChanged(nameof(UrlImage));
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

            var pn = new ObservableRangeCollection<PhoneNumberVM>();
            pn.AddRange(uc.Card.PhoneNumbers.Select(p => new PhoneNumberVM(p)));
            PhoneNumbers = pn;
            UpdatePhoneNumberDisplayList();
            _myBusidex = myBusidex;
            
            AllowSave = true;

            EmailImage = ImageSource.FromResource("Busidex3.Resources.email.png",
                typeof(ShareVM).GetTypeInfo().Assembly);
            UrlImage = ImageSource.FromResource("Busidex3.Resources.browser.png",
                typeof(ShareVM).GetTypeInfo().Assembly);
            AddPhoneImage = ImageSource.FromResource("Busidex3.Resources.add-plus.png",
                typeof(ShareVM).GetTypeInfo().Assembly);

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

        public void AddNewPhoneNumber()
        {
            PhoneNumbers.Add(new PhoneNumberVM(new PhoneNumber()));
            UpdatePhoneNumberDisplayList();
        }

        public void RemovePhoneNumber(int idx)
        {
            if (idx >= 0)
            {
                PhoneNumbers[idx].Deleted = true;
                UpdatePhoneNumberDisplayList();
            }
        }

        private void UpdatePhoneNumberDisplayList()
        {
            _deletedPhoneNumbers = new ObservableRangeCollection<PhoneNumberVM>(
                PhoneNumbers.Where(p => p.Deleted && p.PhoneNumberId > 0)
            );
            PhoneNumbers.RemoveAll(p => p.Deleted);
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

        public async Task<bool> SaveCardVisibility()
        {
            OnCardInfoUpdating?.Invoke();
            var v = (byte) SelectedCard.Card.Visibility;
            var result = await _cardHttpService.UpdateCardVisibility(v);
            await App.LoadOwnedCard();

            App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.CardVisibilityUpdated, SelectedCard.Card.Visibility.ToString());

            SaveToFile();

            return result;
        }

        public async Task SaveNotes(string notes)
        {
            AllowSave = false;
            await _notesHttpService.SaveNotes(SelectedCard.UserCardId, notes);

            App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.UserCardNotesUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);

            SaveToFile();

            AllowSave = true;
        }

        public async Task<bool> SaveCardInfo()
        {
            AllowSave = false;

            OnCardInfoUpdating?.Invoke();

            var card = new CardDetailModel(SelectedCard.Card)
            {
                PhoneNumbers = new List<PhoneNumber>(
                    PhoneNumbers.Select(p => new PhoneNumber
                    {
                        PhoneNumberId = p.PhoneNumberId,
                        Number = p.Number, 
                        PhoneNumberType = p.GetSelectedPhoneNumberType(), 
                        PhoneNumberTypeId = p.GetSelectedPhoneNumberType().PhoneNumberTypeId,
                        Deleted = p.Deleted
                    })
                )
            };
            
            
            var result = await _cardHttpService.UpdateCardContactInfo(card);
            if (result)
            {
                var resp = await _cardHttpService.GetCardById(card.CardId);
                SelectedCard.Card.PhoneNumbers.Clear();
                SelectedCard.Card.PhoneNumbers.AddRange(resp.Model.PhoneNumbers);

                SaveToFile();

                await App.LoadOwnedCard();

                PhoneNumbers.Clear();
                PhoneNumbers.AddRange(
                    new List<PhoneNumberVM>(SelectedCard.Card.PhoneNumbers.Select(p => new PhoneNumberVM(p))));

                App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.ContactInfoUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);
            }
            
            AllowSave = true;

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
