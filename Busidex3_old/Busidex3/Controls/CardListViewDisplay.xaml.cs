using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Busidex3.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardListViewDisplay : ContentView
	{
	    public static readonly BindableProperty SelectedCardProperty =
	        BindableProperty.Create("SelectedCard", typeof(UserCard), typeof(UserCard), default(UserCard));

	    public UserCard SelectedCard
	    {
	        get => (UserCard)GetValue(SelectedCardProperty);
	        set => SetValue(SelectedCardProperty, value);
	    }

		public CardListViewDisplay ()
		{
			InitializeComponent ();
		}

	    //private async void TapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
	    //{
	    //    if (SelectedCard == null) return;

	    //    var vm = SelectedCard;
	    //    var cardViewModel = new CardVM(vm);
            
	    //    await Navigation.PushAsync(new CardDetailView(cardViewModel));
	    //}
	}
}