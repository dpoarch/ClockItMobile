
using ClockIt.Mobile.Models;
using ClockIt.Mobile.Views;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Services
{
    public static class AppService
    {
        public static MasterMainPage MasterMenu;
        public static NavigationPage CurrentPage;

        public static void InitializeMenu() {

            MasterMenu = new MasterMainPage(App.Nav)
            {
                Detail = App.Nav._navigation,
            };
        }

        public static bool IsWifiConnected() {
            return CrossConnectivity.Current.IsConnected;
        }

        public static void ShowHamburgerIcon() {
            SetMasterMenuItems();
            MasterMenu.IsGestureEnabled = true;
            
        }

        public static void HideHamburgerIcon() {
            MasterMenu.IsGestureEnabled = false;

        }

        public static void SetMasterMenuItems() {
            var masterPage = App.MasterMenu;
            //var emp = App.LoginResponse.Employee;

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

        public static void ClearMasterMenuItems() {
            MasterMenu.ListView.ItemsSource = new List<MasterPageItem>();
            MasterMenu.ContentPage.Icon = new FileImageSource()
            {
                File = "hamburger@2x.png"
            };
        }
    }
}
