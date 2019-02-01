//using Android.Gms.Analytics;
using Busidex3.Analytics;
using Busidex3.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AnalyticsManager))]
namespace Busidex3.Droid
{
    public class AnalyticsManager : IAnalyticsManager
    {
        //private static GoogleAnalytics gaInstance;
        //private static Tracker tracker;

        public IAnalyticsManager InitWithId()
        {
           // gaInstance = GoogleAnalytics.GetInstance(Android.App.Application.Context);
           // gaInstance.SetLocalDispatchPeriod(3);
           // tracker = gaInstance.NewTracker(StringResources.GOOGLE_ANALYTICS_KEY_ANDROID);
            return this;
        }

        public void TrackScreen(ScreenName screen)
        {
            //tracker.SetScreenName(screen.ToString());
            //var builder = new HitBuilders.ScreenViewBuilder();
            //tracker.Send(builder.Build());
            //gaInstance.DispatchLocalHits();
        }

        public void TrackEvent(EventCategory category, EventAction action, string label)
        {
            //var builder = new HitBuilders.EventBuilder(category.ToString(), action.ToString());
            //builder.SetLabel(label);
            //builder.SetValue(1);
            //tracker.Send(builder.Build());
            //gaInstance.DispatchLocalHits();
        }
    }
}