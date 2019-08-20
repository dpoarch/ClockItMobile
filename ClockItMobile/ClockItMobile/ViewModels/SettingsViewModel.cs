using ClockIt.Mobile.Helpers;
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
    public class SettingsViewModel : ViewModelBaseWithServices
    {
        public readonly INavigationService _navigationService;
        HttpClient _client;
        public SettingsViewModel(INavigationService navigationService) {
            _navigationService = navigationService ?? throw new ArgumentNullException("navigationService");
            _client = ClientHelper.GetClient();
            App.MasterMenu.ContentPage.Icon = new FileImageSource()
            {
                File = "hamburger@2x.png"
            };
        }
    }
}
