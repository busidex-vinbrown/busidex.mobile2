using Busidex.Professional.Views;
using Busidex.Professional.Views.EditCard;
using Busidex.Resources.String;
using Xamarin.Forms;

namespace Busidex.Professional
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(AppRoutes.HOME, typeof(HomeMenuView));
            //Routing.RegisterRoute(AppRoutes.LOGIN, typeof(LoginView));
            Routing.RegisterRoute(AppRoutes.ACCOUNT, typeof(MyProfileView));
            Routing.RegisterRoute(AppRoutes.MY_PROFILE, typeof(MyProfileView));
            Routing.RegisterRoute(AppRoutes.MY_BUSIDEX, typeof(MyBusidexView));
            Routing.RegisterRoute(AppRoutes.SHARE, typeof(ShareView));
            Routing.RegisterRoute(AppRoutes.CARD_DETAIL, typeof(CardDetailView));
            Routing.RegisterRoute(AppRoutes.NOTES, typeof(NotesView));
            Routing.RegisterRoute(AppRoutes.CARD_IMAGE, typeof(CardImageView));
            Routing.RegisterRoute(AppRoutes.PHONE, typeof(PhoneView));
            Routing.RegisterRoute(AppRoutes.SEARCH, typeof(SearchView));
            Routing.RegisterRoute(AppRoutes.QUICKSHARE, typeof(QuickShareView));
            Routing.RegisterRoute(AppRoutes.CARD_EDIT_MENU, typeof(EditCardMenuView));
            Routing.RegisterRoute(AppRoutes.CONFIRM_OWNER, typeof(ConfirmCardOwnerView));
            Routing.RegisterRoute(AppRoutes.EDIT_VISIBILITY, typeof(EditVisibilityView));
            Routing.RegisterRoute(AppRoutes.EDIT_CONTACT_INFO, typeof(EditContactInfoView));
            Routing.RegisterRoute(AppRoutes.EDIT_ADDRESS, typeof(EditAddressView));
            Routing.RegisterRoute(AppRoutes.EDIT_CARD_IMAGE, typeof(EditCardImageView));
            Routing.RegisterRoute(AppRoutes.EDIT_EXTERNAL_LINKS, typeof(EditExternalLinksView));
            Routing.RegisterRoute(AppRoutes.EDIT_SEARCH_INFO, typeof(EditSearchInfoView));
            Routing.RegisterRoute(AppRoutes.EDIT_TAGS, typeof(EditTagsView));
        }
        
        protected override bool OnBackButtonPressed()
        {
            // true or false to disable or enable the action
            return base.OnBackButtonPressed();
        }
    }
}
