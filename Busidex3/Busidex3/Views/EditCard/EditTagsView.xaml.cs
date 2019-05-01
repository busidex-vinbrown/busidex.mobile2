using System;
using Busidex3.ViewModels;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditTagsView
	{
        protected CardVM ViewModel { get; set; }

		public EditTagsView (ref CardVM vm)
		{
			InitializeComponent ();

            var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            Title = "Tags";

            vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            ViewModel = vm;
            BindingContext = ViewModel;
		}

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await ViewModel.SaveTags();
        }        
    }
}