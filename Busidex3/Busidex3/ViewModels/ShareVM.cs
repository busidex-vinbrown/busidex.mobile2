using System.Collections.Generic;
using System.Reflection;
using Busidex3.DomainModels;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class ShareVM : BaseViewModel
    {
        public ShareVM()
        {
            SendMethod.Add(0, "Text");
            SendMethod.Add(1, "Email");
            SuccessImage = ImageSource.FromResource("Busidex3.Resources.checkmark.png",
                typeof(ShareVM).GetTypeInfo().Assembly);
        }

        public UserCard SelectedCard { get; set; }

        private string _sentFrom { get; set; }
        public string SentFrom { get => _sentFrom;
            set
            {
                _sentFrom = value;
                OnPropertyChanged(nameof(SentFrom));
            }
        }

        private string _sendTo { get; set; }
        public string SendTo { get => _sendTo;
            set
            {
                _sendTo = value;
                OnPropertyChanged(nameof(SendTo));
            }
        }

        private string _message { get; set; }
        public string Message { get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private bool _sendError { get; set; }
        public bool SendError { get => _sendError;
            set
            {
                _sendError = value;
                OnPropertyChanged(nameof(SendError));
            }
        }

        private bool _messageSent { get; set; }
        public bool MessageSent { get => _messageSent;
            set
            {
                _messageSent = value;
                OnPropertyChanged(nameof(MessageSent));
            }
        }

        private ImageSource _successImage { get; set; }
        public ImageSource SuccessImage { get => _successImage;
            set
            {
                _successImage = value;
                OnPropertyChanged(nameof(SuccessImage));
            }
        }

        private Dictionary<int, string> _sendMethod = new Dictionary<int, string>();
        public Dictionary<int, string> SendMethod
        {
            get => _sendMethod;
            set
            {
                _sendMethod = value;
                OnPropertyChanged(nameof(SendMethod));
            }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set {
                if (value == selectedIndex) return;
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
    }
}
