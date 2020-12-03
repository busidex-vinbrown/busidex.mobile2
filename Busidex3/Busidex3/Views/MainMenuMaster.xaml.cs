using System;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.SharedUI;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    public delegate void LogoutResult();

    public delegate void OnShareClickedResult(ref UserCard card);
    public delegate void OnCardEditClickedResult(ref UserCard card);
    public delegate void OnProfileClickedResult();
    public delegate void OnAdminClickedResult();
    public delegate void OnHomeClickedResult();

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuMaster
    {
        public ListView ListView;
        public event LogoutResult OnLogout;
        public event OnShareClickedResult OnShareClicked;
        public event OnCardEditClickedResult OnCardEditClicked;
        public event OnProfileClickedResult OnProfileClicked;
        public event OnAdminClickedResult OnAdminClicked;
        public event OnHomeClickedResult OnHomeClicked;

        protected MainMenuMasterVM _viewModel { get; set; }

        public MainMenuMaster()
        {
            InitializeComponent();

            _viewModel = new MainMenuMasterVM {IsAdmin = false};

            BindingContext = _viewModel;
            //_viewModel.DisplaySettings = new UserCardDisplay(
            //    UserCardDisplay.DisplaySetting.Detail,
            //    _viewModel.MyCard.Card.FrontOrientation == "H"
            //        ? UserCardDisplay.CardOrientation.Horizontal
            //        : UserCardDisplay.CardOrientation.Vertical,
            //    _viewModel.MyCard.Card.FrontFileName,
            //    _viewModel.MyCard.Card.FrontOrientation);

            ctrlProfileImage.OnCardImageClicked += CtrlProfileImage_OnCardImageClicked;
        }

        private void CtrlProfileImage_OnCardImageClicked(IUserCardDisplay ucd)
        {
            OnProfileClicked?.Invoke();
        }

        //public void RefreshProfile()
        //{
        //    //_viewModel.RefreshProfile();
        //    //_viewModel.EditTitle = _viewModel.HasCard ? ViewNames.Edit : ViewNames.Add;
        //    //_viewModel.IsAdmin = Security.CurrentUser?.IsAdmin;
        //}

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

        private void stkAdmin_Tapped(object sender, EventArgs e)
        {
            OnAdminClicked?.Invoke();
        }

        private void stkHome_Tapped(object sender, EventArgs e)
        {
            OnHomeClicked?.Invoke();
        }
    }
}