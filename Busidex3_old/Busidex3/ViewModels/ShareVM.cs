using System.Collections.Generic;
using Busidex3.DomainModels;

namespace Busidex3.ViewModels
{
    public class ShareVM : BaseViewModel
    {
        public ShareVM()
        {
            SendMethod.Add(0, "Text");
            SendMethod.Add(1, "Email");
        }

        public string Message { get; set; }
        public UserCard SelectedCard { get; set; }

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
