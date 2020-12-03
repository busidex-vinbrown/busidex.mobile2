using Busidex.Models.Constants;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneView : ContentPage
    {
        public CardVM _viewModel { get; set; }

        public PhoneView(CardVM vm)
        {
            InitializeComponent();          

            _viewModel = vm;

            Title = vm.SelectedCard.Card.Name ?? vm.SelectedCard.Card.CompanyName;
            BindingContext = _viewModel;
            _viewModel.DisplaySettings = new UserCardDisplay(
                DisplaySetting.Detail,
                vm.SelectedCard.Card.FrontOrientation == "H"
                    ? CardOrientation.Horizontal
                    : CardOrientation.Vertical,
                vm.SelectedCard.Card.FrontFileName,
                vm.SelectedCard.Card.FrontOrientation);
        }
    }
}