using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Resources.String;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMenuView
    {
        private readonly HomeMenuVM _viewModel = new HomeMenuVM();

        public HomeMenuView()
        {
            InitializeComponent();

            Shell.SetNavBarIsVisible(this, false);

            imgBackground.HeightRequest = DeviceDisplay.MainDisplayInfo.Height;
            imgBackground.WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
            
            imgBackground.Margin = new Thickness(-60, 0, 0, 0);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            var quickSharePath = Path.Combine(Serialization.LocalStorageFolder, StringResources.QUICKSHARE_LINK);

            if (File.Exists(quickSharePath))
            {
                var quickShareLink = Serialization.LoadData<QuickShareLink>(quickSharePath);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var uc = await SaveFromQuickShareLink(quickShareLink);
                    var ucJson = HttpUtility.UrlEncode(JsonConvert.SerializeObject(uc).ToHexString());// Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(uc)));

                    var page = quickShareLink.SaveOwner
                        ? AppRoutes.CONFIRM_OWNER
                        : AppRoutes.QUICKSHARE;

                    await Shell.Current.GoToAsync($"{page}?ucJson={HttpUtility.UrlEncode(ucJson)}&from={quickShareLink?.DisplayName}&message={quickShareLink?.PersonalMessage}");
                });
            }
            base.OnAppearing();
        }

        private async Task<UserCard> SaveFromQuickShareLink(QuickShareLink link)
        {
            var cardService = new CardHttpService();
            var result = await cardService.GetCardById(link.CardId);
            if (result.Success)
            {
                var card = new Card(result.Model);
                var fImageUrl = StringResources.THUMBNAIL_PATH + card.FrontFileId + ".jpg";
                var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileId + ".jpg";
                var frontImgResult = await App.DownloadImage(fImageUrl, Serialization.LocalStorageFolder, fName);
                if(card.BackFileId != null && card.BackFileId != Guid.Empty)
                {
                    fImageUrl = StringResources.THUMBNAIL_PATH + card.BackFileId + ".jpg";
                    fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileId + ".jpg";
                    var backImgResult = await App.DownloadImage(fImageUrl, Serialization.LocalStorageFolder, fName);
                }
                var myBusidexService = new MyBusidexHttpService();
                await myBusidexService.AddToMyBusidex(card.CardId);

                var sharedCardService = new SharedCardHttpService();

                await sharedCardService.AcceptQuickShare(card, Security.CurrentUser.Email, link.From, link.PersonalMessage);
        
                Serialization.RemoveQuickShareLink();

                var userCard = new UserCard
                {
                    Card = card,
                    CardId = card.CardId,
                    ExistsInMyBusidex = true,
                    OwnerId = card.OwnerId,
                    UserId = Security.CurrentUser.UserId,
                    Notes = string.Empty,
                };
                return userCard;
            }
            else
            {
                return null;
            }
        }

        private async void stkShare_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(AppRoutes.SHARE);
        }

        private async void stkSearch_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(AppRoutes.SEARCH);
            //await Navigation.PushAsync(new SearchView());
        }

        private async void stkMyBusidex_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(AppRoutes.MY_BUSIDEX);
        }

        private async void stkOrganizations_Tapped(object sender, EventArgs e)
        {
            const int BusidexOrgId = 16;
            var _organizationsHttpService = new OrganizationsHttpService();
            var resp = await _organizationsHttpService.GetMyOrganizations();//.GetOrganizationById(BusidexOrgId);
            var org = resp.Model.FirstOrDefault(o => o.OrganizationId == BusidexOrgId);
            if (org != null)
            {
                await Navigation.PushAsync(new OrganizationDetailView(org));
            }
            else
            {
                var resp2 = await _organizationsHttpService.GetOrganizationById(BusidexOrgId);//.GetOrganizationById(BusidexOrgId);
                org = resp2.Model;

                await Navigation.PushAsync(new OrganizationDetailView(org, false));
            }
        }

        private async void stkEvents_Tapped(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new EventsView());
        }

        protected override bool OnBackButtonPressed()
        {
            //App.LoadHomePage();
            return true;
        }

        private async void stkManageAccount_Tapped(object sender, EventArgs e)
        {            
            await Shell.Current.GoToAsync(AppRoutes.ACCOUNT);
        }

        private async void stkManageCard_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(AppRoutes.CARD_EDIT_MENU);
        }

        private async void stkFaq_Tapped(object sender, EventArgs e)
        {
            await _viewModel.LaunchBrowser(StringResources.FAQ_URL);
        }

        private async void stkPresentation_Tapped(object sender, EventArgs e)
        {
            await _viewModel.LaunchPresentation();
        }
    }
}