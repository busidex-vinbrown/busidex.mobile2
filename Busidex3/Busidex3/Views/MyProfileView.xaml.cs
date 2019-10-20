using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyProfileView
    {
        protected MyProfileVM _viewModel;

        public MyProfileView()
        {
            InitializeComponent();
            
            Title = "My Profile";

            _viewModel = new MyProfileVM
            {
                Email = Security.CurrentUser?.Email, 
                DisplayName = Security.CurrentUser?.UserAccount.DisplayName
            };

            if (!string.IsNullOrEmpty(Security.AuthToken))
            {
                _viewModel.SaveButtonEnabled = true;
            }

            _viewModel.NewUser = string.IsNullOrEmpty(Security.AuthToken);
            _viewModel.Message = _viewModel.NewUser
                ? "Choose an email address and password here so you can access your cards on any device."
                : "Update your account information here.";
            _viewModel.SaveButtonText = _viewModel.NewUser
                ? "Continue"
                : "Save";
            _viewModel.SaveButtonEnabled = isValid();
            txtUserName.IsReadOnly = !_viewModel.NewUser;

            BindingContext = _viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                App.LoadStartupPage();
            }
            else
            {
                App.LoadHomePage();
            }
            return true;
        }

        private void TxtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.UserNameInUse = false;
            _viewModel.SaveButtonEnabled = isValid();
        }

        private void TxtDisplayName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }

        private void TxtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.ConfirmPasswordError = _viewModel.Password != _viewModel.ConfirmPassword;
            _viewModel.SaveButtonEnabled = isValid();
        }

        private void TxtConfirmPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.ConfirmPasswordError = _viewModel.Password != _viewModel.ConfirmPassword;
            _viewModel.SaveButtonEnabled = isValid();
        }

        private bool isValid()
        {
            if (!_viewModel.NewUser)
            {
                return !string.IsNullOrEmpty(txtEmail.Text) &&
                       !string.IsNullOrEmpty(txtDisplayName.Text);
            }
            else
            {
                return chkAccept.IsChecked &&
                !string.IsNullOrEmpty(txtEmail.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text) &&
                !_viewModel.IsSaving &&
                !_viewModel.UserNameInUse &&
                !string.IsNullOrEmpty(txtConfirmPassword.Text) &&
                txtPassword.Text.Equals(txtConfirmPassword.Text);
            }
        }

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            _viewModel.IsSaving = true;

            if (_viewModel.NewUser)
            {
                var userNameOk = await _viewModel.IsEmailAvailable();
                if (userNameOk)
                {
                    var ok = await _viewModel.CheckAccount();

                    if (ok)
                    {
                        App.LoadMyBusidexPage();
                    }
                }
            }
            else
            {
                var ok = await _viewModel.UpdateUser();
            }
            
            _viewModel.IsSaving = false;
        }

        private void BtnCancel_OnTapped(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(Security.AuthToken))
            {
                App.LoadStartupPage();
            }
        }

        protected void ChkAccept_CheckChanged(object sender, EventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }
    }
}