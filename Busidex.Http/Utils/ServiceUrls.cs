namespace Busidex.Http.Utils
{
    public class ServiceUrls
    {
        private const string BASE_API_URL = "https://www.busidexapi.com/api/";

        public static string GetUnownedCardsUrl => BASE_API_URL + "Admin/UnownedCards";
        public static string SendOwnerEmailsUrl => BASE_API_URL + "Admin/SendOwnerEmails?cardId={0}&email={1}";


        public static string UpdateDisplayNameUrl => "Account/UpdateDisplayName?name=";
        public static string CheckAccountUrl => BASE_API_URL + "Registration/CheckAccount";
        public static string GetAccountUrl => BASE_API_URL + "Account/Get?id=0";
        public static string CheckUserNameUrl => BASE_API_URL + "UserName/IsEmailAvailabile?email={0}";
        public static string UpdateDeviceTypeUrl => BASE_API_URL + "User/UpdateDeviceType?deviceType=";
        public static string UpdateUserUrl => BASE_API_URL + "User/UpdateUser";

        public static string ActivityUrl => BASE_API_URL + "Activity";

        public static string CardDetailUrl => BASE_API_URL + "card/details/{0}";
        public static string MyCardUrl => BASE_API_URL + "card/Get";
        public static string SaveMobileCardImageUrl => BASE_API_URL + "card/SaveMobileCardImage";
        public static string SaveCardVisibilityUrl => BASE_API_URL + "card/SaveCardVisibility?visibility={0}";
        public static string SaveSearchInfoUrl => BASE_API_URL + "card/SaveCardSearchInfo";
        public static string SaveContactInfoUrl => BASE_API_URL + "card/SaveCardContactInfo";
        public static string SaveCardAddressUrl => BASE_API_URL + "card/SaveCardAddress";
        public static string SaveExternalLinksUrl => BASE_API_URL + "card/SaveCardExternalLinks";
        public static string SaveTagsUrl => BASE_API_URL + "card/SaveCardTags";
        public static string UpdateOwnerIdUrl => BASE_API_URL + "card/ConfirmCardOwner";

        public static string EmailTemplateUrl => BASE_API_URL + "EmailTemplate/Get?code={0}";

        public static string LoginUrl => BASE_API_URL + "Account/Login";

        public static string MyBusidexUrl => BASE_API_URL + "busidex?all=true";
        public static string AddToMyBusidexUrl => BASE_API_URL + "busidex?userId=0&cId={0}";
        public static string RemoveFromMyBusidexUrl => BASE_API_URL + "busidex?id={0}&userId=0";

        public static string UpdateNotesUrl => BASE_API_URL + "Notes?id={0}&notes={1}";

        public static string OrganizationListUrl => BASE_API_URL + "Organization";
        public static string OrganizationUrl => BASE_API_URL + "Organization?id={0}";
        public static string OrganizationMembersUrl => BASE_API_URL + "Organization/GetMembers?organizationId={0}";
        public static string OrganizationReferralsUrl => BASE_API_URL + "Organization/GetReferrals?organizationId={0}";

        public static string SearchUrl => BASE_API_URL + "search/Search";
        public static string SearchBySystemTagUrl => BASE_API_URL + "search/SystemTagSearch?systag={0}&ownedOnly=true";

        public static string GetEventTagsUrl => BASE_API_URL + "search/GetEventTags";
        public static string GetUserEventTagsUrl => BASE_API_URL + "search/GetUserEventTags";
        public static string ChangeUserNameUrl => BASE_API_URL + "User/ChangeUserName?userId=0&name={0}";
        public static string ChangePasswordUrl => BASE_API_URL + "Password";
        public static string ChangeEmailUrl => BASE_API_URL + "User/ChangeEmail?email={0}";

        public static string ShareCardUrl => BASE_API_URL + "SharedCard/Post";
        public static string GetSharedCardUrl => BASE_API_URL + "SharedCard/Get";
        public static string UpdateSharedCardUrl => BASE_API_URL + "SharedCard/Put";
        public static string AcceptQuickShareUrl => BASE_API_URL + "QuickShare/Post";

        public static string SmsShareUrl => BASE_API_URL + "SmSShare/Post";

        public static string UpdateUserDeviceUrl => BASE_API_URL + "UserDevice/UpdateUserDevice";
        public static string DeviceDetailsUrl => BASE_API_URL + "UserDevice/GetDeviceDetails?deviceType={0}";
        public static string AppInfoUrl => BASE_API_URL + "UserDevice/GetCurrentAppInfo";
        public static string SystemSettingsUrl => BASE_API_URL + "Settings/GetSystemSettings";
    }
}
