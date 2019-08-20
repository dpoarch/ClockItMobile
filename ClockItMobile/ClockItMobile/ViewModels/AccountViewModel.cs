using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.ViewModels
{
    public class AccountViewModel : ViewModelBaseWithServices
    {
        readonly INavigationService _navigationService;
        HttpClient _client;
        ClockItUser _user;
        RelayCommand _saveCommand;
        RelayCommand _backCommand;

        public RelayCommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(() =>
                {
                    _navigationService.NavigateTo(App.Locator.SchedulesPage);
                }, () => true));
            }
        }
        public string DateCreated
        {
            get { return "Account Created: " + App.ClockItUser.DateCreated.ToString("g"); }
        }
        public string Email
        {
            get { return App.ClockItUser.Email; }
        }
        public string Phone
        {
            get { return App.ClockItUser.Phone; }
        }
        public string SchedulesCount
        {
            get { return "Schedules Created: "+App.CISchedules.Count(); }
        }

        public AccountViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException("navigationService");
            _client = ClientHelper.GetClient();
            _user = App.ClockItUser;
            App.MasterMenu.ContentPage.Icon = new FileImageSource()
            {
                File = "hamburger@2x.png"
            };

        }

    }
}
