using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class OrganizationDetailVM : BaseViewModel
    {
        private string _logo;
        public string Logo
        {
            get => _logo;
            set
            {
                _logo = value;
                OnPropertyChanged(nameof(Logo));
            }
        }

        private Organization _organization;
        public Organization Organization
        {
            get => _organization;
            set
            {
                _organization = value;
                OnPropertyChanged(nameof(Organization));
            }
        }

        public ImageSource FacebookImage
        {
            get
            {
                return ImageSource.FromResource("Busidex.Resources.Images.fb.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }

        public ImageSource TwitterImage
        {
            get
            {
                return ImageSource.FromResource("Busidex.Resources.Images.twitter.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }

        public ImageSource EmailImage
        {
            get
            {
                return ImageSource.FromResource("Busidex.Resources.Images.email.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }

        public ImageSource BrowserImage
        {
            get
            {
                return ImageSource.FromResource("Busidex.Resources.Images.browser.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }

        public ImageSource PhoneImage
        {
            get
            {
                return ImageSource.FromResource("Busidex.Resources.Images.phone.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }

        public ImageSource ContactsImage
        {
            get
            {
                return ImageSource.FromResource("Busidex.Resources.Images.contacts_64x64.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }

        private bool _hasFacebook;
        public bool HasFacebook
        {
            get => _hasFacebook;
            set
            {
                _hasFacebook = value;
                OnPropertyChanged(nameof(HasFacebook));
            }
        }

        private bool _hasTwitter;
        public bool HasTwitter
        {
            get => _hasTwitter;
            set
            {
                _hasTwitter = value;
                OnPropertyChanged(nameof(HasTwitter));
            }
        }

        private bool _hasUrl;
        public bool HasUrl
        {
            get => _hasUrl;
            set
            {
                _hasUrl = value;
                OnPropertyChanged(nameof(HasUrl));
            }
        }

        private bool _hasEmail;
        public bool HasEmail
        {
            get => _hasEmail;
            set
            {
                _hasEmail = value;
                OnPropertyChanged(nameof(HasEmail));
            }
        }

        private bool _hasContacts;
        public bool HasContacts
        {
            get => _hasContacts;
            set
            {
                _hasContacts = value;
                OnPropertyChanged(nameof(HasContacts));
            }
        }

        private bool _hasPhone1;
        public bool HasPhone1
        {
            get => _hasPhone1;
            set
            {
                _hasPhone1 = value;
                OnPropertyChanged(nameof(HasPhone1));
            }
        }

        private bool _hasPhone2;
        public bool HasPhone2
        {
            get => _hasPhone2;
            set
            {
                _hasPhone2 = value;
                OnPropertyChanged(nameof(HasPhone2));
            }
        }

        private bool _showReferrals;
        public bool ShowReferrals {
            get => _showReferrals;
            set {
                _showReferrals = value;
                OnPropertyChanged(nameof(ShowReferrals));
            }
        }

        public int ImageSize { get { return 65; } }

        public OrganizationDetailVM(Organization org)
        {
            Organization = org;

            HasFacebook = !string.IsNullOrEmpty(org.Facebook);
            HasTwitter = !string.IsNullOrEmpty(org.Twitter);
            HasUrl = !string.IsNullOrEmpty(org.Url);
            HasEmail = !string.IsNullOrEmpty(org.Email);
            HasContacts = !string.IsNullOrEmpty(org.Contacts);
            HasPhone1 = !string.IsNullOrEmpty(org.Phone1);
            HasPhone2 = !string.IsNullOrEmpty(org.Phone2);

            ShowReferrals = false;

            Task.Factory.StartNew(async () =>
            {
                var _organizationsHttpService = new OrganizationsHttpService();
                var result = await _organizationsHttpService.GetOrganizationMembers(Organization.OrganizationId);
                if (result.Model.Any(card => card.OwnerId == Security.CurrentUser.UserId))
                {
                    ShowReferrals = true;
                }
            });
            
            var fileName = Organization.LogoFileName + "." + Organization.LogoType;
            Logo = Path.Combine(Serialization.LocalStorageFolder, fileName);
            if (!File.Exists(Logo))
            {
                var remoteFile = StringResources.CARD_PATH + fileName;
                Task.Factory.StartNew(async ()=>
                {
                    await App.DownloadImage(remoteFile, Serialization.LocalStorageFolder, fileName);
                    OnPropertyChanged(nameof(Logo));
                });
            }
        }

        public async void DialPhoneNumber(string number)
        {
            await Launcher.OpenAsync(new Uri($"tel:{number}"));
        }

        public async void SendEmail(string email)
        {
            await Launcher.OpenAsync(new Uri($"mailto:{email}"));
        }

        public async void LaunchBrowser(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            url = !url.StartsWith("http", StringComparison.Ordinal)
                ? "http://" + url
                : url;
            await Launcher.OpenAsync(new Uri(url));
        }
    }
}
