using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Newtonsoft.Json;

namespace Busidex3.Services.Utils
{    
    public delegate void OnBusidexUserLoadedEventHandler (BusidexUser user);

    public static class Security
    {
        public static event OnBusidexUserLoadedEventHandler OnBusidexUserLoaded;

        private static readonly AccountHttpService _accountHttpService = new AccountHttpService();

        public static string AuthToken { get; set; }
        public static BusidexUser CurrentUser { get; set; }

        public static async Task<BusidexUser> LoadUser()
        {
            try
            {
                if (string.IsNullOrEmpty(AuthToken)) return CurrentUser;

                var account = await _accountHttpService.GetAccount();
                var accountJson = JsonConvert.SerializeObject(account);
                Serialization.SaveResponse(accountJson, StringResources.BUSIDEX_USER_FILE);

                if (string.IsNullOrEmpty(accountJson)) return CurrentUser;

                CurrentUser = JsonConvert.DeserializeObject<BusidexUser>(accountJson);

                OnBusidexUserLoaded?.Invoke(CurrentUser);
                return CurrentUser;
            }
            catch (Exception ex)
            {
                //Xamarin.Insights.Report(ex);
            }
            return null;
        }

        public static async void SaveAuthCookie(long userId)
        {
            var nCookie = new Cookie
            {
                Name = StringResources.AUTHENTICATION_COOKIE_NAME
            };
            var expiration = DateTime.Now.AddYears (1);
            nCookie.Expires = expiration;
            nCookie.Value = EncodeUserId (userId);
            
            var cookieString = JsonConvert.SerializeObject(nCookie);
            var localPath = Path.Combine (Serialization.GetAppLocalStorageFolder(), StringResources.AUTHENTICATION_COOKIE_NAME + ".txt");
            
            File.WriteAllText(localPath, cookieString); // writes to local storage  

            await LoadUser();
            AuthToken = nCookie.Value;
        }

        public static Cookie ReadAuthCookie()
        {
            var cookieFile = Path.Combine (Serialization.GetAppLocalStorageFolder(), StringResources.AUTHENTICATION_COOKIE_NAME + ".txt");

            if (!File.Exists(cookieFile)) return null;

            var file = File.ReadAllText(cookieFile);
            var cookie = JsonConvert.DeserializeObject<Cookie>(file);
            
            AuthToken = cookie.Value;

            return cookie;
        }

        public static void LogOut()
        {
            AuthToken = string.Empty;

            Serialization.SaveResponse(null, StringResources.OWNED_CARD_FILE);
            Serialization.SaveResponse(null, StringResources.MY_BUSIDEX_FILE);
            Serialization.SaveResponse(null, StringResources.EVENT_LIST_FILE);
            Serialization.SaveResponse(null, StringResources.EVENT_CARDS_FILE);
            Serialization.SaveResponse(null, StringResources.SHARED_CARDS_FILE);
            Serialization.SaveResponse(null, StringResources.MY_ORGANIZATIONS_FILE);
            Serialization.SaveResponse(null, StringResources.THUMBNAIL_FILE_NAME_PREFIX);
            Serialization.SaveResponse(null, StringResources.ORGANIZATION_REFERRALS_FILE);
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
                Serialization.SaveResponse (JsonConvert.SerializeObject (CurrentUser), StringResources.BUSIDEX_USER_FILE);
                OnBusidexUserLoaded?.Invoke (CurrentUser);
            } catch (Exception ex) {
                //Xamarin.Insights.Report (ex);
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
