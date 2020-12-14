
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using iHelp.Activities;
using iHelp.Adapters;
using iHelp.Helpers;
using iHelp.Models;
using Newtonsoft.Json;

namespace iHelp.Fragments
{
    public class RequestsFragment : Android.Support.V4.App.Fragment
    {
        BackgroundWorker bgworker = new BackgroundWorker();
        RecyclerView recycler;
        SwipeRefreshLayout swipeRefresh;
        TabLayout tabLayout;
        Spinner spinner;
        Button buttonAdd;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Requests, container, false);

            buttonAdd = view.FindViewById<Button>(Resource.Id.requestsAddButton);
            buttonAdd.Click += delegate
            {
                var intent = new Intent(view.Context, typeof(AddRequestActivity));
                StartActivityForResult(intent, 1);
            };
            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            tabLayout = View.FindViewById<TabLayout>(Resource.Id.requestsTabs);
            spinner = View.FindViewById<Spinner>(Resource.Id.categorySpinner);

            var adapter = ArrayAdapter.CreateFromResource(View.Context, Resource.Array.categories, Resource.Layout.spinner_firstitem);
            adapter.SetDropDownViewResource(Resource.Layout.spinner_item);

            spinner.Adapter = adapter;

            recycler = View.FindViewById<RecyclerView>(Resource.Id.requestsRecycler);
            recycler.SetLayoutManager(new LinearLayoutManager(Context));

            swipeRefresh = View.FindViewById<SwipeRefreshLayout>(Resource.Id.requestsSwipe);
            swipeRefresh.Refresh += (s, e) =>
            {
                worker().RunWorkerAsync();
            };

            tabLayout.TabSelected += (s, e) =>
            {
                swipeRefresh.Refreshing = true;
                worker().RunWorkerAsync();
            };

            spinner.ItemSelected += (s, e) =>
            {
                swipeRefresh.Refreshing = true;
                worker().RunWorkerAsync();
            };
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as ResponseModel;
            if (result.Code != System.Net.HttpStatusCode.OK)
            {
                Toast.MakeText(Context, "Произошла ошибка, попробуйте позже", ToastLength.Long).Show();
                return;
            }

            var requests = JsonConvert.DeserializeObject<List<Request>>(result.Body);
            recycler.SetAdapter(new RequestsAdapter(requests));

            swipeRefresh.Refreshing = false;
        }

        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            var performed = Convert.ToBoolean(tabLayout.SelectedTabPosition);
            var category = spinner.SelectedItemPosition;

            string url = performed ? $"account/requests/performed?category={category}" : $"request?category={category}";

            var client = new RestClient();
            var result = client.GetAsync(url).Result;

            e.Result = result;
        }

        private BackgroundWorker worker()
        {
            swipeRefresh.Refreshing = false;
            bgworker = new BackgroundWorker();
            bgworker.DoWork += BackgroundWork;
            bgworker.RunWorkerCompleted += WorkCompleted;
            return bgworker;
        }
    }
}
