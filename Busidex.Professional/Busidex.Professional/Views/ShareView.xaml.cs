using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Analytics;
using Busidex.Models.Constants;
using Busidex.Models.Domain;
using Busidex.Professional.ViewModels;
using Busidex.Resources.String;
using Plugin.ContactService.Shared;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShareView
	{
        protected readonly ShareVM _viewModel;
        private readonly BusidexUser _currentUser;
        private const string SEND_TO_EMAIL_LABEL = "Send To Email";
        private const string SEND_TO_PHONE_LABEL = "Send To Phone Number";
        

        public ShareView(ref UserCard uc)
	    {
	        InitializeComponent();
	        App.AnalyticsManager.TrackScreen(ScreenName.Share);

            _viewModel = new ShareVM
            {
                MessageSent = false,
                SelectedCard = uc
            };
            _viewModel.DisplaySettings = new UserCardDisplay(
                DisplaySetting.Detail,
                uc.Card.FrontOrientation == "H"
                    ? CardOrientation.Horizontal
                    : CardOrientation.Vertical,
                uc.Card.FrontFileName,
                uc.Card.FrontOrientation);
            rdoSendUsing.SelectedIndex = 0;
	        BindingContext = _viewModel;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.LoadContacts();
            });

            _currentUser = Serialization.LoadData<BusidexUser>(Path.Combine (Serialization.LocalStorageFolder, StringResources.BUSIDEX_USER_FILE));
            var ownedCard = Serialization.LoadData<Card> (Path.Combine (Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE));
            _viewModel.SentFrom = ownedCard?.Name ?? ownedCard?.CompanyName ?? _currentUser?.UserAccount.DisplayName;
            _viewModel.AllowSend = false;

            txtSendTo.Placeholder = SEND_TO_PHONE_LABEL;
            txtSentFrom.Placeholder = "From";
        }

        private async void Home_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            if(_viewModel.ShowContacts || _viewModel.ShowContact)
            {
                _viewModel.ShowContacts = false;
                _viewModel.HideContacts = true;
                _viewModel.ShowContact = false;
            }
            else
            {
                //App.LoadHomePage();
                Navigation.PopToRootAsync();
            }
            
            return true;
        }

        private void RadioButton_OnClicked(object sender, EventArgs e)
	    {
            if (!(sender is Plugin.InputKit.Shared.Controls.RadioButton radio))
            {
                return;
            }

            if(radio.Text == "Text" && txtSendTo.Keyboard == Keyboard.Email)
            {
                txtSendTo.Text = string.Empty;
            }
            if (radio.Text == "Email" && txtSendTo.Keyboard == Keyboard.Telephone)
            {
                txtSendTo.Text = string.Empty;
            }

            txtSendTo.Keyboard = radio.Text == "Text"
                ? Keyboard.Telephone
                : Keyboard.Email;

	        txtSendTo.Placeholder = radio.Text == "Text"
	            ? SEND_TO_PHONE_LABEL
                : SEND_TO_EMAIL_LABEL;
	    }

        private async void BtnShare_OnClicked(object sender, EventArgs e)
        {
            _viewModel.MessageSent = false;

            if (string.IsNullOrEmpty(_viewModel.SentFrom))
            {
                await DisplayAlert(
                    "Required", 
                    StringResources.MESSAGE_SEND_FROM_REQUIRED, 
                    "Ok");
                txtSentFrom.Focus();
                return;
            }
            if (string.IsNullOrEmpty(_viewModel.SendTo))
            {
                await DisplayAlert(
                    "Required", 
                    StringResources.MESSAGE_SEND_TO_REQUIRED, 
                    "Ok");
                txtSendTo.Focus();
                return;
            }

            if (rdoSendUsing.SelectedIndex == 0)
            {
                _viewModel.MessageSent = await sendSMS();
            }
            else
            {
                _viewModel.MessageSent = await sendEmail();
            }
        }

        private async Task<bool> sendEmail()
        {
            return await SharedCardHttpService.ShareCard (_viewModel.SelectedCard.Card, _viewModel.SendTo, _viewModel.Message);
        }

        private async Task<bool> sendSMS()
        {
            var response = await EmailTemplateHttpService.GetTemplate(EmailTemplateCode.SharedCardSMS);

            if (response == null) return false;

            var message = string.Format(response.Template.Subject, _viewModel.SentFrom) + Environment.NewLine +
                          Environment.NewLine +
                          response.Template.Body;
            var parameters = new QuickShareLink
            {
                CardId = _viewModel.SelectedCard.Card.CardId,
                From = _currentUser.UserId,
                DisplayName = _viewModel.SentFrom,
                PersonalMessage = _viewModel.Message
            };
            try
            {
                var shortendUrl = await BranchApiHttpService.GetBranchUrl(parameters);
                if (shortendUrl == null || shortendUrl.Contains("error")) return false;

                message = message + 
                    Environment.NewLine + 
                    _viewModel.Message + 
                    Environment.NewLine + 
                    shortendUrl;

                var smsTask = CrossMessaging.Current.SmsMessenger;
                if (!smsTask.CanSendSms) return false;

                smsTask.SendSms(_viewModel.SendTo, message);

                await SMSShareHttpService.SaveSmsShare(_currentUser.UserId, _viewModel.SelectedCard.CardId,
                    _viewModel.SendTo, message);
                
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }

        private void TxtSendTo_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.AllowSend = validate();
            _viewModel.MessageSent = _viewModel.SendError = false;
        }

        private void TxtSentFrom_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.AllowSend = validate();
            _viewModel.MessageSent = _viewModel.SendError = false;
        }

        private bool validate()
        {
            return !string.IsNullOrEmpty(_viewModel.SendTo) && 
                   !string.IsNullOrEmpty(_viewModel.SentFrom);
        }

        private void Contacts_Tapped(object sender, EventArgs e)
        {
            //NavigationPage.SetHasNavigationBar(this, false);
            Shell.SetNavBarIsVisible(this, false);
            _viewModel.ShowContacts = true;
            _viewModel.HideContacts = false;
            _viewModel.ShowContact = false;
        }

        private void Contact_Tapped(object sender, EventArgs e)
        {
            _viewModel.ShowContacts = false;
            _viewModel.HideContacts = false;
            _viewModel.ShowContact = true;

            var contact = ((TappedEventArgs)e).Parameter as Contact;        
            if(contact != null)
            {
                _viewModel.SelectedContact = contact;
                _viewModel.SelectedContactPhotoUri = string.IsNullOrEmpty(contact.PhotoUriThumbnail)
                    ? "defaultprofile.png"
                    : contact.PhotoUriThumbnail;


                var fromMyBusidex = Serialization.IsImageNameGuid(contact.PhotoUriThumbnail);
                imgContact.IsVisible = !fromMyBusidex &&!string.IsNullOrEmpty(contact.PhotoUriThumbnail);
                imgDefaultContact.IsVisible = !fromMyBusidex && string.IsNullOrEmpty(contact.PhotoUriThumbnail);
                imgMyBusidexContact.IsVisible = false;

                _viewModel.ContactImageHeight = 150;
                _viewModel.ContactImageWidth = 150;
                _viewModel.ContactFrameHeight = 200;
                _viewModel.ContactFrameWidth = 200;

                // I'm storing the orientation for the myBusidex contacts in the PhotoUri
                var orientation = contact.PhotoUri;
                if (fromMyBusidex)
                {
                    imgMyBusidexContact.IsVisible = true;
                    _viewModel.ContactImageHeight = orientation == "H" ? 152 : 250;
                    _viewModel.ContactImageWidth = orientation == "H" ? 250 : 152;
                    _viewModel.ContactFrameHeight = orientation == "H" ? 183 : 300;
                    _viewModel.ContactFrameWidth = orientation == "H" ? 300 : 183;
                }
            }
        }

        private void ShowShareView()
        {
            //NavigationPage.SetHasNavigationBar(this, true);
            Shell.SetNavBarIsVisible(this, true);
            _viewModel.SelectedContact = null;
            _viewModel.ShowContacts = false;
            _viewModel.HideContacts = true;
            _viewModel.ShowContact = false;
            _viewModel.RefreshContacts();
        }

        private void ContactEmail_Tapped(object sender, EventArgs e)
        {
            var email = ((TappedEventArgs)e).Parameter as string;
            rdoSendUsing.SelectedIndex = 1;
            txtSendTo.Keyboard = Keyboard.Email;
            txtSendTo.Placeholder = SEND_TO_EMAIL_LABEL;
            txtSendTo.Text = email;
            ShowShareView();
        }

        private void ContactNumber_Tapped(object sender, EventArgs e)
        {
            var number = ((TappedEventArgs)e).Parameter as string;
            rdoSendUsing.SelectedIndex = 0;
            txtSendTo.Keyboard = Keyboard.Telephone;
            txtSendTo.Placeholder = SEND_TO_PHONE_LABEL;
            txtSendTo.Text = number;
            ShowShareView();
        }

        private void BtnCancelContactList_Clicked(object sender, EventArgs e)
        {
            ShowShareView();
        }

        private void BtnCancelContact_Clicked(object sender, EventArgs e)
        {
            _viewModel.SelectedContact = null;
            _viewModel.ShowContacts = true;
            _viewModel.HideContacts = false;
            _viewModel.ShowContact = false;
          }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                _viewModel.ClearSearch();
            }
        }

        private void TxtSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            _viewModel.DoSearch();
        }
    }
}