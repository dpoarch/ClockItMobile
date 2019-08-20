using ClockIt.Mobile.Models;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Helpers
{


    public class ViewModelBaseWithServices : ViewModelBase
    {
        public CIScheduleMapped MapSchedule(CISchedule s) {
            var i = new CIScheduleMapped
            {
                A = s.DateTime,
                B = s.Name,
                C = s.Periods.Select(_ => { return new CIPeriodMapped() { A = _.Interval, B = _.Name, C = _.Index }; }).ToList(),
                D = s.TotalHours,
                E = s.Id,
                F = s.Time
            };
            return i;
        }

        public CISchedule UnmapSchedule(CIScheduleMapped s) {
            var i = new CISchedule() {
                DateTime = s.A,
                Name = s.B,
                Periods = s.C.Select(_ => { return new CIPeriod() { Interval = _.A, Name = _.B, Index = _.C }; }).ToList(),
                TotalHours = s.D,
                Id = s.E,
                Time = s.F
            };
            return i;
        }

        public static string Compress(string s)
        {
            return s.Replace('\"', '*');
        }

        public static string Decompress(string s)
        {
            return s.Replace('*', '\"');
        }
        /*
        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray(), 0, mso.ToArray().Count());
            }
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray(),0,mso.ToArray().Count());
            }
        }
        */
        public async Task<bool> SaveUserToAzureDB(HttpClient _client) {
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
                        DependencyService.Get<Toast>().Show("Saved data failed but deleted old data.");
                        return false;
                    }

                }
                else
                {
                    DependencyService.Get<Toast>().Show("Data delete failed before save new data, save failed.");
                    return false;
                }
                App.Locator.Schedules.Refresh();
                return true;

            }
            else {

                DependencyService.Get<Toast>().Show("Save unsuccessful because of no internet connection.");
                return false;
            }
            

        }

        public async Task RegisterUser(string em, string ph, HttpClient _client) {

            var newIndex = (App.ClockItUsers != null ? App.ClockItUsers.Count + 1 : 1) + "";
            
            if (App.ClockItUsers.FirstOrDefault(_ => _.Id == newIndex) != null) {
                newIndex = (int.Parse(newIndex) + 1) + "";
            }
            var json = JsonConvert.SerializeObject(new
            {
                id = newIndex,
                email = em,
                phone = ph,
                schedules = new List<CISchedule>() {
                    /*
                    new CISchedule(){
                        Id=1,
                        Name = "Test5 Schedule",
                        DateTime = DateTime.Now,
                        Periods = new List<CIPeriod>(){
                            new CIPeriod(){
                                Name = "5 seconds",
                                Index = 1,
                                Interval = new TimeSpan(0,0,5)
                            },
                            new CIPeriod(){
                                Name = "10 seconds",
                                Index = 2,
                                Interval = new TimeSpan(0,0,10)
                            }
                        },
                        Time = DateTime.Now.TimeOfDay.ToString(),
                        TotalHours = 0.5
                    }
                    */
                },
                dateCreated = DateTime.Now
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            /*
            var stringContent = new FormUrlEncodedContent(new[]
{
                new KeyValuePair<string, string>("id", "0"),
                new KeyValuePair<string, string>("data", json),
            });
            */
            //_client.DefaultRequestHeaders.Remove("Authorization");
            //var authHeader = DependencyService.Get<ICryptoService>().Cipher("RcDjlGzwd0CA4I3uGWcOYYB6jocrAnrrSZhcpMQZjqcsvZYYcJtI5KZA75Z5uZqtayqNZnLHN1MBb850IW3HCg==", "GET", "docs", "dbs/clockitdb/colls/clockitcol");
            //_client.DefaultRequestHeaders.Add("Authorization", authHeader);
            /*
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-type", "application/json");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Host", "meshmanager-test.documents.azure.com");
            */
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", "x-ms-session-token#0=602; x-ms-session-token=602");
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("Content-length", "0");
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("Expect", "100-continue");
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Microsoft.Azure.Documents.Client / 1.6.0.0");
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
            //request.Content = stringContent;
            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


            //await ClientHelper.SetAuthAndHeaders(ClientHelper.DELETE, "testId2");
            //var response = await _client.DeleteAsync("");


            await ClientHelper.SetAuthAndHeaders(ClientHelper.POST, "");
            var response = await _client.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                App.IsUserLoggedIn = true;

                string jsonMessage;
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    jsonMessage = new StreamReader(responseStream).ReadToEnd();
                }
                App.ClockItUser = JsonConvert.DeserializeObject<ClockItUser>(jsonMessage);
                App.CISchedules = new ObservableCollection<CISchedule>(App.ClockItUser.Schedules);
                DependencyService.Get<Toast>().Show("Registration successful for email "+em);

            }
            /*
            await App.todoTable.InsertAsync(new TodoItem() {
                Email = email,
                Phone = phone,
                Schedules = ""
            });
            */
        }

        public static Task<string> InputBox()
        {
            var navigation = App.Current.MainPage.Navigation;
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();

            var lblTitle = new Label { Text = "Jump Period", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var lblMessage = new Label { Text = "Enter period index:" };
            var txtInput = new Entry { Text = "", Keyboard = Keyboard.Numeric };

            var btnOk = new Button
            {
                Text = "Ok",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = txtInput.Text;
                await navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, lblMessage, txtInput, slButtons },

            };/*
            var layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { layout2 },
                BackgroundColor = Color.Transparent

            };*/

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }

        public static Task<string> InputBoxRegister()
        {
            var navigation = App.Current.MainPage.Navigation;
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();

            var lblTitle = new Label { Text = "Register", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var emailLabel = new Label { Text = "Enter email:" };
            var emailInput = new Entry { Text = "", Keyboard = Keyboard.Email };
            var phoneLabel = new Label { Text = "Enter phone:" };
            var phoneInput = new Entry { Text = "", Keyboard = Keyboard.Telephone };

            var btnOk = new Button
            {
                Text = "Ok",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = emailInput.Text;
                await navigation.PopModalAsync();
                // pass result
                await RegisterUserToDBAsync(emailInput.Text, phoneInput.Text);
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                WidthRequest = 100,
                BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, emailLabel, emailInput,phoneLabel,phoneInput, slButtons },

            };/*
            var layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { layout2 },
                BackgroundColor = Color.Transparent

            };*/

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            emailInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }

        private static async Task RegisterUserToDBAsync(string email, string phone)
        {
            var temp = new
            {
                Email = email,
                Phone = phone,
                Id = App.ClockItUsers.Count + "",
                Schedules = new List<CISchedule>()
            };
            var json = JsonConvert.SerializeObject(temp);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            //ClientHelper.InsertId(App.ClockItUser.Id);
            var _client = ClientHelper.GetClient();
            var response = await _client.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                App.IsUserLoggedIn = true;

                string jsonMessage;
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    jsonMessage = new StreamReader(responseStream).ReadToEnd();
                }
                //await App.Current.MainPage.DisplayAlert("Registration Success", "Account for "+email+" created.", "OK");
                DependencyService.Get<Toast>().Show("Registration Successful. "+ "Account for " + email + " created.");
                await App.Locator.Main.LoadDB();
            }
            //ClientHelper.RevertUri();
        }

        public void NavigateAccount(NavigationService nav)
        {
            //nav.NavigateTo(App.Locator.AccountPage);
        }
        /*
        public void NavigateSetting(NavigationService nav)
        {
            nav.NavigateTo(App.Locator.AccountPage);
        }*/
    }
    

}
