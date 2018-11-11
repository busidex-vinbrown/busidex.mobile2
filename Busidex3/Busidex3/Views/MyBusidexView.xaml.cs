using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyBusidexView
	{
	    private readonly MyBusidexVM _viewModel = new MyBusidexVM();

		public MyBusidexView ()
		{
			InitializeComponent ();

		    Task.Factory.StartNew(async () => { await LoadData(); });
		   
		    lstMyBusidex.RefreshCommand = RefreshCommand;
		}

	    public ICommand RefreshCommand
	    {
	        get { return new Command(async () => { await _viewModel.LoadUserCards(); }); }
	    }

	    private async Task LoadData()
	    {
	        await _viewModel.Init();
	        BindingContext = _viewModel;
	    }
	}
}