using Busidex.SharedUI;

namespace Busidex3.ViewModels
{
    public class CardImageVM : BaseViewModel
    {

		private IUserCardDisplay _displaySettings;
		public IUserCardDisplay DisplaySettings {
			get {
				return _displaySettings;
			}
			set {
				_displaySettings = value;
				OnPropertyChanged(nameof(DisplaySettings));
			}
		}
	}
}
