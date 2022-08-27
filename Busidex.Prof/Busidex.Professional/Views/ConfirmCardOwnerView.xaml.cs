using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Resources.String;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [QueryProperty(nameof(UserCardJson), "ucJson")]
    [QueryProperty(nameof(LinkFrom), "from")]
    [QueryProperty(nameof(LinkMessage), "message")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmCardOwnerView : ContentPage, IQueryAttributable, INotifyPropertyChanged
    {
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _userCardJson = HttpUtility.UrlDecode(query["ucJson"]);
            _linkFrom = HttpUtility.UrlDecode(query["from"]);
            _linkMessage = HttpUtility.UrlDecode(query["message"]);
        }

        private string _userCardJson = "";
        public string UserCardJson
        {
            get => _userCardJson;
            set
            {
                _userCardJson = value;
            }
        }

        private string _linkFrom = "";
        public string LinkFrom
        {
            get => _linkFrom;
            set
            {
                _linkFrom = value;
            }
        }

        private string _linkMessage = "";
        public string LinkMessage
        {
            get => _linkMessage;
            set
            {
                _linkMessage = value;
            }
        }

        private QuickShareVM _viewModel;
        public UserCardDisplay DisplaySettings { get; set; }

        public ConfirmCardOwnerView()
        {
            InitializeComponent();
            Shell.SetNavBarIsVisible(this, false);

            App.AnalyticsManager.TrackScreen(ScreenName.Share);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            if(App.IsCardOwnerConfirmed)
            {
                Shell.Current.Navigation.PopAsync();
            }
            else
            {
                App.IsCardOwnerConfirmed = true;

                var ucJson = Encoding.UTF8.GetString(UserCardJson.FromHex());
                var uc = JsonConvert.DeserializeObject<UserCard>(ucJson);
                _viewModel = new QuickShareVM(uc);
                _viewModel.Greeting = $"Congratulations! Your card is now on Busidex. Take a moment to review your card information by tapping the button below.";
                _viewModel.PersonalMessage = LinkMessage;
                BindingContext = _viewModel;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    uc.Card.OwnerId = Security.CurrentUser.UserId;
                    uc.Card.Searchable = true;
                    uc.Card.Deleted = false;

                    await _viewModel.UpdateOwner(uc.CardId, Security.CurrentUser.UserId);
                    //await _viewModel.AddCardToMyBusidex(uc.CardId);
                    Serialization.RemoveQuickShareLink();

                    var path = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
                    Serialization.SaveResponse(JsonConvert.SerializeObject(uc.Card), path);

                });
            }
            base.OnAppearing();
        }

        private async void BtnContinue_Clicked(object sender, System.EventArgs e)
        {
            //await App.LoadOwnedCard(useThumbnail: false, mustSucceed: true);
            //await App.LoadMyBusidex();
            //var cardPath = Path.Combine(Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE);
            //var card = Serialization.LoadData<Card>(cardPath);

            //var uc = new UserCard(card);
            //var ucJson = JsonConvert.SerializeObject(uc).ToHexString();

            //await Shell.Current.GoToAsync($"{AppRoutes.CARD_EDIT_MENU}?ucJson={ucJson}");
            await Shell.Current.GoToAsync(AppRoutes.CARD_EDIT_MENU);
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync("home").Wait();
            return true;
        }
    }
}