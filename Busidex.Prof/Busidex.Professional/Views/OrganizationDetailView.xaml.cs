using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrganizationDetailView : ContentPage
    {
        private readonly OrganizationDetailVM _viewModel;

        public OrganizationDetailView(Organization org, bool isMember = true)
        {
            InitializeComponent();

            Title = org.Name;

            _viewModel = new OrganizationDetailVM(org, isMember);
            BindingContext = _viewModel;
        }

        public async void WebView_Navigating(object sender, WebNavigatingEventArgs args)
        {
            if (args.Url.StartsWith("file://"))
            {
                return;
            }

            await Launcher.TryOpenAsync(new Uri(args.Url));

            args.Cancel = true;
        }

        private void OnMembers_Tapped(object sender, System.EventArgs e) {
            var page = new OrganizationMembersView(_viewModel.Organization);
            Navigation.PushAsync(page);
        }

        private void OnReferrals_Tapped(object sender, System.EventArgs e) {
            var page = new OrganizationReferralsView(_viewModel.Organization);
            Navigation.PushAsync(page);
        }

        private void img_Tapped(object sender, System.EventArgs e)
        {
            var url = ((TappedEventArgs)e).Parameter.ToString();
            _viewModel.LaunchBrowser(url);
        }

        private void phone_Tapped(object sender, System.EventArgs e)
        {
            var number = ((TappedEventArgs)e).Parameter.ToString();
            _viewModel.DialPhoneNumber(number);
        }

        private void email_Tapped(object sender, System.EventArgs e)
        {
            var email = ((TappedEventArgs)e).Parameter.ToString();
            _viewModel.SendEmail(email);
        }
    }
}