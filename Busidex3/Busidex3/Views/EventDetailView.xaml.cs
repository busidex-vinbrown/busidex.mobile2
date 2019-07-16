using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventDetailView : ContentPage
    {

        private readonly EventCardsVM _viewModel;
        public EventDetailView(EventTag e)
        {
            InitializeComponent();
            Title = e.Text;
            _viewModel = new EventCardsVM(e);
            Task.Factory.StartNew(async () => { await _viewModel.LoadUserCards(); });
            BindingContext = _viewModel;
            _viewModel.ShowFilter = true;

            lstCards.RefreshCommand = RefreshCommand;

            App.AnalyticsManager.TrackScreen(ScreenName.EventDetail);
        }

        private void TxtSearch_OnSearchButtonPressed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_viewModel.SearchValue)) return;

            var subset = from uc in _viewModel.UserCards
                         where (uc.Card.Name?.Contains(_viewModel.SearchValue) ?? false) ||
                               (uc.Card.CompanyName?.Contains(_viewModel.SearchValue) ?? false) ||
                               (uc.Card.Email?.Contains(_viewModel.SearchValue) ?? false) ||
                               (uc.Card.Url?.Contains(_viewModel.SearchValue) ?? false) ||
                               (uc.Card.PhoneNumbers?.Any(pn => !string.IsNullOrEmpty(pn.Number) && pn.Number.Contains(_viewModel.SearchValue)) ?? false) ||
                               (uc.Card.Tags?.Any(t => !String.IsNullOrEmpty(t.Text) && t.Text.Contains(_viewModel.SearchValue)) ?? false)
                         select uc;

            var filter = new ObservableRangeCollection<UserCard>();
            filter.AddRange(subset);
            _viewModel.SetFilteredList(filter);
        }

        private void TxtSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                _viewModel.SetFilteredList(_viewModel.UserCards);
            }
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var uc = ((TappedEventArgs)e).Parameter as UserCard;
            var myBusidex = _viewModel.UserCards;
            var newViewModel = new CardVM(ref uc, ref myBusidex);

            await Navigation.PushAsync(new CardDetailView(ref newViewModel));
        }

        public ICommand RefreshCommand
        {
            get { return new Command(async () => { await _viewModel.LoadUserCards(); }); }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TxtSearch_SearchButtonPressed(object sender, EventArgs e)
        {

        }

        private void BtnGoToSearch_Clicked(object sender, EventArgs e)
        {

        }
    }
}