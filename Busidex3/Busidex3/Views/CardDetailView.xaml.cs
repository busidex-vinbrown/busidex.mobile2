using System;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardDetailView
	{
        protected CardVM _viewModel { get; set; }
        
		public CardDetailView (CardVM vm)
		{
		    InitializeComponent ();

		    _viewModel = vm;
		    BindingContext = _viewModel;

		    LoadButtons();
		}

	    private async void LoadButtons()
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

	    private void ButtonTapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
	    {
	        var option = (CardActionButton) e.Parameter;
            
	        switch (option)
	        {
	            case CardActionButton.Maps:
                    _viewModel.LaunchMapApp();
	                break;
	            case CardActionButton.Notes:
	                break;
	            case CardActionButton.Email:
                    _viewModel.LaunchEmail();
	                break;
	            case CardActionButton.Web:
                    _viewModel.LaunchBrowser();
	                break;
	            case CardActionButton.Phone:
	                break;
	            case CardActionButton.Share:
	                break;
	            case CardActionButton.Tags:
	                break;
	            case CardActionButton.Add:
	                break;
	            case CardActionButton.Remove:
	                break;
	            default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }
	}
}