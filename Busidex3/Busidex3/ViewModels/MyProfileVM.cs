using Busidex3.Services;
using Busidex3.Services.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class MyProfileVM : BaseViewModel
    {
        public ICommand TermsAndConditionsCommand { get; private set; }
        public ICommand PrivacyPolicyCommand { get; private set; }

        private string _saveButtonText;
        public string SaveButtonText
        {
            get { return _saveButtonText;}
            set
            {
                _saveButtonText = value;
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }

        public string UserName
        {
            get { return Security.CurrentUser.UserName; }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        private bool _newUser;
        public bool NewUser
        {
            get { return _newUser; }
            set
            {
                _newUser = value;
                OnPropertyChanged(nameof(NewUser));
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                _isSaving = value;
                OnPropertyChanged(nameof(IsSaving));
            }
        }

        private bool _userNameInUse;
        public bool UserNameInUse
        {
            get { return _userNameInUse; }
            set
            {
                _userNameInUse = value;
                OnPropertyChanged(nameof(UserNameInUse));
            }
        }

        private bool _profileError;
        public bool ProfileError
        {
            get { return _profileError; }
            set
            {
                _profileError = value;
                OnPropertyChanged(nameof(ProfileError));
            }
        }

        private bool _confirmPasswordError;
        public bool ConfirmPasswordError
        {
            get { return _confirmPasswordError; }
            set
            {
                _confirmPasswordError = value;
                OnPropertyChanged(nameof(ConfirmPasswordError));
            }
        }
        
        private bool _saveButtonEnabled;
        public bool SaveButtonEnabled
        {
            get { return _saveButtonEnabled; }
            set
            {
                _saveButtonEnabled = value;
                OnPropertyChanged(nameof(SaveButtonEnabled));
            }
        }

        public MyProfileVM()
        {
            TermsAndConditionsCommand = PrivacyPolicyCommand = new Command<string>(LaunchBrowser);
        }
        
        void LaunchBrowser(string url)
        {            
            Device.OpenUri(new Uri(url));
        }

        public async Task<bool> IsEmailAvailabile()
        {
            var accountService = new AccountHttpService();
            var isAvailable = await accountService.IsEmailAvailabile(Email);
            UserNameInUse = !isAvailable;
            return isAvailable;
        }

        public async Task<bool> CheckAccount()
        {
            ProfileError = false;
            var accountService = new AccountHttpService();
            var result = await accountService.CheckAccount(Email, Password);
            if(result.UserId > 0)
            {
                Security.SaveAuthCookie(result.UserId);
                await accountService.UpdateDeviceType(Security.AuthToken, DomainModels.DeviceType.Android);
                return true;
            }
            else
            {
                ProfileError = true;
                return false;
            }
        }
    }
}
