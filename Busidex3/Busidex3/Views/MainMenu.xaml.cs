//using Busidex3.Views.EditCard;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu
    {
        public MainMenu()
        {
            InitializeComponent();
            //if(MasterPage != null)
            //{
            //    //MasterPage.OnLogout += RedirectToLogin;
            //    //MasterPage.OnShareClicked += MasterPage_OnShareClicked;
            //    //MasterPage.OnCardEditClicked += MasterPage_OnCardEditClicked;
            //    MasterPage.OnProfileClicked += MasterPage_OnProfileClicked;
            //    //MasterPage.OnAdminClicked += MasterPage_OnAdminClicked;
            //    MasterPage.OnHomeClicked += MasterPage_OnHomeClicked;
            //}
            

            //IsPresentedChanged += MainMenu_IsPresentedChanged;

            //var quickSharePath = Path.Combine(Serialization.LocalStorageFolder, StringResources.QUICKSHARE_LINK);

            //if (string.IsNullOrEmpty(Security.AuthToken))
            //{
            //    RedirectToLogin();
            //}
            //else if (File.Exists(quickSharePath))
            //{
            //    var quickShareLink = Serialization.LoadData<QuickShareLink>(quickSharePath);
            //    Task.Factory.StartNew(async () =>
            //    {
            //        var uc = await SaveFromUrl(quickShareLink);
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            Detail = uc.Card?.FrontFileId != Guid.Empty && uc.Card?.FrontFileId != null
            //                ? new QuickShareView(uc, quickShareLink.DisplayName, quickShareLink.PersonalMessage) as Page
            //                : new ConfirmCardOwnerView(uc, quickShareLink.DisplayName, quickShareLink.PersonalMessage) as Page;

            //            IsPresented = false;
            //            IsGestureEnabled = false;
            //            NavigationPage.SetHasNavigationBar(Detail, false);
            //        });
            //    });
            //}
            //else
            //{
            //    var page = (Page)Activator.CreateInstance(typeof(HomeMenuView));
            //    page.Title = ViewNames.Home;

            //    Detail = new NavigationPage(page);
            //    IsPresented = false;
            //}
        }

        //private async Task<UserCard> SaveFromUrl(QuickShareLink link)
        //{
        //    var cardService = new CardHttpService();
        //    var result = await cardService.GetCardById(link.CardId);
        //    if (result.Success)
        //    {
        //        var card = new Card(result.Model);

        //        var myBusidexService = new MyBusidexHttpService();
        //        await myBusidexService.AddToMyBusidex(card.CardId);

        //        var sharedCardService = new SharedCardHttpService();
        //        if (card.OwnerId.HasValue)
        //        {
        //            await sharedCardService.AcceptQuickShare(card, Security.CurrentUser.Email, link.From, link.PersonalMessage);
        //        }
                
        //        Serialization.RemoveQuickShareLink();

        //        //var orientation = card.FrontOrientation == "H" ? UserCardDisplay.CardOrientation.Horizontal : UserCardDisplay.CardOrientation.Vertical;
        //        var userCard = new UserCard
        //        {
        //            Card = card,
        //            CardId = card.CardId,
        //            ExistsInMyBusidex = true,
        //            OwnerId = card.OwnerId,
        //            UserId = Security.CurrentUser.UserId,
        //            Notes = string.Empty,
        //            //DisplaySettings = new UserCardDisplay(UserCardDisplay.DisplaySetting.Detail, orientation, card.FrontFileName)
        //        };
        //        return userCard;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        private void MasterPage_OnProfileClicked()
        {
            //App.LoadProfilePage();
        }

        //private void MasterPage_OnShareClicked(ref UserCard card)
        //{
        //    var page = new ShareView(ref card) {Title = "Share My Card"};

        //    Detail = new NavigationPage(page);
        //    IsPresented = false;
        //}     

        //private void MasterPage_OnAdminClicked()
        //{
        //    var page = new AdminView() { Title = "Admin" };

        //    Detail = new NavigationPage(page);
        //    IsPresented = false;
        //}

        //private void MasterPage_OnHomeClicked()
        //{
        //    var page = new HomeMenuView() { Title = "Home" };

        //    Detail = new NavigationPage(page);
        //    IsPresented = false;
        //}

        //private void MainMenu_IsPresentedChanged(object sender, EventArgs e)
        //{
        //    if (IsPresented)
        //    {
        //        MasterPage.RefreshProfile();
        //    }
        //}

        //private void RedirectToLogin()
        //{
        //    var page = (Page)Activator.CreateInstance(typeof(Login));
        //    Detail = page;
        //    IsPresented = false;
        //    NavigationPage.SetHasNavigationBar(Detail, false);
        //    IsGestureEnabled = false;
        //}
    }
}