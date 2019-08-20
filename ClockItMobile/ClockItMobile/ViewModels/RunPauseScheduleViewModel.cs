using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ClockIt.Mobile.Behaviors;
using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using Plugin.Messaging;
using Xamarin.Forms;

namespace ClockIt.Mobile.ViewModels
{
    public class RunPauseScheduleViewModel : ViewModelBaseWithServices
    {

        readonly INavigationService _navigationService;
        HttpClient _client;
        string _totalHours;
        CISchedule _schedule;
        string _timerDisplay;
        string _periodName;
        string _periodIndex;
        Task _init;
        bool _isPaused;
        bool _isStopped;
        int _currentPeriod;
        TimeSpan _currentTime;
        Image _image;
        RelayCommand _backToSchedulesCommand;
        RelayCommand _jumpCommand;
        RelayCommand _showHideListCommand;
        public StackLayout stackLayout;
        public ListView listView;

        public CancellationTokenSource ts = new CancellationTokenSource();

        public double ListViewHeight
        {
            get
            {
                if (_schedule.Periods == null) return 60;
                int max = (int)(App.DeviceHeight * 0.4 / 30);
                if (_schedule.Periods.Count <= 2)
                {
                    return 60;
                }
                else if (_schedule.Periods.Count >= max)
                {
                    return max * 30;
                }
                else
                {
                    return _schedule.Periods.Count * 30;
                }
            }
        }
        RelayCommand _accountCommand;
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
        public RelayCommand JumpCommand
        {
            get
            {
                return _jumpCommand ?? (_jumpCommand = new RelayCommand(() =>
                {
                    JumpPeriod();
                }, () => true));
            }
        }

        public async void JumpPeriod()
        {
            var i = await InputBox();
            if (i != null)
            {
                try
                {
                    var t = int.Parse(i);
                    if (t <= _schedule.Periods.Count && t >= 1) { }
                    else throw new System.Exception();
                    JumpPeriod(t);
                }catch (System.Exception e){
                    await App.Current.MainPage.DisplayAlert("Invalid Index input","Please input a valid period index.","OK");
                }
            }
        }
        public async void JumpPeriod(int n)
        {
            IsPaused = true;

            ts.Cancel();
            ts = new CancellationTokenSource();
            _currentPeriod = n-1;
            _currentTime = _schedule.Periods.ElementAt(n - 1).Interval;
            //var ts = new TimeSpan(0, 0, 0);
            PeriodName = _schedule.Periods.ElementAt(n-1).Name;
            PeriodIndex = "Period:   Time Remaining:";
            TimerDisplay = _schedule.Periods.ElementAt(n - 1).Index + "         " + _currentTime.ToString("c");
            RunTimer();
        }

        public RelayCommand ShowHideListCommand
        {
            get
            {
                return _showHideListCommand ?? (_showHideListCommand = new RelayCommand(() =>
                {
                    ShowHideList();
                }, () => true));
            }
        }

        public void ShowHideList()
        {
            stackLayout.IsVisible = !stackLayout.IsVisible;
            listView.IsVisible = !listView.IsVisible;
        }
        public Image Image
        {
            get { return _image; }
            set
            {
                if (Set(() => Image, ref _image, value))
                {
                    RaisePropertyChanged();
                }
            }
        }
        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                if (Set(() => IsPaused, ref _isPaused, value))
                {
                    RaisePropertyChanged();
                }
            }
        }
        public bool IsStopped
        {
            get { return _isStopped; }
            set
            {
                if (Set(() => IsStopped, ref _isStopped, value))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public string PeriodIndex
        {
            get { return _periodIndex; }
            set
            {
                if (Set(() => PeriodIndex, ref _periodIndex, value))
                {
                    RaisePropertyChanged();
                }
            }
        }
        public string PeriodName
        {
            get { return _periodName; }
            set
            {
                if (Set(() => PeriodName, ref _periodName, value))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public string TimerDisplay
        {
            get { return _timerDisplay; }
            set
            {
                if (Set(() => TimerDisplay, ref _timerDisplay, value))
                {
                    RaisePropertyChanged();
                }
            }
        }



        public CISchedule Schedule {
            get { return _schedule; }
            set
            {
                if (Set(() => Schedule, ref _schedule, value))
                {
                    _currentTime = _schedule.Periods.ElementAt(0).Interval;
                    RaisePropertyChanged(() => ListViewHeight);
                    _init = Init();
                    RaisePropertyChanged();
                }
            }
        }
        public string Hours
        {
            get { return System.Math.Round(_schedule.TotalHours,2) + " Hours"; }
        }
        public string Periods
        {
            get { return _schedule.Periods.Count + " Periods"; }
        }

        public string Title {

            get { return _schedule.Name+" Running"; }
        }

        public double LabelWidth
        {
            get { return (App.DeviceWidth) * .35; }

        }
        public double EmailButtonWidth
        {
            get { return ((App.DeviceWidth) * .1) - 10; }

        }
        public double ViewByLabelWidth
        {
            get { return (App.DeviceWidth) * .35; }

        }
        public double ViewPickerWidth
        {
            get { return (App.DeviceWidth) * .20; }

        }
        public List<CIPeriod> PeriodsDisplay
        {
            get
            {
                //return _schedule.Periods.GetRange(_currentPeriod, _schedule.Periods.Count - (_currentPeriod));
                return _schedule.Periods;
            }

        }

        public string TotalHours
        {
            get { return _totalHours; }
            set
            {
                if (Set(() => TotalHours, ref _totalHours, value))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public RelayCommand BackToSchedulesCommand
        {
            get
            {
                return _backToSchedulesCommand ?? (_backToSchedulesCommand = new RelayCommand(() =>
                {
                    _navigationService.NavigateTo(App.Locator.SchedulesPage);
                }, () => true));
            }
        }


        public RunPauseScheduleViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException("navigationService");
            _client = ClientHelper.GetClient();
            App.MasterMenu.ContentPage.Icon = new FileImageSource()
            {
                File = "hamburger@2x.png"
            };

        }
        public async Task Init()
        {
            IsPaused = false;
            IsStopped = false;
            _currentPeriod = 0;
            _currentTime = _schedule.Periods.ElementAt(0).Interval;
            
            PeriodName = _schedule.Periods.ElementAt(0).Name;
            PeriodIndex = "Period:   Time Remaining:";
            TimerDisplay = "1" + "         " + _currentTime.ToString("c");
            
            //Task i = Heartbeat();
            RunTimer();

        }
        public void StartTimer()
        {
            IsPaused = false;
            IsStopped = false;
            /*
            var ts = new TimeSpan(0, 0, 0);
            PeriodName = _schedule.Periods.ElementAt(_currentPeriod).Name;
            PeriodIndex = "Period:   Time Remaining:";
            TimerDisplay = (_currentPeriod) + "         " + _currentTime.ToString("c");
            */
            RunTimer();
        }
        public void PauseTimer()
        {
            IsPaused = true;
            ts.Cancel();
            ts = new CancellationTokenSource();
            var source = (FileImageSource)_image.Source;
            source.File = "play.png";
        }

        public void OneSecondBackgroundUpdate()
        {
            if (_currentPeriod + 1 >= _schedule.Periods.Count)
            {
                IsStopped = true;
            }
            else
            {
                _currentTime = _currentTime.Subtract(new TimeSpan(0, 0, 1));

                PeriodName = _schedule.Periods.ElementAt(_currentPeriod).Name;
                PeriodIndex = "Period:   Time Remaining:";
                TimerDisplay = _schedule.Periods.ElementAt(_currentPeriod).Index + "         " + _currentTime.ToString("c");
                if (_currentTime.TotalSeconds <= 0)
                {
                    if (_currentPeriod + 1 >= _schedule.Periods.Count())
                    {
                        CrossLocalNotifications.Current.Cancel(101);
                        CrossLocalNotifications.Current.Show(_schedule.Periods.ElementAt(_currentPeriod).Name + " Period ended", _schedule.Periods.ElementAt(_currentPeriod).Interval.ToString("c") + " elapsed.");
                        _currentPeriod = 0;
                        _currentTime = _schedule.Periods.ElementAt(_currentPeriod).Interval;
                        PeriodName = _schedule.Periods.ElementAt(_currentPeriod).Name;
                        PeriodIndex = "Period:   Time Remaining:";
                        TimerDisplay = _schedule.Periods.ElementAt(_currentPeriod).Index + "         " + _currentTime.ToString("c");
                        var fileService1 = DependencyService.Get<ISaveAndLoad>();
                        fileService1.NotifySound("", "");
                        ts = new CancellationTokenSource();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                        // Put your Code here for updating Any UI Related code  from Background Task
                        ChangeImage();
                        });
                        IsStopped = true;

                    }
                    else
                    {
                        CrossLocalNotifications.Current.Cancel(101);
                        CrossLocalNotifications.Current.Show(_schedule.Periods.ElementAt(_currentPeriod).Name + " Period ended", _schedule.Periods.ElementAt(_currentPeriod).Interval.ToString("c") + " elapsed.");
                        _currentPeriod = _currentPeriod + 1;
                        _currentTime = _schedule.Periods.ElementAt(_currentPeriod).Interval;
                        PeriodName = _schedule.Periods.ElementAt(_currentPeriod).Name;
                        PeriodIndex = "Period:   Time Remaining:";
                        TimerDisplay = _schedule.Periods.ElementAt(_currentPeriod).Index + "         " + _currentTime.ToString("c");

                    }
                    var fileService = DependencyService.Get<ISaveAndLoad>();
                    fileService.NotifySound("", "");
                }
            }
        }

        public void StopTimer()
        {
            IsStopped = true;
            ts.Cancel();
            ts = new CancellationTokenSource();
            var source = (FileImageSource)_image.Source;
            source.File = "play.png";
            _currentPeriod = 0;
            _currentTime = _schedule.Periods.ElementAt(0).Interval;
            PeriodName = _schedule.Periods.ElementAt(0).Name;
            PeriodIndex = "Period:   Time Remaining:";
            TimerDisplay = "1" + "         " + _currentTime.ToString("c");
            /*
            try
            {
                _currentPeriod = 0;
                _currentTime = _schedule.Periods.ElementAt(0).Interval;
                var ts = new TimeSpan(0, 0, 0);
                PeriodName = _schedule.Periods.ElementAt(0).Name;
                PeriodIndex = "Period:   Time Remaining:";
                TimerDisplay = "1" + "         " + _currentTime.ToString("c");
                var source = (FileImageSource)_image.Source;
                source.File = "play.png";
            }
            catch (Exception e) { }
            */
        }

        public void RunTimer()
        {
            CancellationToken ct = ts.Token;
            var i = 0;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    // do some heavy work here
                    await Task.Delay(100);
                    ++i;
                    if (i % 10 == 0) {
                        i = 0;
                        _currentTime = _currentTime.Subtract(new TimeSpan(0, 0, 1));

                        PeriodName = _schedule.Periods.ElementAt(_currentPeriod).Name;
                        PeriodIndex = "Period:   Time Remaining:";
                        TimerDisplay = _schedule.Periods.ElementAt(_currentPeriod).Index + "         " + _currentTime.ToString("c");
                        if (_currentTime.TotalSeconds <= 0) {
                            if (_currentPeriod+1 >= _schedule.Periods.Count())
                            {
                                CrossLocalNotifications.Current.Cancel(101);
                                CrossLocalNotifications.Current.Show(_schedule.Periods.ElementAt(_currentPeriod).Name + " Period ended", _schedule.Periods.ElementAt(_currentPeriod).Interval.ToString("c") + " elapsed.");
                                _currentPeriod = 0;
                                _currentTime = _schedule.Periods.ElementAt(_currentPeriod).Interval;
                                PeriodName = _schedule.Periods.ElementAt(_currentPeriod).Name;
                                PeriodIndex = "Period:   Time Remaining:";
                                TimerDisplay = _schedule.Periods.ElementAt(_currentPeriod).Index + "         " + _currentTime.ToString("c");
                                var fileService1 = DependencyService.Get<ISaveAndLoad>();
                                fileService1.NotifySound("", "");
                                ts = new CancellationTokenSource();
                                Device.BeginInvokeOnMainThread(() => {
                                    // Put your Code here for updating Any UI Related code  from Background Task
                                    ChangeImage();
                                });
                                IsStopped = true;
                                break;
                            }
                            else
                            {
                                CrossLocalNotifications.Current.Cancel(101);
                                CrossLocalNotifications.Current.Show(_schedule.Periods.ElementAt(_currentPeriod).Name + " Period ended", _schedule.Periods.ElementAt(_currentPeriod).Interval.ToString("c") + " elapsed.");
                                _currentPeriod = _currentPeriod+1;
                                _currentTime = _schedule.Periods.ElementAt(_currentPeriod).Interval;
                                PeriodName = _schedule.Periods.ElementAt(_currentPeriod).Name;
                                PeriodIndex = "Period:   Time Remaining:";
                                TimerDisplay = _schedule.Periods.ElementAt(_currentPeriod).Index + "         " + _currentTime.ToString("c");

                            }
                            var fileService = DependencyService.Get<ISaveAndLoad>();
                            fileService.NotifySound("", "");
                        }
                    }
                    if (ct.IsCancellationRequested)
                    {
                        // another thread decided to cancel
                        //Console.WriteLine("task canceled");
                        break;
                    }
                }
            }, ct);
            
            
        }
        public void ChangeImage() {
            
            var source = (FileImageSource)_image.Source;
            source.File = "play.png";
                                
        }
        private async Task Heartbeat()
        {

            for (int x= _currentPeriod;x<_schedule.Periods.Count;++x)
            {
                var s = _schedule.Periods.ElementAt(x);
                var ts = _currentTime;
                PeriodName = s.Name;
                PeriodIndex = "Period:   Time Remaining:";
                TimerDisplay = "" + s.Index + "         " + ts.ToString("c");
                //RaisePropertyChanged(()=>PeriodsDisplay);
                while (true)
                {
                    /*
                    if (IsStopped)
                    {
                        break;
                    }
                    if (ts.Ticks <= 0) {
                        if (x + 1 < _schedule.Periods.Count)
                        {
                            _currentPeriod = x + 1;
                            _currentTime = _schedule.Periods.ElementAt(x + 1).Interval;
                            CrossLocalNotifications.Current.Cancel(101);
                            CrossLocalNotifications.Current.Show(s.Name+" Period ended", s.Interval.ToString("c")+" elapsed.");
                            if (s.Index == _schedule.Periods.Count - 1)
                            {
                                var source = (FileImageSource)_image.Source;
                                source.File = "play.png";
                            }
                        }
                        else
                        {
                            _currentPeriod = 0;
                            _currentTime = _schedule.Periods.ElementAt(0).Interval;
                            CrossLocalNotifications.Current.Cancel(101);
                            CrossLocalNotifications.Current.Show(s.Name + " Period ended", s.Interval.ToString("c") + " elapsed.");
                            var source = (FileImageSource)_image.Source;
                            source.File = "play.png";
                        }
                        var fileService = DependencyService.Get<ISaveAndLoad>();
                        fileService.NotifySound("", "");
                        break;
                    }
                    if (IsPaused)
                    {
                        _currentPeriod = x;
                        _currentTime = ts;
                        break;
                    }*/
                    if (IsStopped)
                    {
                        try
                        {
                            _currentPeriod = 0;
                            _currentTime = _schedule.Periods.ElementAt(0).Interval;
                            PeriodName = _schedule.Periods.ElementAt(0).Name;
                            PeriodIndex = "Period:   Time Remaining:";
                            TimerDisplay = "1" + "         " + _currentTime.ToString("c");
                            var source = (FileImageSource)_image.Source;
                            source.File = "play.png";
                        }
                        catch (System.Exception e) { }

                        break;
                    }
                    else if (ts.Ticks <= 0)
                    {
                        if (x + 1 < _schedule.Periods.Count)
                        {
                            //_currentPeriod = x + 1;
                            _currentTime = _schedule.Periods.ElementAt(x + 1).Interval;
                            PeriodName = _schedule.Periods.ElementAt(x).Name;
                            PeriodIndex = "Period:   Time Remaining:";
                            TimerDisplay = x + "         " + (new TimeSpan(0, 0, 0)).ToString("c");
                            CrossLocalNotifications.Current.Cancel(101);
                            CrossLocalNotifications.Current.Show(s.Name + " Period ended", s.Interval.ToString("c") + " elapsed.");
                            /*
                            if (s.Index == _schedule.Periods.Count)
                            {
                                var source = (FileImageSource)_image.Source;
                                source.File = "play.png";
                            }
                            */
                        }
                        else
                        {
                            _currentPeriod = 0;
                            _currentTime = _schedule.Periods.ElementAt(0).Interval;
                            PeriodName = _schedule.Periods.ElementAt(x).Name;
                            PeriodIndex = "Period:   Time Remaining:";
                            TimerDisplay = "1         " + _currentTime.ToString("c");
                            CrossLocalNotifications.Current.Cancel(101);
                            CrossLocalNotifications.Current.Show(s.Name + " Period ended", s.Interval.ToString("c") + " elapsed.");
                            var source = (FileImageSource)_image.Source;
                            source.File = "play.png";
                        }
                        var fileService = DependencyService.Get<ISaveAndLoad>();
                        fileService.NotifySound("", "");
                        break;
                    }
                    else if (IsPaused)
                    {
                        _currentPeriod = x;
                        _currentTime = ts;
                        PeriodName = s.Name;
                        PeriodIndex = "Period:   Time Remaining:";
                        TimerDisplay = "" + s.Index + "         " + ts.ToString("c");
                        break;
                    }
                    else
                    {

                        PeriodName = s.Name;
                        PeriodIndex = "Period:   Time Remaining:";
                        TimerDisplay = "" + s.Index + "         " + ts.ToString("c");
                    }

                    await Task.Delay(1000);
                    //if (IsPaused || IsStopped) { break; } else 

                    ts = ts.Subtract(new TimeSpan(0, 0, 1));
                    if (IsStopped)
                    {
                        try
                        {
                            _currentPeriod = 0;
                            _currentTime = _schedule.Periods.ElementAt(0).Interval;
                            PeriodName = _schedule.Periods.ElementAt(0).Name;
                            PeriodIndex = "Period:   Time Remaining:";
                            TimerDisplay = "1" + "         " + _currentTime.ToString("c");
                            var source = (FileImageSource)_image.Source;
                            source.File = "play.png";
                        }
                        catch (System.Exception e) { }

                        break;
                    }
                    else if (ts.Ticks <= 0)
                    {
                        if (x + 1 < _schedule.Periods.Count)
                        {
                            //_currentPeriod = x + 1;
                            _currentTime = _schedule.Periods.ElementAt(x + 1).Interval;
                            PeriodName = _schedule.Periods.ElementAt(x).Name;
                            PeriodIndex = "Period:   Time Remaining:";
                            TimerDisplay = x + "         " + (new TimeSpan(0,0,0)).ToString("c");
                            CrossLocalNotifications.Current.Cancel(101);
                            CrossLocalNotifications.Current.Show(s.Name + " Period ended", s.Interval.ToString("c") + " elapsed.");
                            /*
                            if (s.Index == _schedule.Periods.Count)
                            {
                                var source = (FileImageSource)_image.Source;
                                source.File = "play.png";
                            }
                            */
                        }
                        else
                        {
                            _currentPeriod = 0;
                            _currentTime = _schedule.Periods.ElementAt(0).Interval;
                            PeriodName = _schedule.Periods.ElementAt(x).Name;
                            PeriodIndex = "Period:   Time Remaining:";
                            TimerDisplay = "1         " + _currentTime.ToString("c");
                            CrossLocalNotifications.Current.Cancel(101);
                            CrossLocalNotifications.Current.Show(s.Name + " Period ended", s.Interval.ToString("c") + " elapsed.");
                            var source = (FileImageSource)_image.Source;
                            source.File = "play.png";
                        }
                        var fileService = DependencyService.Get<ISaveAndLoad>();
                        fileService.NotifySound("", "");
                        break;
                    }
                    else if (IsPaused)
                    {
                        _currentPeriod = x;
                        _currentTime = ts;
                        PeriodName = s.Name;
                        PeriodIndex = "Period:   Time Remaining:";
                        TimerDisplay = "" + s.Index + "         " + ts.ToString("c");
                        break;
                    }
                    else {

                        PeriodName = s.Name;
                        PeriodIndex = "Period:   Time Remaining:";
                        TimerDisplay = "" + s.Index + "         " + ts.ToString("c");
                    }
                }
            }
        }
        public Task InitializeAsync()
        {
            return null;
        }
        


    
    }
}
