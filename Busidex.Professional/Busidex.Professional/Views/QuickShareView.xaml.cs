using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickShareView : ContentPage
    {
        protected readonly QuickShareVM _viewModel;
        
        public QuickShareView(UserCard uc, string displayName, string message)
        {
            InitializeComponent();
            App.AnalyticsManager.TrackScreen(ScreenName.Share);

            // var fileName = card.DisplaySettings.CurrentFileName;

            // card.DisplaySettings = new UserCardDisplay(fileName: fileName);

            _viewModel = new QuickShareVM(uc);

            _viewModel.Greeting = string.Format("This referral was shared with you from {0} and has been added to your collection.", displayName);
            _viewModel.PersonalMessage = message;

            BindingContext = _viewModel;

            Task.Factory.StartNew(async () => await _viewModel.AddCardToMyBusidex(uc.CardId));

            Serialization.RemoveQuickShareLink();
        }

        private void BtnContinue_Clicked(object sender, System.EventArgs e)
        {
            //App.LoadHomePage();
            Navigation.PopToRootAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            //App.LoadHomePage();
            Navigation.PopToRootAsync();
            return true;
        }
    }
}