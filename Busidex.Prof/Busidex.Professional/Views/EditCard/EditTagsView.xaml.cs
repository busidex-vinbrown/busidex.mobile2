using System;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditTagsView : BaseEditCardView
	{
        public EditTagsView ()
		{
			InitializeComponent ();

            Title = "Tags";            

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.SaveTags();
        }

        protected override bool OnBackButtonPressed()
        {
            if (_viewModel.AllowSave)
            {
                return base.OnBackButtonPressed();
            }
            return true;       
        }
    }
}