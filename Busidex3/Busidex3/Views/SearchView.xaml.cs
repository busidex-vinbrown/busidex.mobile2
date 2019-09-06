using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchView : ContentPage
	{
	    // private readonly MyBusidexVM _myBusidex = new MyBusidexVM();
        private readonly SearchVM _viewModel = new SearchVM();

		public SearchView ()
		{
			InitializeComponent ();
		    BindingContext = _viewModel;		    
            Title = "Search";

		    // Task.Factory.StartNew(async () => { await _myBusidex.Init(); });

		    App.AnalyticsManager.TrackScreen(ScreenName.Search);
		}

        //protected override bool OnBackButtonPressed()
        //{
        //    App.LoadMyBusidexPage();
        //    return true;
        //}

        private void TxtSearch_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        if (string.IsNullOrEmpty(e.NewTextValue))
	        {
	            _viewModel.ClearSearch();
	        }
	    }

	    private async void TxtSearch_OnSearchButtonPressed(object sender, EventArgs e)
	    {
	        await _viewModel.DoSearch();
	    }

	    private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        var uc = ((TappedEventArgs)e).Parameter as UserCard;
	        var myBusidex = Serialization.LoadData<List<UserCard>>(
	            Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));

	        var newViewModel = new CardVM(ref uc, ref myBusidex);

	        await Navigation.PushAsync(new CardDetailView(ref newViewModel));
	    }
	}
}