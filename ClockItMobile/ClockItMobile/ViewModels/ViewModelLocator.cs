using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ClockIt.Mobile.ViewModels
{
	public class ViewModelLocator
	{

		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<SchedulesViewModel>();
            SimpleIoc.Default.Register<AddEditScheduleViewModel>();
            SimpleIoc.Default.Register<RunPauseScheduleViewModel>();
            SimpleIoc.Default.Register<AccountViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public AccountViewModel Account
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AccountViewModel>();
            }
        }
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
		public SchedulesViewModel Schedules
		{
			get
			{
				return ServiceLocator.Current.GetInstance<SchedulesViewModel>();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
		public AddEditScheduleViewModel AddEditSchedule
		{
			get
			{
				return ServiceLocator.Current.GetInstance<AddEditScheduleViewModel>();
			}
		}
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public RunPauseScheduleViewModel RunPauseSchedule
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RunPauseScheduleViewModel>();
            }
        }

        public string MainPage { get { return "MainPage"; } }
		public string SchedulesPage { get { return "SchedulesPage"; } }
		public string AddEditSchedulePage { get { return "AddEditSchedulePage"; } }
        public string RunPauseSchedulePage { get { return "RunPauseSchedulePage"; } }
        public string AccountPage { get { return "AccountPage"; } }
        public string SettingsPage { get { return "SettingsPage"; } }

    }
}
