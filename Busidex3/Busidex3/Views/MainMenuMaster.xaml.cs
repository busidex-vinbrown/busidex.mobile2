using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    public delegate void LogoutResult();

    public delegate void OnShareClickedResult(ref UserCard card);
    public delegate void OnCardEditClickedResult(ref UserCard card);
    public delegate void OnProfileClickedResult();
    public delegate void OnMyBusidexClickedResult();
    public delegate void OnSearchClickedResult();
    public delegate void OnEventsClickedResult();
    public delegate void OnOrganizationsClickedResult();

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuMaster
    {
        public ListView ListView;
        public event LogoutResult OnLogout;
        public event OnShareClickedResult OnShareClicked;
        public event OnCardEditClickedResult OnCardEditClicked;
        public event OnProfileClickedResult OnProfileClicked;
        public event OnMyBusidexClickedResult OnMyBusidexClicked;
        public event OnSearchClickedResult OnSearchClicked;
        public event OnEventsClickedResult OnEventsClicked;
        public event OnOrganizationsClickedResult OnOrganizationsClicked;

        protected MainMenuMasterVM _viewModel { get; set; }

        public MainMenuMaster()
        {
            InitializeComponent();

            _viewModel = new MainMenuMasterVM();
            BindingContext = _viewModel;
            ctrlProfileImage.OnCardImageClicked += CtrlProfileImage_OnCardImageClicked;
        }

        private void CtrlProfileImage_OnCardImageClicked(UserCard uc)
        {
            OnProfileClicked?.Invoke();
        }

        public void RefreshProfile()
        {
            _viewModel.RefreshProfile();
            _viewModel.EditTitle = _viewModel.HasCard ? ViewNames.Edit : ViewNames.Add;
            var events = Serialization.GetCachedResult<List<EventTag>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.EVENT_LIST_FILE));
            if (events.Any())
            {
                _viewModel.ShowEvents = true;
            }
        }

        private async void BtnLogout_OnClicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "Cancel")) return;

            Security.LogOut();

            OnLogout?.Invoke();
        }

        private void CardEditTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var mc = _viewModel.MyCard;
            OnCardEditClicked?.Invoke(ref mc);
        }

        private void ShareCardTapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var mc = _viewModel.MyCard;
            OnShareClicked?.Invoke(ref mc);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            OnProfileClicked?.Invoke();
        }

        private void stkMyBusidex_Tapped(object sender, EventArgs e)
        {
            OnMyBusidexClicked?.Invoke();
        }

        private void stkSearch_Tapped(object sender, EventArgs e)
        {
            OnSearchClicked?.Invoke();
        }

        private void stkEvents_Tapped(object sender, EventArgs e)
        {
            OnEventsClicked?.Invoke();
        }

        private void stkOrganizations_Tapped(object sender, EventArgs e)
        {
            OnOrganizationsClicked?.Invoke();
        }
    }
}