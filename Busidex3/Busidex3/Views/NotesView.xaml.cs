using System;
using Busidex.Models.Analytics;
using Busidex.Models.Constants;
using Busidex3.ViewModels;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotesView
	{
	    public CardVM ViewModel { get; set; }
        
	    public NotesView(CardVM vm)
	    {
	        InitializeComponent();

			vm.DisplaySettings = new UserCardDisplay(
				DisplaySetting.Detail,
				vm.SelectedCard.Card.FrontOrientation == "H"
					? CardOrientation.Horizontal
					: CardOrientation.Vertical,
				vm.SelectedCard.Card.FrontFileName,
				vm.SelectedCard.Card.FrontOrientation);
			ViewModel = vm;
	        BindingContext = ViewModel;
            Title = vm.SelectedCard.Card.Name ?? vm.SelectedCard.Card.CompanyName;
            App.AnalyticsManager.TrackScreen(ScreenName.Notes);
	    }

	    private async void BtnSave_OnClicked(object sender, EventArgs e)
	    {
	        prgSpinner.IsVisible = false;

	        await ViewModel.SaveNotes(txtNotes.Text);
	        
	        prgSpinner.IsVisible = true;
	    }
	}
}