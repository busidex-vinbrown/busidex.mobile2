using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using Busidex.Professional.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrganizationReferralsView
    {
        private readonly OrganizationReferralsVM _viewModel;

        public OrganizationReferralsView(Organization org)
        {
            InitializeComponent();
            Title = org.Name;

            _viewModel = new OrganizationReferralsVM(org);
        }

        protected override void OnAppearing()
        {
            var cachedPath = Path.Combine(Serialization.LocalStorageFolder, _viewModel.OrganizationReferralsFile);
            Task.Factory.StartNew(async () => { await _viewModel.Init(cachedPath); });

            BindingContext = _viewModel;

            lstCards.RefreshCommand = RefreshCommand;

            App.AnalyticsManager.TrackScreen(ScreenName.OrganizationReferrals);

            base.OnAppearing();
        }

        private void TxtSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            _viewModel.DoSearch();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                _viewModel.SetFilteredList(_viewModel.UserCards);
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var uc = ((TappedEventArgs)e).Parameter as UserCard;
            var cards = Serialization.GetCachedResult<List<UserCard>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));
            var myBusidex = new List<UserCard>();
            myBusidex.AddRange(cards);
            var newViewModel = new CardVM(ref uc, ref myBusidex);

            await Navigation.PushAsync(new CardDetailView(ref newViewModel));
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () => {
                    _viewModel.IsRefreshing = true;
                    await _viewModel.LoadUserCards(_viewModel.OrganizationReferralsFile);
                });
            }
        }
    }
}