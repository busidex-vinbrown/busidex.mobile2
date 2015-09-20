using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;
using Android.Webkit;
using System.IO;
using Android.Net;
using Busidex.Mobile;

namespace Busidex.Presentation.Droid.v2
{
	public class OrganizationPanelFragment : Fragment
	{
		readonly Organization SelectedOrganization;
		View view;

		public OrganizationPanelFragment(){
			
		}
		public OrganizationPanelFragment(Organization organization){
			SelectedOrganization = organization;
		}

		void DialPhoneNumber(Organization org){
			var userToken = BaseApplicationResource.GetAuthCookie ();
			var uri = Uri.Parse ("tel:" + org.Phone1);
			var intent = new Intent (Intent.ActionView, uri); 
			ActivityController.SaveActivity ((long)EventSources.Call, org.OrganizationId, userToken);
			Activity.StartActivity (intent); 
		}

		public override void OnResume ()
		{
			base.OnResume ();

			var btnHideInfo = view.FindViewById<ImageButton> (Resource.Id.btnHideInfo);

			btnHideInfo.Click += (sender, e) => ((MainActivity)Activity).UnloadFragment (Resource.Animation.SlideUpAnimation, Resource.Animation.SlideDownAnimation, Resource.Id.fragment_holder);

			var ingOrgDetailLogoBanner = view.FindViewById<ImageView> (Resource.Id.ingOrgDetailLogoBanner);
			var txtContacts = view.FindViewById<TextView> (Resource.Id.txtContacts);
			var btnEmail = view.FindViewById<TextView> (Resource.Id.btnEmail);
			var btnPhone = view.FindViewById<TextView> (Resource.Id.btnPhone);
			var txtFax = view.FindViewById<TextView> (Resource.Id.txtFax);
			var btnOrganizationWeb = view.FindViewById<ImageButton> (Resource.Id.btnOrganizationWeb);
			var btnOrganizationTwitter = view.FindViewById <ImageButton> (Resource.Id.btnOrganizationTwitter);
			var btnOrganizationFacebook = view.FindViewById<ImageButton> (Resource.Id.btnOrganizationFacebook);
			var btnOrgMembers = view.FindViewById<Button> (Resource.Id.btnOrgMembers);
			var btnOrgReferrals = view.FindViewById<Button> (Resource.Id.btnOrgReferrals);

			var vwWeb = view.FindViewById<WebView> (Resource.Id.vwWeb);

			const int IMAGE_HEIGHT = 82;

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, SelectedOrganization.LogoFileName + "." + SelectedOrganization.LogoType);
			using (var imgOrgDetailLogoBanner = view.FindViewById<ImageView> (Resource.Id.imgOrganizationHeaderImage)) {
				var bm = AndroidUtils.DecodeSampledBitmapFromFile (fileName, Resources.DisplayMetrics.WidthPixels, IMAGE_HEIGHT);
				ingOrgDetailLogoBanner.SetImageBitmap (bm);
			}

			txtContacts.Text = SelectedOrganization.Contacts;
			btnEmail.Text = SelectedOrganization.Email;
			btnPhone.Text = SelectedOrganization.Phone1;
			txtFax.Text = SelectedOrganization.Phone2;
			vwWeb.LoadData (SelectedOrganization.HomePage, "text/html", "utf-8");


			btnPhone.Click += delegate {
				DialPhoneNumber(SelectedOrganization);
			}; 

			var SendEmailIntent = new Intent(Intent.ActionSend);
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(SelectedOrganization);
			SendEmailIntent.PutExtra ("Organization", data);

			SendEmailIntent.PutExtra (Intent.ExtraEmail, new []{SelectedOrganization.Email} );
			SendEmailIntent.SetType ("message/rfc822");

			btnEmail.Click += delegate{
				((MainActivity)Activity).SendEmail(SendEmailIntent, SelectedOrganization.OrganizationId);
			};

			var webUri = Uri.Parse (SelectedOrganization.Url);
			var webIntent = new Intent (Intent.ActionView);
			webIntent.SetData (webUri);

			var fbUri = Uri.Parse (SelectedOrganization.Facebook);
			var fbIntent = new Intent (Intent.ActionView);
			fbIntent.SetData (fbUri);

			var twitterUri = Uri.Parse (SelectedOrganization.Twitter);
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

			btnOrgMembers.Click += delegate {
				((MainActivity)Activity).LoadOrganizationMembers(SelectedOrganization);
			};
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			view = inflater.Inflate(Resource.Layout.Organization, container, false);

			return view;
		}
	}
}

