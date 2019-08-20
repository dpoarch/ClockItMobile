using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Plugin.SecureStorage;
using UIKit;
using CoreLocation;
using Microsoft.WindowsAzure.MobileServices;
using AVFoundation;
using UserNotifications;
using Xamarin.Forms;
using System.Threading;

namespace ClockIt.Mobile.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
        

        protected CLLocationManager locMgr;
        Task backgroundTask;
        iOSLongRunningTaskExample longRunningTaskExample;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            CurrentPlatform.Init();
            Plugin.SecureStorage.CrossSecureStorage.Current.SetValue("Session Password","dadf2b61441e47d8a91d9056b0a6ed06");
            global::Xamarin.Forms.Forms.Init();
            //var dbPath = FileAccessHelper.GetLocalFilePath("commandScheduling.db3
            App.DeviceWidth = (int)UIScreen.MainScreen.Bounds.Width;
            App.DeviceHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.Xdpi = 0; //temporary constant
            App.IsIOS = true;
            //Xamarin.FormsGoogleMaps.Init("AIzaSyAg4qblST3s8rfCI8-qT9PanptZPcll9Gs");
            LoadApplication(new App());

            WireUpLongRunningTask();

            var songURL = new NSUrl("Sounds/notificationSound.mp3");
            var audio = new AVAudioPlayer(songURL, "mp3", out NSError err);
            audio.Volume = 0.5f;
            //audio.FinishedPlaying += delegate { audio = null; };
            audio.NumberOfLoops = 2;
            App.Player = audio;
            NotificationInit();
            //LocationManagerInit();
            //StartBackgroundLocationUpdating();
            return base.FinishedLaunching(app, options);

		}

        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", async message => {
                longRunningTaskExample = new iOSLongRunningTaskExample();
                await longRunningTaskExample.Start();
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message => {
                longRunningTaskExample.Stop();
            });
        }
        private void NotificationInit()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                        UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                        (approved, error) => { });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
        }

        public void LocationManagerInit() {
            this.locMgr = new CLLocationManager();
            this.locMgr.PausesLocationUpdatesAutomatically = false;
            // iOS 8 has additional permissions requirements
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization(); // works in background
                                                     //locMgr.RequestWhenInUseAuthorization (); // only in foreground
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }
            if (CLLocationManager.LocationServicesEnabled)
            {
                //set the desired accuracy, in meters

                locMgr.DesiredAccuracy = 1;
                locMgr.LocationsUpdated += async (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    //LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                    //await App.Locator.ClockIn.ClockInUpdateAsync();
                };
                
                locMgr.StartUpdatingLocation();
            }
        }
        
        public override void DidEnterBackground(UIApplication application)
        //public void StartBackgroundLocationUpdating()
        {
            /*
            App.IsBackground = true;

            nint taskID = UIApplication.SharedApplication.BeginBackgroundTask(() => { });
            backgroundTask = new Task(() => {
                App.Locator.RunPauseSchedule.OneSecondBackgroundUpdate();
                UIApplication.SharedApplication.EndBackgroundTask(taskID);

            });
            backgroundTask.Start();*/
        }

        public override void WillEnterForeground(UIApplication application)
        {
            /*
            Console.WriteLine("App will enter foreground");
            App.IsBackground = false;
            */
        }
    }
    public class iOSLongRunningTaskExample
    {
        nint _taskId;
        CancellationTokenSource _cts;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();

            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("LongRunningTask", OnExpiration);

            try
            {
                //INVOKE THE SHARED CODE
                var b = App.ShouldRunOnSleep();
                while (App.IsSleep && b)
                {
                    await Task.Delay(1000);
                    App.Locator.RunPauseSchedule.OneSecondBackgroundUpdate();
                }

            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_cts.IsCancellationRequested)
                {
                    var message = new CancelledMessage();
                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "CancelledMessage")
                    );
                }
            }

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        void OnExpiration()
        {
            _cts.Cancel();
        }
    }
    public class CancelledMessage
    {
    }
}
