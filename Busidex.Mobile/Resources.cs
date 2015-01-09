using System;


namespace Busidex.Mobile
{
	public static class Resources
	{
		public const string AUTHENTICATION_COOKIE_NAME = "UserId";
		public const string BUSIDEX_REFRESH_COOKIE_NAME = "BusidexRefresh";
		public const string ORGANIZATION_REFRESH_COOKIE_NAME = "OrganizationRefresh";
		public const string USER_SETTING_USERNAME = "UserName";
		public const string USER_SETTING_PASSWORD = "Password";
		public const string USER_SETTING_EMAIL = "Email";
		public const string USER_SETTING_AUTOSYNC = "AutoSync";
		public const string USER_SETTING_USE_STAR_82 = "UseStar82";
		public const string BASE_API_URL = "https://www.busidexapi.com/api/";
		public const string EMPTY_CARD_ID = "b66ff0ee-e67a-4bbc-af3b-920cd0de56c6";
		public const string CARD_PATH =  "https://busidexcdn.blob.core.windows.net/cards/";//"https://az381524.vo.msecnd.net/cards/";
		public const string THUMBNAIL_PATH =  "https://busidexcdn.blob.core.windows.net/mobile-images/";//"https://az381524.vo.msecnd.net/cards/";
		public const string THUMBNAIL_FILE_NAME_PREFIX = "lowres_";
		public const string MY_BUSIDEX_FILE = "mybusidex.json";
		public const string MY_ORGANIZATIONS_FILE = "myorganizations.json";
		public const string SHARED_CARDS_FILE = "sharedcards.json";
		public const string ORGANIZATION_MEMBERS_FILE = "organization_members_";
		public const string ORGANIZATION_REFERRALS_FILE = "organization_referrals_";
		public const string COOKIE_URI = "https://localhost";
		public const int SECONDS_IN_A_YEAR = 60 * 60 * 24 * 365; // we don't really care about leap years

		public enum HttpActions{ GET, POST, PUT, DELETE };

		public static string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public enum UIElements{
			CardImage = 1,
			NameLabel = 2,
			CompanyLabel = 3,
			MapButton = 4,
			NotesButton = 5,
			EmailButton = 6,
			WebsiteButton = 7,
			PhoneNumberButton = 8,
			OrganizationImage = 9,
			TwitterButton = 10,
			FacebookButton = 11,
			ButtonPanel = 12,
			ShareCardButton = 13,
			AddToMyBusidexButton = 14,
			RemoveFromMyBusidexButton = 15,
			AcceptCard = 16,
			DeclineCard = 17,
			PersonalMessageLabel = 18
		}
	}
}

