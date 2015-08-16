
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Busidex.Mobile.Models;
using Busidex.Mobile;
using System.IO;
using Android.Util;

namespace Busidex.Presentation.Android
{
	[Activity (Label = "Members")]			
	public class OrganizationMembersActivity : BaseCardActivity
	{
		List<UserCard> Cards { get; set; }
		static SearchView txtSearchOrgMembers { get; set; }
		static UserCardAdapter MembersAdapter { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.OrganizationMembers);

			base.OnCreate (savedInstanceState);

			var data = Intent.GetStringExtra ("Organization");
			var organization = Newtonsoft.Json.JsonConvert.DeserializeObject<Organization> (data);

			var fullFilePath = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, Busidex.Mobile.Resources.ORGANIZATION_MEMBERS_FILE + organization.OrganizationId);
			//LoadFromFile (fullFilePath);

			const int IMAGE_HEIGHT = 82;

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var img = FindViewById<ImageView> (Resource.Id.imgOrganizationHeaderImage);
			var bm = AndroidUtils.DecodeSampledBitmapFromFile (fileName, Resources.DisplayMetrics.WidthPixels, IMAGE_HEIGHT);
			img.SetImageBitmap (bm);
		}

		static void DoFilter(string filter){
			if(string.IsNullOrEmpty(filter)){
				MembersAdapter.Filter.InvokeFilter ("");
			}else{
				MembersAdapter.Filter.InvokeFilter(filter);
			}
		}

		protected override void ProcessFile(string data){

			var orgMembersResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgMemberResponse> (data);

			Cards = new List<UserCard> ();

			foreach (var item in orgMembersResponse.Model) {
				if (item != null) {

					var userCard = new UserCard ();

					userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
					userCard.Card = item;
					userCard.CardId = item.CardId;

					Cards.Add (userCard);
				}
			}

			txtSearchOrgMembers = FindViewById<SearchView> (Resource.Id.txtSearchOrgMembers);

			var lstOrganizationMembers = FindViewById<ListView> (Resource.Id.lstOrganizationMembers);
			MembersAdapter = new UserCardAdapter (this, Resource.Id.lstCards, Cards);

//			MembersAdapter.Redirect += ShowCard;
//			MembersAdapter.SendEmail += SendEmail;
//			MembersAdapter.OpenBrowser += OpenBrowser;
//			MembersAdapter.CardAddedToMyBusidex += AddCardToMyBusidex;
//			MembersAdapter.CardRemovedFromMyBusidex += RemoveCardFromMyBusidex;
//			MembersAdapter.OpenMap += OpenMap;

			MembersAdapter.ShowNotes = false;

			lstOrganizationMembers.Adapter = MembersAdapter;

			txtSearchOrgMembers.QueryTextChange += delegate {
				DoFilter(txtSearchOrgMembers.Query);
			};

			txtSearchOrgMembers.Iconified = false;
			lstOrganizationMembers.RequestFocus (global::Android.Views.FocusSearchDirection.Down);
			//DismissKeyboard (txtSearchOrgMembers.WindowToken);

			txtSearchOrgMembers.Touch += delegate {
				txtSearchOrgMembers.Focusable = true;
				txtSearchOrgMembers.RequestFocus();
			};
		}
	}
}

