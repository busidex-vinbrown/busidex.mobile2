using Busidex3.Analytics;
using Busidex3.iOS;
using Foundation;
using Google.Analytics;
using Xamarin.Forms;

[assembly: Dependency(typeof(AnalyticsManager))]
namespace Busidex3.iOS
{
    public class AnalyticsManager : IAnalyticsManager
    {

        private static Gai gaInstance;
        private static ITracker tracker;

        public IAnalyticsManager InitWithId()
        {
            gaInstance = Gai.SharedInstance;
            gaInstance.DispatchInterval = 3;
            gaInstance.TrackUncaughtExceptions = true;
            tracker = gaInstance.GetTracker(StringResources.GOOGLE_ANALYTICS_KEY_IOS);

            return this;
        }

        public void TrackScreen(ScreenName screen)
        {
            tracker.Set(GaiConstants.ScreenName, screen.ToString());
            tracker.Send(DictionaryBuilder.CreateScreenView().Build());
            gaInstance.Dispatch();
        }

        public void TrackEvent(EventCategory category, EventAction action, string label)
        {
            tracker.Set(GaiConstants.EventCategory, category.ToString());
            tracker.Set(GaiConstants.EventAction, action.ToString());
            tracker.Set(GaiConstants.EventLabel, label);
            tracker.Set(GaiConstants.EventValue, "1");
            var dict = DictionaryBuilder.CreateEvent(
                category.ToString(), 
                action.ToString(), 
                label,
                NSNumber.FromInt32(1));
            tracker.Send(dict.Build());
            gaInstance.Dispatch();
        }
    }
} 