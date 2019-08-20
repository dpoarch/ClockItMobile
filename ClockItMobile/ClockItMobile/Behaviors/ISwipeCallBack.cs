using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Behaviors
{
    public interface ISwipeCallBack
    {

        void onLeftSwipe(View view, object p);
        void onRightSwipe(View view, object p);
        void onTopSwipe(View view, object p);
        void onBottomSwipe(View view, object p);
        void onNothingSwiped(View view, object p);
    }
}
