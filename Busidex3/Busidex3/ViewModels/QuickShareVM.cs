using Busidex3.DomainModels;

namespace Busidex3.ViewModels
{
    public class QuickShareVM : BaseViewModel
    {
        public UserCard SelectedCard { get; set; }

        private string _greeting { get; set; }
        public string Greeting
        {
            get => _greeting;
            set
            {
                _greeting = value;
                OnPropertyChanged(nameof(Greeting));
            }
        }

        private string _personalMessage { get; set; }
        public string PersonalMessage
        {
            get => _personalMessage;
            set
            {
                _personalMessage = value;
                OnPropertyChanged(nameof(PersonalMessage));
            }
        }
    }
}
