

using Android.App;
using Android.OS;
using Android.Widget;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "LoginActivity")]			
	public class LoginActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var button = FindViewById<Button> (Resource.Id.btnLogin);
			button.Click += delegate {
				DoLogin();
			};
		}

		void DoLogin(){

			var txtUserName = FindViewById<TextView> (Resource.Id.txtUserName);
			var txtPassword = FindViewById<TextView> (Resource.Id.txtPassword);

			var userName = txtUserName.Text;
			var password = txtPassword.Text;

			var controller = Busidex.Mobile.LoginController.DoLogin (userName, password);

		}
	}
}

