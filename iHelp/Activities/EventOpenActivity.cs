
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using iHelp.Models;
using FFImageLoading;
using iHelp.Helpers;

namespace iHelp.Activities
{
    [Activity(Label = "EventOpenActivity")]
    public class EventOpenActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(Resource.Style.AppTheme_NoActionBar);
            SetContentView(Resource.Layout.EventOpened);

            var title = FindViewById<TextView>(Resource.Id.eventOpenTitle);
            var date = FindViewById<TextView>(Resource.Id.eventOpenDate);
            var location = FindViewById<TextView>(Resource.Id.eventOpenLocation);
            var description = FindViewById<TextView>(Resource.Id.eventOpenDescription);
            var image = FindViewById<ImageView>(Resource.Id.eventOpenImage);

            var @event = JsonConvert.DeserializeObject<Event>(Intent.GetStringExtra("event"));

            title.Text = @event.Title;
            date.Text = @event.Date.ToShortDateString();
            location.Text = @event.Location;
            description.Text = @event.Description;

            if (@event.Image == null)
            {
                image.Visibility = ViewStates.Gone;
            }
            else
            {
                ImageService.Instance.LoadUrl($"{RestClient.Uri}{@event.Image}").Retry(3, 2000).Into(image);
            }
        }
    }
}
