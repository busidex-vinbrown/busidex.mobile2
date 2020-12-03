using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Models.Dto;
using Busidex3.ViewModels;
using Plugin.Messaging;
using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendOwnerCardView : ContentPage
    {
        public readonly SendOwnerCardVM _viewModel;

        public SendOwnerCardView(UnownedCard uc)
        {
            InitializeComponent();
            rdoSendUsing.SelectedIndex = 0;
            _viewModel = new SendOwnerCardVM(uc);

            BindingContext = _viewModel;
        }

        private void RadioButton_OnClicked(object sender, EventArgs e)
        {
            if (!(sender is Plugin.InputKit.Shared.Controls.RadioButton radio))
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


        private async Task<OwnerEmailResponse> sendEmail()
        {
            var service = new AdminHttpService();
            return await service.SendOwnerEmails(_viewModel.SelectedCard.CardId, _viewModel.SendTo);
        }

        private async Task<bool> sendSMS()
        {
            try
            {
                var response = await EmailTemplateHttpService.GetTemplate(EmailTemplateCode.ConfirmOwner);

                if (response == null) return false;

                var template = response.Template;
                var card = _viewModel.SelectedCard;

                var message = template.Subject + Environment.NewLine + card.CompanyName ?? card.Name;

                var parameters = new QuickShareLink
                {
                    CardId = card.CardId,
                    From = Security.CurrentUser.UserId,
                    DisplayName = Security.CurrentUser.UserAccount.DisplayName,
                    PersonalMessage = _viewModel.Message
                };

                var shortendUrl = await BranchApiHttpService.GetBranchUrl(parameters);
                if (shortendUrl == null || shortendUrl.Contains("error")) return false;

                message = message + Environment.NewLine + shortendUrl;

                var smsTask = CrossMessaging.Current.SmsMessenger;
                if (!smsTask.CanSendSms) return false;
                
                await SMSShareHttpService.SaveSmsShare(Security.CurrentUser.UserId, card.CardId,
                    _viewModel.SendTo, _viewModel.Message ?? string.Empty);

                smsTask.SendSms(_viewModel.SendTo, message);
                
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }

        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            if (rdoSendUsing.SelectedIndex == 0)
            {
                _viewModel.MessageSent = await sendSMS();
            }
            else
            {
                var response = await sendEmail();
                _viewModel.MessageSent = response.Success;
            }
        }

        private void TxtSendTo_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.MessageSent = _viewModel.SendError = false;
        }
    }
}