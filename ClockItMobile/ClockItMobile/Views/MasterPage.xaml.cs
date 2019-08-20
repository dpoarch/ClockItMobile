using ClockIt.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClockIt.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public ContentPage ContentPage { get { return this; } }
        public MasterPage()
        {
            InitializeComponent();
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                // handle the tap
                //var version = App.SysInfoResponse["versionNumber"].ToString();
                var response = await Application.Current.MainPage.DisplayAlert("About Clock It:", "Current Version No.: 1.0" +
                    "\nCurrent Time: " + String.Format("{0:ddd MMM d yyyy hh:mm:ss}" + " " + TimeZoneInfo.Local.StandardName, DateTime.Now), "Ok","Cancel");
                /*
                if (response) {
                    App.MasterMenu.IsPresented=false;
                }*/
            };
            icon.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }
}