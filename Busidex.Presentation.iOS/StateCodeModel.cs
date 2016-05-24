using System;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Presentation.iOS
{
	public class StateCodeModel : BasePickerViewModel
	{
		State selectedState;

		List<State> states { get; set; }

		public event OnItemSelectedHandler OnItemSelected;

		public StateCodeModel (UILabel lbl, State _selectedState)
		{
			this.lbl = lbl;
			states = new List<State> ();
			states.AddRange (getStates ());

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
			var selectedIdx = idx;
			states.ForEach (s => {
				idx++;
				if (state.Code == s.Code) {
					selectedIdx = idx;
				}
			});
			return selectedIdx;	
		}

		List<State> getStates ()
		{
			var _states = new List<State> ();
			states.Add (new State {
				                StateCodeId = 1,
				                Code = "AL",
				                Name = "Alabama"
			});
			states.Add (new State {
				                StateCodeId = 2,
				                Code = "AK",
				                Name = "Alaska"
			});
			states.Add (new State {
				                StateCodeId = 3,
				                Code = "AZ",
				                Name = "Arizona"
			});
			states.Add (new State {
				                StateCodeId = 4,
				                Code = "AR",
				                Name = "Arkansas"
			});
			states.Add (new State {
				                StateCodeId = 5,
				                Code = "CA",
				                Name = "California"
			});
			states.Add (new State {
				                StateCodeId = 6,
				                Code = "CO",
				                Name = "Colorado"
			});
			states.Add (new State {
				                StateCodeId = 7,
				                Code = "CT",
				                Name = "Connecticut"
			});
			states.Add (new State {
				                StateCodeId = 8,
				                Code = "DE",
				                Name = "Delaware"
			});
			states.Add (new State {
				                StateCodeId = 9,
				                Code = "DC",
				                Name = "District Of Columbia"
			});
			states.Add (new State {
				                StateCodeId = 10,
				                Code = "FL",
				                Name = "Florida"
			});
			states.Add (new State {
				                StateCodeId = 11,
				                Code = "GA",
				                Name = "Georgia"
			});
			states.Add (new State {
				                StateCodeId = 12,
				                Code = "HI",
				                Name = "Hawaii"
			});
			states.Add (new State {
				                StateCodeId = 13,
				                Code = "ID",
				                Name = "Idaho"
			});
			states.Add (new State {
				                StateCodeId = 14,
				                Code = "IL",
				                Name = "Illinois"
			});
			states.Add (new State {
				                StateCodeId = 15,
				                Code = "IN",
				                Name = "Indiana"
			});
			states.Add (new State {
				                StateCodeId = 16,
				                Code = "IA",
				                Name = "Iowa"
			});
			states.Add (new State {
				                StateCodeId = 17,
				                Code = "KS",
				                Name = "Kansas"
			});
			states.Add (new State {
				                StateCodeId = 18,
				                Code = "KY",
				                Name = "Kentucky"
			});
			states.Add (new State {
				                StateCodeId = 19,
				                Code = "LA",
				                Name = "Louisiana"
			});
			states.Add (new State {
				                StateCodeId = 20,
				                Code = "ME",
				                Name = "Maine"
			});
			states.Add (new State {
				                StateCodeId = 21,
				                Code = "MD",
				                Name = "Maryland"
			});
			states.Add (new State {
				                StateCodeId = 22,
				                Code = "MA",
				                Name = "Massachusetts"
			});
			states.Add (new State {
				                StateCodeId = 23,
				                Code = "MI",
				                Name = "Michigan"
			});
			states.Add (new State {
				                StateCodeId = 24,
				                Code = "MN",
				                Name = "Minnesota"
			});
			states.Add (new State {
				                StateCodeId = 25,
				                Code = "MS",
				                Name = "Mississippi"
			});
			states.Add (new State {
				                StateCodeId = 26,
				                Code = "MO",
				                Name = "Missouri"
			});
			states.Add (new State {
				                StateCodeId = 27,
				                Code = "MT",
				                Name = "Montana"
			});
			states.Add (new State {
				                StateCodeId = 28,
				                Code = "NE",
				                Name = "Nebraska"
			});
			states.Add (new State {
				                StateCodeId = 29,
				                Code = "NV",
				                Name = "Nevada"
			});
			states.Add (new State {
				                StateCodeId = 30,
				                Code = "NH",
				                Name = "New Hampshire"
			});
			states.Add (new State {
				                StateCodeId = 31,
				                Code = "NJ",
				                Name = "New Jersey"
			});
			states.Add (new State {
				                StateCodeId = 32,
				                Code = "NM",
				                Name = "New Mexico"
			});
			states.Add (new State {
				                StateCodeId = 33,
				                Code = "NY",
				                Name = "New York"
			});
			states.Add (new State {
				                StateCodeId = 34,
				                Code = "NC",
				                Name = "North Carolina",
			});
			states.Add (new State {
				                StateCodeId = 35,
				                Code = "ND",
				                Name = "North Dakota"
			});
			states.Add (new State {
				                StateCodeId = 36,
				                Code = "OH",
				                Name = "Ohio"
			});
			states.Add (new State {
				                StateCodeId = 37,
				                Code = "OK",
				                Name = "Oklahoma"
			});
			states.Add (new State {
				                StateCodeId = 38,
				                Code = "OR",
				                Name = "Oregon"
			});
			states.Add (new State {
				                StateCodeId = 39,
				                Code = "PA",
				                Name = "Pennsylvania"
			});
			states.Add (new State {
				                StateCodeId = 40,
				                Code = "RI",
				                Name = "Rhode Island"
			});
			states.Add (new State {
				                StateCodeId = 41,
				                Code = "SC",
				                Name = "South Carolina"
			});
			states.Add (new State {
				                StateCodeId = 42,
				                Code = "SD",
				                Name = "South Dakota"
			});
			states.Add (new State {
				                StateCodeId = 43,
				                Code = "TN",
				                Name = "Tennessee"
			});
			states.Add (new State {
				                StateCodeId = 44,
				                Code = "TX",
				                Name = "Texas"
			});
			states.Add (new State {
				                StateCodeId = 45,
				                Code = "UT",
				                Name = "Utah"
			});
			states.Add (new State {
				                StateCodeId = 46,
				                Code = "VT",
				                Name = "Vermont"
			});
			states.Add (new State {
				                StateCodeId = 47,
				                Code = "VA",
				                Name = "Virginia"
			});
			states.Add (new State {
				                StateCodeId = 48,
				                Code = "WA",
				                Name = "Washington"
			});
			states.Add (new State {
				                StateCodeId = 49,
				                Code = "WV",
				                Name = "West Virginia"
			});
			states.Add (new State {
				                StateCodeId = 50,
				                Code = "WI",
				                Name = "Wisconsin"
			});
			states.Add (new State {
				                StateCodeId = 51,
				                Code = "WY",
				                Name = "Wyoming"
			});
			return _states;
		}
	}
}

