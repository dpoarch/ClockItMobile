using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.iOS;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Toast_IOS))]
namespace ClockIt.Mobile.iOS
{
    public class Toast_IOS : Toast
    {
        const double LONG_DELAY = 1;


        NSTimer alertDelay;
        UIAlertController alert;

        public void Show(string message)
        {
            ShowAlert(message, LONG_DELAY);
        }


        void ShowAlert(string message, double seconds)
        {
            alertDelay = NSTimer.CreateScheduledTimer(seconds, (obj) =>
            {
                DismissMessage();
            });
            alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }
        void DismissMessage()
        {
            if (alert != null)
            {
                alert.DismissViewController(true, null);
            }
            if (alertDelay != null)
            {
                alertDelay.Dispose();
            }
        }

    }
}