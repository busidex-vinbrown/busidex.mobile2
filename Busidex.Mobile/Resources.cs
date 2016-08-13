using System;


namespace Busidex.Mobile
{
	public static class Resources
	{
		public const string APPLICATION_NAME = "Busidex";
		public const string AUTHENTICATION_COOKIE_NAME = "UserId";
		public const string BUSIDEX_REFRESH_COOKIE_NAME = "BusidexRefresh";
		public const string ORGANIZATION_REFRESH_COOKIE_NAME = "OrganizationRefresh";
		public const string EVENT_LIST_REFRESH_COOKIE_NAME = "EventListRefresh";
		public const string EVENT_CARDS_REFRESH_COOKIE_NAME = "EventCards{0}";

		public const string USER_SETTING_DEVICE_TYPE_SET = "DeviceTypeSet";
		public const string USER_SETTING_DISPLAYNAME = "DisplayName";
		public const string USER_SETTING_USERNAME = "UserName";
		public const string USER_SETTING_PASSWORD = "Password";
		public const string USER_SETTING_EMAIL = "Email";
		public const string USER_SETTING_AUTOSYNC = "AutoSync";
		public const string USER_SETTING_USE_STAR_82 = "UseStar82";

		public const string PREFERENCE_FIRST_USE_POPUP_SEEN = "StarupPopupSeen";

		public const string MY_CARD_ADD_URL = "https://start.busidex.com/#/front/{0}";
		public const string MY_CARD_EDIT_URL = MY_CARD_ADD_URL + "?m=edit";
		public const string TERMS_AND_CONDITIONS_URL = "https://www.busidex.com/partials/account/terms.html";
		public const string FORGOT_PASSWORD_URL = "https://www.busidex.com/#/account/passwordrecover";
		public const string FORGOT_USERNAME_URL = "https://www.busidex.com/#/account/usernamerecover";
		public const string PRIVACY_URL = "https://www.busidex.com/partials/account/privacy.html";
		public const string BASE_API_URL = "https://www.busidexapi.com/api/";
		public const string EMPTY_CARD_ID = "b66ff0ee-e67a-4bbc-af3b-920cd0de56c6";
		public const string NULL_CARD_ID = "00000000-0000-0000-0000-000000000000";
		public const string CARD_PATH = "https://busidexcdn.blob.core.windows.net/cards/";
		public const string THUMBNAIL_PATH = "https://busidexcdn.blob.core.windows.net/mobile-images/";
		public const string THUMBNAIL_FILE_NAME_PREFIX = "lowres_";

		public const string MY_BUSIDEX_FILE = "mybusidex.json";
		public const string MY_ORGANIZATIONS_FILE = "myorganizations.json";
		public const string EVENT_LIST_FILE = "eventlist.json";
		public const string SHARED_CARDS_FILE = "sharedcards.json";
		public const string EVENT_CARDS_FILE = "eventcards_{0}.json";
		public const string ORGANIZATION_MEMBERS_FILE = "organization_members_{0}.json";
		public const string ORGANIZATION_REFERRALS_FILE = "organization_referrals_{0}.json";
		public const string BUSIDEX_USER_FILE = "busidex_user.json";
		public const string QUICKSHARE_LINK = "quickShare_Link.json";
		public const string OWNED_CARD_FILE = "owned_card.json";

		public const string COOKIE_URI = "https://localhost";
		public const int SECONDS_IN_A_YEAR = 60 * 60 * 24 * 365;
		// we don't really care about leap years

		public const string SHARE_CONTENT_TEXT = "{0} has shared a ";

		public const string XAMARIN_INSIGHTS_KEY = "064b046194c04b614f285ac8b2c9677468d93e71";
		public const string BRANCH_KEY = "key_live_cnh1UbVspZWKB1nLPE0NygmorydPic2z";
		public const string BRANCH_APP_LINK_DOMAIN = "jqle.app.link";
		public const string GOOGLE_ANALYTICS_KEY_ANDROID = "UA-29820162-2";
		public const string GOOGLE_ANALYTICS_KEY_IOS = "UA-29820162-3";
		public const string GA_CATEGORY_ACTIVITY = "Activity";
		public const string GA_MY_BUSIDEX_LABEL = "My Busidex";
		public const string GA_MY_ORGANIZATIONS_LABEL = "My Organizations";
		public const string GA_NOTIFICATIONS_LABEL = "Notifications";
		public const string GA_LABEL_OPEN = "Opened";
		public const string GA_LABEL_LIST = "List";
		public const string GA_LABEL_DETAILS = "Details";
		public const string GA_LABEL_PHONE = "Called";
		public const string GA_LABEL_EMAIL = "EMailed";
		public const string GA_LABEL_URL = "Url";
		public const string GA_LABEL_NOTES = "Notes";
		public const string GA_LABEL_SHARE = "Shared";
		public const string GA_LABEL_MAP = "Map";
		public const string GA_LABEL_QUESTIONS = "Questions";
		public const string GA_LABEL_ADD = "Added";
		public const string GA_LABEL_REMOVED = "Removed";
		public const string GA_LABEL_EVENT_LIST = "Event List";
		public const string GA_LABEL_EVENT = "Event";
		public const string GA_LABEL_EVENT_NAME = "Event: {0}";
		public const string GA_LABEL_APP_START = "Application Started";
		public const string GA_LABEL_MY_BUSIDEX_REFRESHED = "My Busidex Refreshed";

		public const string GA_SCREEN_HOME = "Home";
		public const string GA_SCREEN_MY_BUSIDEX = "My Busidex";
		public const string GA_SCREEN_SEARCH = "Search";
		public const string GA_SCREEN_ORGANIZATIONS = "Organizations";
		public const string GA_SCREEN_EVENTS = "Events";
		public const string GA_SCREEN_REFERRALS = "Referrals";
		public const string GA_SCREEN_PROFILE = "Profile";
		public const string GA_SCREEN_TERMS = "Terms and Conditions";
		public const string GA_SCREEN_PRIVACY = "Privacy Policy";
		public const string GA_SCREEN_CARD_MENU = "Card Menu";
		public const string GA_SCREEN_VISIBILITY = "Card Visibility";
		public const string GA_SCREEN_SEARCH_INFO = "Search Info";
		public const string GA_SCREEN_CONTACT_INFO = "Contact Info";
		public const string GA_SCREEN_TAGS = "Card Tags";
		public const string GA_SCREEN_ADDRESS_INFO = "Address Info";
		public const string GA_SCREEN_CARD_IMAGE = "Card Image";

		public const string TWILLIO_SID_PROD = "AC74907b35cd4dc40f595dc4f8809d10ba";
		public const string TWILLIO_AUTH_TOKEN_PROD = "401066fdfc0fe0fbc7a1ead2eadc55c5";
		public const string TWILLIO_SID_TEST = "AC1e82c094d6bad0431b578a44816b7d84";
		public const string TWILLIO_AUTH_TOKEN_TEST = "a9a8e56e4e009a55aae754c7aafcf995";
		public const string TWILLIO_PHONE_NUMBER = "(754) 333-5330";

		//public const string BITLY_ACCESS_TOKEN = "eb1e285ba9ebb90b8cbf58182fa3df10321aae2e";
		//public const string BITLY_LOGIN = "busidexapp";

		public enum HttpActions
		{
			GET,
			POST,
			PUT,
			DELETE
		}

		public static string DocumentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

		public enum UIElements
		{
			CardImage = 1,
			NameLabel = 2,
			CompanyLabel = 3,
			MapButton = 4,
			NotesButton = 5,
			EmailButton = 6,
			WebsiteButton = 7,
			PhoneNumberButton = 8,
			TextMessageButton = 9,
			OrganizationImage = 10,
			TwitterButton = 11,
			FacebookButton = 12,
			ButtonPanel = 13,
			ShareCardButton = 14,
			AddToMyBusidexButton = 15,
			RemoveFromMyBusidexButton = 16,
			AcceptCard = 17,
			DeclineCard = 18,
			PersonalMessageLabel = 19,
			QuickShare = 20
		}
	}
}

