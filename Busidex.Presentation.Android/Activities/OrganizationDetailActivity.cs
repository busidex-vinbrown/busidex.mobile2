

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using Android.Webkit;
using System.IO;
using Android.Net;
using Android.Content;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "OrganizationDetailActivity")]			
	public class OrganizationDetailActivity : Activity
	{
		Organization organization;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.Organization);

			base.OnCreate (savedInstanceState);

			var data = Intent.GetStringExtra ("Organization");
			organization = Newtonsoft.Json.JsonConvert.DeserializeObject<Organization> (data);

			var ingOrgDetailLogoBanner = FindViewById<ImageView> (Resource.Id.ingOrgDetailLogoBanner);
			var txtContacts = FindViewById<TextView> (Resource.Id.txtContacts);
			var txtEmail = FindViewById<TextView> (Resource.Id.txtEmail);
			var txtPhone = FindViewById<TextView> (Resource.Id.txtPhone);
			var txtFax = FindViewById<TextView> (Resource.Id.txtFax);
			var btnOrganizationWeb = FindViewById<ImageButton> (Resource.Id.btnOrganizationWeb);
			var btnOrganizationTwitter = FindViewById <ImageButton> (Resource.Id.btnOrganizationTwitter);
			var btnOrganizationFacebook = FindViewById<ImageButton> (Resource.Id.btnOrganizationFacebook);

			var vwWeb = FindViewById<WebView> (Resource.Id.vwWeb);

			const int IMAGE_HEIGHT = 82;

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			using (var imgOrgDetailLogoBanner = FindViewById<ImageView> (Resource.Id.imgOrganizationHeaderImage)) {
				var bm = AndroidUtils.DecodeSampledBitmapFromFile (fileName, Resources.DisplayMetrics.WidthPixels, IMAGE_HEIGHT);
				ingOrgDetailLogoBanner.SetImageBitmap (bm);
			}

			txtContacts.Text = organization.Contacts;
			txtEmail.Text = organization.Email;
			txtPhone.Text = organization.Phone1;
			txtFax.Text = organization.Phone2;
			vwWeb.LoadData (organization.HomePage, "text/html", "utf-8");

			var webUri = Uri.Parse (organization.Url);
			var webIntent = new Intent (Intent.ActionView);
			webIntent.SetData (webUri);

			var fbUri = Uri.Parse (organization.Facebook);
			var fbIntent = new Intent (Intent.ActionView);
			fbIntent.SetData (fbUri);

			var twitterUri = Uri.Parse (organization.Twitter);
			var fbTwitter = new Intent (Intent.ActionView);
			fbTwitter.SetData (twitterUri);

			btnOrganizationWeb.Click += delegate {
				var intent = Intent.CreateChooser(webIntent, "Open with");
				StartActivity (intent);
			};

			btnOrganizationTwitter.Click += delegate {
				var intent = Intent.CreateChooser(fbTwitter, "Open with");
				StartActivity (intent);
			};

			btnOrganizationFacebook.Click += delegate {
				var intent = Intent.CreateChooser(fbIntent, "Open with");
				StartActivity (intent);
			};
		}

		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			if(organization != null){
				Window.SetTitle(organization.Name);
			}
		}
	}
}

