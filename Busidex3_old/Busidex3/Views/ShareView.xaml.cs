using System;
using System.Collections.Generic;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
//using PSC.Xamarin.Controls.BindableRadioButton;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShareView
	{
        protected readonly ShareVM _viewModel = new ShareVM();

	    public ShareView(ref UserCard card)
	    {
	        //InitializeComponent();
	        App.AnalyticsManager.TrackScreen(ScreenName.Share);

	        _viewModel.SelectedCard = card;
	       // rdoSendUsing.SelectedIndex = 0;
	        BindingContext = _viewModel;
	    }

	
	    private void RdoSendUsing_OnCheckedChanged(object sender, int e)
        {
            //if (!(sender is CustomRadioButton radio) || radio.Id < 0)
            //{
            //    return;
            //}

            //txtSendTo.Keyboard = radio.Text == "Phone Number"
            //    ? Keyboard.Telephone
            //    : Keyboard.Email;

        }
    }
}