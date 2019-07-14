using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyProfileView : ContentPage
    {
        protected MyProfileVM _viewModel = new MyProfileVM();

        public MyProfileView()
        {
            InitializeComponent();
            BindingContext = _viewModel;
            Title = "My Profile";
            
            _viewModel.Email = Security.CurrentUser?.Email;
            
            if (!string.IsNullOrEmpty(Security.AuthToken))
            {
                _viewModel.SaveButtonEnabled = true;
            }
            _viewModel.NewUser = string.IsNullOrEmpty(Security.AuthToken);
            _viewModel.Message = _viewModel.NewUser
                ? "Choose an email address and password here so you can access your cards on the web or another mobile device."
                : "Update your account email address here.";
            _viewModel.SaveButtonText = _viewModel.NewUser
                ? "Continue"
                : "Save";
        }

        protected override bool OnBackButtonPressed()
        {
            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                App.LoadStartupPage();
            }
            else
            {
                App.LoadMyBusidexPage();
            }
            return true;
        }

        private void TxtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.UserNameInUse = false;
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
            return (chkAccept.IsChecked || !_viewModel.NewUser)&&
                !string.IsNullOrEmpty(txtEmail.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text) &&
                !_viewModel.IsSaving &&
                !_viewModel.UserNameInUse && 
                !string.IsNullOrEmpty(txtConfirmPassword.Text) && 
                txtPassword.Text.Equals(txtConfirmPassword.Text);
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            _viewModel.IsSaving = true;

            var userNameOk = await _viewModel.IsEmailAvailabile();
            if (userNameOk)
            {
                var ok = await _viewModel.CheckAccount();

                if (ok && _viewModel.NewUser)
                {
                    App.LoadMyBusidexPage();
                }
            }
            _viewModel.IsSaving = false;
        }

        private void ChkAccept_CheckChanged(object sender, EventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }
    }
}