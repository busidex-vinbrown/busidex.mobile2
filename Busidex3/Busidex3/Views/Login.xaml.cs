using System;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentPage
	{
        private readonly LoginVM _viewModel = new LoginVM();

		public Login ()
		{
			InitializeComponent ();

		    BindingContext = _viewModel;
		}

        protected override bool OnBackButtonPressed()
        {
            App.LoadStartupPage();
            return true;
        }

        private async void BtnLogin_OnClicked(object sender, EventArgs e)
	    {
	        btnLogin.IsVisible = false;
	        btnForgotPassword.IsVisible = false;
	        prgSpinner.IsVisible = true;

	        var loggedIn = await _viewModel.DoLogin();
	        if (loggedIn)
	        {
	            lblLoginError.IsVisible = false;
	            App.Reload();
	        }
	        else
	        {
	            btnLogin.IsVisible = true;
	            btnForgotPassword.IsVisible = true;
	            prgSpinner.IsVisible = false;
	            lblLoginError.IsVisible = true;
	        }
	    }

	    private void Reset()
	    {
	        lblLoginError.IsVisible = false;
	    }

	    private void TxtUserName_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        Reset();
	    }

	    private void TxtPassword_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        Reset();
	    }

	    private void BtnForgotPassword_OnClicked(object sender, EventArgs e)
	    {
	        var uri = new Uri(StringResources.FORGOT_PASSWORD_URL);
	        Device.OpenUri(uri);
	    }
	}
}