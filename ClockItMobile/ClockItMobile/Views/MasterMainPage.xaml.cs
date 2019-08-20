using ClockIt.Mobile.Helpers;
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
    public partial class MasterMainPage : MasterDetailPage
    {
        public ListView ListView { get; set; }
        public ContentPage ContentPage { get; set; }
        public NavigationService _nav;
        public MasterPage masterPage;
        public MasterMainPage(NavigationService nav)
        {
            _nav = nav;
            InitializeComponent();

            masterPage = new MasterPage();
            masterPage.ListView.ItemSelected += OnItemSelected;
            ListView = masterPage.ListView;
            ContentPage = masterPage.ContentPage;

            Master = masterPage; 
        }

        
        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MasterPageItem item)
            {
                if (item.TargetType.Equals("DashboardPage")|| item.TargetType.Equals("ReportTimeTrackingPage"))
                {
                    try { _nav.NavigateTo(item.TargetType, true); }
                    catch { _nav.NavigateTo(item.TargetType); }
                    IsPresented = false;
                    masterPage.ListView.SelectedItem = null;
                }
                else {

                    _nav.NavigateTo(item.TargetType);
                    IsPresented = false;
                    masterPage.ListView.SelectedItem = null;
                    if (item.TargetType.Equals("MainPage"))
                    {
                        IsGestureEnabled = false;
                    }
                }

                masterPage.ContentPage.Icon = new FileImageSource()
                {
                    File = "hamburger@2x.png"
                };
            }
        }
    }
}