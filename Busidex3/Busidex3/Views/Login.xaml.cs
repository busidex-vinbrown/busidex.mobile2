using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentPage
	{
        private LoginVM viewModel = new LoginVM();

		public Login ()
		{
			InitializeComponent ();

		    BindingContext = viewModel;
		}

	    private async void BtnLogin_OnClicked(object sender, EventArgs e)
	    {
	        await viewModel.DoLogin();
	    }
	}
}