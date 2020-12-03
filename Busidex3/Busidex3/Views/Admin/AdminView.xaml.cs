using Busidex.Models.Domain;
using Busidex3.ViewModels;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminView : ContentPage
    {
        private readonly AdminVM _viewModel = new AdminVM();

        public AdminView()
        {
            InitializeComponent();

            Task.Factory.StartNew(async ()=> await _viewModel.LoadUnownedCards());

            BindingContext = _viewModel;
        }

        private void TxtSearch_OnSearchButtonPressed(object sender, EventArgs e)
        {
            _viewModel.DoSearch();
        }

        private void TxtSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                _viewModel.SetFilteredList(_viewModel.UnownedCards);
            }
        }

        private async void btnSelect_Clicked(object sender, EventArgs e)
        {
            var uc = ((Button)sender).CommandParameter as UnownedCard;

            var page = new SendOwnerCardView(uc)
            {
                Title = $"Send {uc.Name ?? uc.CompanyName}"
            };
            await Navigation.PushAsync(page);
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopToRootAsync();
            //App.LoadHomePage();
            return true;
        }
    }
}