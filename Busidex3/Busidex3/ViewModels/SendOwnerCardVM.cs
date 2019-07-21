using Busidex3.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Busidex3.ViewModels
{
    public class SendOwnerCardVM : BaseViewModel
    {
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

        private string _sendTo { get; set; }
        public string SendTo
        {
            get => _sendTo;
            set
            {
                _sendTo = value;
                OnPropertyChanged(nameof(SendTo));
            }
        }

        private bool _sendError { get; set; }
        public bool SendError
        {
            get => _sendError;
            set
            {
                _sendError = value;
                OnPropertyChanged(nameof(SendError));
            }
        }

        private bool _messageSent { get; set; }
        public bool MessageSent
        {
            get => _messageSent;
            set
            {
                _messageSent = value;
                OnPropertyChanged(nameof(MessageSent));
            }
        }

        private string _message { get; set; }
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        private UnownedCard _selectedCard;
        public UnownedCard SelectedCard {
            get => _selectedCard;
            set
            {
                _selectedCard = value;
                OnPropertyChanged(nameof(SelectedCard));
            }
        }

        public double VFrameHeight { get; set; }
        public double HFrameHeight { get; set; }
        public double VFrameWidth { get; set; }
        public double HFrameWidth { get; set; }

        public double VImageHeight { get; set; }
        public double HImageHeight { get; set; }
        public double VImageWidth { get; set; }
        public double HImageWidth { get; set; }

        public SendOwnerCardVM(UnownedCard uc)
        {
            SelectedCard = uc;
            SendMethod.Add(0, "Text");
            SendMethod.Add(1, "Email");

            FileName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + uc.FrontFileName;

            double scale = 1;
            VFrameHeight = 300 * scale;
            VFrameWidth = 183 * scale;
            VImageHeight = 250 * scale;
            VImageWidth = 152 * scale;

            HFrameHeight = 183 * scale;
            HFrameWidth = 300 * scale;
            HImageHeight = 152 * scale;
            HImageWidth = 250 * scale;
        }

        
    }
}
