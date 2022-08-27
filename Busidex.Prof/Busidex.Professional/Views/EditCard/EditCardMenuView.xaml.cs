using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Constants;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Resources.String;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
    //[QueryProperty(nameof(UserCardJson), "ucJson")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardMenuView : ContentPage
    {
        //private string _userCardJson = "";
        //public string UserCardJson
        //{
        //    get => _userCardJson;
        //    set
        //    {
        //        _userCardJson = value;
        //    }
        //}

        //public void ApplyQueryAttributes(IDictionary<string, string> query)
        //{
        //    if(query.Count > 0)
        //    {
        //        _userCardJson = HttpUtility.UrlDecode(query["ucJson"]) ?? string.Empty;
        //    }
        //    else
        //    {
        //        _userCardJson = String.Empty;
        //    }
        //}

        public CardMenuVM _viewModel { get; set; }

		public EditCardMenuView ()
		{
            InitializeComponent();
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    Shell.Current.GoToAsync(AppRoutes.HOME).Wait();
        //    return true;
        //}

        private async Task Init()
        {
            //var json = Encoding.UTF8.GetString(UserCardJson.FromHex());
            //var uc = JsonConvert.DeserializeObject<UserCard>(json);
            var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
            var ownedCard = Serialization.LoadData<Card>(path);
            if(ownedCard.FrontFileId == null || ownedCard.FrontFileId == Guid.Empty)
            {
                ownedCard = await App.LoadOwnedCard();
            }
            var uc = new UserCard(ownedCard);
            if (uc != null && _viewModel == null)
            {
                try
                {
                    if(uc.Card.FrontFileId == Guid.Empty)
                    {
                        uc.Card = await App.LoadOwnedCard();
                    }
                    _viewModel = new CardMenuVM
                    {
                        SelectedCard = uc,
                        ImageSize = 65
                    };
                    _viewModel.CheckHasCard();
                    _viewModel.DisplaySettings = new UserCardDisplay(
                        DisplaySetting.Detail,
                        uc.Card.FrontOrientation == "H"
                            ? CardOrientation.Horizontal
                            : CardOrientation.Vertical,
                        uc.Card.FrontFileName,
                        uc.Card.FrontOrientation);

                    BindingContext = _viewModel;
                    _viewModel.SetViewHeightForOrientation(_viewModel.SelectedCard.Card.FrontOrientation);

                    Title = uc.Card.Name ?? uc.Card.CompanyName;
                }
                catch(Exception ex)
                {
                    var err = ex;
                }
                this.ctrlCardImageHeader.IsVisible = _viewModel.ShowCardImage;
            }
        }

        protected override void OnAppearing()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Init();
            });

            App.AnalyticsManager.TrackScreen(ScreenName.MyCard);
            base.OnAppearing();
        }

        private async Task GoToRoute(string route)
        {
            var uc = _viewModel.SelectedCard;
            //var mb = Serialization.LoadData<List<UserCard>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));

            var ucJson = HttpUtility.UrlEncode(JsonConvert.SerializeObject(uc).ToHexString());// HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(uc))));
            //var mbJson = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mb))));

            await Shell.Current.GoToAsync($"{route}?ucJson={ucJson}");
        }

        private async void EditCardImageTapped(object sender, EventArgs e)
        {
            await GoToRoute(AppRoutes.EDIT_CARD_IMAGE);
        }

        private async void VisibilityTapped(object sender, EventArgs e)
        {
            await GoToRoute(AppRoutes.EDIT_VISIBILITY);
        }

        private async void EditContactInfoTapped(object sender, EventArgs e)
        {
            await GoToRoute(AppRoutes.EDIT_CONTACT_INFO);
        }

        private async void SearchInfoTapped(object sender, EventArgs e)
        {
            await GoToRoute(AppRoutes.EDIT_SEARCH_INFO);
        }

        private async void TagsTapped(object sender, EventArgs e)
        {
            await GoToRoute(AppRoutes.EDIT_TAGS);
        }

        private async void AddressInfoTapped(object sender, EventArgs e)
        {
            await GoToRoute(AppRoutes.EDIT_ADDRESS);
        }

        private async void ExternalLinksTapped(object sender, EventArgs e)
        {
            await GoToRoute(AppRoutes.EDIT_EXTERNAL_LINKS);
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    Navigation.PopToRootAsync();
        //    return true;
        //}
    }
}