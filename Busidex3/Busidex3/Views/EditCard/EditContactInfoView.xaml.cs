using System;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditContactInfoView
	{
        protected CardVM _viewModel { get; set; }
         

		public EditContactInfoView (ref CardVM vm)
		{
			InitializeComponent ();

            var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            
            Title = "How Will They Contact You?";
            _viewModel = vm;
            
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
