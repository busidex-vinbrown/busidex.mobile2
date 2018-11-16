﻿
namespace Busidex3.Analytics
{
    public enum ScreenName
    {
        Login, 
        MyBusidex,
        CardDetail,
        Search,
        Organizations, 
        OrganizationDetail, 
        Events, 
        EventDetail,
        Settings, 
        MyCard,
        Share,
        Terms,
        Privacy
    }

    public enum EventCategory
    {
        CardEdit,
        UserInteractWithCard
    }

    public enum EventAction
    {
        Login,
        AppLoaded,
        CardImageViewed,
        PhoneDialed,
        EmailSent,
        SMSSent,
        MapViewed,
        WebPageViewed,
        NotesViewed,
        CardImageUpdated,
        CardVisibilityUpdated,
        ContactInfoUpdated,
        SearchInfoUpdated,
        CardTagsUpdated,
        CardAddressInfoUpdated,
        CardShared,
        CardAdded,
        CardRemoved,
        ForgotPassword,
        Logout
    }

    public interface IAnalyticsManager
    {
        IAnalyticsManager InitWithId();
        void TrackScreen(ScreenName screenName);
        void TrackEvent(EventCategory category, EventAction action, string label);
    }
}