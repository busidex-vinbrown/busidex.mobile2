//using System;
using Android.Widget;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Java.Lang;
using System.Linq;

namespace Busidex.Presentation.Droid.v2
{
	public class UserCardFilter : Filter
	{
		readonly UserCardAdapter _adapter;

		public UserCardFilter (UserCardAdapter adapter)
		{
			_adapter = adapter;
		}

		protected override FilterResults PerformFiltering(ICharSequence constraint)
		{
			var returnObj = new FilterResults();
			var results = new List<UserCard>();
			if (_adapter.Cards == null || constraint == null || string.IsNullOrWhiteSpace (constraint.ToString ())) {
				_adapter.Cards = _adapter._originalItems;
				results.AddRange (_adapter.Cards);
			} else {

				//if (constraint == null) return returnObj;

				if (_adapter.Cards != null && _adapter.Cards.Any ()) {
					// Compare constraint to all names lowercased. 
					// It they are contained they are added to results.
					try{
					results.AddRange (
						_adapter.Cards.Where (
							card => 
							(!string.IsNullOrEmpty (card.Card.Name) && card.Card.Name.ToLower ().Contains (constraint.ToString ())) ||
							(!string.IsNullOrEmpty (card.Card.CompanyName) && card.Card.CompanyName.ToLower ().Contains (constraint.ToString ())) ||
							(!string.IsNullOrEmpty (card.Card.Email) && card.Card.Email.ToLower ().Contains (constraint.ToString ())) ||
							(!string.IsNullOrEmpty (card.Card.Url) && card.Card.Url.ToLower ().Contains (constraint.ToString ()))
						) 
					);
					}catch(Exception ex){
						// ignore
					}
				}
			}

			// Nasty piece of .NET to Java wrapping, be careful with this!
			returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
			returnObj.Count = results.Count;

			constraint.Dispose();

			return returnObj;
		}

		protected override void PublishResults(ICharSequence constraint, FilterResults results)
		{
			if (results != null) {
				using (var values = results.Values)
					_adapter.Cards = values.ToArray<Object> ()
					.Select (r => r.ToNetObject<UserCard> ()).ToList ();
			}

			_adapter.NotifyDataSetChanged ();

			// Don't do this and see GREF counts rising
			constraint.Dispose ();
			results.Dispose ();
		}
		
	}
}

