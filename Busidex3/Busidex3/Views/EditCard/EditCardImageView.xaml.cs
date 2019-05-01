using System;
using Busidex3.ViewModels;
using Plugin.InputKit.Shared.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardImageView
	{
        protected CardVM ViewModel { get; set; }

		public EditCardImageView (ref CardVM vm)
		{
			InitializeComponent ();

            var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            Title = "Choose your card picture";

            vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            ViewModel = vm;
            BindingContext = ViewModel;

            var selectedOrientation = vm.FrontOrientation == "H"
                ? "Horizontal"
                : "Vertical";
            setControls(selectedOrientation);
		}

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await ViewModel.SaveCardImage(null);
        }

        private void BtnFront_OnClicked(object sender, EventArgs e)
        {
            btnFront.Style = (Style) Application.Current.Resources["toggleButtonOn"];
            btnBack.Style = (Style) Application.Current.Resources["toggleButtonOff"];
            ViewModel.SelectedSide = UserCardDisplay.CardSide.Front;
        }

        private void BtnBack_OnClicked(object sender, EventArgs e)
        {
            btnFront.Style = (Style) Application.Current.Resources["toggleButtonOff"];
            btnBack.Style = (Style) Application.Current.Resources["toggleButtonOn"];
            ViewModel.SelectedSide = UserCardDisplay.CardSide.Back;
        }

        private void BtnChooseImage_OnClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RadioButton_OnClicked(object sender, EventArgs e)
        {
            if (!(sender is RadioButton radio))
            {
                return;
            }

            setControls(radio.Text);
        }

        private void setControls(string selectedOrientation)
        {
            double vFrameHeight = 300;
            double vFrameWidth = 183;
            double vImageHeight = 250;
            double vImageWidth = 152;
            double hFrameHeight = 183;
            double hFrameWidth = 300;
            double hImageHeight = 152;
            double hImageWidth = 250;
            
            var frontOrientation = ViewModel.SelectedCard.Card.FrontOrientation;
            var backOrientation = ViewModel.SelectedCard.Card.BackOrientation;


            if (ViewModel.SelectedSide == UserCardDisplay.CardSide.Front)
            {
                frontOrientation = 
                    selectedOrientation == "Horizontal"
                        ? "H"
                        : "V";
            }
            if (ViewModel.SelectedSide == UserCardDisplay.CardSide.Back)
            {
                backOrientation = 
                    selectedOrientation == "Horizontal"
                        ? "H"
                        : "V";
            }

            frmSelectedCardImage.HeightRequest = frontOrientation == "H"
                ? hFrameHeight
                : vFrameHeight;
            frmSelectedCardImage.WidthRequest = frontOrientation == "H"
                ? hFrameWidth
                : vFrameWidth;
            imgSelectedCardImage.HeightRequest = frontOrientation == "H"
                ? hImageHeight
                : vImageHeight;
            imgSelectedCardImage.WidthRequest = frontOrientation == "H"
                ? hImageWidth
                : vImageWidth;
            ViewModel.SelectedCard.Card.FrontOrientation = frontOrientation;
            ViewModel.SelectedCard.Card.BackOrientation = backOrientation;
        }
    }
}