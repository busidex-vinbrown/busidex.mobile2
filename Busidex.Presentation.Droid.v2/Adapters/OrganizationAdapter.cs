using Android.Widget;
using Busidex.Mobile.Models;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using Android.Graphics;

namespace Busidex.Presentation.Droid.v2
{
	public delegate void RedirectToOrganizationDetailsHandler(Organization organization);
	public delegate void RedirectToOrganizationMembersHandler(Organization organization);

	public class OrganizationAdapter : ArrayAdapter<Organization>
	{
		List<Organization> Organizations;
		readonly Activity context;

		public event RedirectToOrganizationDetailsHandler RedirectToOrganizationDetails;
		public event RedirectToOrganizationMembersHandler RedirectToOrganizationMembers;

		public OrganizationAdapter (Activity ctx, int id, List<Organization> organizations) : base(ctx, id, organizations)
		{
			Organizations = organizations;
			context = ctx;
		}

		public override int Count {
			get {
				return Organizations.Count;
			}
		}

		void OnRedirectToOrganizationMembers(Organization org){
			if(RedirectToOrganizationMembers != null){
				RedirectToOrganizationMembers (org);
			}
		}

		void OnRedirectToOrganizationDetails(Organization org){
			if(RedirectToOrganizationDetails != null){
				RedirectToOrganizationDetails (org);
			}
		}

		public void UpdateData(List<Organization> organizations){
			Organizations = organizations;
			NotifyDataSetChanged ();
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view =  context.LayoutInflater.Inflate (Resource.Layout.OrganizationListItem, null);
			if (Organizations != null && position >= 0 && position < Organizations.Count) {
				var organization = Organizations [position];

				var fileName = System.IO.Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);

				var imgOrganizationThumbnail = view.FindViewById<ImageButton> (Resource.Id.imgOrganizationThumbnail);
				var txtOrganizationName = view.FindViewById<TextView> (Resource.Id.txtOrganizationName);

				if (organization.IsMember) {
				
					txtOrganizationName.Visibility = ViewStates.Gone;
					imgOrganizationThumbnail.Visibility = ViewStates.Visible;

					const int LOGO_WIDTH = 286;
					const int LOGO_HEIGHT = 181;
					var bm = AndroidUtils.DecodeSampledBitmapFromFile (fileName, LOGO_WIDTH, LOGO_HEIGHT);

					imgOrganizationThumbnail.SetImageBitmap (bm);

					imgOrganizationThumbnail.Click += delegate {
						OnRedirectToOrganizationDetails (organization);
					};
				} else {
					txtOrganizationName.Visibility = ViewStates.Visible;
					imgOrganizationThumbnail.Visibility = ViewStates.Gone;
					txtOrganizationName.Text = organization.Name;
					txtOrganizationName.Click += delegate {
						OnRedirectToOrganizationMembers (organization);
					};
				}
			}
			return view;
		}
	}
}

