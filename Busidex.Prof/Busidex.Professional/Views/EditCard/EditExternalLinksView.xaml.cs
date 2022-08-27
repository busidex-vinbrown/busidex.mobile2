using System;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditExternalLinksView : BaseEditCardView
    {
        public EditExternalLinksView()
        {
            InitializeComponent();

            Title = "External Links";            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.SaveExternalLinks();
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