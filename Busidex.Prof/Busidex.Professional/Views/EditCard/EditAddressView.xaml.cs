using System;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditAddressView : BaseEditCardView
    {

        public EditAddressView ()
		{
			InitializeComponent ();

            Title = "What is your office address?";            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.SaveAddress();
        }
    }
}