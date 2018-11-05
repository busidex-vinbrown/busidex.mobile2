using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuMaster : ContentPage
    {
        public ListView ListView;

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
                    new MainMenuMenuItem { Id = 0, Title = "My Busidex", TargetType = typeof(MyBusidexView), Image = ImageSource.FromResource("Busidex3.Resources.mybusidexicon.png",
                        typeof(Resources).GetTypeInfo().Assembly)},
                    new MainMenuMenuItem { Id = 1, Title = "Search", TargetType = typeof(SearchView), Image = ImageSource.FromResource("Busidex3.Resources.searchicon.png",
                        typeof(Resources).GetTypeInfo().Assembly)},
                    new MainMenuMenuItem { Id = 2, Title = "Events", TargetType = typeof(EventsView), Image = ImageSource.FromResource("Busidex3.Resources.eventicon.png",
                        typeof(Resources).GetTypeInfo().Assembly)},
                    new MainMenuMenuItem { Id = 3, Title = "Organizations", TargetType = typeof(OrganizationsView), Image = ImageSource.FromResource("Busidex3.Resources.organizationsicon.png",
                        typeof(Resources).GetTypeInfo().Assembly)},
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

        private void BtnLogout_OnClicked(object sender, EventArgs e)
        {
            // Logout here
        }
    }
}