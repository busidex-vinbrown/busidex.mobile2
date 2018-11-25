using System;
using Busidex3.Analytics;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardDetailView
	{
        protected CardVM _viewModel { get; set; }
        
		public CardDetailView (ref CardVM vm)
		{
		    InitializeComponent ();

		    _viewModel = vm;
		    BindingContext = _viewModel;

		    LoadButtons();
		    
		    App.AnalyticsManager.TrackScreen(ScreenName.CardDetail);
		}

	    private void LoadButtons()
	    {
	        btnMap.Source = ImageSource.FromResource("Busidex3.Resources.maps.png");
            btnNotes.Source = ImageSource.FromResource("Busidex3.Resources.notes.png");
            btnEmail.Source = ImageSource.FromResource("Busidex3.Resources.email.png");
            btnWeb.Source = ImageSource.FromResource("Busidex3.Resources.browser.png");
            btnPhone.Source = ImageSource.FromResource("Busidex3.Resources.phone.png");
            btnShare.Source = ImageSource.FromResource("Busidex3.Resources.share.png");
            btnTag.Source = ImageSource.FromResource("Busidex3.Resources.tags.png");
            btnAdd.Source = ImageSource.FromResource("Busidex3.Resources.add.png");
            btnRemove.Source = ImageSource.FromResource("Busidex3.Resources.remove.png");
        }

	    private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        await Navigation.PushAsync(new CardImageView(_viewModel.SelectedCard));
	    }

	    private async void ButtonTapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
	    {
	        var option = (CardActionButton) e.Parameter;
            
	        switch (option)
	        {
	            case CardActionButton.Maps:
                    _viewModel.LaunchMapApp();
	                break;
	            case CardActionButton.Notes:
	                if (_viewModel.SelectedCard.ExistsInMyBusidex)
	                {
	                    await Navigation.PushAsync(new NotesView(_viewModel));
	                }
	                break;
	            case CardActionButton.Email:
                    _viewModel.LaunchEmail();
	                break;
	            case CardActionButton.Web:
                    _viewModel.LaunchBrowser();
	                break;
	            case CardActionButton.Phone:
	                await Navigation.PushAsync(new PhoneView(_viewModel));
	                break;
	            case CardActionButton.Share:
	                await Navigation.PushAsync(new ShareView(_viewModel));
	                break;
	            case CardActionButton.Tags:
	                break;
	            case CardActionButton.Add:
                    _viewModel.AddToMyBusidex();
	                break;
	            case CardActionButton.Remove:
	                if (!await DisplayAlert("Remove", "Are you sure you want to remove this card from your collection?", "Yes", "Cancel")) return;
                    _viewModel.RemoveFromMyBusidex();
	                break;
	            default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }
	}
}