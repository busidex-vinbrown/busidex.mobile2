using System;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditSearchInfoView : BaseEditCardView
	{
        public EditSearchInfoView ()
		{
			InitializeComponent ();

            Title = "How will your card be found?";            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.SaveSearchInfo();
        }
    }
}