using System;
using System.IO;
using System.Threading.Tasks;
using Busidex3.Analytics;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Busidex3.ViewModels;
using Plugin.InputKit.Shared.Controls;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShareView
	{
        protected readonly ShareVM _viewModel = new ShareVM();
        private readonly BusidexUser _currentUser;

	    public ShareView(ref UserCard uc)
	    {
	        InitializeComponent();
	        App.AnalyticsManager.TrackScreen(ScreenName.Share);

            _viewModel.MessageSent = false;
            
            var fileName = uc.DisplaySettings.CurrentFileName;

            uc.DisplaySettings = new UserCardDisplay(fileName: fileName);
	        _viewModel.SelectedCard = uc;
	        rdoSendUsing.SelectedIndex = 0;
	        BindingContext = _viewModel;

            _currentUser = Serialization.LoadData<BusidexUser>(Path.Combine (Serialization.LocalStorageFolder, StringResources.BUSIDEX_USER_FILE));
            var ownedCard = Serialization.LoadData<Card> (Path.Combine (Serialization.LocalStorageFolder, StringResources.OWNED_CARD_FILE));
            _viewModel.SentFrom = ownedCard?.Name ?? ownedCard?.CompanyName ?? _currentUser?.UserAccount.DisplayName;
        }

        protected override bool OnBackButtonPressed()
        {
            App.LoadHomePage();
            return true;
        }

        private void RadioButton_OnClicked(object sender, EventArgs e)
	    {
            if (!(sender is RadioButton radio))
            {
                return;
            }

            txtSendTo.Keyboard = radio.Text == "Text"
                ? Keyboard.Telephone
                : Keyboard.Email;

	        txtSendTo.Placeholder = radio.Text == "Text"
	            ? "Phone Number"
	            : "Email";
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

                message = message + Environment.NewLine + shortendUrl;

                var smsTask = CrossMessaging.Current.SmsMessenger;
                if (!smsTask.CanSendSms) return false;

                await SMSShareHttpService.SaveSmsShare(_currentUser.UserId, _viewModel.SelectedCard.CardId,
                    _viewModel.SendTo, _viewModel.Message);

                smsTask.SendSms(_viewModel.SendTo, message);
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
            _viewModel.MessageSent = _viewModel.SendError = false;
        }
    }
}