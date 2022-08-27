using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using Busidex.Resources.String;

namespace Busidex.Professional.Views
{
    [QueryProperty(nameof(UserCardJson), "ucJson")]
    [QueryProperty(nameof(LinkFrom), "from")]
    [QueryProperty(nameof(LinkMessage), "message")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickShareView : ContentPage, IQueryAttributable, INotifyPropertyChanged
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
        public string LinkFrom {
            get => _linkFrom;
            set
            {
                _linkFrom = value;
            }
        }

        private string _linkMessage = "";
        public string LinkMessage { 
        get => _linkMessage;
        set
            {
                _linkMessage = value;
            }   
        }

        protected QuickShareVM _viewModel;
        
        public QuickShareView()
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
            _viewModel.Greeting = $"This referral was shared with you from {LinkFrom} and has been added to your collection.";
            _viewModel.PersonalMessage = LinkMessage;
            BindingContext = _viewModel;
            Task.Factory.StartNew(async () => await _viewModel.AddCardToMyBusidex(uc.CardId));

            Serialization.RemoveQuickShareLink();
        }
        private async void BtnContinue_Clicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync(AppRoutes.HOME);
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync(AppRoutes.HOME).Wait();
            return true;
        }
    }
}