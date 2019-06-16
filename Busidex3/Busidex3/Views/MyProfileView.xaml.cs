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

            _viewModel.SaveButtonText = string.IsNullOrEmpty(Security.AuthToken)
                ? "Continue"
                : "Save";
            _viewModel.UserName = Security.CurrentUser?.UserName;
            
            if (!string.IsNullOrEmpty(Security.AuthToken))
            {
                _viewModel.SaveButtonEnabled = true;
            }
            _viewModel.NewUser = string.IsNullOrEmpty(Security.AuthToken);
        }

        protected override bool OnBackButtonPressed()
        {
            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                App.LoadStartupPage();
            }
            else
            {
                App.LoadMainMenuPage();
            }
            return true;
        }

        private void TxtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }

        private void TxtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }

        private void TxtConfirmPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }

        private bool isValid()
        {
            return (chkAccept.IsChecked || !_viewModel.NewUser)&&
                !string.IsNullOrEmpty(txtUserName.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text) &&
                !string.IsNullOrEmpty(txtConfirmPassword.Text) && 
                txtPassword.Text.Equals(txtConfirmPassword.Text);
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            var ok = await _viewModel.CheckAccount();
            if(ok && _viewModel.NewUser)
            {
                App.LoadMainMenuPage();
            }
        }

        private void ChkAccept_CheckChanged(object sender, EventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }
    }
}