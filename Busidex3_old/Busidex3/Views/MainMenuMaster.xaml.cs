using System;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    public delegate void LogoutResult();

    public delegate void OnShareClickedResult(ref UserCard card);
    public delegate void OnCardEditClickedResult();

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuMaster
    {
        public ListView ListView;
        public event LogoutResult OnLogout;
        public event OnShareClickedResult OnShareClicked;
        public event OnCardEditClickedResult OnCardEditClicked;

        protected MainMenuMasterVM _viewModel { get; set; }

        public MainMenuMaster()
        {
            InitializeComponent();

            _viewModel = new MainMenuMasterVM();
            BindingContext = _viewModel;
            ListView = MenuItemsListView;

        }

        public void RefreshProfile()
        {
            _viewModel.RefreshProfile();
        }

        private async void BtnLogout_OnClicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "Cancel")) return;

            Security.LogOut();

            OnLogout?.Invoke();
        }

        private void CardEditTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            OnCardEditClicked?.Invoke();
        }

        private void ShareCardTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var mc = _viewModel.MyCard;

            OnShareClicked?.Invoke(ref mc);
        }
    }
}