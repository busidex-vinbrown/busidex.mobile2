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
	   
		public MyBusidexView ()
		{
			InitializeComponent ();
		    BindingContext = _viewModel;
		    Task.Factory.StartNew(async () => { await _viewModel.Init(); });
		    
		    lstMyBusidex.RefreshCommand = RefreshCommand;

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

	    private async void TapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
	    {
	        int.TryParse(e.Parameter.ToString(), out int id);
	        var card = _viewModel.UserCards.SingleOrDefault(uc => uc.UserCardId == id);
            var newViewModel = new CardVM(ref card);
	        await Navigation.PushAsync(new CardDetailView(ref newViewModel));
	    }
	}
}