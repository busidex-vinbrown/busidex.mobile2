using System;
using System.IO;
using Busidex3.Services.Utils;
using Newtonsoft.Json;
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
            MasterPage.OnLogout += Logout;

            var needsLogin = false;
            var localPath = Path.Combine (Serialization.GetAppLocalStorageFolder(), Busidex3.Resources.AUTHENTICATION_COOKIE_NAME + ".txt");
            if (File.Exists(localPath))
            {
                var cookieText = File.ReadAllText(localPath);

                var cookie = JsonConvert.DeserializeObject<System.Net.Cookie>(cookieText);

                if (string.IsNullOrEmpty(cookie?.Value))
                {
                    needsLogin = true;
                }
            }
            else
            {
                needsLogin = true;
            }

            if (needsLogin)
            {
                Logout();
            }
        }

        private void Logout()
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