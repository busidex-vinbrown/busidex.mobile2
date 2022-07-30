using System;
using System.Linq;
using System.Reflection;
using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Constants;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.SharedUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardDetailView
	{
        protected CardVM _viewModel { get; set; }
		

		public CardDetailView (ref CardVM vm)
		{
		    InitializeComponent ();

		    _viewModel = vm;
			_viewModel.DisplaySettings = new UserCardDisplay(
				DisplaySetting.Detail,
				vm.SelectedCard.Card.FrontOrientation == "H"
					? CardOrientation.Horizontal
					: CardOrientation.Vertical,
				vm.SelectedCard.Card.FrontFileName,
				vm.SelectedCard.Card.FrontOrientation);

			BindingContext = _viewModel;

			LoadButtons();
            Title = vm.SelectedCard.Card.Name ?? vm.SelectedCard.Card.CompanyName;

            Header.OnCardImageClicked += Header_OnCardImageClicked;
		    App.AnalyticsManager.TrackScreen(ScreenName.CardDetail);

            _viewModel.NotesButtonOpacity = vm.SelectedCard.Card.ExistsInMyBusidex ? 1 : .3;
			_viewModel.UrlButtonOpacity = vm.HasUrl ? 1 : .3;
			_viewModel.EmailButtonOpacity = vm.HasEmail ? 1 : .3;
			_viewModel.AddressButtonOpacity = vm.HasAddress ? 1 : .3;

			btnFacebook.IsVisible = false;
			btnTwitter.IsVisible = false;
			btnInstagram.IsVisible = false;
			btnLinkedIn.IsVisible = false;
			btnShareOwner.IsVisible = false;

			var linkCol = 1;
			var linkRow = 2;
			if (_viewModel.SelectedCard.Card.ExternalLinks.SingleOrDefault(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.Facebook) != null)
            {
				setExternalLinkButton(btnFacebook, ref linkCol, ref linkRow);
			}
			if (_viewModel.SelectedCard.Card.ExternalLinks.SingleOrDefault(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.Twitter) != null)
			{
				setExternalLinkButton(btnTwitter, ref linkCol, ref linkRow);
			}
			if (_viewModel.SelectedCard.Card.ExternalLinks.SingleOrDefault(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.Instagram) != null)
			{
				setExternalLinkButton(btnInstagram, ref linkCol, ref linkRow);
			}
			if (_viewModel.SelectedCard.Card.ExternalLinks.SingleOrDefault(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.LinkedIn) != null)
			{
				setExternalLinkButton(btnLinkedIn, ref linkCol, ref linkRow);
			}
			if (Security.CurrentUser.IsAdmin)
			{
				setExternalLinkButton(btnShareOwner, ref linkCol, ref linkRow);
			}
		}

        private async void Home_Clicked(object sender, EventArgs e)
        {
			await Navigation.PopToRootAsync();
        }

        private void setExternalLinkButton(Image img, ref int col, ref int row)
        {
			img.IsVisible = true;
			Grid.SetRow(img, row);
			Grid.SetColumn(img, col);
			col++;
			if (col > 2)
			{
				col = 0;
				row++;
			}
		}

        private async void Header_OnCardImageClicked(IUserCardDisplay ucd)
        {
			var uc = _viewModel.SelectedCard;
			await Shell.Current.Navigation.PushAsync(new CardImageView(ref uc));
			//await Navigation.PushAsync(new CardImageView(ref uc));
		}

        private void LoadButtons()
	    {
	        btnMap.Source = ImageSource.FromResource("Busidex.Resources.Images.maps.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            btnNotes.Source = ImageSource.FromResource("Busidex.Resources.Images.notes.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            btnEmail.Source = ImageSource.FromResource("Busidex.Resources.Images.email.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            btnWeb.Source = ImageSource.FromResource("Busidex.Resources.Images.browser.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            btnPhone.Source = ImageSource.FromResource("Busidex.Resources.Images.phone.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            btnShare.Source = ImageSource.FromResource("Busidex.Resources.Images.share.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            btnAdd.Source = ImageSource.FromResource("Busidex.Resources.Images.add.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            btnRemove.Source = ImageSource.FromResource("Busidex.Resources.Images.remove.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
			btnFacebook.Source = ImageSource.FromResource("Busidex.Resources.Images.fb.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
			btnTwitter.Source = ImageSource.FromResource("Busidex.Resources.Images.twitter.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
			btnInstagram.Source = ImageSource.FromResource("Busidex.Resources.Images.instagram.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
			btnLinkedIn.Source = ImageSource.FromResource("Busidex.Resources.Images.linkedin.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
			btnShareOwner.Source = ImageSource.FromResource("Busidex.Resources.Images.shareowner.png", typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
		}

	    private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        var uc = _viewModel.SelectedCard;
			//await Navigation.PushAsync(new CardImageView(ref uc));
			await Shell.Current.Navigation.PushAsync(new CardImageView(ref uc));
		}

	    private async void ButtonTapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        var option = (CardActionButton) ((TappedEventArgs)e).Parameter;
            
	        switch (option)
	        {
	            case CardActionButton.Maps:
                    _viewModel.LaunchMapApp();
	                break;
	            case CardActionButton.Notes:
					//await Navigation.PushAsync(new NotesView(_viewModel));
					await Shell.Current.Navigation.PushAsync(new NotesView(_viewModel));
					break;
	            case CardActionButton.Email:
                    _viewModel.LaunchEmail();
	                break;
	            case CardActionButton.Web:
                    await _viewModel.LaunchBrowser(_viewModel.SelectedCard.Card.Url);
	                break;
	            case CardActionButton.Phone:
					//await Navigation.PushAsync(new PhoneView(_viewModel));
					await Shell.Current.Navigation.PushAsync(new PhoneView(_viewModel));
					break;
	            case CardActionButton.Share:
	                var uc = _viewModel.SelectedCard;
                    var page = new ShareView(ref uc)
                    {
                        Title = $"Share {uc.Card.Name ?? uc.Card.CompanyName}"
                    };
					await Shell.Current.Navigation.PushAsync(page);
					break;
	            case CardActionButton.Tags:
	                break;
	            case CardActionButton.Add:
                    _viewModel.ShowSpinner = true;
                    await _viewModel.AddToMyBusidex();
                    _viewModel.ShowSpinner = false;
	                break;
	            case CardActionButton.Remove:
	                if (!await DisplayAlert("Remove", "Are you sure you want to remove this card from your collection?", "Yes", "Cancel")) return;
                    _viewModel.ShowSpinner = true;
                    _viewModel.RemoveFromMyBusidex();
                    _viewModel.ShowSpinner = false;
	                break;
				case CardActionButton.Facebook:
					if(!await _viewModel.LaunchBrowser(_viewModel.SelectedCard.Card.ExternalLinks.Single(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.Facebook).Link))
                    {
						await DisplayAlert("Invalid Link", $"The Facebook link is invalid.", "OK");
					}
					break;
				case CardActionButton.Twitter:
					if(!await _viewModel.LaunchBrowser(_viewModel.SelectedCard.Card.ExternalLinks.Single(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.Twitter).Link))
                    {
						await DisplayAlert("Invalid Link", $"The Twitter link is invalid.", "OK");
					}
					break;
				case CardActionButton.Instagram:
					if(!await _viewModel.LaunchBrowser(_viewModel.SelectedCard.Card.ExternalLinks.Single(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.Instagram).Link))
                    {
						await DisplayAlert("Invalid Link", $"The Instagram link is invalid.", "OK");
					}
					break;
				case CardActionButton.Linkedin:
					if(!await _viewModel.LaunchBrowser(_viewModel.SelectedCard.Card.ExternalLinks.Single(l => l.ExternalLinkTypeId == (int)ExternalLinkTypes.LinkedIn).Link))
                    {
						await DisplayAlert("Invalid Link", $"The LinkedIn link is invalid.", "OK");
					}
					break;
				case CardActionButton.ShareOwner:
					var ownerUc = _viewModel.SelectedCard;
					var showShareOwner = true;
					var ownerPage = new ShareView(ref ownerUc, showShareOwner)
					{
						Title = $"Share {ownerUc.Card.Name ?? ownerUc.Card.CompanyName}"
					};
					await Shell.Current.Navigation.PushAsync(ownerPage);
					break;
				default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }

	    private async void CardImageHeader_OnTapped(object sender, TappedEventArgs e)
	    {
            var uc = e.Parameter as UserCard;;
			//await Navigation.PushAsync(new CardImageView(ref uc));
			await Shell.Current.Navigation.PushAsync(new CardImageView(ref uc));

		}        
    }
}