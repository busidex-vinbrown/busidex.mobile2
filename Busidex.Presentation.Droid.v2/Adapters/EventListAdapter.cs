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
		readonly List<EventTag> Tags;
		readonly List<View> ViewCache;
		readonly Activity context;

		public event RedirectToEventCardsHandler RedirectToEventCards;

		Intent EventCardsIntent{ get; set; }

		public EventListAdapter (Activity ctx, int id, List<EventTag> tags) : base(ctx, id, tags)
		{
			Tags = tags;
			context = ctx;
			ViewCache = new List<View> ();
			for(var i=0;i<tags.Count;i++){
				ViewCache.Add (new View (ctx));
			}
		}

		void OnRedirectToEventCards(object sender, EventArgs e){

			int position = (int)((Button)sender).Tag;
			SetEventTagData (Tags [position]);

			if(RedirectToEventCards != null){
				RedirectToEventCards (Tags [position]);
			}
		}

		void SetEventTagData(object tag){
			//EventCardsIntent = new Intent(context, typeof(EventCardsActivity));

			//var data = Newtonsoft.Json.JsonConvert.SerializeObject(tag);

			//EventCardsIntent.PutExtra ("Card", data);
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.EventListListItem, null);
			ViewCache [position] = view;


			var btnEventTag = view.FindViewById<Button> (Resource.Id.btnEventTag);

			btnEventTag.Tag = position;
			btnEventTag.Click -= OnRedirectToEventCards;
			btnEventTag.Click += OnRedirectToEventCards;
			btnEventTag.Text = Tags [position].Description;

			return view;
		}
	}
}

