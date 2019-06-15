using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Startup : ContentPage
    {
        public Startup()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            App.LoadLoginPage();
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            App.LoadProfilePage();
        }
    }
}