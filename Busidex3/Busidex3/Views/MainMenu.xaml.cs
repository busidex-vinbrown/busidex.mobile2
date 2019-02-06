using System;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu
    {
        public MainMenu()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            MasterPage.OnLogout += RedirectToLogin;
            MasterPage.OnShareClicked += MasterPage_OnShareClicked;
            MasterPage.OnCardEditClicked += MasterPage_OnCardEditClicked;

            if (string.IsNullOrEmpty(Security.AuthToken))
            {
                RedirectToLogin();
            }
            else
            {
                var page = (Page)Activator.CreateInstance(typeof(MyBusidexView));
                page.Title = ViewNames.MyBusidex;

                Detail = new NavigationPage(page);
                IsPresented = false;

                this.IsPresentedChanged += MainMenu_IsPresentedChanged;
            }            
        }

        private void MasterPage_OnCardEditClicked(ref UserCard card)
        {
            var page = new EditCardMenuView(ref card) {Title = "Edit My Card"};
            page.Title = "Edit My Card";

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }

        private void MasterPage_OnShareClicked(ref UserCard card)
        {
            var page = new ShareView(ref card) {Title = "Share My Card"};

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }

        private void MainMenu_IsPresentedChanged(object sender, EventArgs e)
        {
            if (IsPresented)
            {
                MasterPage.RefreshProfile();
            }
        }

        private void RedirectToLogin()
        {
            var page = (Page)Activator.CreateInstance(typeof(Login));
            Detail = page;
            IsPresented = false;
            NavigationPage.SetHasNavigationBar (Detail, false);
            IsGestureEnabled = false;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MainMenuMenuItem item))
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}