using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;

namespace ClockIt.Mobile.Droid
{
	//You can specify additional application information in this attribute
    [Application]
    [MetaData("com.google.android.maps.v2.API_KEY",
              Value = "AIzaSyA8tNRvOWfktsIeCms0YqFm_02qOmzkHvQ")]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        }

		public override void OnCreate()
		{
			base.OnCreate();
			RegisterActivityLifecycleCallbacks(this);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //A great place to initialize Xamarin.Insights and Dependency Services!
            App.DeviceWidth = Resources.DisplayMetrics.WidthPixels; // real pixels
            App.DeviceHeight = Resources.DisplayMetrics.HeightPixels; // real pixels
            App.Xdpi = Resources.DisplayMetrics.Xdpi; // real pixels
        }

		public override void OnTerminate()
		{
			base.OnTerminate();
			UnregisterActivityLifecycleCallbacks(this);
		}

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
            App.Locator.RunPauseSchedule.StopTimer();
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
			CrossCurrentActivity.Current.Activity = activity;
		}

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
			CrossCurrentActivity.Current.Activity = activity;
		}

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}