using System;

using UIKit;
using GoogleAnalytics.iOS;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Presentation.iOS
{
	public partial class AddressInfoController : BaseCardEditController
	{
		public AddressInfoController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			const string EMPTY_STATE = "(Select State)";

			var address = SelectedCard.Addresses != null && SelectedCard.Addresses.Count >= 0 ? SelectedCard.Addresses [0] : new Address ();

			txtAddress1.Text = address.Address1;
			txtAddress2.Text = address.Address2;
			txtCity.Text = address.City;
			lblSelectedState.Text = address.State != null ? address.State.Name : EMPTY_STATE;

			pckState.Hidden = true;

			var frame = lblInstructions.Frame;
			lblInstructions.Frame = new CoreGraphics.CGRect (frame.X, 95f, frame.Width, frame.Height);

			btnPicker.TouchUpInside += delegate {
				pckState.Hidden = !pckState.Hidden;
			};

		}



		public override void ViewDidAppear (bool animated)
		{
			GAI.SharedInstance.DefaultTracker.Set (GAIConstants.ScreenName, "Address Info");

			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var model = new StateCodeModel (lblSelectedState);
			model.OnItemSelected += delegate {
				pckState.Hidden = true;
			};

			pckState.Model = model;
		}

		public delegate void OnItemSelectedHandler ();

		public class StateCodeModel : UIPickerViewModel
		{
			public event OnItemSelectedHandler OnItemSelected;

			List<State> states { get; set; }

			UILabel lbl;

			public StateCodeModel (UILabel lbl)
			{
				this.lbl = lbl;
				states = new List<State> ();
				states.AddRange (getStates ());
			}

			public override nint GetComponentCount (UIPickerView v)
			{
				return 1;
			}

			public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
			{
				return states.Count;
			}

			public override string GetTitle (UIPickerView picker, nint row, nint component)
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

			public override void Selected (UIPickerView picker, nint row, nint component)
			{
				lbl.Text = states [(int)picker.SelectedRowInComponent (new nint (0))].Name;
				if (OnItemSelected != null) {
					OnItemSelected ();
				}
			}

			public override nfloat GetComponentWidth (UIPickerView picker, nint component)
			{
				return 220f;
//				if (component == 0)
//					return 220f;
//				else
//					return 30f;
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
}
