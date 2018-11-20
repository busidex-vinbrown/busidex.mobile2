using System;
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
            }
        }

        private void RedirectToLogin()
        {
            var page = (Page)Activator.CreateInstance(typeof(Login));
            Detail = page;// new NavigationPage(page);
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