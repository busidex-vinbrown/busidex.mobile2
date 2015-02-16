
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.IO;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Referrals")]			
	public class OrganizationReferralsActivity : BaseCardActivity
	{
		List<UserCard> Cards { get; set; }
		static UserCardAdapter ReferralsAdapter { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.OrganizationMembers);

			base.OnCreate (savedInstanceState);

			var data = Intent.GetStringExtra ("Organization");
			var organization = Newtonsoft.Json.JsonConvert.DeserializeObject<Organization> (data);

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.ORGANIZATION_REFERRALS_FILE + organization.OrganizationId);
			LoadFromFile (fullFilePath);

			const int IMAGE_HEIGHT = 82;

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var img = FindViewById<ImageView> (Resource.Id.imgOrganizationHeaderImage);
			var bm = AndroidUtils.DecodeSampledBitmapFromFile (fileName, Resources.DisplayMetrics.WidthPixels, IMAGE_HEIGHT);
			img.SetImageBitmap (bm);
		}

		static void DoFilter(string filter){
			if(string.IsNullOrEmpty(filter)){
				ReferralsAdapter.Filter.InvokeFilter ("");
			}else{
				ReferralsAdapter.Filter.InvokeFilter(filter);
			}
		}

		protected override void ProcessFile(string data){

			var orgMembersResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (data);

			var lstOrganizationMembers = FindViewById<ListView> (Resource.Id.lstOrganizationMembers);
			ReferralsAdapter = new UserCardAdapter (this, Resource.Id.lstCards, orgMembersResponse.Model);

			ReferralsAdapter.Redirect += ShowCard;
			ReferralsAdapter.SendEmail += SendEmail;
			ReferralsAdapter.OpenBrowser += OpenBrowser;
			ReferralsAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			ReferralsAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			ReferralsAdapter.OpenMap += OpenMap;

			ReferralsAdapter.ShowNotes = false;

			lstOrganizationMembers.Adapter = ReferralsAdapter;

			var txtSearchOrgMembers = FindViewById<SearchView> (Resource.Id.txtSearchOrgMembers);

			txtSearchOrgMembers.QueryTextChange += delegate {
				DoFilter(txtSearchOrgMembers.Query);
			};

			txtSearchOrgMembers.Iconified = false;
			lstOrganizationMembers.RequestFocus (global::Android.Views.FocusSearchDirection.Down);
			DismissKeyboard (txtSearchOrgMembers.WindowToken);

			txtSearchOrgMembers.Touch += delegate {
				txtSearchOrgMembers.Focusable = true;
				txtSearchOrgMembers.RequestFocus();
			};
		}
	}
}

