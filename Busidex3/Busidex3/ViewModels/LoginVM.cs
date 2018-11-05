using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Busidex3.Annotations;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Newtonsoft.Json;

namespace Busidex3.ViewModels
{
    public class LoginVM : BaseViewModel, INotifyPropertyChanged
    {
        private readonly AccountHttpService _accountHttpService = new AccountHttpService();
        private readonly LoginHttpService _loginHttpService = new LoginHttpService();
        
        public string UserName { get; set; }
        public string Password { get; set; }

        public async Task<bool> DoLogin()
        {
            var user = await _loginHttpService.DoLogin(UserName, Password);

            var nCookie = new System.Net.Cookie
            {
                Name = Resources.AUTHENTICATION_COOKIE_NAME
            };
            var expiration = DateTime.Now.AddYears (1);
            nCookie.Expires = expiration;
            nCookie.Value = Security.EncodeUserId (user.UserId);
            
            AuthToken = nCookie.Value;

            var cookieString = JsonConvert.SerializeObject(nCookie);
            var localPath = Path.Combine (Serialization.GetAppLocalStorageFolder(), Resources.AUTHENTICATION_COOKIE_NAME + ".txt");
            
            File.WriteAllText(localPath, cookieString); // writes to local storage  
            
            return user != null && user.UserId > 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
