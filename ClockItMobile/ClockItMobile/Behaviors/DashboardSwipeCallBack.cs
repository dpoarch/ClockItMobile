using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Behaviors
{
    public class DashboardSwipeCallBack : ViewModelBaseWithServices, ISwipeCallBack
    {
        public void onBottomSwipe(View view, object p)
        {
            //Application.Current.MainPage.DisplayAlert("Swipe", "bottom", "Ok");
        }

        public void onLeftSwipe(View view, object p)
        {
        }

        public void onNothingSwiped(View view, object p)
        {
            //Application.Current.MainPage.DisplayAlert("Swipe", "none", "Ok");
        }

        public void onRightSwipe(View view, object p)
        {
        }

        public void onTopSwipe(View view, object p)
        {
            //Application.Current.MainPage.DisplayAlert("Swipe", "top", "Ok");
        }

        
    }
}
