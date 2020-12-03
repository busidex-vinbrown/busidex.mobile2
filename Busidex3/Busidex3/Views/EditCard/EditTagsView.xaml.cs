using System;
using Busidex.Models.Constants;
using Busidex3.ViewModels;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditTagsView
	{
        protected CardVM _viewModel { get; set; }
        public UserCardDisplay DisplaySettings { get; set; }

        public EditTagsView (ref CardVM vm)
		{
			InitializeComponent ();

            // var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            Title = "Tags";

            // vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            DisplaySettings = new UserCardDisplay(
                DisplaySetting.Detail,
                vm.SelectedCard.Card.FrontOrientation == "H"
                    ? CardOrientation.Horizontal
                    : CardOrientation.Vertical,
                vm.SelectedCard.Card.FrontFileName,
                vm.SelectedCard.Card.FrontOrientation);

            _viewModel = vm;
            BindingContext = _viewModel;
            _viewModel.SetViewHeightForOrientation(_viewModel.SelectedCard.Card.FrontOrientation);
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