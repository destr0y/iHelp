using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using iHelp.Adapters;
using iHelp.Fragments;

namespace iHelp.Activities
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); 
            SetContentView(Resource.Layout.Main);

            var navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            var viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager);

            viewPager.PageSelected += (s, e) =>
            {
                var item = navigation.Menu.GetItem(e.Position);
                navigation.SelectedItemId = item.ItemId;
            };

            navigation.NavigationItemSelected += (s, e) =>
            {
                viewPager.SetCurrentItem(e.Item.Order, true);
            };
        }
    }
}
