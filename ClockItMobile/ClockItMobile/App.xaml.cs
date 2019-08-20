using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Models;
using ClockIt.Mobile.ViewModels;
using ClockIt.Mobile.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.SecureStorage;
using Xamarin.Forms;

namespace ClockIt.Mobile
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }
        private static ViewModelLocator _locator;
        public static ViewModelLocator Locator { get { return _locator ?? (_locator = new ViewModelLocator()); } }

        public static ObservableCollection<CISchedule> CISchedules { get; set; }
        public static ObservableCollection<ClockItUser> ClockItUsers { get; set; }
        public static ClockItUser ClockItUser { get; set; }
        public static CISchedule RunningSchedule { get; set; }
        public static MasterMainPage MasterMenu { get; set; }
        public static string Tabname { get; set; }
        public static double DeviceWidth { get; set; }
        public static double DeviceHeight { get; set; }
        public static double Xdpi { get; set; }
        public static bool IsIOS { get; set; }
        public static bool IsSleep { get; set; }
        public static bool IsNotInit { get; set; }
        public static bool IsBackground { get; set; }
        public static bool IsOnlineMode { get; set; }
        public static object Player { get; set; }
        public static NavigationService Nav { get; set; }

        private static Stopwatch StopWatch = new Stopwatch();
        private const int defaultTimespan = 1;
        

        public static IMobileServiceTable<TodoItem> todoTable { get; set; }
        public static IEnumerable<TodoItem> items { get; set; }
        public  void SaveDB()
        {
            
            CrossSecureStorage.Current.SetValue("schedules", JsonConvert.SerializeObject(App.CISchedules));
            
        }


        public void SetMainPage(NavigationPage page) {
            MainPage = page;
        }

        public Page GetMainPage() {
            return MainPage;
        }

        public App()
        {
            InitializeComponent();
            if (App.Xdpi != 0&&!IsNotInit) //if Android
            {
                App.DeviceWidth = (App.DeviceWidth) / App.Xdpi * 160;
            }
            IsNotInit = true;
            
            Nav = new NavigationService();

            if (!SimpleIoc.Default.IsRegistered<INavigationService>()&&Nav!=null)
            {
                Nav.Configure(Locator.MainPage, typeof(MainPage));
                Nav.Configure(Locator.SchedulesPage, typeof(SchedulesPage));
                Nav.Configure(Locator.AddEditSchedulePage, typeof(AddEditSchedulePage));
                Nav.Configure(Locator.RunPauseSchedulePage, typeof(RunPauseSchedulePage));
                Nav.Configure(Locator.AccountPage, typeof(AccountPage));
                SimpleIoc.Default.Register<INavigationService>(() => Nav);
            }
            else
            {
                Nav = (NavigationService)SimpleIoc.Default.GetInstance<INavigationService>();
            }


            var mainPage = new NavigationPage();
            //var mainPage = new NavigationPage(new MainPage());
            if ((!CrossSecureStorage.Current.HasKey("username")&& !CrossSecureStorage.Current.HasKey("password"))||true)
            {
                mainPage = new NavigationPage(new MainPage());
            }
            else
            {
                mainPage = new NavigationPage(new SchedulesPage());
            }

            Nav.Initialize(mainPage);
            MasterMenu = new MasterMainPage(Nav)
            {
                Detail = mainPage,
                IsGestureEnabled = false
            };
            //MainPage = MasterMenu;
            MainPage = mainPage;

            //NavigationPage.SetTitleIcon(mainPage, "clockitlogo.png");
            /*
            if (CrossConnectivity.Current.IsConnected||true)
            {
                MainPage = MasterMenu;
            }
            else
            {
                MainPage = new NoNetworkPage();
            }*/
            if (RunningSchedule != null & IsNotInit)
            {
                Nav.NavigateTo(Locator.RunPauseSchedulePage);
                App.Locator.Schedules.SetMenuItems();
                App.MasterMenu.IsGestureEnabled = true;
            }
        }

        protected override void OnStart()
        {
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }



        void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Type currentPageType = MainPage.GetType();
        }
        public static bool ShouldRunOnSleep() {
            return IsSleep && RunningSchedule != null && !App.Locator.RunPauseSchedule.IsPaused && !Locator.RunPauseSchedule.IsStopped;
        }
        protected override void OnSleep()
        {
            
            IsSleep = true;
            var message = new StartLongRunningTaskMessage();
            MessagingCenter.Send(message, "StartLongRunningTaskMessage");
            //Task onsleep = SleepTimerStart();
        }
        public async Task SleepTimerStart() {
            while (true) {
                await Task.Delay(1000);
                App.Locator.RunPauseSchedule.OneSecondBackgroundUpdate();
                if (!IsSleep) {
                    break;
                }
            }
        }


        protected override void OnResume()
        {
            // Handle when your app resumes
            IsSleep = false;
            var message = new StopLongRunningTaskMessage();
            MessagingCenter.Send(message, "StopLongRunningTaskMessage");
        }

    }
    public class StopLongRunningTaskMessage
    {
    }
    public class StartLongRunningTaskMessage
    {
    }
}
