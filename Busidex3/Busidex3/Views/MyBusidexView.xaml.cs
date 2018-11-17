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

		    App.AnalyticsManager.TrackScreen(ScreenName.MyBusidex);

		    Task.Factory.StartNew(async () => { await LoadData(); });
		   
		    lstMyBusidex.RefreshCommand = RefreshCommand;
		}

	    public ICommand RefreshCommand
	    {
	        get { return new Command(async () => { await _viewModel.LoadUserCards(); }); }
	    }

	    private async Task LoadData()
	    {
	        await _viewModel.Init();
	        BindingContext = _viewModel;
	    }

	    private void TxtSearch_OnSearchButtonPressed(object sender, EventArgs e)
	    {
	        var subset = from uc in _viewModel.UserCards
	            where (uc.Card.Name?.Contains(_viewModel.SearchValue) ?? false) ||
	                  (uc.Card.CompanyName?.Contains(_viewModel.SearchValue) ?? false) ||
	                  (uc.Card.Email?.Contains(_viewModel.SearchValue) ?? false) ||
	                  (uc.Card.Url?.Contains(_viewModel.SearchValue) ?? false) ||
	                  (uc.Card.PhoneNumbers?.Any(pn => pn.Number.Contains(_viewModel.SearchValue)) ?? false) ||
	                  (uc.Card.Tags?.Any(t => t.Text.Contains(_viewModel.SearchValue)) ?? false)
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
	        await Navigation.PushAsync(new CardDetailView(new CardVM(card)));
	    }
	}
}