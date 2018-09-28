using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Busidex3.DomainModels;

namespace Busidex3.Services.Utils
{    
    public delegate void OnBusidexUserLoadedEventHandler (BusidexUser user);

    public class Security
    {
        public static event OnBusidexUserLoadedEventHandler OnBusidexUserLoaded;

        private static readonly AccountHttpService _accountHttpService = new AccountHttpService();

        public static string AuthToken { get; set; }
        public static BusidexUser CurrentUser { get; set; }

        public static async Task<BusidexUser> LoadUser ()
        {
            try {
                if (!string.IsNullOrEmpty (AuthToken)) {
                    var account = await _accountHttpService.GetAccount ();
                    var accountJson = Newtonsoft.Json.JsonConvert.SerializeObject(account);
                    Serialization.SaveResponse(accountJson, Resources.BUSIDEX_USER_FILE);

                    if (!string.IsNullOrEmpty (accountJson)) {
                        CurrentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<BusidexUser> (accountJson);

                        OnBusidexUserLoaded?.Invoke (CurrentUser);
                    }
                }
                return CurrentUser;
            } catch (Exception ex) {
                Xamarin.Insights.Report (ex);
            }
            return null;
        }

        public static void LogOut()
        {
            AuthToken = string.Empty;

            Serialization.SaveResponse (null, Resources.OWNED_CARD_FILE);
            Serialization.SaveResponse (null, Resources.MY_BUSIDEX_FILE);
            Serialization.SaveResponse (null, Resources.EVENT_LIST_FILE);
            Serialization.SaveResponse (null, Resources.EVENT_CARDS_FILE);
            Serialization.SaveResponse (null, Resources.SHARED_CARDS_FILE);
            Serialization.SaveResponse (null, Resources.MY_ORGANIZATIONS_FILE);
            Serialization.SaveResponse (null, Resources.THUMBNAIL_FILE_NAME_PREFIX);
            Serialization.SaveResponse (null, Resources.ORGANIZATION_REFERRALS_FILE);
        }

        public static long DecodeUserId()
        {
            byte[] raw = Convert.FromBase64String(AuthToken);
            string s = Encoding.UTF8.GetString(raw);
            long.TryParse(s, out var userId);

            return userId;
        }

        public static void ChangeEmail (string email)
        {
            try {
                CurrentUser.Email = email;
                Serialization.SaveResponse (Newtonsoft.Json.JsonConvert.SerializeObject (CurrentUser), Resources.BUSIDEX_USER_FILE);
                OnBusidexUserLoaded?.Invoke (CurrentUser);
            } catch (Exception ex) {
                Xamarin.Insights.Report (ex);
            }
        }

        public static string EncodeUserId (long userId)
        {
            byte [] toEncodeAsBytes = Encoding.ASCII.GetBytes (userId.ToString (CultureInfo.InvariantCulture));
            string returnValue = Convert.ToBase64String (toEncodeAsBytes);
            return returnValue;
        }

        public static char [] GetDigits (string text)
        {
            return Regex.Replace (text, @"[^\d]", "").ToCharArray ();
        }
    }
}
