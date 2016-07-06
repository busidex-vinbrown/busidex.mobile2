using System;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Busidex.Mobile;

namespace Busidex.Presentation.iOS
{
	public class StateCodeModel : BasePickerViewModel
	{
		public State selectedState { get; set; }

		List<State> states { get; set; }

		public event OnItemSelectedHandler OnItemSelected;

		public StateCodeModel (UILabel lbl, State _selectedState)
		{
			this.lbl = lbl;
			states = new List<State> ();
			states.AddRange (UISubscriptionService.GetStates ());

			selectedState = _selectedState;
			if (selectedState != null) {
				lbl.Text = selectedState.Name;
			}
		}

		public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			return states.Count;
		}

		public override string GetTitle (UIPickerView pickerView, nint row, nint component)
		{
			return states [(int)row].Name;
			//
			//				switch (component) {
			//				case 0:
			//					return states [(int)row].Name;
			//				case 1:
			//					return states [(int)row].StateCodeId.ToString ();
			//				case 2:
			//					return states [(int)row].Code;
			//				default:
			//					throw new NotImplementedException ();
			//				}
		}

		public override void Selected (UIPickerView pickerView, nint row, nint component)
		{
			selectedState = states [(int)pickerView.SelectedRowInComponent (new nint (0))];
			lbl.Text = selectedState.Name;
			if (OnItemSelected != null) {
				OnItemSelected ();
			}
		}

		public override nfloat GetComponentWidth (UIPickerView pickerView, nint component)
		{
			return 220f;
		}

		public int IndexOf (State state)
		{
			var idx = -1;
			if (state == null) {
				return idx;
			}

			var selectedIdx = idx;
			states.ForEach (s => {
				idx++;
				if (state.Code == s.Code) {
					selectedIdx = idx;
				}
			});
			return selectedIdx;
		}


	}
}

