using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
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
            base.OnAppearing();
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(UserCardJson));
            var uc = JsonConvert.DeserializeObject<UserCard>(json);
            _viewModel = new QuickShareVM(uc);
            _viewModel.Greeting = $"Congratulations! Your card is now on Busidex. Take a moment to review your card information by tapping the button below.";
            _viewModel.PersonalMessage = LinkMessage;
            BindingContext = _viewModel;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.UpdateOwner(uc.CardId, Security.CurrentUser.UserId);
                await _viewModel.AddCardToMyBusidex(uc.CardId);
                Serialization.RemoveQuickShareLink();
            });
        }

        private async void BtnContinue_Clicked(object sender, System.EventArgs e)
        {
            await App.LoadOwnedCard();
            await App.LoadMyBusidex();
            await Shell.Current.GoToAsync("home");
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync("home").Wait();
            return true;
        }
    }
}