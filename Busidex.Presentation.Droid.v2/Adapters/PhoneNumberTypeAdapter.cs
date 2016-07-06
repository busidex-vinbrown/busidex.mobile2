using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.Droid.v2
{
	public class PhoneNumberTypeAdapter : ArrayAdapter<PhoneNumberType>
	{
		public List<PhoneNumberType> PhoneNumberTypes { get; set; }
		private readonly Activity _context;
		private readonly IList<View> _views = new List<View> ();

		public PhoneNumberTypeAdapter (Activity ctx, int id, List<PhoneNumberType> phoneNumberTypes) : base (ctx, id, phoneNumberTypes)
		{
			PhoneNumberTypes = new List<PhoneNumberType> ();
			PhoneNumberTypes.AddRange (phoneNumberTypes);
			_context = ctx;
		}

		public PhoneNumberType GetItemAtPosition (int position)
		{
			return PhoneNumberTypes != null && position < PhoneNumberTypes.Count ? PhoneNumberTypes [position] : null;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count {
			get {
				return PhoneNumberTypes == null ? 0 : PhoneNumberTypes.Count;
			}
		}

		public override View GetDropDownView (int position, View convertView, ViewGroup parent)
		{
			return GetCustomView (position, convertView, parent, true);
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			return GetCustomView (position, convertView, parent, false);
		}

		private View GetCustomView (int position, View convertView, ViewGroup parent, bool dropdown)
		{
			var item = PhoneNumberTypes [position];

			var inflater = LayoutInflater.From (_context);
			var view = convertView ?? inflater.Inflate (Resource.Layout.PhoneTypeSpinnerItem, parent, false);

			var text = view.FindViewById<TextView> (Resource.Id.txtSpinnerText);

			if (text != null)
				text.Text = item.Name;

			if (!_views.Contains (view))
				_views.Add (view);

			return view;
		}

		private void ClearViews ()
		{
			foreach (var view in _views) {
				view.Dispose ();
			}
			_views.Clear ();
		}

		protected override void Dispose (bool disposing)
		{
			ClearViews ();
			base.Dispose (disposing);
		}
	}
}

