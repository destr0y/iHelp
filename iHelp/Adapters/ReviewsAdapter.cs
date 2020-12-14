using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using iHelp.Models;

namespace iHelp.Adapters
{
    public class ReviewsAdapter : RecyclerView.Adapter
    {
        private List<Review> reviews = new List<Review>();

        public ReviewsAdapter(List<Review> reviews)
        {
            this.reviews = reviews;
        }

        public override int ItemCount => reviews.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ReviewsItem, parent, false);
            ReviewViewHolder viewHolder = new ReviewViewHolder(view);

            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ReviewViewHolder;

            var review = reviews[position];
            viewHolder.Author.Text = $"{review.Author.Name} {review.Author.Surname}";
            viewHolder.Text.Text = review.Text;
            viewHolder.Mark.Text = review.Mark.ToString();
        }

        public class ReviewViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Image { get; private set; }
            public TextView Author { get; private set; }
            public TextView Text { get; private set; }
            public TextView Mark { get; private set; }

            public ReviewViewHolder(View view) : base(view)
            {
                Author = view.FindViewById<TextView>(Resource.Id.reviewAuthor);
                Text = view.FindViewById<TextView>(Resource.Id.reviewText);
                Mark = view.FindViewById<TextView>(Resource.Id.reviewMark);
            }
        }
    }
}
