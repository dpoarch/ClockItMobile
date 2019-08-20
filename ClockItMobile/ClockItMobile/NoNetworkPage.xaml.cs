using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClockIt.Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NoNetworkPage : ContentPage
	{
		public NoNetworkPage ()
		{
			InitializeComponent ();
		}

        void OnButtonClicked(object sender, EventArgs args)
        {
            App.Nav.NavigateTo(App.Locator.MainPage);
        }
    }
}