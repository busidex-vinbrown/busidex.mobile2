using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EventsView : ContentPage
	{
        private readonly EventListVM _viewModel = new EventListVM();

        public EventsView ()
		{
			InitializeComponent ();
		    App.AnalyticsManager.TrackScreen(ScreenName.Events);
            _viewModel.EventList = Serialization.GetCachedResult<List<EventTag>>(Path.Combine(Serialization.LocalStorageFolder, StringResources.EVENT_LIST_FILE));
            BindingContext = _viewModel;
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var eventTag = ((TappedEventArgs)e).Parameter as EventTag;
            var page = new EventDetailView(eventTag);
            Navigation.PushAsync(page);
        }
    }
}