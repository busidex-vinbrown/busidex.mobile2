
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


		protected override void ProcessFile(string data){

			var orgMembersResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgReferralResponse> (data);

			var lstOrganizationMembers = FindViewById<ListView> (Resource.Id.lstOrganizationMembers);
			var adapter = new UserCardAdapter (this, Resource.Id.lstCards, orgMembersResponse.Model);

			adapter.Redirect += ShowCard;
			adapter.SendEmail += SendEmail;
			adapter.OpenBrowser += OpenBrowser;
			adapter.CardAddedToMyBusidex += AddCardToMyBusidex;
			adapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
			adapter.OpenMap += OpenMap;

			adapter.ShowNotes = true;

			lstOrganizationMembers.Adapter = adapter;
		}
	}
}

