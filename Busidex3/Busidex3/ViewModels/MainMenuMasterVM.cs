using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Busidex3.Views;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class MainMenuMasterVM : INotifyPropertyChanged
    {
        public ObservableCollection<MainMenuMenuItem> MenuItems { get; set; }
            
        public MainMenuMasterVM()
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
}
