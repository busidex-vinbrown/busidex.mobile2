using Busidex3.Analytics;
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
            BindingContext = _viewModel;
        }
    }
}