﻿using System;
using UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;

namespace Busidex.Presentation.iOS
{
	public class PhoneNumberTypeModel : BasePickerViewModel
	{
		public event OnItemSelectedHandler OnItemSelected;

		List<PhoneNumberType> phoneNumberTypes { get; set; }

		PhoneNumberType _selectedPhoneNumberType;
		public PhoneNumberType SelectedPhoneNumberType {
			get {
				return _selectedPhoneNumberType;
			}
			set {
				_selectedPhoneNumberType = value;
				if (_selectedPhoneNumberType != null) {
					lbl.Text = _selectedPhoneNumberType.Name;
				}
			}
		}

		public PhoneNumberTypeModel (UILabel lbl, PhoneNumberType _selectedType)
		{
			this.lbl = new UILabel ();
			phoneNumberTypes = new List<PhoneNumberType> ();
			phoneNumberTypes.AddRange (getPhoneNumberTypes ());

			_selectedPhoneNumberType = _selectedType;
			if (_selectedPhoneNumberType != null) {
				this.lbl.Text = _selectedPhoneNumberType.Name;
			}
		}

		public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			return phoneNumberTypes.Count;
		}

		public override string GetTitle (UIPickerView pickerView, nint row, nint component)
		{
			return phoneNumberTypes [(int)row].Name;
		}

		public override void Selected (UIPickerView pickerView, nint row, nint component)
		{
			_selectedPhoneNumberType = phoneNumberTypes [(int)pickerView.SelectedRowInComponent (new nint (0))];
			lbl.Text = _selectedPhoneNumberType.Name;
			if (OnItemSelected != null) {
				OnItemSelected ();
			}
		}

		public override nfloat GetComponentWidth (UIPickerView pickerView, nint component)
		{
			return 115f;
		}

		public int IndexOf (PhoneNumberType phoneNumberType)
		{
			var idx = -1;
			var selectedIdx = idx;
			phoneNumberTypes.ForEach (s => {
				idx++;
				if (phoneNumberType.Name == s.Name) {
					selectedIdx = idx;
				}
			});
			return selectedIdx;
		}

		static List<PhoneNumberType> getPhoneNumberTypes ()
		{
			return new List<PhoneNumberType> {
				new PhoneNumberType {
					PhoneNumberTypeId = 1,
					Name = "Business"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 2,
					Name = "Home"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 3,
					Name = "Mobile"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 4,
					Name = "Fax"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 5,
					Name = "Toll Free"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 6,
					Name = "eFax"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 7,
					Name = "Other"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 8,
					Name = "Direct"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 9,
					Name = "Voice Mail"
				},
				new PhoneNumberType {
					PhoneNumberTypeId = 10,
					Name = "Business 2"
				}
			};
		}
	}
}

