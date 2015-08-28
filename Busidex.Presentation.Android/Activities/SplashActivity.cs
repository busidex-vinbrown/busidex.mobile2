using Android.App;
using Android.OS;
using System.Collections.Generic;

namespace Busidex.Presentation.Android
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges=global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize)]			
	public class SplashActivity : BaseActivity
	{

		public Dictionary<string, Fragment> fragments;


		protected override void OnStart ()
		{
			base.OnStart ();

			fragments = new Dictionary<string, Fragment> ();

			fragments.Add (typeof(EventListFragment).Name, new EventListFragment ());
			fragments.Add (typeof(LoginFragment).Name, new LoginFragment ());
			fragments.Add (typeof(MainFragment).Name, new MainFragment ());
			fragments.Add (typeof(MyBusidexFragment).Name, new MyBusidexFragment ());
			fragments.Add (typeof(MyOrganizationsFragment).Name, new MyOrganizationsFragment ());
			fragments.Add (typeof(ProfileFragment).Name, new ProfileFragment ());
			fragments.Add (typeof(SearchFragment).Name, new SearchFragment ());
			fragments.Add (typeof(SharedCardsFragment).Name, new SharedCardsFragment ());
			fragments.Add (typeof(StartUpFragment).Name, new StartUpFragment ());
			fragments.Add (typeof(CardDetailFragment).Name, new CardDetailFragment ());

		    
			RedirectToMainIfLoggedIn ();
		}
			

		public override void OnBackPressed ()
		{
			var mainFragment = FragmentManager.FindFragmentByTag ("MainFragment");
			var loginFragment = FragmentManager.FindFragmentByTag ("LoginFragment");
			if (mainFragment != null && mainFragment.IsVisible) {
				
			} else if (loginFragment != null && loginFragment.IsVisible) {

			}else {
				UnloadFragment ();
			}

		}

		protected void RedirectToMainIfLoggedIn(){

			if(applicationResource.GetAuthCookie () == null){
				LoadFragment (fragments[typeof(StartUpFragment).Name]);
			}else{
				LoadFragment (fragments[typeof(MainFragment).Name]);
			}

		}

		void setUpTabs(){

			ActionBar.Tab tab = ActionBar.NewTab();
			//tab.SetText(Resources.GetString(Resource.String.tab1_text));
			tab.SetIcon(Resource.Drawable.SearchIcon);
			tab.TabSelected += (sender, args) => LoadFragment (fragments [typeof(SearchFragment).Name]);
			ActionBar.AddTab(tab);

			tab = ActionBar.NewTab();
			//tab.SetText(Resources.GetString(Resource.String.tab2_text));
			tab.SetIcon(Resource.Drawable.MyBusidexIcon);
			tab.TabSelected += (sender, args) => LoadFragment (fragments [typeof(MyBusidexFragment).Name]);
			ActionBar.AddTab(tab);

			tab = ActionBar.NewTab();
			//tab.SetText(Resources.GetString(Resource.String.tab2_text));
			tab.SetIcon(Resource.Drawable.OrganizationsIcon);
			tab.TabSelected += (sender, args) => LoadFragment (fragments [typeof(MyOrganizationsFragment).Name]);
			ActionBar.AddTab(tab);

			tab = ActionBar.NewTab();
			//tab.SetText(Resources.GetString(Resource.String.tab2_text));
			tab.SetIcon(Resource.Drawable.EventIcon);
			tab.TabSelected += (sender, args) => LoadFragment (fragments [typeof(EventListFragment).Name]);
			ActionBar.AddTab(tab);
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			//ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			SetContentView (Resource.Layout.StartUp);
			//setUpTabs ();
		}



		protected BaseFragment getFragment(string tag){
			return (BaseFragment)FragmentManager.FindFragmentByTag (tag);		
		}

		public void UnloadFragment(){
			FragmentManager.PopBackStack ();
		}

		/*
		private BaseFragment getFragmentByType(string typeName){

			BaseFragment fragment;

			switch (typeName) {
			case "MainFragment":
				{
					fragment = new MainFragment ();
					break;
				}
			case "StartUpFragment":
				{
					fragment = new StartUpFragment ();
					break;
				}
			case "LoginFragment":
				{
					fragment = new LoginFragment ();
					break;
				}
			case "MyBusidexFragment":
				{
					fragment = new MyBusidexFragment ();
					break;
				}
			case "SearchFragment":
				{
					fragment = new SearchFragment ();
					break;
				}
			case "EventListFragment":
				{
					fragment = new EventListFragment ();
					break;
				}
			case "MyOrganizationsFragment":
				{
					fragment = new MyOrganizationsFragment ();
					break;
				}
			case "CardDetailFragment":{
					fragment = new CardDetailFragment ();
					break;
				}
			default:{
					fragment = new MainFragment ();
					break;
				}
			}
			return fragment;
		}
*/

	}
}