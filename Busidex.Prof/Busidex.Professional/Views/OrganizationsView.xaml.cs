using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using Busidex.Professional.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
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
            lstOrganizations.RefreshCommand = RefreshCommand;
        }

        private void OnDetail_Tapped(object sender, System.EventArgs e)
        {
            //var organization = ((TappedEventArgs)e).Parameter as Organization;
            //var page = new OrganizationDetailView(organization);
            //Navigation.PushAsync(page);
        }

        public ICommand RefreshCommand {
            get {
                return new Command(async () => {
                    _viewModel.IsRefreshing = true;
                    await _viewModel.LoadOrganizations();
                    _viewModel.IsRefreshing = false;
                });
            }
        }
    }
}