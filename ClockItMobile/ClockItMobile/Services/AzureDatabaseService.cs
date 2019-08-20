using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Models;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClockIt.Mobile.Services
{
    public static class AzureDatabaseService
    {
        private static HttpClient _client = ClientHelper.GetClient();
        public static async Task LoadDBAsync() {
            await ClientHelper.SetAuthAndHeaders(ClientHelper.GET_ALL, "");
            var response = await _client.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                //App.IsUserLoggedIn = true;

                string jsonMessage;
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    jsonMessage = new StreamReader(responseStream).ReadToEnd();
                }
                try
                {
                    //var docs = JsonConvert.DeserializeObject<JObject>(jsonMessage);
                    var docs = JsonConvert.DeserializeObject<AzureResponse>(jsonMessage);
                    App.ClockItUsers = docs.Documents;

                    //CredentialsAreCorrect();
                    //RaisePropertyChanged(() => ListViewHeight);
                    //SetMenuItems();
                    //App.MasterMenu.IsGestureEnabled = true;
                }
                catch (Exception e)
                {
                }

            }
        }
        public static async Task<bool> SaveDBAsync() {
            if (CrossConnectivity.Current.IsConnected)
            {


                await ClientHelper.SetAuthAndHeaders(ClientHelper.DELETE, App.ClockItUser.Id);
                var response = await _client.DeleteAsync("");
                if (response.IsSuccessStatusCode)
                {

                    var json = JsonConvert.SerializeObject(App.ClockItUser);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    await ClientHelper.SetAuthAndHeaders(ClientHelper.POST, "");
                    var response2 = await _client.PostAsync("", content);

                    if (response2.IsSuccessStatusCode)
                    {
                        App.IsUserLoggedIn = true;

                        string jsonMessage;
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            jsonMessage = new StreamReader(responseStream).ReadToEnd();
                        }
                        //App.ClockItUser = JsonConvert.DeserializeObject<ClockItUser>(jsonMessage);
                        App.CISchedules = new ObservableCollection<CISchedule>(App.ClockItUser.Schedules);

                    }
                    else
                    {
                        MessageService.ShowToast("Saved data failed but deleted old data.");
                        return false;
                    }

                }
                else
                {
                    MessageService.ShowToast("Data delete failed before save new data, save failed.");
                    return false;
                }
                App.Locator.Schedules.Refresh();
                return true;

            }
            else
            {

                MessageService.ShowToast("Save unsuccessful because of no internet connection.");
                return false;
            }

        }
    }
}
