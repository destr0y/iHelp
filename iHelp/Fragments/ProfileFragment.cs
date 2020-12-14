
using System;
using System.Text;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using API.Models;
using Android.Content;
using Android.Provider;
using iHelp.Helpers;
using iHelp.Activities;
using System.ComponentModel;
using iHelp.Models;
using System.Linq;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using iHelp.Adapters;
using Android.Support.V4.App;

namespace iHelp.Fragments
{
    public class ProfileFragment : Fragment
    {
        TextView performedRequests;
        TextView createdRequests;
        Android.Support.V4.Widget.SwipeRefreshLayout swipeRefresh;
        BackgroundWorker worker = new BackgroundWorker();
        RecyclerView recycler;
        Button logoutButton;
        TextView cityLabel;
        TextView nameLabel;
        TextView ageLabel;
        TextView ratingLabel;

        public override async void OnCreate(Bundle savedInstanceState)
        {   
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Profile, container, false);

            logoutButton = view.FindViewById<Button>(Resource.Id.logoutButton);

            swipeRefresh = view.FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout>(Resource.Id.profileSwipe);
            recycler = view.FindViewById<RecyclerView>(Resource.Id.reviewsRecycler);

            performedRequests = view.FindViewById<TextView>(Resource.Id.profilePerformedRequests);
            createdRequests = view.FindViewById<TextView>(Resource.Id.profileCreatedRequests);
            ratingLabel = view.FindViewById<TextView>(Resource.Id.profileRating);

            nameLabel = view.FindViewById<TextView>(Resource.Id.nameLabel);
            ageLabel = view.FindViewById<TextView>(Resource.Id.ageLabel);
            cityLabel = view.FindViewById<TextView>(Resource.Id.cityLabel);

            recycler.SetLayoutManager(new LinearLayoutManager(Context));

            worker.RunWorkerAsync();
            worker.DoWork += BackgroundWork;
            worker.RunWorkerCompleted += WorkCompleted;

            logoutButton.Click += (s, e) =>
            {
                Xamarin.Essentials.Preferences.Clear();
                Activity.StartActivity(typeof(LoginActivity));
                Activity.Finish();
            };

            swipeRefresh.Refresh += (s, e) =>
            {
                worker.RunWorkerAsync();
            };

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            

            var result = e.Result as ResponseModel[];
            if ((result == null) || (result.Any(x => x.Code != HttpStatusCode.OK)))
            {
                Toast.MakeText(Context, "Возникла непредвиденная ошибка", ToastLength.Long).Show();
                swipeRefresh.Refreshing = false;
                return;
            }

            var account = JsonConvert.DeserializeObject<Account>(result[0].Body);

            Xamarin.Essentials.Preferences.Set("email", account.Email);

            DateTime now = DateTime.Today;
            int age = now.Year - account.BirthdayDate.Year;
            if (account.BirthdayDate > now.AddYears(-age)) age--;

            nameLabel.Text = $"{account?.Name} {account?.Surname}";
            ageLabel.Text = $"{age} лет";
            cityLabel.Text = $"{account?.City}";

            performedRequests.Text = $"{account?.PerformedCount}";
            createdRequests.Text = $"{account?.RequestsCount}";
            ratingLabel.Text = $"{account?.Reviews?.Average(x => x.Mark) ?? 0}";

            var reviews = JsonConvert.DeserializeObject<List<Review>>(result[1].Body);
            recycler.SetAdapter(new ReviewsAdapter(reviews));

            swipeRefresh.Refreshing = false;
        }

        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            swipeRefresh.Refreshing = true;

            var client = new RestClient();
            var accountRequest = client.GetAsync("account").Result;
            var reviewsRequest = client.GetAsync("account/reviews").Result;

            e.Result = new[] { accountRequest, reviewsRequest};
        }
    }
}
