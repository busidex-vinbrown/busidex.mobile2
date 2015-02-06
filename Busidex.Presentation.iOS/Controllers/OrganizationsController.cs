using System;
using Foundation;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.IO;
using Busidex.Mobile;
using GoogleAnalytics.iOS;

namespace Busidex.Presentation.iOS
{
	partial class OrganizationsController : BaseController
	{
		public static NSString BusidexCellId = new NSString ("cellId");

		public OrganizationsController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null) {
				NavigationController.SetNavigationBarHidden (false, true);
				NavigationController.NavigationBar.SetBackgroundImage (null, UIBarMetrics.Default);
			}
		}

		OrganizationTableSource ConfigureTableSourceEventHandlers(List<Organization> data){

			var src = new OrganizationTableSource (data);
		
			src.ViewOrganization += delegate(long orgId) {

				try{
					UIStoryboard board = UIStoryboard.FromName ("OrganizationStoryBoard_iPhone", null);

					var orgDetailController = board.InstantiateViewController ("OrganizationDetailController") as OrganizationDetailController;

					if (orgDetailController != null) {
						orgDetailController.OrganizationId = orgId;
						NavigationController.PushViewController (orgDetailController, true);
					}
				}catch(Exception ex){
					new UIAlertView("Busidex", ex.Message, null, "OK", null).Show();
				}
			};

			src.ViewOrganizationMembers += delegate(Organization org) {

				try{
					UIStoryboard board = UIStoryboard.FromName ("OrganizationStoryBoard_iPhone", null);

					var orgMembersController = board.InstantiateViewController ("OrgMembersController") as OrgMembersController;

					if (orgMembersController != null) {
						orgMembersController.OrganizationId = org.OrganizationId;
						orgMembersController.OrganizationMemberMode = OrgMembersController.MemberMode.Members;
						orgMembersController.OrganizationName = org.Name;
						orgMembersController.OrganizationLogo = org.LogoFileName + "." + org.LogoType;
						NavigationController.PushViewController (orgMembersController, true);
					}
				}catch(Exception ex){
					new UIAlertView("Busidex", ex.Message, null, "OK", null).Show();
				}
			};

			src.ViewOrganizationReferrals += delegate(Organization org) {

				try{
					UIStoryboard board = UIStoryboard.FromName ("OrganizationStoryBoard_iPhone", null);

					var orgMembersController = board.InstantiateViewController ("OrgMembersController") as OrgMembersController;

					if (orgMembersController != null) {
						orgMembersController.OrganizationId = org.OrganizationId;
						orgMembersController.OrganizationName = org.Name;
						orgMembersController.OrganizationLogo = org.LogoFileName + "." + org.LogoType;
						orgMembersController.OrganizationMemberMode = OrgMembersController.MemberMode.Referrals;
						NavigationController.PushViewController (orgMembersController, true);
					}
				}catch(Exception ex){
					new UIAlertView("Busidex", ex.Message, null, "OK", null).Show();
				}
			};

			return src;
		}

		void LoadMyOrganizationsFromFile(string fullFilePath){
			if(File.Exists(fullFilePath)){
				var myOrganizationFile = File.OpenText (fullFilePath);
				var myOrganizationJson = myOrganizationFile.ReadToEnd ();
				ProcessMyOrganizations (myOrganizationJson);
			}
		}

		void ProcessMyOrganizations(string data){

			var myOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (data);
			var orgList = new List<Organization> ();
			orgList.AddRange (myOrganizationsResponse.Model);

			vwOrganizations.Source = ConfigureTableSourceEventHandlers(orgList);
		}

		async void LoadMyOrganizations(){

			var fullFilePath = Path.Combine (documentsPath, Resources.MY_ORGANIZATIONS_FILE);
			if (File.Exists (fullFilePath)) {
				LoadMyOrganizationsFromFile (fullFilePath);
			} else {
				var controller = new DataViewController ();
				await controller.LoadMyOrganizationsAsync ();
			}
		}
			
		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "My Organizations");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			vwOrganizations.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			LoadMyOrganizations ();

			txtSearch.SearchButtonClicked += delegate {
				StartSearch ();
				DoSearch();

				txtSearch.ResignFirstResponder(); // hide keyboard
			};
		} 
	}
}
