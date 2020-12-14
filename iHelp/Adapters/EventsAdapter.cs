using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using iHelp.Activities;
using iHelp.Helpers;
using iHelp.Models;
using Newtonsoft.Json;

namespace iHelp.Adapters
{
    public class EventsAdapter : RecyclerView.Adapter
    {
        private List<Event> events;
        private Context context;

        public EventsAdapter(List<Event> events)
        {
            this.events = events;
        }

        public override int ItemCount => events.Count();

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as EventViewHolder;

            var @event = events[position];
            viewHolder.Title.Text = @event.Title;
            viewHolder.Date.Text = @event.Date.ToShortDateString();

            if (@event.Image != null)
            {
                ImageService.Instance.LoadUrl($"{RestClient.Uri}{@event.Image}").Retry(3, 2000).Into(viewHolder.Image);
            }
            else
            {
                viewHolder.Image.Visibility = ViewStates.Gone;
            }

            viewHolder.Layout.Click += delegate {
                var intent = new Intent(context, typeof(EventOpenActivity));
                intent.PutExtra("event", JsonConvert.SerializeObject(@event));
                context.StartActivity(intent);
	        };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            context = parent.Context;

            View view = LayoutInflater.From(context).Inflate(Resource.Layout.events_item, parent, false);
            var viewHolder = new EventViewHolder(view);

            return viewHolder;
        }

        public class EventViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Image { get; set; }
            public TextView Title { get; set; }
            public TextView Date { get; set; }
            public LinearLayout Layout { get; set; }

            public EventViewHolder(View view) : base(view)
            {
                Image = view.FindViewById<ImageView>(Resource.Id.eventImage);
                Title = view.FindViewById<TextView>(Resource.Id.eventTitle);
                Date = view.FindViewById<TextView>(Resource.Id.eventDate);
                Layout = view.FindViewById<LinearLayout>(Resource.Id.eventsLayout);
            }
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            url = RestClient.Uri + url; 

            var client = new RestClient();

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}
