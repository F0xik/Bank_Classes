﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Bank
{
    class Helper : IMultiValueConverter

    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            if (values[0] is bool && values[1] is bool)

            {

                bool hasText = !(bool)values[0];

                bool hasFocus = (bool)values[1];



                if (hasFocus || hasText)

                    return Visibility.Collapsed;

            }

            return Visibility.Visible;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)

        {

            throw new NotImplementedException();

        }

    }
}
