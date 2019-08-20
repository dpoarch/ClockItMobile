using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClockIt.Mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SchedulesPage : ContentPage
	{
		public SchedulesPage()
		{
			InitializeComponent();
			BindingContext = App.Locator.Schedules;
		}
	}
}