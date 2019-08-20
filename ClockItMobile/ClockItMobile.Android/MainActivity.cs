using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.SecureStorage;
using Plugin.Permissions;
using System;
using System.Globalization;
using Android.Util;
using Microsoft.WindowsAzure.MobileServices;
using Android.Media;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Droid
{
	[Activity(Label = "ClockIt.Mobile", Icon = "@drawable/clockitlogo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode =LaunchMode.SingleInstance)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
        /*
        public static MobileServiceClient MobileService =
            new MobileServiceClient(
            "https://clockitdatabase.azurewebsites.net"
        );*/
        public static MediaPlayer player;
        protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            App.Player = MediaPlayer.Create(this, Resource.Raw.notificationSound);
            var mediaPlayer = (MediaPlayer)App.Player;
            var count = 0;
            var maxCount = 2;
            mediaPlayer.Completion += delegate {

                if (count < maxCount)
                {
                    count++;
                    mediaPlayer.SeekTo(0);
                    mediaPlayer.Start();
                }
                else
                {
                    count = 0;
                }
            }; 
            //CurrentPlatform.Init();
            SecureStorageImplementation.StoragePassword = "dadf2b61441e47d8a91d9056b0a6ed06";
            global::Xamarin.Forms.Forms.Init(this, bundle);
            //Xamarin.FormsGoogleMaps.Init(this, bundle);
            //Xamarin.FormsGoogleMapsBindings.Init();
            //var dbPath = FileAccessHelper.GetLocalFilePath("commandScheduling.db3");

            LoadApplication(new App());
            WireUpLongRunningTask();
        }

        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", message => {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                StartService(intent);
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message => {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                StopService(intent);
            });
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

        public override void OnBackPressed()
        {
            if (App.Nav.GoBackBool()) { }
            else base.OnBackPressed();
        }
    }
    [Service]
    public class LongRunningTaskService : Service
    {
        CancellationTokenSource _cts;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();

            Task.Run(() => {
                try
                {
                    //INVOKE THE SHARED CODE
                    var b = App.ShouldRunOnSleep();
                    while (App.IsSleep&&b) {
                        Task.Delay(1000);
                        App.Locator.RunPauseSchedule.OneSecondBackgroundUpdate();
                    }
                }
                catch (System.OperationCanceledException)
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

            }, _cts.Token);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }
            base.OnDestroy();
        }
    }

    public class CancelledMessage
    {
    }
    public class StopLongRunningTaskMessage
    {
    }
    public class StartLongRunningTaskMessage
    {
    }
}

