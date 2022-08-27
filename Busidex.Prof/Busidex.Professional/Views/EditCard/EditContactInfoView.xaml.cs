using System;
using Busidex.Professional.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditContactInfoView : BaseEditCardView
    {
        public EditContactInfoView ()
		{
			InitializeComponent ();

            Title = "How Will They Contact You?";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
