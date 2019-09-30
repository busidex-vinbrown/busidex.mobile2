using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Busidex3.DomainModels;
using Busidex3.Models;
using Busidex3.Services.Utils;
using Microsoft.AppCenter.Crashes;
using Plugin.ContactService.Shared;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class ShareVM : BaseViewModel
    {
        public ShareVM()
        {
            SendMethod.Add(0, "Text");
            SendMethod.Add(1, "Email");
            SuccessImage = ImageSource.FromResource("Busidex3.Resources.checkmark.png",
                typeof(ShareVM).GetTypeInfo().Assembly);

            if (App.ContactGroups.Count == 0)
            {
                App.OnContactsLoaded += mergeMyBusidexWithContacts;
                try
                {
                    App.LoadContactList();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }
            else
            {
                mergeMyBusidexWithContacts();
            }

            HideContacts = true;
            ShowContacts = false;
            ShowContact = false;

            ContactsImage = ImageSource.FromResource("Busidex3.Resources.contacts_64x64.png",
                typeof(CardVM).GetTypeInfo().Assembly);
        }

        private async void mergeMyBusidexWithContacts()
        {
            try
            {
                var myBusidex =
                    Serialization.LoadData<List<UserCard>>(Path.Combine(Serialization.LocalStorageFolder,
                        StringResources.MY_BUSIDEX_FILE)) ??
                    new List<UserCard>();

                foreach (var userCard in myBusidex)
                {
                    var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + userCard.Card.FrontFileId + ".jpg";

                    var path = Path.Combine(Serialization.LocalStorageFolder, fName);
                    if (!File.Exists(path))
                    {
                        var fImageUrl = StringResources.THUMBNAIL_PATH + userCard.Card.FrontFileId + ".jpg";

                        var storagePath = Serialization.LocalStorageFolder;
                        await App.DownloadImage(fImageUrl, storagePath, fName).ConfigureAwait(false);
                    }
                }

                var subset = new List<ContactList>();
                foreach (var group in App.ContactGroups)
                {
                    var myBusidexCards = myBusidex.Where(b =>
                        (b.Card?.Name ?? b.Card?.CompanyName ?? string.Empty).ToLower()
                        .StartsWith(group.Heading.ToLower())).ToList();
                    var myBusidexContacts = myBusidexCards.Select(b => new Contact
                    {
                        Name = b.Card.Name ?? b.Card.CompanyName,
                        Email = b.Card.Email,
                        PhotoUri = b.Card.FrontOrientation, // ok. a hack. sue me.
                        PhotoUriThumbnail = b.Card.FrontFileName,
                        Numbers = b.Card.PhoneNumbers?.Select(p => p.Number).ToList()
                    });
                    var phoneContacts = group.Where(g => g.Name.ToLower().StartsWith(group.Heading.ToLower())).ToList();
                    var mergedContacts = myBusidexContacts.Concat(phoneContacts).OrderBy(c => c.Name);
                    var newGroup = new ContactList
                    {
                        Heading = group.Heading
                    };
                    newGroup.AddRange(mergedContacts);
                    subset.Add(newGroup);
                    ContactGroups = new ObservableCollection<ContactList>(subset);
                }

                ShowContactButton = ContactGroups.Count > 0;

                App.OnContactsLoaded -= mergeMyBusidexWithContacts;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public void RefreshContacts()
        {
            // mergeMyBusidexWithContacts();
        }

        public void ClearSearch()
        {
            ContactGroups = new ObservableCollection<ContactList>(App.ContactGroups);
        }

        public void DoSearch()
        {
            var subset = new List<ContactList>();
            foreach(var group in App.ContactGroups)
            {
                var filteredList = group.Where(g => g.Name.ToLower().StartsWith(group.Heading.ToLower()) && g.Name.ToLower().Contains(SearchValue.ToLower())).ToList();
                var newGroup = new ContactList
                {
                    Heading = group.Heading
                };
                newGroup.AddRange(filteredList);
                subset.Add(newGroup);
            }
            ContactGroups = new ObservableCollection<ContactList>(subset);
        }

        public UserCard SelectedCard { get; set; }

        private ObservableCollection<ContactList> _contactGroups = new ObservableCollection<ContactList>();
        public ObservableCollection<ContactList> ContactGroups {
            get => _contactGroups;
            set
            {
                _contactGroups = value;
                OnPropertyChanged(nameof(ContactGroups));
            }
        }

        
        private int _contactImageHeight;
        public int ContactImageHeight
        {
            get => _contactImageHeight;
            set
            {
                _contactImageHeight = value;
                OnPropertyChanged(nameof(ContactImageHeight));
            }
        }

        private int _contactImageWidth;
        public int ContactImageWidth
        {
            get => _contactImageWidth;
            set
            {
                _contactImageWidth = value;
                OnPropertyChanged(nameof(ContactImageWidth));
            }
        }

        private int _contactFrameHeight;
        public int ContactFrameHeight
        {
            get => _contactFrameHeight;
            set
            {
                _contactFrameHeight = value;
                OnPropertyChanged(nameof(ContactFrameHeight));
            }
        }

        private int _contactFrameWidth;
        public int ContactFrameWidth
        {
            get => _contactFrameWidth;
            set
            {
                _contactFrameWidth = value;
                OnPropertyChanged(nameof(ContactFrameWidth));
            }
        }

        private string _selectedContactPhotoUri;
        public string SelectedContactPhotoUri
        {
            get => _selectedContactPhotoUri;
            set
            {
                _selectedContactPhotoUri = value;
                OnPropertyChanged(nameof(SelectedContactPhotoUri));
            }
        }

        private ImageSource _contactsImage { get; set; }
        public ImageSource ContactsImage
        {
            get => _contactsImage;
            set
            {
                _contactsImage = value;
                OnPropertyChanged(nameof(ContactsImage));
            }
        }

        private Contact _selectedContact;
        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
            }
        }

        private bool _showContactButton;
        public bool ShowContactButton
        {
            get => _showContactButton;
            set
            {
                _showContactButton = value;
                OnPropertyChanged(nameof(ShowContactButton));
            }
        }

        private bool _showContact;
        public bool ShowContact
        {
            get => _showContact;
            set
            {
                _showContact = value;
                OnPropertyChanged(nameof(ShowContact));
            }
        }

        private bool _showContacts;
        public bool ShowContacts
        {
            get => _showContacts;
            set
            {
                _showContacts = value;
                OnPropertyChanged(nameof(ShowContacts));
            }
        }

        private bool _hideContacts;
        public bool HideContacts
        {
            get => _hideContacts;
            set
            {
                _hideContacts = value;
                OnPropertyChanged(nameof(HideContacts));
            }
        }

        private string _sentFrom { get; set; }
        public string SentFrom { get => _sentFrom;
            set
            {
                _sentFrom = value;
                OnPropertyChanged(nameof(SentFrom));
            }
        }

        private string _sendTo { get; set; }
        public string SendTo { get => _sendTo;
            set
            {
                _sendTo = value;
                OnPropertyChanged(nameof(SendTo));
            }
        }

        private string _message { get; set; }
        public string Message { get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private bool _sendError { get; set; }
        public bool SendError { get => _sendError;
            set
            {
                _sendError = value;
                OnPropertyChanged(nameof(SendError));
            }
        }

        private bool _messageSent { get; set; }
        public bool MessageSent { get => _messageSent;
            set
            {
                _messageSent = value;
                OnPropertyChanged(nameof(MessageSent));
            }
        }

        private ImageSource _successImage { get; set; }
        public ImageSource SuccessImage { get => _successImage;
            set
            {
                _successImage = value;
                OnPropertyChanged(nameof(SuccessImage));
            }
        }

        private Dictionary<int, string> _sendMethod = new Dictionary<int, string>();
        public Dictionary<int, string> SendMethod
        {
            get => _sendMethod;
            set
            {
                _sendMethod = value;
                OnPropertyChanged(nameof(SendMethod));
            }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set {
                if (value == selectedIndex) return;
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        private bool _allowSend;
        public bool AllowSend
        {
            get => _allowSend;
            set
            {
                _allowSend = value;
                OnPropertyChanged(nameof(AllowSend));
            }
        }
    }
}
