using System;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Startup
    {
        private readonly StartUpVM _viewModel = new StartUpVM();

        public Startup()
        {
            InitializeComponent();

            BindingContext = _viewModel;
        }

        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
            App.LoadLoginPage();
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            App.LoadProfilePage();
        }
    }
}