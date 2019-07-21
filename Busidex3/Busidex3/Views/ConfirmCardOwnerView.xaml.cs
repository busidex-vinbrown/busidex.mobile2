using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmCardOwnerView
    {
        private readonly QuickShareVM _viewModel = new QuickShareVM();

        public ConfirmCardOwnerView(UserCard card, string displayName, string message)
        {
            InitializeComponent();
            App.AnalyticsManager.TrackScreen(ScreenName.ConfirmOwner);

            var fileName = card.DisplaySettings.CurrentFileName;

            card.DisplaySettings = new UserCardDisplay(fileName: fileName);

            card.Card.OwnerId = Security.CurrentUser.UserId;

            _viewModel.SelectedCard = card;
            _viewModel.Greeting = string.Format("Congratulations! Your card is now on Busidex. Take a moment to review your card information by clicking the button below.");
            _viewModel.PersonalMessage = message;

            Task.Factory.StartNew(async () => await _viewModel.UpdateOwner(card.CardId, Security.CurrentUser.UserId));
            Task.Factory.StartNew(async () => await _viewModel.AddCardToMyBusidex(card.CardId));

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