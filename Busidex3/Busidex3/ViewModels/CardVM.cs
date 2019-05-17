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
    //public delegate void OnCardInfoSavedHandler ();

    public class CardVM : BaseViewModel
    {
        public static event OnCardInfoUpdatingHandler OnCardInfoUpdating;

        private readonly CardHttpService _cardHttpService = new CardHttpService();
        private readonly MyBusidexHttpService _myBusidexHttpService = new MyBusidexHttpService();
        private readonly NotesHttpService _notesHttpService = new NotesHttpService();
        private readonly ActivityHttpService _activityHttpService = new ActivityHttpService();
        private readonly ObservableRangeCollection<UserCard> _myBusidex;

        public UserCard SelectedCard { get; }

        private List<Tag> _tags;
        public List<Tag> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _companyName;
        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged(nameof(CompanyName));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _selectedStateName;
        public string SelectedStateName
        {
            get => _selectedStateName;
            set
            {
                _selectedStateName = value;
                OnPropertyChanged(nameof(SelectedStateName));
            }
        }

        public bool FrontImageChanged { get;set; }
        public bool BackImageChanged { get; set; }

        private ObservableRangeCollection<PhoneNumberVM> _phoneNumbers;
        public ObservableRangeCollection<PhoneNumberVM> PhoneNumbers { get => _phoneNumbers;
            set
            {
                _phoneNumbers = value;
                OnPropertyChanged(nameof(PhoneNumbers));
            }
        }

        private List<string> _stateNames;
        public List<string> StateNames
        {
            get => _stateNames;
            set
            {
                _stateNames = value;
                OnPropertyChanged(nameof(StateNames));
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

        public bool ShowAddButton => !SelectedCard.ExistsInMyBusidex && SelectedCard.Card.OwnerId != Security.DecodeUserId();
        public bool ShowRemoveButton => SelectedCard.ExistsInMyBusidex && SelectedCard.Card.OwnerId != Security.DecodeUserId();

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

        private string _frontOrientation { get; set; }
        public string FrontOrientation { get => _frontOrientation;
            set
            {
                _frontOrientation = value;
                OnPropertyChanged(nameof(FrontOrientation));
            }
        }

        private string _backOrientation { get; set; }
        public string BackOrientation { get => _backOrientation;
            set
            {
                _backOrientation = value;
                OnPropertyChanged(nameof(BackOrientation));
            }
        }

        public Guid FrontFileId { get; set; }
        public Guid BackFileId { get; set; }

        private string _orientationDisplay { get; set; }
        public string OrientationDisplay { get => _orientationDisplay;
            set
            {
                _orientationDisplay = value;
                OnPropertyChanged(nameof(OrientationDisplay));
            }
        }

        public string EncodedFrontCardImage { get; set; }
        public string EncodedBackCardImage { get; set; }

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

        private ImageSource _cameraImage { get; set; }
        public ImageSource CameraImage { get => _cameraImage;
            set
            {
                _cameraImage = value;
                OnPropertyChanged(nameof(CameraImage));
            }
        }
        
        private string _selectedCardFrontImage { get; set; }
        public string SelectedCardFrontImage { get => _selectedCardFrontImage;
            set
            {
                _selectedCardFrontImage = value;
                OnPropertyChanged(nameof(SelectedCardFrontImage));
            }
        }

        private string _selectedCardBackImage { get; set; }
        public string SelectedCardBackImage { get => _selectedCardBackImage;
            set
            {
                _selectedCardBackImage = value;
                OnPropertyChanged(nameof(SelectedCardBackImage));
            }
        }

        private List<State> _states { get; set; }
        public List<State> States { get => _states;
            set
            {
                _states = value;
                OnPropertyChanged(nameof(States));
            }
        }

        private Address _address { get; set; }
        public Address Address { get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private UserCardDisplay.CardSide _selectedSide { get; set; }
        public UserCardDisplay.CardSide SelectedSide { get => _selectedSide;
            set
            {
                _selectedSide = value;
                OnPropertyChanged(nameof(SelectedSide));
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
                typeof(CardVM).GetTypeInfo().Assembly);
            UrlImage = ImageSource.FromResource("Busidex3.Resources.browser.png",
                typeof(CardVM).GetTypeInfo().Assembly);
            AddPhoneImage = ImageSource.FromResource("Busidex3.Resources.add-plus.png",
                typeof(CardVM).GetTypeInfo().Assembly);

            CameraImage = ImageSource.FromResource("Busidex3.Resources.editimage.png",
                typeof(CardVM).GetTypeInfo().Assembly);

            SelectedCardFrontImage = SelectedCard.Card.FrontFileName;
            SelectedCardBackImage = SelectedCard.Card.BackFileName;

            Task.Factory.StartNew(async () => await App.LoadOwnedCard());
            States = GetStates();
            StateNames = States.Select(s => s.Name).ToList();
            SelectedStateName = SelectedCard.Card.Addresses[0].State?.Name;
            Address = SelectedCard.Card.Addresses[0];
            Title = SelectedCard.Card.Title;
            Name = SelectedCard.Card.Name;
            CompanyName = SelectedCard.Card.CompanyName;
            Tags = new List<Tag>(SelectedCard.Card.Tags.Where(t => t.TagType == 1));
            FrontOrientation = SelectedCard.Card.FrontOrientation;
            BackOrientation = SelectedCard.Card.BackOrientation;
            SelectedSide = UserCardDisplay.CardSide.Front;
        }
        
        #region UserCard Actions 
        //public ICommand SendSMS
        //{
        //    get { return new Command(async (number) =>
        //    {
        //        Device.OpenUri(new Uri($"sms:{number.ToString()}"));
        //        await _activityHttpService.SaveActivity ((long)EventSources.Text, SelectedCard.CardId);
        //        App.AnalyticsManager.TrackEvent(EventCategory.UserInteractWithCard, EventAction.SMSSent, number.ToString());
        //    }); }
        //}

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

        private void initTagDisplay()
        {
            const int MAX_TAGS = 7;
            var tmpList = new List<Tag>(SelectedCard.Card.Tags);
            for (var i = 0; i < MAX_TAGS - SelectedCard.Card.Tags.Count; i++)
            {
                tmpList.Add(new Tag
                {
                    TagTypeId = 1,
                    TagType = 1,
                    Text = string.Empty
                });
            }
            Tags = new List<Tag>(tmpList);
        }

        public async Task<bool> SaveCardImage()
        {
            var storagePath = Serialization.LocalStorageFolder;

            if (FrontImageChanged)
            {
                var cardFront = new MobileCardImage
                {
                    BackFileId = BackFileId,
                    FrontFileId = FrontFileId,
                    EncodedCardImage = EncodedFrontCardImage,
                    Orientation = FrontOrientation,
                    Side = MobileCardImage.DisplayMode.Front
                };

                var frontResult = await _cardHttpService.UpdateCardImage(cardFront);
                if (frontResult)
                {
                    SelectedCard.Card.FrontFileId = FrontFileId;
                    SelectedCard.Card.FrontOrientation = FrontOrientation;

                    var fImageUrl = StringResources.THUMBNAIL_PATH + FrontFileId + ".jpg";
                    var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + FrontFileId + ".jpg";
                    await App.DownloadImage(fImageUrl, storagePath, fName).ConfigureAwait(false);
                }
            }

            if (BackImageChanged)
            {
                var cardBack = new MobileCardImage
                {
                    BackFileId = BackFileId,
                    FrontFileId = FrontFileId,
                    EncodedCardImage = EncodedBackCardImage,
                    Orientation = FrontOrientation,
                    Side = MobileCardImage.DisplayMode.Back
                };

                var backResult = await _cardHttpService.UpdateCardImage(cardBack);
                if (backResult)
                {
                    SelectedCard.Card.BackFileId = BackFileId;
                    SelectedCard.Card.BackOrientation = BackOrientation;
                    
                    var bImageUrl = StringResources.THUMBNAIL_PATH + BackFileId + ".jpg";
                    var bName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + BackFileId + ".jpg";

                    await App.DownloadImage(bImageUrl, storagePath, bName).ConfigureAwait(false);
                }
            }

            if (BackImageChanged || FrontImageChanged)
            {
                await App.LoadOwnedCard();

                App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.CardImageUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);

                SaveToFile();
            }

            return true;
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

        public async Task<bool> SaveAddress()
        {
            AllowSave = false;

            OnCardInfoUpdating?.Invoke();

            var card = new CardDetailModel(SelectedCard.Card)
            {
                Addresses = new List<Address>()
            };

            var state = GetStates().SingleOrDefault(s => s.Name == SelectedStateName);

            card.Addresses.Add(new Address
            {
                CardAddressId = Address.CardAddressId,
                Address1 = Address.Address1,
                Address2 = Address.Address2,
                ZipCode = Address.ZipCode,
                State = state,
                City = Address.City
            });

            var result = await _cardHttpService.UpdateCardContactInfo(card);
            if (result)
            {
                var resp = await _cardHttpService.GetCardById(card.CardId);
                SelectedCard.Card.Addresses.Clear();
                SelectedCard.Card.Addresses.Add(resp.Model.Addresses[0]);

                SaveToFile();

                Address = resp.Model.Addresses[0];

                await App.LoadOwnedCard();

                App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.ContactInfoUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);
            }
            
            AllowSave = true;

            return result;
        }

        public async Task<bool> SaveTags()
        {
            AllowSave = false;

            Tags.RemoveAll(t => string.IsNullOrEmpty(t.Text));
            Tags.RemoveAll(t => string.IsNullOrWhiteSpace(t.Text));

            var card = new CardDetailModel(SelectedCard.Card)
            {
                Tags = new List<Tag>(Tags)
            };

            var result = await _cardHttpService.UpdateCardContactInfo(card);
            if (result)
            {
                var resp = await _cardHttpService.GetCardById(card.CardId);
                SelectedCard.Card.Tags = new List<Tag>(resp.Model.Tags);

                initTagDisplay();

                SaveToFile();

                await App.LoadOwnedCard();

                App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.ContactInfoUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);
            }
            
            AllowSave = true;

            return result;
        }

        public async Task<bool> SaveSearchInfo()
        {
            AllowSave = false;

            var card = new CardDetailModel(SelectedCard.Card)
            {
                CompanyName = CompanyName,
                Name = Name,
                Title = Title
            };

            var result = await _cardHttpService.UpdateCardContactInfo(card);
            if (result)
            {
                var resp = await _cardHttpService.GetCardById(card.CardId);
                SelectedCard.Card.CompanyName = resp.Model.CompanyName;
                SelectedCard.Card.Name = resp.Model.Name;
                SelectedCard.Card.Title = resp.Model.Title;

                SaveToFile();

                await App.LoadOwnedCard();

                App.AnalyticsManager.TrackEvent(EventCategory.CardEdit, EventAction.ContactInfoUpdated, SelectedCard.Card.Name ?? SelectedCard.Card.CompanyName);
            }
            
            AllowSave = true;

            return result;
        }

        public async Task<bool> SaveContactInfo()
        {
            AllowSave = false;

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
            card.PhoneNumbers.AddRange(_deletedPhoneNumbers.Select(p => new PhoneNumber
            {
                PhoneNumberId = p.PhoneNumberId,
                Number = p.Number, 
                PhoneNumberType = p.GetSelectedPhoneNumberType(), 
                PhoneNumberTypeId = p.GetSelectedPhoneNumberType().PhoneNumberTypeId,
                Deleted = p.Deleted
            }));
            
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

        private static List<State> GetStates ()
		{
            var states = new List<State>
            {
                new State {StateCodeId = 1, Code = "AL", Name = "Alabama"},
                new State {StateCodeId = 2, Code = "AK", Name = "Alaska"},
                new State {StateCodeId = 3, Code = "AZ", Name = "Arizona"},
                new State {StateCodeId = 4, Code = "AR", Name = "Arkansas"},
                new State {StateCodeId = 5, Code = "CA", Name = "California"},
                new State {StateCodeId = 6, Code = "CO", Name = "Colorado"},
                new State {StateCodeId = 7, Code = "CT", Name = "Connecticut"},
                new State {StateCodeId = 8, Code = "DE", Name = "Delaware"},
                new State {StateCodeId = 9, Code = "DC", Name = "District Of Columbia"},
                new State {StateCodeId = 10, Code = "FL", Name = "Florida"},
                new State {StateCodeId = 11, Code = "GA", Name = "Georgia"},
                new State {StateCodeId = 12, Code = "HI", Name = "Hawaii"},
                new State {StateCodeId = 13, Code = "ID", Name = "Idaho"},
                new State {StateCodeId = 14, Code = "IL", Name = "Illinois"},
                new State {StateCodeId = 15, Code = "IN", Name = "Indiana"},
                new State {StateCodeId = 16, Code = "IA", Name = "Iowa"},
                new State {StateCodeId = 17, Code = "KS", Name = "Kansas"},
                new State {StateCodeId = 18, Code = "KY", Name = "Kentucky"},
                new State {StateCodeId = 19, Code = "LA", Name = "Louisiana"},
                new State {StateCodeId = 20, Code = "ME", Name = "Maine"},
                new State {StateCodeId = 21, Code = "MD", Name = "Maryland"},
                new State {StateCodeId = 22, Code = "MA", Name = "Massachusetts"},
                new State {StateCodeId = 23, Code = "MI", Name = "Michigan"},
                new State {StateCodeId = 24, Code = "MN", Name = "Minnesota"},
                new State {StateCodeId = 25, Code = "MS", Name = "Mississippi"},
                new State {StateCodeId = 26, Code = "MO", Name = "Missouri"},
                new State {StateCodeId = 27, Code = "MT", Name = "Montana"},
                new State {StateCodeId = 28, Code = "NE", Name = "Nebraska"},
                new State {StateCodeId = 29, Code = "NV", Name = "Nevada"},
                new State {StateCodeId = 30, Code = "NH", Name = "New Hampshire"},
                new State {StateCodeId = 31, Code = "NJ", Name = "New Jersey"},
                new State {StateCodeId = 32, Code = "NM", Name = "New Mexico"},
                new State {StateCodeId = 33, Code = "NY", Name = "New York"},
                new State {StateCodeId = 34, Code = "NC", Name = "North Carolina"},
                new State {StateCodeId = 35, Code = "ND", Name = "North Dakota"},
                new State {StateCodeId = 36, Code = "OH", Name = "Ohio"},
                new State {StateCodeId = 37, Code = "OK", Name = "Oklahoma"},
                new State {StateCodeId = 38, Code = "OR", Name = "Oregon"},
                new State {StateCodeId = 39, Code = "PA", Name = "Pennsylvania"},
                new State {StateCodeId = 40, Code = "RI", Name = "Rhode Island"},
                new State {StateCodeId = 41, Code = "SC", Name = "South Carolina"},
                new State {StateCodeId = 42, Code = "SD", Name = "South Dakota"},
                new State {StateCodeId = 43, Code = "TN", Name = "Tennessee"},
                new State {StateCodeId = 44, Code = "TX", Name = "Texas"},
                new State {StateCodeId = 45, Code = "UT", Name = "Utah"},
                new State {StateCodeId = 46, Code = "VT", Name = "Vermont"},
                new State {StateCodeId = 47, Code = "VA", Name = "Virginia"},
                new State {StateCodeId = 48, Code = "WA", Name = "Washington"},
                new State {StateCodeId = 49, Code = "WV", Name = "West Virginia"},
                new State {StateCodeId = 50, Code = "WI", Name = "Wisconsin"},
                new State {StateCodeId = 51, Code = "WY", Name = "Wyoming"}
            };
            return states;
		}
    }
}
