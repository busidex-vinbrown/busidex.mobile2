using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Professional.Views.EditCard;
using Busidex.Resources.String;
using Newtonsoft.Json;
using System;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
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
                DisplayName = Security.CurrentUser?.UserAccount.DisplayName,
            };

            if (!string.IsNullOrEmpty(Security.AuthToken))
            {
                _viewModel.SaveButtonEnabled = true;
            }

            _viewModel.NewUser = string.IsNullOrEmpty(Security.AuthToken);
            _viewModel.ShowLogout = !_viewModel.NewUser;
            _viewModel.Message = _viewModel.NewUser
                ? "Create Your Account"
                : "Update your account information here.";
            _viewModel.SaveButtonText = _viewModel.NewUser
                ? "Continue"
                : "Save";
            _viewModel.SaveButtonEnabled = isValid();

            BindingContext = _viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                var page = (Page)Activator.CreateInstance(typeof(LoginView));
                NavigationPage.SetHasNavigationBar(page, false);
                Navigation.PushAsync(page);
            }
            else
            {
                Navigation.PopToRootAsync();
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
                var emailOk = await _viewModel.IsEmailAvailable();
                if (emailOk)
                {
                    var ok = await _viewModel.CheckAccount();

                    if (ok)
                    {
                        await Shell.Current.GoToAsync(AppRoutes.HOME);
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
                var page = (Page)Activator.CreateInstance(typeof(LoginView));
                NavigationPage.SetHasNavigationBar(page, false);
                Navigation.PushAsync(page);
            }
        }

        protected void ChkAccept_CheckChanged(object sender, EventArgs e)
        {
            _viewModel.SaveButtonEnabled = isValid();
        }

        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            App.IsCardOwnerConfirmed = false;

            var page = (Page)Activator.CreateInstance(typeof(LoginView));
            Security.LogOut();
            Navigation.PushAsync(page);
        }

        private async void btnEditCard_Clicked(object sender, EventArgs e)
        {
            var card = await App.LoadOwnedCard();
            var uc = new UserCard(card);
            var ucJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(uc)));

            //var page = new EditCardMenuView();
            //await Shell.Current.Navigation.PushAsync(page);
            await Shell.Current.GoToAsync($"{AppRoutes.CARD_EDIT_MENU}?ucJson={ucJson}");
        }
    }
}