﻿using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrganizationsView : ContentPage
	{
        private readonly OrganizationListVM _viewModel = new OrganizationListVM();

        public OrganizationsView ()
		{
			InitializeComponent ();
		    App.AnalyticsManager.TrackScreen(ScreenName.Organizations);

            _viewModel.OrganizationList = Serialization.GetCachedResult<List<Organization>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_ORGANIZATIONS_FILE));
            BindingContext = _viewModel;
            Title = "Organizations";
            _viewModel.HeaderFont = Device.RuntimePlatform == Device.Android
                ? NamedSize.Medium
                : NamedSize.Header;
        }

        private void OnDetail_Tapped(object sender, System.EventArgs e)
        {
            var organization = ((TappedEventArgs)e).Parameter as Organization;
            var page = new OrganizationDetailView(organization);
            Navigation.PushAsync(page);
        }        
    }
}