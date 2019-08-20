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
    public partial class RunPauseSchedulePage : ContentPage
    {
        public RunPauseSchedulePage()
        {
            InitializeComponent();
            App.Locator.RunPauseSchedule.Schedule = App.RunningSchedule;
            App.Locator.RunPauseSchedule.Image = playPauseImage;
            App.Locator.RunPauseSchedule.stackLayout = layout;
            App.Locator.RunPauseSchedule.listView = periods;
            BindingContext = App.Locator.RunPauseSchedule;
        }

        public void PausePlayCommand(object sender, EventArgs e)
        {
            var image = (Image)sender;
            var source = (FileImageSource)image.Source;
            if (source.File == "pause.png")
            {
                source.File = "play.png";
                App.Locator.RunPauseSchedule.PauseTimer();
            }
            else
            {
                source.File = "pause.png";
                App.Locator.RunPauseSchedule.RunTimer();
            }
        }
        public void StopCommand(object sender, EventArgs e)
        {
            var image = playPauseImage;
            var source = (FileImageSource)image.Source;
            App.Locator.RunPauseSchedule.StopTimer();
            source.File = "play.png";
        }
        public void JumpCommand(object sender, EventArgs e)
        {
            var image = playPauseImage;
            var source = (FileImageSource)image.Source;
            App.Locator.RunPauseSchedule.JumpPeriod();
            source.File = "pause.png";
        }
        public void ShowHideListCommand(object sender, EventArgs e)
        {
            App.Locator.RunPauseSchedule.ShowHideList();
        }
        public void BackToSchedulesCommand(object sender, EventArgs e)
        {
            App.Locator.AddEditSchedule.BackToSchedulesCommand.Execute(new object());

        }


    }
}