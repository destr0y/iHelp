
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
using iHelp.Helpers;

namespace iHelp.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class StartActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);

            var token = Xamarin.Essentials.Preferences.Get("token", string.Empty);
            RestClient.Token = token;

            var client = new RestClient();
            var response = await client.GetAsync("account/check");

            if (response.Code == System.Net.HttpStatusCode.OK)
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
            else
            {
                StartActivity(typeof(LoginActivity));
                Finish();
            }
        }
    }
}
