using System;
using Busidex.Models.Domain;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditVisibilityView
	{
        protected CardVM _viewModel { get; set; }

		public EditVisibilityView (ref CardVM vm)
		{
			InitializeComponent ();

            _viewModel = vm;
            BindingContext = _viewModel;
            Title = "Who Can See Your Card?";
            UpdatePage(_viewModel.SelectedCard.Card.Visibility);
            _viewModel.SetViewHeightForOrientation(_viewModel.SelectedCard.Card.FrontOrientation);
        }

        private void UpdatePage(CardVisibility value)
        {
            switch (value)
            {                    
                case CardVisibility.Public:
                {
                    rdoPublic.IsChecked = true;
                    lblSemiPublic.IsVisible = lblPrivate.IsVisible = false;
                    lblPublic.IsVisible = true;
                    break;
                }
                case CardVisibility.SemiPublic:
                {
                    rdoSemiPublic.IsChecked = true;
                    lblPublic.IsVisible = lblPrivate.IsVisible = false;
                    lblSemiPublic.IsVisible = true;
                    break;
                }
                case CardVisibility.Private:
                {
                    rdoPrivate.IsChecked = true;
                    lblSemiPublic.IsVisible = lblPublic.IsVisible = false;
                    lblPrivate.IsVisible = true;
                    break;
                }
            }
        }

        private void RdoPublic_OnClicked(object sender, EventArgs e)
        {
            _viewModel.SelectedCard.Card.Visibility = CardVisibility.Public;
            UpdatePage(_viewModel.SelectedCard.Card.Visibility);
        }

        private void RdoSemiPublic_OnClicked(object sender, EventArgs e)
        {
            _viewModel.SelectedCard.Card.Visibility = CardVisibility.SemiPublic;
            UpdatePage(_viewModel.SelectedCard.Card.Visibility);
        }

        private void RdoPrivate_OnClicked(object sender, EventArgs e)
        {
            _viewModel.SelectedCard.Card.Visibility = CardVisibility.Private;
            UpdatePage(_viewModel.SelectedCard.Card.Visibility);
        }

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            btnSave.IsEnabled = false;
            btnSave.Opacity = .3;
            bool error = false;
            try
            {
                var result = await _viewModel.SaveCardVisibility();
                if (!result)
                {
                    error = true;                
                }
            }
            catch 
            {
                error = true;
            }
            finally
            {
                if (error)
                {
                    await DisplayAlert("ERROR", "There was a problem saving your card", "Ok");
                }
                btnSave.IsEnabled = true;
                btnSave.Opacity = 1;
            }
        }
    }
}