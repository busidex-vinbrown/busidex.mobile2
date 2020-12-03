using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.SharedUI.Controls
{
    public delegate void OnCardImageClickedResult(IUserCardDisplay ucd);

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardImageHeader : ContentView
    {
	    public event OnCardImageClickedResult OnCardImageClicked;

	    public CardImageHeader()
	    {	       
			InitializeComponent ();
	    }

	    private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
	    {
	        var ucd = ((TappedEventArgs)e).Parameter as IUserCardDisplay;
            if(ucd != null)
            {
                OnCardImageClicked?.Invoke(ucd);
            }
	    }
	}
}