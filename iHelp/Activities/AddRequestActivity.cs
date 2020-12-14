
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iHelp.Helpers;
using iHelp.Models;

namespace iHelp.Activities
{
    [Activity(Label = "@string/app_name")]
    public class AddRequestActivity : Activity
    {
        Spinner category;
        EditText address;
        EditText description;
        EditText title;
        Button addRequest;
        Button cancelButton;

        BackgroundWorker worker;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddRequest);

            category = FindViewById<Spinner>(Resource.Id.addRequestCategory);
            address = FindViewById<EditText>(Resource.Id.addRequestAddress);
            description = FindViewById<EditText>(Resource.Id.addRequestDesc);
            title = FindViewById<EditText>(Resource.Id.addRequestTitle);
            addRequest = FindViewById<Button>(Resource.Id.addRequestButton);
            cancelButton = FindViewById<Button>(Resource.Id.addRequestCancel);

            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.RequestCategories, Resource.Layout.spinner_firstitem);
            adapter.SetDropDownViewResource(Resource.Layout.spinner_item);
            category.Adapter = adapter;


            worker = new BackgroundWorker();
            worker.DoWork += BackgroundWork;
            worker.RunWorkerCompleted += WorkCompleted;

            addRequest.Click += (s, e) =>
            {
                var request = new Request(title.Text, description.Text, (RequestCategory)category.SelectedItemPosition + 1, address.Text);
                var context = new ValidationContext(request);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(request, context, results, true))
                {
                    string str = string.Join("\n", results);
                    Toast.MakeText(this, str, ToastLength.Long).Show();
                    return;
                }

                worker.RunWorkerAsync(request);
            };

            cancelButton.Click += (s, e) =>
            {
                Finish();
            };
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (ResponseModel)e.Result;
            if (result.Code != System.Net.HttpStatusCode.OK)
            {
                Toast.MakeText(this, "Произошла ошибка. Попробуйте позже", ToastLength.Long).Show();
                return;
            }

            Toast.MakeText(this, "Запрос успешно создан", ToastLength.Short).Show();
            Finish();
        }

        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            var client = new RestClient();
            var request = (Request)e.Argument;
            var result = client.PostAsync("request", request).Result;

            e.Result = result;
        }
    }
}
