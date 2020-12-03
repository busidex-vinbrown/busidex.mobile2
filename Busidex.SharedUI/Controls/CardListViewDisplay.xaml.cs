using Busidex.Models.Domain;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.SharedUI.Controls
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
	}
}