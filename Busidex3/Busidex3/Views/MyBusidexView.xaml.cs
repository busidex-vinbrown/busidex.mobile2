using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyBusidexView
	{
	    private readonly MyBusidexVM _viewModel = new MyBusidexVM();

        protected override async void OnAppearing()
        {
            await _viewModel.Init();
            base.OnAppearing();
        }

        public MyBusidexView ()
		{
			InitializeComponent ();
		    BindingContext = _viewModel;
		    Task.Factory.StartNew(async () => { await _viewModel.Init(); });
		    
		    lstMyBusidex.RefreshCommand = RefreshCommand;

		    _viewModel.ShowFilter = true;

		    App.AnalyticsManager.TrackScreen(ScreenName.MyBusidex);
		}

        
	    public ICommand RefreshCommand
	    {
	        get { return new Command(async () => { await _viewModel.LoadUserCards(); }); }
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

        private async void BtnGoToSearch_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchView());
        }
    }
}