﻿using Busidex3.Analytics;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShareView : ContentPage
	{
	    public CardVM _viewModel { get; set; }

	    public ShareView(CardVM vm)
	    {
	        InitializeComponent();
	        App.AnalyticsManager.TrackScreen(ScreenName.Share);

	        _viewModel = vm;
	        BindingContext = _viewModel;
	    }
	}
}