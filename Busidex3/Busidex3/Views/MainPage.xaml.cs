using System.IO;
using System.Reflection;
using Busidex3.Services.Utils;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Busidex3.Views
{
    public partial class MainPage
    {

        public MainPage()
        {
            InitializeComponent();

            imgSearchIcon.Source = ImageSource.FromResource("Busidex3.Resources.searchicon.png",
                typeof(Resources).GetTypeInfo().Assembly);
            imgMyBusidexIcon.Source = ImageSource.FromResource("Busidex3.Resources.mybusidexicon.png",
                typeof(Resources).GetTypeInfo().Assembly);
            imgOrganizationsIcon.Source = ImageSource.FromResource("Busidex3.Resources.eventicon.png",
                typeof(Resources).GetTypeInfo().Assembly);
            imgEventIcon.Source = ImageSource.FromResource("Busidex3.Resources.searchicon.png",
                typeof(Resources).GetTypeInfo().Assembly);
            imgShareIcon.Source = ImageSource.FromResource("Busidex3.Resources.shareicon.png",
                typeof(Resources).GetTypeInfo().Assembly);

            //var localPath = Path.Combine (Serialization.GetAppLocalStorageFolder(), Busidex3.Resources.AUTHENTICATION_COOKIE_NAME + ".txt");
            //if (File.Exists(localPath))
            //{
            //    var cookieText = File.ReadAllText(localPath);

            //    var cookie = JsonConvert.DeserializeObject<System.Net.Cookie>(cookieText);

            //    if (string.IsNullOrEmpty(cookie?.Value))
            //    {
            //        Navigation.PushAsync(new Login());
            //    }
            //    else
            //    {
            //        Security.AuthToken = cookie.Value;
            //    }
            //}
            //else
            //{
            //    Navigation.PushAsync(new Login());
            //}
            
            //btnSearch.Image = Serialization.CopyIcon("searchicon.png").Result;
            //btnMyBusidex.Image = Serialization.CopyIcon("mybusidexicon.png").Result;
            //btnOrganization.Image = Serialization.CopyIcon("organizationsicon.png").Result;
            //btnEvents.Image = Serialization.CopyIcon("eventicon.png").Result;
            //btnShare.Image = Serialization.CopyIcon("shareicon.png").Result;
        }
    }
}
