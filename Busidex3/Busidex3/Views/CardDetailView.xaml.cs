using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardDetailView : ContentPage
	{
		public CardDetailView (UserCard card)
		{
			InitializeComponent ();

		    BindingContext = card;
		}
	}
}