using Busidex.Http.Utils;
using Busidex.Models.Constants;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Resources.String;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using Xamarin.Forms;

namespace Busidex.Professional.Views.EditCard
{
    [QueryProperty(nameof(UserCardJson), "ucJson")]
    //[QueryProperty(nameof(MyBusidexJson), "mbJson")]
    public partial class BaseEditCardView : ContentPage, IQueryAttributable, INotifyPropertyChanged
    {
        public BaseEditCardView()
        {

        }

        public UserCardDisplay DisplaySettings { get; set; }

        protected CardVM _viewModel { get; set; }

        private string _userCardJson = "";
        protected string UserCardJson
        {
            get => _userCardJson;
            set
            {
                _userCardJson = value;
            }
        }

        //private string _myBusidexJson = "";
        //protected string MyBusidexJson
        //{
        //    get => _myBusidexJson;
        //    set
        //    {
        //        _myBusidexJson = value;
        //    }
        //}

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _userCardJson = HttpUtility.UrlDecode(query["ucJson"]);
            //_myBusidexJson = HttpUtility.UrlDecode(query["mbJson"]);
        }

        protected override void OnAppearing()
        {
            var ucJson = Encoding.UTF8.GetString(UserCardJson.FromHex());
            var userCard = JsonConvert.DeserializeObject<UserCard>(ucJson);

            var mbPath = Path.Combine(Serialization.LocalStorageFolder, StringResources.MY_BUSIDEX_FILE);
            var myBusidex = Serialization.GetCachedResult<List<UserCard>>(mbPath) ?? new List<UserCard>();

            //var mbJson = Encoding.UTF8.GetString(Convert.FromBase64String(MyBusidexJson));
            //var myBusidex = JsonConvert.DeserializeObject<List<UserCard>>(mbJson);

            var vm = new CardVM(ref userCard, ref myBusidex);

            _viewModel = vm;
            _viewModel.DisplaySettings = new UserCardDisplay(
                DisplaySetting.Detail,
                vm.SelectedCard.Card.FrontOrientation == "H"
                    ? CardOrientation.Horizontal
                    : CardOrientation.Vertical,
                vm.SelectedCard.Card.FrontFileName,
                vm.SelectedCard.Card.FrontOrientation);

            BindingContext = _viewModel;
            _viewModel.SetViewHeightForOrientation(_viewModel.SelectedCard.Card.FrontOrientation);
            base.OnAppearing();
        }
    }
}
