using System;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditSearchInfoView
	{
        protected CardVM ViewModel { get; set; }

		public EditSearchInfoView (ref CardVM vm)
		{
			InitializeComponent ();

            var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            Title = "How will your card be found?";

            vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            ViewModel = vm;
            BindingContext = ViewModel;
		}

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await ViewModel.SaveSearchInfo();
        }
    }
}