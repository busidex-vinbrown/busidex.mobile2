using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotesView : ContentPage
	{
	    public CardVM _viewModel { get; set; }

	    public NotesView(CardVM vm)
	    {
	        InitializeComponent();

	        _viewModel = vm;
	        BindingContext = _viewModel;
	    }
	}
}