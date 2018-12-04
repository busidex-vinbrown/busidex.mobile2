using Busidex3.Analytics;
using Busidex3.DomainModels;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShareView
	{
	    public UserCard _viewModel { get; set; }

	    public ShareView(ref UserCard vm)
	    {
	        InitializeComponent();
	        App.AnalyticsManager.TrackScreen(ScreenName.Share);

	        _viewModel = vm;
	        BindingContext = _viewModel;
	    }
	}
}