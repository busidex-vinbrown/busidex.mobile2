using Android.Widget;
using Busidex.Mobile.Models;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using System.IO;
using Android.Net;
using Android.Content;
using Android.Views.Animations;

namespace Busidex.Presentation.Android
{
	public delegate void RedirectToOrganizationDetailsHandler(Intent intent);
	public delegate void RedirectToOrganizationMembersHandler(Intent intent);
	public delegate void RedirectToOrganizationReferralsHandler(Intent intent);

	public class OrganizationAdapter : ArrayAdapter<Organization>
	{
		readonly List<Organization> Organizations;
		readonly Activity context;

		public event RedirectToOrganizationDetailsHandler RedirectToOrganizationDetails;
		public event RedirectToOrganizationMembersHandler RedirectToOrganizationMembers;
		public event RedirectToOrganizationReferralsHandler RedirectToOrganizationReferrals;

		Intent OrgDetailIntent{ get; set; }
		Intent OrgMembersIntent{ get; set; }
		Intent OrgReferralsIntent{ get; set; }

		public OrganizationAdapter (Activity ctx, int id, List<Organization> organizations) : base(ctx, id, organizations)
		{
			Organizations = organizations;
			context = ctx;
		}

		void OnRedirectToOrganizationDetails(object sender, System.EventArgs e){
			if(RedirectToOrganizationDetails != null){
				RedirectToOrganizationDetails (OrgDetailIntent);
			}
		}

		void OnRedirectToOrganizationMembers(object sender, System.EventArgs e){
			if(RedirectToOrganizationMembers != null){
				RedirectToOrganizationMembers (OrgMembersIntent);
			}
		}

		void OnRedirectToOrganizationReferrals(object sender, System.EventArgs e){
			if(RedirectToOrganizationReferrals != null){
				RedirectToOrganizationReferrals (OrgReferralsIntent);
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.OrganizationListItem, null);

			var organization = Organizations [position];

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var uri = Uri.Parse (fileName);

			var img = view.FindViewById<ImageView> (Resource.Id.imgOrganizationThumbnail);
			img.SetImageURI (uri);

			OrgDetailIntent = new Intent(context, typeof(OrganizationDetailActivity));
			OrgMembersIntent = new Intent(context, typeof(OrganizationMembersActivity));
			OrgReferralsIntent = new Intent(context, typeof(OrganizationReferralsActivity));

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(organization);

			OrgDetailIntent.PutExtra ("Organization", data);
			OrgMembersIntent.PutExtra ("Organization", data);
			OrgReferralsIntent.PutExtra ("Organization", data);

			var btnOrgDetails = view.FindViewById<Button> (Resource.Id.btnOrgDetails);
			var btnOrgMembers = view.FindViewById<Button> (Resource.Id.btnOrgMembers);
			var btnOrgReferrals = view.FindViewById<Button> (Resource.Id.btnOrgReferrals);
			var orgButtonContainer = view.FindViewById<LinearLayout> (Resource.Id.orgButtonContainer);
			var btnOrgInfo = view.FindViewById<ImageButton> (Resource.Id.btnOrgInfo);

			var layoutParams = orgButtonContainer.LayoutParameters;
			if(layoutParams == null){
				layoutParams = new ViewGroup.LayoutParams (parent.Width, view.Height);
			}else{
				layoutParams.Width = parent.Width;
			}
			orgButtonContainer.LayoutParameters = layoutParams;
			orgButtonContainer.Visibility = ViewStates.Gone;
			btnOrgInfo.Click += (sender, e) => {
				var slideIn = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideAnimation);
				var slideOut = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideOutAnimation);
				slideOut.AnimationEnd += delegate {
					orgButtonContainer.Visibility = ViewStates.Gone;
				};

				if(orgButtonContainer.Visibility == ViewStates.Visible){
					orgButtonContainer.StartAnimation (slideOut);
				}else{
					orgButtonContainer.Visibility = ViewStates.Visible;
					orgButtonContainer.StartAnimation (slideIn);
				}
			};

			btnOrgDetails.Click -= OnRedirectToOrganizationDetails;
			btnOrgDetails.Click += OnRedirectToOrganizationDetails;

			btnOrgMembers.Click -= OnRedirectToOrganizationMembers;
			btnOrgMembers.Click += OnRedirectToOrganizationMembers;

			btnOrgReferrals.Click -= OnRedirectToOrganizationReferrals;
			btnOrgReferrals.Click += OnRedirectToOrganizationReferrals;

			return view;
		}
	}
}

