using System;
using System.IO;
using System.Windows.Input;
using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using Busidex.Professional.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyBusidexView
	{
	    private MyBusidexVM _viewModel;

        public MyBusidexView ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            Title = ViewNames.MyBusidex;
            var cachedPath = Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                IsEnabled = true
            });

            if (_viewModel == null)
            {
                _viewModel = new MyBusidexVM();
                BindingContext = _viewModel;
                
                _viewModel.Init(cachedPath).ContinueWith((result) =>
                {
                    lstMyBusidex.RefreshCommand = RefreshCommand;

                    if (!_viewModel.HasCards)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            lstMyBusidex.IsVisible = false;
                            stkNoCards.IsVisible = true;
                        });
                    }
                });
            }
            else
            {
                _viewModel.LoadFromCache(cachedPath);
                lstMyBusidex.RefreshCommand = RefreshCommand;

                Device.BeginInvokeOnMainThread(() =>
                {
                    lstMyBusidex.IsVisible = _viewModel.HasCards;
                    stkNoCards.IsVisible = !_viewModel.HasCards;
                });
            }
            App.AnalyticsManager.TrackScreen(ScreenName.MyBusidex);
            
            base.OnAppearing();
        }

        private async void Home_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        public ICommand RefreshCommand
	    {
	        get { return new Command(async () => {
                _viewModel.IsRefreshing = true;
                await _viewModel.LoadUserCards(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));
            }); }
	    }

	    private void TxtSearch_OnSearchButtonPressed(object sender, EventArgs e)
	    {
            _viewModel.DoSearch();
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

            await Shell.Current.Navigation.PushAsync(new CardDetailView(ref newViewModel));

        }

        private async void BtnGoToSearch_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new SearchView());
            await Shell.Current.GoToAsync("search");
        }        
    }
}