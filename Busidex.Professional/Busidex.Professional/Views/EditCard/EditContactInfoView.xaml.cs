using System;
using Busidex.Models.Constants;
using Busidex.Professional.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditContactInfoView
	{
        protected CardVM _viewModel { get; set; }
        public UserCardDisplay DisplaySettings { get; set; }

        public EditContactInfoView (ref CardVM vm)
		{
			InitializeComponent ();

            // var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            // vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);

            Title = "How Will They Contact You?";
            _viewModel = vm;
            _viewModel.DisplaySettings = new UserCardDisplay(
                DisplaySetting.Detail,
                vm.SelectedCard.Card.FrontOrientation == "H"
                    ? CardOrientation.Horizontal
                    : CardOrientation.Vertical,
                vm.SelectedCard.Card.FrontFileName,
                vm.SelectedCard.Card.FrontOrientation);

            BindingContext = _viewModel;
            _viewModel.SetViewHeightForOrientation(_viewModel.SelectedCard.Card.FrontOrientation);
        }

        
        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.SaveContactInfo();
        }

        private void AddPhoneNumberImage_OnTapped(object sender, EventArgs e)
        {
            _viewModel.AddNewPhoneNumber();
        }

        private void RemovePhoneNumberImage_OnTapped(object sender, EventArgs e)
        {
            var idx = _viewModel.PhoneNumbers.IndexOf(((TappedEventArgs)e).Parameter as PhoneNumberVM);
            _viewModel.RemovePhoneNumber(idx);
        }
    }
}
