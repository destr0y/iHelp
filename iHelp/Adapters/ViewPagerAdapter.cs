using System;
using Android.Support.V4.App;
using iHelp.Fragments;

namespace iHelp.Adapters
{
    public class ViewPagerAdapter : FragmentPagerAdapter
    {
        Fragment[] _fragments;

        public ViewPagerAdapter(FragmentManager fm) : base(fm)
        {
            _fragments = new Fragment[] {
                new EventsFragment(),
                new RequestsFragment(),
                new ProfileFragment()
            };
        }

        public override int Count => _fragments.Length;

        public override Fragment GetItem(int position) => _fragments[position];
    }
}
