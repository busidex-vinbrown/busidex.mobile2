using System;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Presentation.iOS
{
	public delegate void OnItemSelectedHandler ();

	public class BasePickerViewModel : UIPickerViewModel
	{
		

		protected UILabel lbl;

		public BasePickerViewModel ()
		{
		}

		public override nint GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

	}
}

