using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditContactInfoView : ContentPage
	{
        protected readonly EditCardVM _viewModel = new EditCardVM();

		public EditContactInfoView (ref UserCard card)
		{
			InitializeComponent ();

            var fileName = card.DisplaySettings.CurrentFileName;

            card.DisplaySettings = new UserCardDisplay(fileName: fileName);
            _viewModel.SelectedCard = card;
            BindingContext = _viewModel;
		}
	}
}