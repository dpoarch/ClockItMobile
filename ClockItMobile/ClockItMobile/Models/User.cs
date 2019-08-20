using SQLite;
using System.Collections.ObjectModel;

namespace ClockIt.Mobile.Models
{
	public class User
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public bool RememberPassword { get; set; }
	}
    public class AzureResponse {
        public ObservableCollection<ClockItUser> Documents { get; set; }
    }
}
