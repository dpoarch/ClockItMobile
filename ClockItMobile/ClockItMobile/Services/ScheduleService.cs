using ClockIt.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockIt.Mobile.Services
{
    public static class ScheduleService
    {
        public static bool SaveSchedule(CISchedule schedule)
        {
            var scheduleToReplace = App.CISchedules.First(_ => _.Id == schedule.Id);
            var index = App.CISchedules.IndexOf(scheduleToReplace);
            if (index < 0) return false;
            App.CISchedules[index] = schedule;
            return true;
        }
        public static bool DeleteSchedule(CISchedule schedule)
        {
            App.CISchedules.Remove(schedule);
            return true;
        }
        public static bool SaveNewSchedule(CISchedule schedule)
        {
            if (App.CISchedules.Any(_ => _.Id == schedule.Id)) schedule.Id = schedule.Id + 1;
            if (App.CISchedules.Any(_ => _.Name == schedule.Name)) schedule.Name = schedule.Name + " (Copy)";
            App.CISchedules.Add(schedule);
            return true;
        }
    }
}
