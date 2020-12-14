
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
using iHelp.Helpers;
using System.ComponentModel;

namespace iHelp.Activities
{
    [Activity(Label = "@string/app_name")]
    public class RequestActivity : Activity
    {
        BackgroundWorker worker;
        BackgroundWorker updater;
        string email;
        Request request;
        string requestId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(Resource.Style.AppTheme_NoActionBar);
            SetContentView(Resource.Layout.RequestInfo);

            email = Xamarin.Essentials.Preferences.Get("email", null);

            var title = FindViewById<TextView>(Resource.Id.requestInfoTitle);
            var author = FindViewById<TextView>(Resource.Id.requestInfoAuthor);
            var image = FindViewById<ImageView>(Resource.Id.requestInfoImage);
            var location = FindViewById<TextView>(Resource.Id.requestInfoLocation);
            var acceptButton = FindViewById<Button>(Resource.Id.requestAcceptButton);
            var date = FindViewById<TextView>(Resource.Id.requestInfoDate);
            var category = FindViewById<TextView>(Resource.Id.requestInfoCategory);
            var description = FindViewById<TextView>(Resource.Id.requestInfoText);


            requestId = Intent.GetStringExtra("requestId");
            
            updater = new BackgroundWorker();
            updater.DoWork += (s, e) =>
            {
                var client = new RestClient();
                e.Result = client.GetAsync($"request/{requestId}").Result;
            };
            updater.RunWorkerCompleted += (s, e) =>
            {
                var result = (ResponseModel)e.Result;
                if (result.Code == System.Net.HttpStatusCode.NotFound)
                {
                    Finish();
                    return;
                }
                if (result.Code != System.Net.HttpStatusCode.OK)
                {
                    Toast.MakeText(this, "Произошла ошибка", ToastLength.Long).Show();
                    return;
                }
                request = JsonConvert.DeserializeObject<Request>(result.Body);

                DateTime now = DateTime.Today;
                int age = now.Year - request.Author.BirthdayDate.Year;
                if (request.Author.BirthdayDate > now.AddYears(-age)) age--;

                title.Text = request.Title;
                author.Text = $"{request.Author.Surname} {request.Author.Name}, {age} лет";
                location.Text = request.Location;
                date.Text = request.CreationDate.ToShortDateString();
                image.SetImageResource(request.Category switch
                {
                    RequestCategory.Medical => Resource.Drawable.medical,
                    RequestCategory.Pet => Resource.Drawable.pet,
                    RequestCategory.Shop => Resource.Drawable.shop,
                    RequestCategory.Transport => Resource.Drawable.transport,
                    _ => Resource.Drawable.medical
                });
                category.Text = Resources.GetStringArray(Resource.Array.categories)[(int)request.Category];
                description.Text = request.Description;

                if (email == request.Author.Email)
                {
                    acceptButton.Text = "Удалить запрос";
                }
                else if (email == request.Performer?.Email)
                {
                    acceptButton.Text = "Отказаться от запроса";
                }
                else
                {
                    acceptButton.Text = "Принять запрос";
                }
            };

            worker = new BackgroundWorker();
            worker.DoWork += BackgroundWork;
            worker.RunWorkerCompleted += (s, e) =>
            {
                var result = (ResponseModel)e.Result;
                if (result.Code != System.Net.HttpStatusCode.OK)
                { 
                    Toast.MakeText(this, "Произошла ошибка", ToastLength.Long).Show();
                }

                updater.RunWorkerAsync();
            };

            acceptButton.Click += (s, e) =>
            {
                worker.RunWorkerAsync();
            };

            updater.RunWorkerAsync();
        }

        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            var client = new RestClient();
            string url = string.Empty;
            if (email == request.Author.Email)
            {
                e.Result = client.DeleteAsync($"request/{request.Id}").Result;
            }
            else if (email == request.Performer?.Email)
            {
                e.Result = client.GetAsync($"request/{request.Id}/refuse").Result;
            }
            else
            {
                e.Result = client.GetAsync($"request/{request.Id}/accept").Result;
            }
        }
    }
}
