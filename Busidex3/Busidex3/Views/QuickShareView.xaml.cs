using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickShareView : ContentPage
    {
        protected readonly QuickShareVM _viewModel = new QuickShareVM();

        public QuickShareView(UserCard card, string displayName, string message)
        {
            InitializeComponent();
            App.AnalyticsManager.TrackScreen(ScreenName.Share);

            var fileName = card.DisplaySettings.CurrentFileName;

            card.DisplaySettings = new UserCardDisplay(fileName: fileName);
            _viewModel.SelectedCard = card;
            _viewModel.Greeting = string.Format("This referral was shared with you from {0} and has been added to your collection.", displayName);
            _viewModel.PersonalMessage = message;

            BindingContext = _viewModel;

            Task.Factory.StartNew(async () => await _viewModel.AddCardToMyBusidex(card.CardId));

            Serialization.RemoveQuickShareLink();
        }

        private void BtnContinue_Clicked(object sender, System.EventArgs e)
        {
            App.LoadHomePage();
        }

        protected override bool OnBackButtonPressed()
        {
            App.LoadHomePage();
            return true;
        }
    }
}