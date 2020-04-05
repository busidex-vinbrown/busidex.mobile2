using Busidex3.Services;
using Busidex3.Services.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Busidex3.DomainModels;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using DeviceType = Busidex3.DomainModels.DeviceType;

namespace Busidex3.ViewModels
{
    public class MyProfileVM : BaseViewModel
    {
        public ICommand TermsAndConditionsCommand { get; private set; }
        public ICommand PrivacyPolicyCommand { get; private set; }

        private string _saveButtonText;
        public string SaveButtonText
        {
            get => _saveButtonText;
            set
            {
                _saveButtonText = value;
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private bool _newUser;
        public bool NewUser
        {
            get => _newUser;
            set
            {
                _newUser = value;
                OnPropertyChanged(nameof(NewUser));
            }
        }

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

        private bool _userNameInUse;
        public bool UserNameInUse
        {
            get => _userNameInUse;
            set
            {
                _userNameInUse = value;
                OnPropertyChanged(nameof(UserNameInUse));
            }
        }

        private bool _profileError;
        public bool ProfileError
        {
            get => _profileError;
            set
            {
                _profileError = value;
                OnPropertyChanged(nameof(ProfileError));
            }
        }

        private bool _confirmPasswordError;
        public bool ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set
            {
                _confirmPasswordError = value;
                OnPropertyChanged(nameof(ConfirmPasswordError));
            }
        }

        private double _saveButtonOpacity;
        public double SaveButtonOpacity {
            get => _saveButtonOpacity;
            set {
                _saveButtonOpacity = value;
                OnPropertyChanged(nameof(SaveButtonOpacity));
            }
        }

        private bool _saveButtonEnabled;
        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set
            {
                _saveButtonEnabled = value;
                SaveButtonOpacity = _saveButtonEnabled ? 1.0 : .3;
                OnPropertyChanged(nameof(SaveButtonEnabled));
            }
        }

        public MyProfileVM()
        {
            TermsAndConditionsCommand = PrivacyPolicyCommand = new Command<string>(LaunchBrowser);            
        }
        
        async void LaunchBrowser(string url)
        {            
            await Launcher.OpenAsync(new Uri(url));
        }

        public async Task<bool> IsEmailAvailable()
        {
            var accountService = new AccountHttpService();
            var isAvailable = await accountService.IsEmailAvailable(Email);
            UserNameInUse = !isAvailable;
            return isAvailable;
        }

        public async Task<bool> CheckAccount()
        {
            ProfileError = false;
            var accountService = new AccountHttpService();
            var result = await accountService.CheckAccount(Email, Password, DisplayName);
            if(result.UserId > 0)
            {
                await Security.SaveAuthCookie(result.UserId);
                var type = Device.RuntimePlatform == Device.Android
                    ? DeviceType.Android
                    : Device.Idiom == TargetIdiom.Tablet
                        ? DeviceType.iPad
                        : DeviceType.iPhone;

                await accountService.UpdateDeviceType(type);
                return true;
            }
            else
            {
                ProfileError = true;
                return false;
            }
        }

        public async Task<bool> UpdateUser()
        {
            var dto = new UserDTO
            {
                UserId = Security.CurrentUser.UserId,
                DisplayName = DisplayName,
                Email = Email
            };
            var accountService = new AccountHttpService();
            var response = await accountService.UpdateUser(dto);
            if (!response.Success) return false;

            var user = Security.CurrentUser;

            user.UserAccount.DisplayName = DisplayName;
            user.Email = Email;
            var accountJson = JsonConvert.SerializeObject(user);
            Serialization.SaveResponse(accountJson, StringResources.BUSIDEX_USER_FILE);                

            return true;
        }
    }
}
