using System;
using Android.Widget;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Content;

namespace Busidex.Presentation.Droid.v2
{
	public delegate void RedirectToEventCardsHandler(EventTag tag);

	public class EventListAdapter : ArrayAdapter<EventTag>
	{
		List<EventTag> Tags;
		readonly Activity context;

		public event RedirectToEventCardsHandler RedirectToEventCards;

		public EventListAdapter (Activity ctx, int id, List<EventTag> tags) : base(ctx, id, tags)
		{
			Tags = tags;
			context = ctx;
		}

		void OnRedirectToEventCards(object sender, EventArgs e){

			int position = (int)((Button)sender).Tag;

			if(RedirectToEventCards != null){
				RedirectToEventCards (Tags [position]);
			}
		}

		public void  UpdateData(List<EventTag> tags){
			Tags = tags;
			NotifyDataSetChanged ();
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.EventListListItem, null);

			var btnEventTag = view.FindViewById<Button> (Resource.Id.btnEventTag);

			btnEventTag.Tag = position;
			btnEventTag.Click -= OnRedirectToEventCards;
			btnEventTag.Click += OnRedirectToEventCards;
			btnEventTag.Text = Tags [position].Description;

			return view;
		}
	}
}

