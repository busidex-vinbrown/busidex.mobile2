using Busidex.Professional.Views;
using Busidex.Professional.Views.EditCard;
using Xamarin.Forms;

namespace Busidex.Professional
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("home", typeof(HomeMenuView));
            Routing.RegisterRoute("login", typeof(Login));
            Routing.RegisterRoute("account", typeof(MyProfileView));
            Routing.RegisterRoute("mybusidex", typeof(MyBusidexView));
            Routing.RegisterRoute("share", typeof(ShareView));
            Routing.RegisterRoute("card-detail", typeof(CardDetailView));
            Routing.RegisterRoute("notes", typeof(NotesView));
            Routing.RegisterRoute("card-image", typeof(CardImageView));
            Routing.RegisterRoute("phone", typeof(PhoneView));
            Routing.RegisterRoute("search", typeof(SearchView));
            Routing.RegisterRoute("card-edit-menu", typeof(EditCardMenuView));
            Routing.RegisterRoute("edit-visibility", typeof(EditVisibilityView));
        }
    }
}
