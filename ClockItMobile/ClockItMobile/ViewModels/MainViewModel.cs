using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.SecureStorage;
using Xamarin.Forms;

namespace ClockIt.Mobile.ViewModels
{
    public class MainViewModel : ViewModelBaseWithServices
	{
		public readonly INavigationService _navigationService;
		RelayCommand _loginCommand;
        RelayCommand _rememberPasswordToggledCommand;
        RelayCommand _registerCommand;
        string _status;
		HttpClient _client;
		bool _isBusy;

		public string Username { get; set; }
		public string Password { get; set; }
		public bool RememberPassword { get; set; }




        public string Status
		{
			get { return _status; }
			set
			{
				if (Set(() => Status, ref _status, value))
				{
					RaisePropertyChanged(() => StatusFormatted);
				}
			}
		}

		public string StatusFormatted
		{
			get { return _status; }
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				if (Set(() => IsBusy, ref _isBusy, value))
				{
					RaisePropertyChanged(() => IsBusyIndicator);
				}
			}
		}

		public bool IsBusyIndicator
		{
			get { return _isBusy; }
		}

		public MainViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService ?? throw new ArgumentNullException("navigationService");

            if (CrossSecureStorage.Current.HasKey("username"))
			{
				Username = CrossSecureStorage.Current.GetValue("username");
			}

			if (CrossSecureStorage.Current.HasKey("password"))
			{
				Password = CrossSecureStorage.Current.GetValue("password");
				RememberPassword = true;
			}
			_client = ClientHelper.GetClient();
            
            
        }
        

        public async Task LoadDB()
        {
            IsBusy = true;

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
                }
                catch (JsonSerializationException e)
                {

                }
                catch (Exception e) {

                    Status = ""+e.Message;
                    IsBusy = false;
                }

            }
            IsBusy = false;
        }

        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand(() =>
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        Status = "Logging in...";
                        Task login = Login();
                    }
                    else {


                        DependencyService.Get<Toast>().Show("No internet connection, can't login.");
                    }
                }, () => true));
            }
        }
        public RelayCommand RegisterCommand
        {
            get
            {
                return _registerCommand ?? (_registerCommand = new RelayCommand(() =>
                {
                    InputBoxRegister();
                }, () => App.ClockItUsers != null));
            }
        }
        

        async Task Login()
		{

            SetMenuItems();
            App.MasterMenu.IsGestureEnabled = true;

            var b = await CredentialsAreCorrect();
            try
            {
                if (b)
                {
                    _navigationService.NavigateTo(App.Locator.SchedulesPage);
                    App.Locator.Schedules.Refresh();

                    Status = "Login success";
                }
                else
                {
                    Status = "Login failed";
                }
            }
            catch (Exception) {

                Status = "App crashed";
            }
        }
        private void SetMenuItems() {
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

            masterPage.ContentPage.Icon = new FileImageSource() {
                File = "hamburger@2x.png"
            };
        }
		async Task<bool> CredentialsAreCorrect()
        {
            Task test = LoadDB();
            
            foreach (var i in App.ClockItUsers) {
                if (i.Email == (Username??"") && i.Phone == (Password??"")) {
                
                    if(false)
                    {
                    }
                    else {
                        if(i.Schedules!=null)
                        App.CISchedules = new ObservableCollection<CISchedule>(i.Schedules);
                    }
                    App.ClockItUser = i;

                    
                    if (!CrossSecureStorage.Current.HasKey("username"))
                    {
                        if (Username == null) {

                            CrossSecureStorage.Current.SetValue("username", "");
                        }
                        else
                        CrossSecureStorage.Current.SetValue("username", Username);
                    }

                    if (!CrossSecureStorage.Current.HasKey("password"))
                    {
                        if (Password == null)
                        {

                            CrossSecureStorage.Current.SetValue("password", "");
                        }
                        else
                            CrossSecureStorage.Current.SetValue("password", Password);
                    }
                    
                    return true;
                }
            }
            var res = await App.Current.MainPage.DisplayActionSheet("User not found, create new account?", "Cancel","OK" );
            if (res=="OK") {
                await RegisterUser(Username??"", Password??"", _client);
                return true;
            }
            return false;
            

        }

		public RelayCommand RememberPasswordToggledCommand
		{
			get
			{
				return _rememberPasswordToggledCommand ?? (_rememberPasswordToggledCommand = new RelayCommand(() =>
				{
					if (!RememberPassword)
					{
						CrossSecureStorage.Current.DeleteKey(Password);
					}
				}));
			}
		}



    }
}
