
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using API.Models;
using iHelp.Helpers;
using iHelp.Models;
using Newtonsoft.Json;

namespace iHelp.Activities
{
    [Activity(Label = "@string/app_name")]
    public class LoginActivity : Activity
    {
        BackgroundWorker worker = new BackgroundWorker();
        ResponseModel result;
        EditText emailEdit;
        EditText passwordEdit;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Login);

            var registerButton = FindViewById<TextView>(Resource.Id.loginRegisterButton);
            var loginButton = FindViewById<Button>(Resource.Id.loginButton);
            emailEdit = FindViewById<EditText>(Resource.Id.loginEmailEdit);
            passwordEdit = FindViewById<EditText>(Resource.Id.loginPasswordEdit);

            worker.DoWork += BackgroundWork;
            worker.RunWorkerCompleted += WorkCompleted;

            registerButton.Click += (s, e) =>
            {
                StartActivity(typeof(RegisterActivity));
                Finish();
            };

            loginButton.Click += (s, e) =>
            {
                worker.RunWorkerAsync();
            };
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (result.Code == System.Net.HttpStatusCode.OK)
            {
                RestClient.Token = result.Body;
                Xamarin.Essentials.Preferences.Set("token", result.Body);
                StartActivity(typeof(MainActivity));
                Finish();
            }
            else if (result.Code == System.Net.HttpStatusCode.NotFound)
            {
                Toast.MakeText(this, "Аккаунт не существует или введены неверные данные. Воспользуйтесь регистрацией", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Произошла ошибка. Пожалуйста, свяжитесь с нами", ToastLength.Short).Show();
            }
        }

        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            var client = new RestClient();
            var loginModel = new LoginModel { Email = emailEdit.Text, Password = passwordEdit.Text };

            result = client.PostAsync("account/login", loginModel).Result;
        }
    }
}
