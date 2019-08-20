using ClockIt.Mobile.Models;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockIt.Mobile.Services
{
    public static class LocalDatabaseService
    {
        public static void LoadDB() {
            var schedulesJson = CrossSecureStorage.Current.GetValue("schedules");
            if (schedulesJson != null) {
                App.CISchedules = JsonConvert.DeserializeObject<ObservableCollection<CISchedule>>(schedulesJson);
            }
        }
        public static void SaveDB() {

            CrossSecureStorage.Current.SetValue("schedules", JsonConvert.SerializeObject(App.CISchedules));
        }
    }
}
