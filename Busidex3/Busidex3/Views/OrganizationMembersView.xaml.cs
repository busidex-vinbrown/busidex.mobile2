using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrganizationMembersView : ContentPage
    {
        private readonly OrganizationMembersVM _viewModel;

        public OrganizationMembersView(Organization org)
        {
            InitializeComponent();
            Title = org.Name;

            _viewModel = new OrganizationMembersVM();
            var cachedPath = Path.Combine(Serialization.LocalStorageFolder, _viewModel.OrganizationFile);
            Task.Factory.StartNew(async () => { await _viewModel.Init(cachedPath); });

            BindingContext = _viewModel;

            lstCards.RefreshCommand = RefreshCommand;

            App.AnalyticsManager.TrackScreen(ScreenName.OrganizationMembers);

        }

        private void TxtSearch_SearchButtonPressed(object sender, EventArgs e)
        {

        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () => {
                    _viewModel.IsRefreshing = true;
                    await _viewModel.LoadUserCards(_viewModel.OrganizationFile);
                });
            }
        }
    }
}