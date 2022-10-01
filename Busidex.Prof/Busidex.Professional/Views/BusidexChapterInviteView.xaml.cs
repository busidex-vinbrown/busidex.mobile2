using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusidexChapterInviteView : ContentPage
    {
        private readonly OrganizationDetailVM _viewModel;
        public BusidexChapterInviteView(Organization org)
        {
            InitializeComponent();

            _viewModel = new OrganizationDetailVM(org, false);

            BindingContext = _viewModel;
        }
    }
}