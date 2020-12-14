
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using iHelp.Adapters;
using iHelp.Helpers;
using iHelp.Models;
using Newtonsoft.Json;

namespace iHelp.Fragments
{
    public class EventsFragment : Fragment
    {
        TabLayout tabs;
        RecyclerView recycler;
        SwipeRefreshLayout swipeRefresh;
        BackgroundWorker worker;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Events, container, false);

            tabs = view.FindViewById<TabLayout>(Resource.Id.eventsTabs);
            recycler = view.FindViewById<RecyclerView>(Resource.Id.eventsRecycler);
            swipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.eventsSwipe);

            worker = new BackgroundWorker();
            worker.DoWork += BackgroundWork;
            worker.RunWorkerCompleted += WorkCompleted;

            recycler.SetLayoutManager(new LinearLayoutManager(Context));

            tabs.TabSelected += (s, e) =>
            {
                swipeRefresh.Refreshing = true;
                worker.RunWorkerAsync();
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
            worker.RunWorkerAsync();
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (ResponseModel)e.Result;
            if (result.Code != System.Net.HttpStatusCode.OK)
            {
                Toast.MakeText(Context, "Произошла ошибка, попробуйте позже", ToastLength.Long).Show();
                return;
            }

            var events = JsonConvert.DeserializeObject<List<Event>>(result.Body);
            recycler.SetAdapter(new EventsAdapter(events));

            swipeRefresh.Refreshing = false;
        }

        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            var client = new RestClient();
            e.Result = client.GetAsync($"event/{tabs.SelectedTabPosition}").Result;
        }
    }
}
