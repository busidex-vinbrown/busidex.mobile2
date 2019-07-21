using Busidex3.DomainModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnownedCardListViewDisplay : ContentView
    {
        public static readonly BindableProperty SelectedCardProperty =
            BindableProperty.Create("SelectedCard", typeof(UnownedCard), typeof(UnownedCard), default(UnownedCard));

        public UnownedCard SelectedCard
        {
            get => (UnownedCard)GetValue(SelectedCardProperty);
            set => SetValue(SelectedCardProperty, value);
        }

        public UnownedCardListViewDisplay()
        {
            InitializeComponent();
        }
    }
}