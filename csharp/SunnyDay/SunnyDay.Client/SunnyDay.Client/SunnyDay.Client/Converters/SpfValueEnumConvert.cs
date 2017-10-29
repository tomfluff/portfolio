using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Models;
using Xamarin.Forms;

namespace SunnyDay.Client.Converters
{
    public class SpfValueEnumConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine($"> {(int)(SpfValues)value} ({(SpfValues)value})");
            return $"{(int)(SpfValues)value} ({(SpfValues)value})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // the null check is required.  when the data template selector is being changed dynamically,
            // this method gets called with the value set to null.
            var str = value.ToString();
            var start = str.IndexOf('(');
            var end = str.IndexOf(')');
            Debug.WriteLine($"> {value}, {start}, {end}");
            Debug.WriteLine($"> {str.Substring(start+1, end-start-1)}");
            return value == null ? null : Enum.Parse(typeof(SpfValues), str.Substring(start+1, end-start-1));
        }
    }
}
