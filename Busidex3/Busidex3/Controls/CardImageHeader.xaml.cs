using Busidex3.DomainModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Controls
{
    public delegate void OnCardImageClickedResult(UserCard uc);

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardImageHeader : ContentView
	{

	    public event OnCardImageClickedResult OnCardImageClicked;

	    public CardImageHeader()
	    {	       
			InitializeComponent ();
	    }

	    private void TapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
	    {
	        var uc = e.Parameter as UserCard;
	        OnCardImageClicked?.Invoke(uc);
	    }
	}
}