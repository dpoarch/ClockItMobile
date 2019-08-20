using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Behaviors
{
    public class SwipeListener : PanGestureRecognizer
    {
        private ISwipeCallBack mISwipeCallback;
        private double translatedX = 0, translatedY = 0;
        private object param;

        public SwipeListener(View view, ISwipeCallBack iSwipeCallBack, object p)
        {
            mISwipeCallback = iSwipeCallBack;
            param = p;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            view.GestureRecognizers.Add(panGesture);
        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {

            View Content = (View)sender;

            switch (e.StatusType)
            {

                case GestureStatus.Running:

                    try
                    {
                        /*
                        double x = 0;
                        if (translatedX > e.TotalX) x =   translatedX - e.TotalX;
                        else if (translatedX < e.TotalX) x = - translatedX + e.TotalX;
                        */
                        translatedX = e.TotalX;
                        translatedY = e.TotalY;
                        //Content.TranslateTo(x, Content.AnchorY, 1, null);
                        //Content.TranslationX = x;

                    }
                    catch (Exception err)
                    {
                        System.Diagnostics.Debug.WriteLine("" + err.Message);
                    }
                    break;

                case GestureStatus.Completed:

                    System.Diagnostics.Debug.WriteLine("translatedX : " + translatedX);
                    System.Diagnostics.Debug.WriteLine("translatedY : " + translatedY);

                    if (translatedX < 0 && Math.Abs(translatedX) > Math.Abs(translatedY))
                    {
                        mISwipeCallback.onLeftSwipe(Content,param);
                    }
                    else if (translatedX > 0 && translatedX > Math.Abs(translatedY))
                    {
                        mISwipeCallback.onRightSwipe(Content, param);
                    }
                    else if (translatedY < 0 && Math.Abs(translatedY) > Math.Abs(translatedX))
                    {
                        mISwipeCallback.onTopSwipe(Content, param);
                    }
                    else if (translatedY > 0 && translatedY > Math.Abs(translatedX))
                    {
                        mISwipeCallback.onBottomSwipe(Content, param);
                    }
                    else
                    {
                        mISwipeCallback.onNothingSwiped(Content, param);
                    }

                    break;

            }
        }

    }
}
