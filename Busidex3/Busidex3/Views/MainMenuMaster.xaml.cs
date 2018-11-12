using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    public delegate void LogoutResult();

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuMaster
    {
        public ListView ListView;
        public event LogoutResult OnLogout;

        public MainMenuMaster()
        {
            InitializeComponent();

            BindingContext = new MainMenuMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MainMenuMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainMenuMenuItem> MenuItems { get; set; }
            
            public MainMenuMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                {
                    new MainMenuMenuItem { Id = 0, Title = ViewNames.MyBusidex, TargetType = typeof(MyBusidexView), Image = ImageSource.FromResource("Busidex3.Resources.mybusidexicon.png",
                        typeof(MainMenuMenuItem).GetTypeInfo().Assembly)},
                    new MainMenuMenuItem { Id = 1, Title = ViewNames.Search, TargetType = typeof(SearchView), Image = ImageSource.FromResource("Busidex3.Resources.searchicon.png",
                        typeof(MainMenuMenuItem).GetTypeInfo().Assembly)},
                    new MainMenuMenuItem { Id = 2, Title = ViewNames.Events, TargetType = typeof(EventsView), Image = ImageSource.FromResource("Busidex3.Resources.eventicon.png",
                        typeof(MainMenuMenuItem).GetTypeInfo().Assembly)},
                    new MainMenuMenuItem { Id = 3, Title = ViewNames.Organizations, TargetType = typeof(OrganizationsView), Image = ImageSource.FromResource("Busidex3.Resources.organizationsicon.png",
                        typeof(MainMenuMenuItem).GetTypeInfo().Assembly)},
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

        private async void BtnLogout_OnClicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "Cancel")) return;

            var localPath = Path.Combine (Serialization.GetAppLocalStorageFolder(), Busidex3.StringResources.AUTHENTICATION_COOKIE_NAME + ".txt");
            if (!File.Exists(localPath)) return;

            File.Delete(localPath);

            OnLogout?.Invoke();
        }
    }
}