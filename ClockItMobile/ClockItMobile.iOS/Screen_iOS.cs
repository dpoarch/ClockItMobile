using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(Screen_iOS))]

namespace ClockIt.Mobile.iOS
{
    public class Screen_iOS : IScreen
    {

        public string Version
        {
            get
            {
                NSObject ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
                return ver.ToString();
            }
        }

    }
}