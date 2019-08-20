using ClockIt.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Services
{
    public static class MessageService
    {
        public static void ShowToast(string message) {
            var toastService = DependencyService.Get<Toast>();
            toastService.Show(message);
        }
        public static Task ShowDialog(string title, string message) {
            return App.Current.MainPage.DisplayAlert(title, message, "OK");
        }
        public static void ShowNotification(string message)
        {
            var toastService = DependencyService.Get<Toast>();
            toastService.Show(message);
        }
    }
}
