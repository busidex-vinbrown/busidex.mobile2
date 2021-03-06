﻿using System;
using Busidex.Resources.String;
using Busidex3.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login
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

	    private void TxtEmail_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        Reset();
	    }

	    private void TxtPassword_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        Reset();
	    }

	    private async void BtnForgotPassword_OnClicked(object sender, EventArgs e)
	    {
	        var uri = new Uri(StringResources.FORGOT_PASSWORD_URL);
	        await Launcher.OpenAsync(uri);
	    }

        private void BtnCreate_Clicked(object sender, EventArgs e)
        {
			var page = new MyProfileView();
			Navigation.PushAsync(page);
			NavigationPage.SetHasNavigationBar(page, false);
		}
    }
}