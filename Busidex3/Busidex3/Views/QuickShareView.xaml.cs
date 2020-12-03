using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using Busidex3.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
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

        private async void BtnContinue_Clicked(object sender, System.EventArgs e)
        {
            //App.LoadHomePage();
            
            var uc = _viewModel.SelectedCard;
            var cachedPath = Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE);
            var myBusidex = Serialization.GetCachedResult<List<UserCard>>(cachedPath) ?? new List<UserCard>();
            var newViewModel = new CardVM(ref uc, ref myBusidex);
            //await Navigation.PopToRootAsync();
            await Navigation.PushAsync(new CardDetailView(ref newViewModel));
        }

        protected override bool OnBackButtonPressed()
        {
            //App.LoadHomePage();
            Navigation.PopToRootAsync();
            return true;
        }
    }
}