using System;

namespace Busidex.Models.Domain
{
    public class UnownedCard : Card
    {
        private DateTime? _lastContactDate;
        public DateTime? LastContactDate {
            get => _lastContactDate;
            set {
                _lastContactDate = value;
                OnPropertyChanged(nameof(LastContactDate));
            }
        }

        private string _emailSentTo;
        public string EmailSentTo {
            get => _emailSentTo;
            set {
                _emailSentTo = value;
                OnPropertyChanged(nameof(EmailSentTo));
            }
        }
    }
}
