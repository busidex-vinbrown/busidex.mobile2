using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Resources.String;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchView
	{
        private readonly SearchVM _viewModel = new SearchVM();

		public SearchView ()
		{
			InitializeComponent ();
		    BindingContext = _viewModel;		    
            Title = "Search";

		    App.AnalyticsManager.TrackScreen(ScreenName.Search);
		}

        protected override void OnDisappearing()
        {
            Task.Factory.StartNew(()=>_viewModel.RefreshList());
            base.OnDisappearing();
        }

        private void TxtSearch_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        if (string.IsNullOrEmpty(e.NewTextValue))
	        {
	            _viewModel.ClearSearch();
                imgBackground.IsVisible = true;
            }
	    }

	    private async void TxtSearch_OnSearchButtonPressed(object sender, EventArgs e)
	    {
	        await _viewModel.DoSearch();
            imgBackground.IsVisible = !_viewModel.HasCards;
        }

	    private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        var uc = ((TappedEventArgs)e).Parameter as UserCard;
	        var myBusidex = Serialization.LoadData<List<UserCard>>(
	            Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE));

	        var newViewModel = new CardVM(ref uc, ref myBusidex);

	        //await Navigation.PushAsync(new CardDetailView(ref newViewModel));
			await Shell.Current.Navigation.PushAsync(new CardDetailView(ref newViewModel));
		}
	}
}