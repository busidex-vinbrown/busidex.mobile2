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
        private bool NeedsSetup { get;set; }
        protected MyProfileVM _viewModel = new MyProfileVM();

        public MyProfileView()
        {
            InitializeComponent();
            BindingContext = _viewModel;
            Title = "My Profile";

            _viewModel.SaveButtonText = string.IsNullOrEmpty(Security.AuthToken)
                ? "Continue"
                : "Save";

            if (!string.IsNullOrEmpty(Security.AuthToken))
            {
                _viewModel.SaveButtonEnabled = true;
            }
            NeedsSetup = string.IsNullOrEmpty(Security.AuthToken);
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
            _viewModel.SaveButtonEnabled = chkAccept.IsChecked &&
                !string.IsNullOrEmpty(txtUserName.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text);
        }

        private void TxtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SaveButtonEnabled = chkAccept.IsChecked &&
                !string.IsNullOrEmpty(txtUserName.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text);
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            var ok = await _viewModel.CheckAccount();
            if(ok && this.NeedsSetup)
            {
                App.LoadMainMenuPage();
            }
        }

        private void ChkAccept_CheckChanged(object sender, EventArgs e)
        {
            _viewModel.SaveButtonEnabled = chkAccept.IsChecked && 
                !string.IsNullOrEmpty(txtUserName.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text);
        }
    }
}