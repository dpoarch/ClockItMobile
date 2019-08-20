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
using ClockIt.Mobile.Droid;
using ClockIt.Mobile.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(ScreenAndroid))]
namespace ClockIt.Mobile.Droid
{
    class ScreenAndroid : Java.Lang.Object, IScreen
    {

        public string Version
        {
            get
            {
                var context = Forms.Context;
                return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            }
        }

    }
}