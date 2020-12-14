using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using iHelp.Activities;
using iHelp.Models;
using Newtonsoft.Json;

namespace iHelp.Adapters
{
    public class RequestsAdapter : RecyclerView.Adapter
    {
        private List<Request> requests;
        private Context context;

        public RequestsAdapter(List<Request> requests)
        {
            this.requests = requests;
        }

        public override int ItemCount => requests.Count();

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as RequestViewHolder;

            var request = requests[position];

            DateTime now = DateTime.Today;
            int age = now.Year - request.Author.BirthdayDate.Year;
            if (request.Author.BirthdayDate > now.AddYears(-age)) age--;

            viewHolder.Title.Text = request.Title;
            viewHolder.Author.Text = $"{request.Author.Surname} {request.Author.Name}, {age} лет";
            viewHolder.Location.Text = $"{request.Location}";

            viewHolder.Image.SetImageResource(request.Category switch
            {
                RequestCategory.Medical => Resource.Drawable.medical,
                RequestCategory.Pet => Resource.Drawable.pet,
                RequestCategory.Shop => Resource.Drawable.shop,
                RequestCategory.Transport => Resource.Drawable.transport,
                _ => Resource.Drawable.medical
            });
            viewHolder.Layout.Click += delegate {
                var intent = new Intent(context, typeof(RequestActivity));
                intent.PutExtra("requestId", request.Id.ToString());
                context.StartActivity(intent);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            context = parent.Context;

            View view = LayoutInflater.From(context).Inflate(Resource.Layout.RequestsItem, parent, false);
            var viewHolder = new RequestViewHolder(view);

            return viewHolder;
        }

        public class RequestViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; private set; }
            public TextView Author { get; private set; }
            public TextView Location { get; private set; }
            public ImageView Image { get; private set; }
            public LinearLayout Layout { get; private set; }

            public RequestViewHolder(View view) : base(view)
            {
                Title = view.FindViewById<TextView>(Resource.Id.requestTitle);
                Author = view.FindViewById<TextView>(Resource.Id.requestAuthor);
                Location = view.FindViewById<TextView>(Resource.Id.requestPosition);
                Image = view.FindViewById<ImageView>(Resource.Id.requestImage);
                Layout = view.FindViewById<LinearLayout>(Resource.Id.requestsItemLayout);
            }
        }
    }
}
