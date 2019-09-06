using System.Threading.Tasks;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class LoginVM : BaseViewModel
    {
        private readonly LoginHttpService _loginHttpService = new LoginHttpService();
        
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginVM()
        {
            UserName = string.Empty;
            Password = string.Empty;

            Logo = ImageSource.FromResource("Busidex3.Resources.busidex_icon_180x180.png",
                typeof(LoginVM).GetType().Assembly);
        }

        private ImageSource _logo { get; set; }
        public ImageSource Logo
        {
            get => _logo;
            set
            {
                _logo = value;
                OnPropertyChanged(nameof(Logo));
            }
        }

        public async Task<bool> DoLogin()
        {
            var user = await _loginHttpService.DoLogin(UserName, Password);

            if (user == null || user.UserId <= 0) return false;

            await Security.SaveAuthCookie(user.UserId);

            return true;
        }
    }
}
