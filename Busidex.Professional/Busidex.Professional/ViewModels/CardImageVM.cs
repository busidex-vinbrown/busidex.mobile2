using Busidex.SharedUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Busidex.Professional.ViewModels
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
