using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.SecureStorage;
using Xamarin.Forms;

namespace ClockIt.Mobile.ViewModels
{
    public class SchedulesViewModel : ViewModelBaseWithServices
    {
        readonly INavigationService _navigationService;
        ICommand _clockInCommand;
        ICommand _runScheduleCommand;
        RelayCommand _clockOutActivitiesCommand;
        RelayCommand _timeTrackerSummaryCommand;
        HttpClient _client;
        public bool _isClockedIn;
        ObservableCollection<CISchedule> _cISchedules;

        RelayCommand _accountCommand;

        public void Refresh() {
            CISchedules = App.CISchedules;
        }

        public double ListViewHeight
        {
            get
            {
                if (App.CISchedules == null) return 150;
                int max = (int)(App.DeviceHeight*0.65/75);
                if (App.CISchedules.Count <= 2)
                {
                    return 150;
                }
                else if (App.CISchedules.Count >= max) {
                    return max * 75;
                }
                else{
                    return App.CISchedules.Count * 75;
                }
            }
        }
        public bool HasSchedules {
            get
            {
                if (App.CISchedules == null) return false;
                return App.CISchedules.Count > 0;
            }
        }
        public RelayCommand AccountCommand
        {
            get
            {
                return _accountCommand ?? (_accountCommand = new RelayCommand(() =>
                {
                    _navigationService.NavigateTo(App.Locator.AccountPage);
                }, () => true));
            }
        }
        public ObservableCollection<CISchedule> CISchedules
        {
            get { return _cISchedules; }
            set
            {
                if (Set(() => CISchedules, ref _cISchedules, value))
                {
                    RaisePropertyChanged();
                }
            }
        }


        public SchedulesViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException("navigationService");
            _client = ClientHelper.GetClient();
            if (App.ClockItUsers == null)
            {
                try
                {
                    Task test = LoadDB();
                }

                catch (HttpRequestException e)
                {
                    DependencyService.Get<Toast>().Show("App crash" + e.Message);
                }
                catch (Exception e)
                {
                    DependencyService.Get<Toast>().Show("App crash" + e.Message);
                }
            }
            else
            {
                CISchedules = App.CISchedules;
            }
        }

        async Task InitializeAsync()
        {

        }

        public async Task LoadDB()
        {
            await ClientHelper.SetAuthAndHeaders(ClientHelper.GET_ALL, "");
            var response = await _client.GetAsync("");

            if (response.IsSuccessStatusCode)
            {

                string jsonMessage;
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    jsonMessage = new StreamReader(responseStream).ReadToEnd();
                }
                try
                {
                    var docs = JsonConvert.DeserializeObject<AzureResponse>(jsonMessage);
                    App.ClockItUsers = docs.Documents;

                    CredentialsAreCorrect();
                    RaisePropertyChanged(()=> ListViewHeight);
                    SetMenuItems();
                    App.MasterMenu.IsGestureEnabled = true;
                }
                catch (Exception e)
                {
                }

            }
        }



        private async void Heartbeat()
        {
            while (true)
            {
                await Task.Delay(1000);
            }
        }


        public ICommand RunScheduleCommand
        {
            get
            {
                return _runScheduleCommand ?? (_runScheduleCommand = new Command(RedirectToRunSchedule));
            }
        }

        public ICommand EditScheduleCommand
        {
            get
            {
                return _clockInCommand ?? (_clockInCommand = new Command(RedirectToEditSchedule));
            }
        }

        private void RedirectToEditSchedule(object obj)
        {
            _navigationService.NavigateTo(App.Locator.AddEditSchedulePage, (CISchedule)obj);
        }
        private void RedirectToRunSchedule(object obj)
        {
            App.RunningSchedule = (CISchedule)obj;
            _navigationService.NavigateTo(App.Locator.RunPauseSchedulePage);
        }

        public RelayCommand ClockOutActivitiesCommand
        {
            get
            {
                return _clockOutActivitiesCommand ?? (_clockOutActivitiesCommand = new RelayCommand(() =>
                {
                    if (CrossConnectivity.Current.IsConnected&&App.CISchedules!=null)
                    {
                        var id = 0;
                        if (App.CISchedules.Count() > 0) id = App.CISchedules.Last().Id + 1;
                        _navigationService.NavigateTo(App.Locator.AddEditSchedulePage, new CISchedule()
                        {
                            Periods = new List<CIPeriod>(),
                            DateTime = DateTime.Now,
                            Name = "",
                            Time = DateTime.Now.TimeOfDay.ToString(),
                            Id = id
                        });
                    }
                    else {

                        DependencyService.Get<Toast>().Show("Schedules might not be loaded properly. Reloading. Please make sure internet connection is on.");
                        Task test = LoadDB();
                    }
                }, () => true));
            }
        }
        public RelayCommand TimeTrackerSummaryCommand
        {
            get
            {
                return _timeTrackerSummaryCommand ?? (_timeTrackerSummaryCommand = new RelayCommand(() =>
                {
                }));
            }
        }
        bool CredentialsAreCorrect()
        {
            var user = CrossSecureStorage.Current.GetValue("username")??"";
            var pass = CrossSecureStorage.Current.GetValue("password")??"";
            foreach (var i in App.ClockItUsers)
            {
                if (i.Email == user  && i.Phone ==pass)
                {
                    

                    if (false)
                    {
                        
                    }
                    else
                    {
                        if(i.Schedules!=null)
                        App.CISchedules = new ObservableCollection<CISchedule>(i.Schedules);
                    }
                    App.ClockItUser = i;
                    

                    CISchedules = App.CISchedules;
                    DependencyService.Get<Toast>().Show("Schedules loaded successfully.");
                    return true;
                }
            }

            DependencyService.Get<Toast>().Show("Schedules load failed. Please relogin.");
            _navigationService.NavigateTo(App.Locator.MainPage);
            return false;
        }

        public void SetMenuItems()
        {
            var masterPage = App.MasterMenu;

            var androidMenuItems = new List<MasterPageItem>
            {
                new MasterPageItem() { Title = "Account", TargetType = "AccountPage" },
                new MasterPageItem() { Title = "Schedules", TargetType = "SchedulesPage" },
                //new MasterPageItem() { Title = "Settings", TargetType = "SettingsPage" },
                new MasterPageItem() { Title = "Log Out", TargetType = "MainPage" },
            };
            masterPage.ListView.ItemsSource = androidMenuItems;

            masterPage.ContentPage.Icon = new FileImageSource()
            {
                File = "hamburger@2x.png"
            };
        }
    }
}
