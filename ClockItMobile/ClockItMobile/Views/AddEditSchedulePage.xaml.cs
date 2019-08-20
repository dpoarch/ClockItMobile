using ClockIt.Mobile.Models;
using Newtonsoft.Json.Linq;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClockIt.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditSchedulePage : ContentPage
    {
        public AddEditSchedulePage(CISchedule s)
        {
            InitializeComponent();
            App.Locator.AddEditSchedule.PeriodsView = PeriodsLayout;
            App.Locator.AddEditSchedule.Schedule = s;
            App.Locator.AddEditSchedule.SetNoSelected();
            BindingContext = App.Locator.AddEditSchedule;
            App.MasterMenu.ContentPage.Icon = new FileImageSource()
            {
                File = "hamburger@2x.png"
            };
        }
        public void ReorderPeriodsCommand(object sender, EventArgs e)
        {
            App.Locator.AddEditSchedule.ReloadPeriodsView(false);

        }
        public void AddPeriodCommand(object sender, EventArgs e)
        {
            App.Locator.AddEditSchedule.AddPeriod();

        }
        public void RunScheduleCommand(object sender, EventArgs e)
        {
            App.Locator.AddEditSchedule.RunScheduleCommand.Execute(new object());

        }
        public void SaveScheduleCommand(object sender, EventArgs e)
        {
            App.Locator.AddEditSchedule.SaveSchedule(App.Locator.AddEditSchedule.Schedule);

        }
        public void BackToSchedulesCommand(object sender, EventArgs e)
        {
            App.Locator.AddEditSchedule.BackToSchedulesCommand.Execute(new object());

        }
        public void DeleteScheduleCommand(object sender, EventArgs e)
        {
            App.Locator.AddEditSchedule.DeleteSchedule();

        }
    }
}