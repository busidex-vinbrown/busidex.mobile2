﻿using System;
using Busidex3.ViewModels;
using Xamarin.Forms;

namespace Busidex3.Converters
{
    public class IsCardSideBackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(UserCardDisplay.CardSide.Back);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}