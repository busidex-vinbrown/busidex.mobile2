using Android.App;
using Android.OS;
using System.Collections.Generic;

namespace Busidex.Presentation.Android
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]			
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

			RedirectToMainIfLoggedIn ();
		}

		protected void RedirectToMainIfLoggedIn(){

			if(applicationResource.GetAuthCookie () == null){
				LoadFragment (fragments[typeof(StartUpFragment).Name]);
			}else{
				LoadFragment (fragments[typeof(MainFragment).Name]);
			}

		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();

			if (FragmentManager.BackStackEntryCount <= 1) {
				LoadFragment (fragments[typeof(MainFragment).Name]);
			} else {
				UnloadFragment ();
			}
		}
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.StartUp);
		}

		Fragment currentFragment { get; set; }
		public void LoadFragment(Fragment fragment){

			using (var transaction = FragmentManager.BeginTransaction ()) {

				transaction
					.SetCustomAnimations (
						Resource.Animator.SlideAnimation, 
						Resource.Animator.SlideOutAnimation, 
						Resource.Animator.SlideAnimation, 
						Resource.Animator.SlideOutAnimation
					)
					.Replace (Resource.Id.fragment_holder, fragment, fragment.GetType ().Name)
					.AddToBackStack (fragment.GetType ().Name)
					.Commit ();
			}
			//fragment.OnResume ();

		}

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
			default:{
					fragment = new MainFragment ();
					break;
				}
			}
			return fragment;
		}

		protected BaseFragment getFragment(string tag){
			return (BaseFragment)FragmentManager.FindFragmentByTag (tag);		
		}

		protected void UnloadFragment(){
			FragmentManager.PopBackStack ();
		}
	}
}