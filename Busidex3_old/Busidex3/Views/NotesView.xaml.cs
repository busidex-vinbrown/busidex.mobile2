﻿using System;
using Busidex3.Analytics;
using Busidex3.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotesView : ContentPage
	{
	    public CardVM ViewModel { get; set; }
        
	    public NotesView(CardVM vm)
	    {
	        InitializeComponent();

	        ViewModel = vm;
	        BindingContext = ViewModel;
	        App.AnalyticsManager.TrackScreen(ScreenName.Notes);
	    }

	    private async void BtnSave_OnClicked(object sender, EventArgs e)
	    {
	        prgSpinner.IsVisible = false;
	        txtNotes.IsEnabled = false;

	        await ViewModel.SaveNotes(txtNotes.Text);
	        
	        prgSpinner.IsVisible = true;
	        txtNotes.IsEnabled = true;
	    }
	}
}