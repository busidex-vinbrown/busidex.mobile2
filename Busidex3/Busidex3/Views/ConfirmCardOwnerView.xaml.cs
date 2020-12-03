using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex3.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmCardOwnerView
    {
        private readonly QuickShareVM _viewModel;
        public UserCardDisplay DisplaySettings { get; set; }

        public ConfirmCardOwnerView(UserCard uc, string displayName, string message)
        {
            InitializeComponent();
            App.AnalyticsManager.TrackScreen(ScreenName.ConfirmOwner);

            // var fileName = card.DisplaySettings.CurrentFileName;

            //DisplaySettings = new UserCardDisplay(fileName: fileName);

            uc.Card.OwnerId = Security.CurrentUser.UserId;
            _viewModel = new QuickShareVM(uc);

            _viewModel.Greeting = string.Format("Congratulations! Your card is now on Busidex. Take a moment to review your card information by clicking the button below.");
            _viewModel.PersonalMessage = message;

            Task.Factory.StartNew(async () => await _viewModel.UpdateOwner(uc.CardId, Security.CurrentUser.UserId));
            Task.Factory.StartNew(async () => await _viewModel.AddCardToMyBusidex(uc.CardId));

            BindingContext = _viewModel;

            Serialization.RemoveQuickShareLink();
        }

        private void BtnContinue_Clicked(object sender, System.EventArgs e)
        {
            var uc = _viewModel.SelectedCard;
            App.LoadCardMenuPage(ref uc);
        }

        protected override bool OnBackButtonPressed()
        {
            var uc = _viewModel.SelectedCard;
            App.LoadCardMenuPage(ref uc);
            return true;
        }
    }
}