﻿using System;
using Busidex3.Analytics;
using Busidex3.DomainModels;
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

            Header.OnCardImageClicked += Header_OnCardImageClicked;
		    App.AnalyticsManager.TrackScreen(ScreenName.CardDetail);
		}

        private async void Header_OnCardImageClicked(DomainModels.UserCard uc)
        {
            
            await Navigation.PushAsync(new CardImageView(ref uc));
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
	        var uc = _viewModel.SelectedCard;
	        await Navigation.PushAsync(new CardImageView(ref uc));
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
	                var uc = _viewModel.SelectedCard;
	                var page = new ShareView(ref uc)
	                {
	                    Title = $"Share {uc.Card.Name ?? uc.Card.CompanyName}"
	                };
	                await Navigation.PushAsync(page);
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

	    private async void CardImageHeader_OnTapped(object sender, TappedEventArgs e)
	    {
            var uc = e.Parameter as UserCard;;
	        await Navigation.PushAsync(new CardImageView(ref uc));
	    }
	}
}