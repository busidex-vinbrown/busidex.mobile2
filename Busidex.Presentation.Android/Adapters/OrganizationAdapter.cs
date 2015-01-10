using Android.Widget;
using Busidex.Mobile.Models;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using Android.Net;
using Android.Content;
using Android.Views.Animations;
using Android.Graphics.Drawables;
using Android.Graphics;

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

		bool ShowingPanel = false;
		List<View> ViewCache;
		Animation SlideOut;
		Animation SlideIn;

		public OrganizationAdapter (Activity ctx, int id, List<Organization> organizations) : base(ctx, id, organizations)
		{
			Organizations = organizations;
			context = ctx;
			ViewCache = new List<View> ();
			for(var i=0;i<organizations.Count;i++){
				ViewCache.Add (new View (ctx));
			}

			SlideOut = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideOutAnimation);
			SlideIn = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideAnimation);

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
			if (RedirectToOrganizationReferrals != null) {
				RedirectToOrganizationReferrals (OrgReferralsIntent);
			}
		}

		public static int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
			// Raw height and width of image
		 	int height = options.OutHeight;
			int width = options.OutWidth;
			int inSampleSize = 2;

			if (height > reqHeight || width > reqWidth) {

				int halfHeight = height / 2;
				int halfWidth = width / 2;

				// Calculate the largest inSampleSize value that is a power of 2 and keeps both
				// height and width larger than the requested height and width.
				while ((halfHeight / inSampleSize) > reqHeight
					&& (halfWidth / inSampleSize) > reqWidth) {
					inSampleSize *= 2;
				}
			}

			return inSampleSize;
		}

		public static Bitmap decodeSampledBitmapFromFile(string fileName,
			int reqWidth, int reqHeight) {

			// First decode with inJustDecodeBounds=true to check dimensions
			BitmapFactory.Options options = new BitmapFactory.Options();
			options.InScaled = true;
			options.InJustDecodeBounds = true;
			BitmapFactory.DecodeFile(fileName, options);

			// Calculate inSampleSize
			options.InSampleSize = calculateInSampleSize(options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;
			options.InScaled = true;
			return BitmapFactory.DecodeFile(fileName, options);
		}

		LinearLayout SetButtonPanel(View view, View parent, int position){

			var orgButtonContainer = view.FindViewById<LinearLayout> (Resource.Id.orgButtonContainer);
			var layoutParams = orgButtonContainer.LayoutParameters;
			if(layoutParams == null){
				layoutParams = new ViewGroup.LayoutParams (parent.Width, view.Height);
			}else{
				layoutParams.Width = parent.Width;
			}
			orgButtonContainer.LayoutParameters = layoutParams;

			return orgButtonContainer;
		}

		void SetOrganizationData(Organization organization){
			OrgDetailIntent = new Intent(context, typeof(OrganizationDetailActivity));
			OrgMembersIntent = new Intent(context, typeof(OrganizationMembersActivity));
			OrgReferralsIntent = new Intent(context, typeof(OrganizationReferralsActivity));

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(organization);

			OrgDetailIntent.PutExtra ("Organization", data);
			OrgMembersIntent.PutExtra ("Organization", data);
			OrgReferralsIntent.PutExtra ("Organization", data);
		}

		void HideAllPanelInstances(int position){
		

			var slideOut = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideOutAnimation);

			for(var i=0; i < ViewCache.Count; i++){
				var view = ViewCache [i];
				var panel = view.FindViewById<LinearLayout> (Resource.Id.orgButtonContainer);
				if(panel != null && position != i){
					panel.StartAnimation (slideOut);
					slideOut.AnimationEnd += delegate {
						panel.Visibility = ViewStates.Gone;
					};
				}
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.OrganizationListItem, null);
			ViewCache [position] = view;
			var organization = Organizations [position];

			var fileName = System.IO.Path.Combine (Busidex.Mobile.Resources.DocumentsPath, organization.LogoFileName + "." + organization.LogoType);

			var img = view.FindViewById<ImageView> (Resource.Id.imgOrganizationThumbnail);
			(img.Drawable as BitmapDrawable).Bitmap.Recycle();

			const int LOGO_WIDTH = 286;
			const int LOGO_HEIGHT = 181;
			var bm = decodeSampledBitmapFromFile (fileName, LOGO_WIDTH, LOGO_HEIGHT);

			img.SetImageBitmap (bm);

			var btnOrgDetails = view.FindViewById<Button> (Resource.Id.btnOrgDetails);
			var btnOrgMembers = view.FindViewById<Button> (Resource.Id.btnOrgMembers);
			var btnOrgReferrals = view.FindViewById<Button> (Resource.Id.btnOrgReferrals);
			var btnOrgInfo = view.FindViewById<ImageButton> (Resource.Id.btnOrgInfo);
			var orgButtonContainer = view.FindViewById<LinearLayout> (Resource.Id.orgButtonContainer);

			btnOrgInfo.Click += (sender, e) => {

				HideAllPanelInstances(position);

				orgButtonContainer = SetButtonPanel (view, parent, position);

				var slideIn = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideAnimation);
				var slideOut = AnimationUtils.LoadAnimation (context, Resource.Animation.SlideOutAnimation);
				slideOut.AnimationEnd += delegate {
					orgButtonContainer.Visibility = ViewStates.Gone;
				};

				if(orgButtonContainer.Visibility == ViewStates.Visible){
					orgButtonContainer.StartAnimation (slideOut);
				}else{
					orgButtonContainer.Visibility = ViewStates.Visible;
					SetOrganizationData(organization);
					orgButtonContainer.StartAnimation (slideIn);
				}
			};

			orgButtonContainer.Visibility = ViewStates.Gone;

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

