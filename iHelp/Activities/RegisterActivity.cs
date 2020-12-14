
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iHelp.Helpers;
using iHelp.Models;
using iHelp.ViewModels;

namespace iHelp.Activities
{
    [Activity(Label = "@string/app_name")]
    public class RegisterActivity : Activity
    {
        Android.Support.V4.Widget.SwipeRefreshLayout registerSwipe;
        BackgroundWorker worker = new BackgroundWorker();
        EditText nameEdit;
        EditText surnameEdit;
        EditText emailEdit;
        EditText passwordEdit;
        EditText repeatPasswordEdit;
        EditText dateEdit;
        Spinner registerCity;
        DateTime birthdayDate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Register);

            registerSwipe = FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout>(Resource.Id.registerSwipe);

            var loginButton = FindViewById<TextView>(Resource.Id.registerLoginButton);
            var registerButton = FindViewById<Button>(Resource.Id.registerButton);

            dateEdit = FindViewById<EditText>(Resource.Id.registerDateEdit);
            nameEdit = FindViewById<EditText>(Resource.Id.registerNameEdit);
            surnameEdit = FindViewById<EditText>(Resource.Id.registerSurnameEdit);
            emailEdit = FindViewById<EditText>(Resource.Id.registerEmailEdit);
            passwordEdit = FindViewById<EditText>(Resource.Id.registerPasswordEdit);
            repeatPasswordEdit = FindViewById<EditText>(Resource.Id.registerRepeatPasswordEdit);

            registerCity = FindViewById<Spinner>(Resource.Id.registerCity);

            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.cities, Resource.Drawable.CustomSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            registerCity.Adapter = adapter;

            worker.RunWorkerCompleted += WorkCompleted;
            worker.DoWork += BackgroundWork;

            loginButton.Click += (s, e) =>
            {
                StartActivity(typeof(LoginActivity));
                Finish();
            };

            registerButton.Click += (s, e) =>
            {
                var account = new RegisterViewModel(nameEdit.Text, surnameEdit.Text, emailEdit.Text, passwordEdit.Text, repeatPasswordEdit.Text, (string)registerCity.SelectedItem, birthdayDate);
                var context = new ValidationContext(account);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(account, context, results, true))
                {
                    string str = string.Join("\n", results);
                    Toast.MakeText(this, str, ToastLength.Long).Show();
                    return;
                }

                registerSwipe.Refreshing = true;
                worker.RunWorkerAsync(account);
            };

            dateEdit.FocusChange += (s, e) =>
            {
                if (((EditText)s).IsFocused)
                {
                    var datetime = DateTime.Now;
                    DatePickerDialog datePicker = new DatePickerDialog(this, OnDateSet, datetime.Year, datetime.Month, datetime.Day);
                    datePicker.Show();
                }
            };

            registerSwipe.Refresh += (s, e) =>
            {
                nameEdit.Text = string.Empty;
                surnameEdit.Text = string.Empty;
                emailEdit.Text = string.Empty;
                passwordEdit.Text = string.Empty;
                repeatPasswordEdit.Text = string.Empty;

                registerSwipe.Refreshing = false;
            };
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            birthdayDate = new DateTime(e.Year, e.Month, e.DayOfMonth);
            dateEdit.Text = birthdayDate.ToShortDateString();
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (ResponseModel)e.Result;

            if (result.Code == System.Net.HttpStatusCode.OK)
            {
                var token = result.Body;

                RestClient.Token = token;
                Xamarin.Essentials.Preferences.Set("token", token);

                StartActivity(typeof(MainActivity));
                Finish();
            }
            else if (result.Code == System.Net.HttpStatusCode.Conflict)
            {
                Toast.MakeText(this, "Аккаунт с такой электронной почтой уже существует", ToastLength.Short).Show();
            }
            else if (result.Code == System.Net.HttpStatusCode.BadRequest)
            {
                Toast.MakeText(this, "Пожалуйста, перепроверьте введеные данные на корректность", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Произошла ошибка. Пожалуйста, свяжитесь с нами", ToastLength.Long).Show();
            }

            registerSwipe.Refreshing = false;
        }

        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            var account = (RegisterViewModel)e.Argument;
            var client = new RestClient();
            var result = client.PostAsync("account/register", account).Result;
            e.Result = result;
        }
    }
}