using System.Threading.Tasks;
using Busidex3.Services;
using Busidex3.Services.Utils;

namespace Busidex3.ViewModels
{
    public class LoginVM : BaseViewModel
    {
        private readonly LoginHttpService _loginHttpService = new LoginHttpService();
        
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginVM()
        {
            UserName = "vinbrown2";
            Password = "ride9736";
        }

        public async Task<bool> DoLogin()
        {
            var user = await _loginHttpService.DoLogin(UserName, Password);

            if (user == null || user.UserId <= 0) return false;

            Security.SaveAuthCookie(user.UserId);
            
            await Task.Factory.StartNew(async ()=> await App.LoadOwnedCard());

            return true;

        }
    }
}
