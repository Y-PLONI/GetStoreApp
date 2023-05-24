﻿using Microsoft.UI.Xaml.Data;
using System;
using Windows.Globalization.DateTimeFormatting;

namespace GetStoreApp.Converters.Formats
{
    public class GMTFormatConverter : IValueConverter
    {
        /// <summary>
        /// GMT时间与当地地区时间转换器
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return string.Empty;
            }

            string RawDataTime = System.Convert.ToString(value);

            DateTime dateTime = System.Convert.ToDateTime(RawDataTime).ToLocalTime();

            DateTimeFormatter dateTimeFormatter = new DateTimeFormatter("month day year hour minute second");
            return dateTimeFormatter.Format(dateTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
