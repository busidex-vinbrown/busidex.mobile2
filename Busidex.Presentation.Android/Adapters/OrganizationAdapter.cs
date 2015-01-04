using Android.Widget;
using Busidex.Mobile.Models;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using System.IO;
using Android.Net;

namespace Busidex.Presentation.Android
{
	public class OrganizationAdapter : ArrayAdapter<Organization>
	{
		readonly List<Organization> Organizations;
		readonly Activity context;

		public OrganizationAdapter (Activity ctx, int id, List<Organization> organizations) : base(ctx, id, organizations)
		{
			Organizations = organizations;
			context = ctx;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.OrganizationListItem, null);

			var organization = Organizations [position];

			var fileName = Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);
			var uri = Uri.Parse (fileName);

			var img = view.FindViewById<ImageView> (Resource.Id.imgOrganizationThumbnail);
			img.SetImageURI (uri);

			return view;
		}
	}
}

