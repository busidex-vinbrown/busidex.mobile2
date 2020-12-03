using System;
using Busidex.Models.Constants;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditSearchInfoView
	{
        protected CardVM _viewModel { get; set; }
        public UserCardDisplay DisplaySettings { get; set; }

        public EditSearchInfoView (ref CardVM vm)
		{
			InitializeComponent ();

            // var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            Title = "How will your card be found?";

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
            await _viewModel.SaveSearchInfo();
        }
    }
}