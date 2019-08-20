using ClockIt.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.ValueConverters
{
    public class TeamLeadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } }

        public class CIDateTimeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var dt = (DateTime)value;
                var dayOfWeek = dt.DayOfWeek.ToString();
                var month = dt.ToString("MMMM");
                var dateDay = dt.Day + "";
                if (dateDay.Substring(dateDay.Length - 1) == "1") dateDay += "st";
                else if (dateDay.Substring(dateDay.Length - 1) == "2") dateDay += "nd";
                else if (dateDay.Substring(dateDay.Length - 1) == "3") dateDay += "rd";
                else dateDay += "th";
                return dayOfWeek + " " + month + " " + dateDay + " | " + dt.ToString("h:mm tt");
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        public class HoursSuffixConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var f = (double)value;
                return Math.Round(f, 2) + " Hours   ";
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
    }
    public class PeriodsSuffixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value + " Periods";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class IntervalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ts = (TimeSpan)value;
            return ts.ToString("c");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}


