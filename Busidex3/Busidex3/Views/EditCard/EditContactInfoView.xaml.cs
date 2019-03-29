using System;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditContactInfoView
	{
        protected CardVM ViewModel { get; set; }
         

		public EditContactInfoView (ref CardVM vm)
		{
			InitializeComponent ();

            var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            
            Title = "How Will They Contact You?";
            ViewModel = vm;
            
            BindingContext = ViewModel;
            
		}

        
        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {
            await ViewModel.SaveContactInfo();
        }

        private void AddPhoneNumberImage_OnTapped(object sender, EventArgs e)
        {
            ViewModel.AddNewPhoneNumber();
        }

        private void RemovePhoneNumberImage_OnTapped(object sender, EventArgs e)
        {
            var idx = ViewModel.PhoneNumbers.IndexOf(((TappedEventArgs)e).Parameter as PhoneNumberVM);
            ViewModel.RemovePhoneNumber(idx);
        }
    }
}
