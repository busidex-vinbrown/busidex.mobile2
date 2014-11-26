﻿using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Linq;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.IO;

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
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
				this.NavigationController.NavigationBar.SetBackgroundImage (null, UIBarMetrics.Default);
			}
			//((OrganizationTableSource)vwOrganizations.Source).ClearOrgNavFromAllCells ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		private void LoadMyOrganizations(){

			var cookie = GetAuthCookie ();

			if (cookie != null) {
				var controller = new Busidex.Mobile.OrganizationController ();
				var response = controller.GetMyOrganizations (cookie.Value);
				if (!string.IsNullOrEmpty (response.Result)) {
					OrganizationResponse MyOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (response.Result);
					string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					foreach(Organization org in MyOrganizationsResponse.Model){
						var fileName = org.LogoFileName + "." + org.LogoType;
						var fImagePath = Busidex.Mobile.Utils.CARD_PATH + fileName;
						if (!File.Exists (documentsPath + "/" + fileName)) {
							Busidex.Mobile.Utils.DownloadImage (fImagePath, documentsPath, fileName).ContinueWith (t => {	});
						} 
					}
					var orgList = new List<Organization> ();
					orgList.AddRange (MyOrganizationsResponse.Model);
					var src = new OrganizationTableSource (orgList);

					src.ViewOrganization += delegate(long orgId) {

						try{
							UIStoryboard board = UIStoryboard.FromName ("OrganizationStoryBoard_iPhone", null);

							var orgDetailController = board.InstantiateViewController ("OrganizationDetailController") as OrganizationDetailController;

							if (orgDetailController != null) {
								orgDetailController.OrganizationId = orgId;
								this.NavigationController.PushViewController (orgDetailController, true);
							}
						}catch(Exception ex){
							new UIAlertView("Row Selected", ex.Message, null, "OK", null).Show();
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
								this.NavigationController.PushViewController (orgMembersController, true);
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
								this.NavigationController.PushViewController (orgMembersController, true);
							}
						}catch(Exception ex){
							new UIAlertView("Busidex", ex.Message, null, "OK", null).Show();
						}
					};

					this.vwOrganizations.Source = src;

				}
			}
		}

		protected override void StartSearch(){
			base.StartSearch ();

		}

		protected override void DoSearch(){

			base.DoSearch ();

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.vwOrganizations.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			LoadMyOrganizations ();

			txtSearch.SearchButtonClicked += delegate {
				StartSearch ();
				DoSearch();

				txtSearch.ResignFirstResponder(); // hide keyboard
			};
		}
	}
}
