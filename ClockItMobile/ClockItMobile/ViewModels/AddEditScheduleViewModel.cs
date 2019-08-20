using System;
using System.Collections.Generic;
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
using Plugin.SecureStorage;
using Xamarin.Forms;

namespace ClockIt.Mobile.ViewModels
{
    public class AddEditScheduleViewModel : ViewModelBaseWithServices
    {
        readonly INavigationService _navigationService;
        HttpClient _client;
        RelayCommand _saveScheduleCommand;
        RelayCommand _runScheduleCommand;
        RelayCommand _deleteScheduleCommand;
        RelayCommand _backToSchedulesCommand;
        RelayCommand _addPeriodCommand;
        RelayCommand _reorderPeriodsCommand;
        RelayCommand _jumpCommand;
        RelayCommand _showHideListCommand;
        CISchedule _schedule;
        CISchedule _selectedTemplate;
        StackLayout _periodsView;
        List<List<Entry>> _addedPeriods;
        int rowCount;

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
        public string Title
        {
            get
            {
                var ans = "Add Schedule";
                if (_schedule.Name != "") ans = "Edit Schedule";
                return ans;
            }
        }
        public bool IsEdit
        {
            get
            {
                var ans = false;
                if (_schedule.Name != "") ans = true;
                return ans;
            }
        }
        public int HeightAddEdit
        {
            get
            {
                var ans = 50;
                if (_schedule.Name != "") ans = 0;
                return ans;
            }
        }
        public List<CISchedule> CISchedules
        {
            get
            {
                return App.CISchedules.ToList();
            }
        }
        public double BlankWidth
        {
            get
            {
                return App.DeviceWidth * .1;
            }
        }
        public double IndexWidth
        {
            get
            {
                return App.DeviceWidth * .17;
            }
        }
        public double NameWidth
        {
            get
            {
                return App.DeviceWidth * .45;
            }
        }
        public double MinutesWidth
        {
            get
            {
                return App.DeviceWidth * .28;
            }
        }
        public double IconSpacing
        {
            get
            {
                return (App.DeviceWidth -250)/3;
            }
        }


        public RelayCommand DeleteScheduleCommand
        {
            get
            {
                return _deleteScheduleCommand ?? (_deleteScheduleCommand = new RelayCommand(() =>
                {
                    DeleteSchedule();
                }, () => true));
            }
        }
        public RelayCommand ReorderPeriodsCommand
        {
            get
            {
                return _reorderPeriodsCommand ?? (_reorderPeriodsCommand = new RelayCommand(() =>
                {
                    ReloadPeriodsView(false);
                }, () => true));
            }
        }
        public RelayCommand AddPeriodCommand
        {
            get
            {
                return _addPeriodCommand ?? (_addPeriodCommand = new RelayCommand(() =>
                {
                    AddPeriod();
                }, () => true));
            }
        }
        public RelayCommand SaveScheduleCommand
        {
            get
            {
                return _saveScheduleCommand ?? (_saveScheduleCommand = new RelayCommand(() =>
                {
                    if(SaveSchedule(_schedule))
                    _navigationService.NavigateTo(App.Locator.SchedulesPage);
                }, () => true));
            }
        }
        public RelayCommand RunScheduleCommand
        {
            get
            {
                return _runScheduleCommand ?? (_runScheduleCommand = new RelayCommand(() =>
                {
                    if (SaveSchedule(_schedule)) { RunSchedule(); }
                    
                }, () => true));
            }
        }
        public void RunSchedule() {

            App.RunningSchedule = _schedule;
            _navigationService.NavigateTo(App.Locator.RunPauseSchedulePage);
        }
        public RelayCommand BackToSchedulesCommand
        {
            get
            {
                return _backToSchedulesCommand ?? (_backToSchedulesCommand = new RelayCommand(() =>
                {
                    if(_addedPeriods!=null) _addedPeriods.Clear();
                    _navigationService.NavigateTo(App.Locator.SchedulesPage);
                }, () => true));
            }
        }

        public List<List<Entry>> AddedPeriods {

            get { return _addedPeriods; }
            set
            {
                if (Set(() => AddedPeriods, ref _addedPeriods, value))
                {
                    RaisePropertyChanged();
                }
            }
        }


        public StackLayout PeriodsView
        {
            get { return _periodsView; }
            set
            {
                if (Set(() => PeriodsView, ref _periodsView, value))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public CISchedule Schedule
        {
            get { return _schedule; }
            set
            {
                _schedule = value;

                    if (_addedPeriods != null ) _addedPeriods.Clear();
                    ReloadPeriodsView(true);

                    RaisePropertyChanged();
                
            }
        }
        public CISchedule SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (Set(() => SelectedTemplate, ref _selectedTemplate, value))
                {
                    Schedule = _selectedTemplate;
                    RaisePropertyChanged();
                }
            }
        }
        public void SetNoSelected() {
            if (!IsEdit)
            {
                _selectedTemplate = null;
                RaisePropertyChanged(() => SelectedTemplate);
            }
        }
        public AddEditScheduleViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException("navigationService");
            _client = ClientHelper.GetClient();

        }
        public void DeletePeriod(CIPeriod i,bool added)
        {
            if (true)
            {
                _addedPeriods.Remove(_addedPeriods.First(_ => _.ElementAt(0).Text == i.Index + ""));
                ReloadPeriodsView(false);
            }
        }
        public async void DeleteSchedule()
        {
            var response = await Application.Current.MainPage.DisplayAlert("Delete Schedule","Confirm "+_schedule.Name+" schedule?","Yes","No");
            if (response)
            {
                App.ClockItUser.Schedules.Remove(_schedule);
                var b = await SaveUserToAzureDB(_client);
                if (b)
                _navigationService.NavigateTo(App.Locator.SchedulesPage);
            }
        }
        public void ReloadPeriodsView(bool firstLoad)
        {
            _periodsView.Children.Clear();
            var deviceWidth = (App.DeviceWidth);
            rowCount = 0;
            Grid mainGrid = new Grid()
            {
                Padding = 0,
                RowSpacing = 0,
            };
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(BlankWidth-10) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(IndexWidth-15) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(NameWidth-45) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(MinutesWidth-10) });
            if (firstLoad)
            {
                
            var ps = _schedule.Periods.OrderBy(_ => _.Index);
            foreach (CIPeriod i in ps)
            {
                AddPeriod(i, mainGrid,true);

                }
                
            }
            else {

                var psx = _addedPeriods.OrderBy(x => int.Parse(x.ElementAt(0).Text));
                var ps = new List<List<Entry>>();
                foreach (List<Entry> t in psx)
                {
                    ps.Add(t);

                }
                _addedPeriods.Clear();
                foreach (List<Entry> t in ps)
                {
                    AddPeriod(t, mainGrid,false);

                }
            }
            _periodsView.Children.Add(mainGrid);
        }

        public void ReorderPeriods() {
            ReloadPeriodsView(false);
        }
        private void AddPeriod(List<Entry> i, Grid mainGrid, bool addToList)
        {
            int minutes;
            int seconds;
            try
            {
                var minutesDouble = double.Parse(i.ElementAt(2).Text);
                minutes = (int)(minutesDouble);
                seconds = (int)((minutesDouble % 1) * 60);
            }
            catch
            {
                minutes = int.Parse(i.ElementAt(2).Text);
                seconds = 0;

            }
            AddPeriod(new CIPeriod()
            {
                Index = int.Parse(i.ElementAt(0).Text),
                Name = (i.ElementAt(1).Text),
                Interval = new TimeSpan(0, minutes, seconds),

            }, mainGrid, addToList);
        }
        
        private void AddPeriod(CIPeriod i, Grid mainGrid, bool addToList)
        {
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
            Image img = new Image()
            {
                Source = "close_circle.png",
                HeightRequest = BlankWidth-10,
                WidthRequest = BlankWidth-10,
            };
            var t = new TapGestureRecognizer();
            t.Tapped += (s, e) =>
            {
                DeletePeriod(i, addToList);
            };
            img.GestureRecognizers.Add(t);
            mainGrid.Children.Add(img, 0, rowCount);
            Entry indexEntry = new Entry()
            {
                Text = i.Index + "",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                Keyboard = Keyboard.Numeric
            };
            mainGrid.Children.Add(indexEntry, 1, rowCount);
            Entry nameEntry = new Entry()
            {
                Text = i.Name + "",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };
            mainGrid.Children.Add(nameEntry, 2, rowCount);
            Entry intervalEntry = new Entry()
            {
                Text = Math.Round(i.Interval.TotalMinutes, 2) + "",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                Keyboard = Keyboard.Numeric
            };
            indexEntry.TextChanged += (s, e) =>
            {
                var entry = (Entry)s;
                List<Entry> periodToCascade = null;
                try
                {
                    periodToCascade = _addedPeriods.First(_ => _.ElementAt(0).Text == entry.Text && _.ElementAt(1).Text != nameEntry.Text);
                }
                catch { }
                if (entry.Text != "" && periodToCascade != null)
                {

                    var indexToCascade = int.Parse(entry.Text);
                    if (indexToCascade == _addedPeriods.Count) indexToCascade = 0;

                    periodToCascade.ElementAt(0).Text = ++indexToCascade + "";
                }
                else if (entry.Text != "" && periodToCascade == null)
                {
                }

            };
            mainGrid.Children.Add(intervalEntry, 3, rowCount++);

            if (_addedPeriods == null) _addedPeriods = new List<List<Entry>>();
            List<Entry> addedPeriod = new List<Entry>() {
                    indexEntry, nameEntry, intervalEntry
                };
            if (true) _addedPeriods.Add(addedPeriod);
        }

        public void AddPeriod() {
            var mainGrid = (Grid)_periodsView.Children.First();
            var periodCount = 1;
            if (_addedPeriods != null) periodCount = _addedPeriods.Count + 1;
            AddPeriod(new CIPeriod() {
                Index = periodCount,
                Name = "New Period #"+(periodCount),
                Interval = new TimeSpan(0,0,0)
            },mainGrid,true);
        }
        public bool SaveSchedule(CISchedule sched) {
            if (sched.Name == "") {
                DependencyService.Get<Toast>().Show("Save unsuccessful. Please input a schedule name.");
                return false;
            }
            else if (!CrossConnectivity.Current.IsConnected) {

                DependencyService.Get<Toast>().Show("Save unsuccessful because of no internet connection.");
                return false;
            }
            else
            {
                try
                {
                    App.ClockItUser.Schedules.Remove(App.ClockItUser.Schedules.Single(r => r.Id == sched.Id));
                }
                catch { }
                sched.Periods.Clear();
                if (_addedPeriods != null)
                {
                    _addedPeriods.OrderBy(x => x.ElementAt(0).Text);
                    int minutes;
                    int seconds;
                    int totalMinutes = 0;
                    int totalSeconds = 0;

                    foreach (var period in _addedPeriods)
                    {
                        try
                        {
                            var minutesDouble = double.Parse(period.ElementAt(2).Text);
                            minutes = (int)(minutesDouble);
                            seconds = (int)((minutesDouble % 1) * 60);
                        }
                        catch
                        {
                            minutes = int.Parse(period.ElementAt(2).Text);
                            seconds = 0;

                        }
                        sched.Periods.Add(new CIPeriod()
                        {
                            Index = int.Parse(period.ElementAt(0).Text),
                            Name = (period.ElementAt(1).Text),
                            Interval = new TimeSpan(0, minutes, seconds),
                        });
                        totalMinutes += minutes;
                        totalSeconds += seconds;
                    }
                    totalMinutes += totalSeconds / 60;
                    sched.TotalHours = totalMinutes / 60;
                    App.ClockItUser.Schedules.Add(sched);
                    App.ClockItUser.Schedules.OrderBy(x => x.Id);
                    SaveUserToAzureDB(_client);
                    var s = IsEdit ? "Edit" : "Create";
                    DependencyService.Get<Toast>().Show("Schedule " + sched.Name + " saved.");
                    return true;


                }
                else {
                    DependencyService.Get<Toast>().Show("Schedule " + sched.Name + " save failed.");
                    return false;
                }
            }
        }
        public void SaveDB()
        {

            CrossSecureStorage.Current.SetValue("schedules", JsonConvert.SerializeObject(App.CISchedules));
            
        }
    }
}
    
