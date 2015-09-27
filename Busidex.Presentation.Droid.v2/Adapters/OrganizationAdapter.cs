using Android.Widget;
using Busidex.Mobile.Models;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using Android.Graphics;

namespace Busidex.Presentation.Droid.v2
{
	public delegate void RedirectToOrganizationDetailsHandler(Organization organization);

	public class OrganizationAdapter : ArrayAdapter<Organization>
	{
		List<Organization> Organizations;
		readonly Activity context;

		public event RedirectToOrganizationDetailsHandler RedirectToOrganizationDetails;

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
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.OrganizationListItem, null);
			var organization = Organizations [position];

			var fileName = System.IO.Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);

			var img = view.FindViewById<ImageButton> (Resource.Id.imgOrganizationThumbnail);

			const int LOGO_WIDTH = 286;
			const int LOGO_HEIGHT = 181;
			var bm = AndroidUtils.DecodeSampledBitmapFromFile (fileName, LOGO_WIDTH, LOGO_HEIGHT);

			img.SetImageBitmap (bm);

			img.Click += delegate {
				OnRedirectToOrganizationDetails(organization);
			};

			return view;
		}
	}
}

