// Author: Mia Ramos
// Date: December 1, 2024
// Email: miarramos29@gmail.com
// #####################################################################################################################################################
using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace RXTrackByMiaRamosCo.Converters {
    //Converts boolean to be able to convert the taken button's status (enabled or disabled)
    public class BoolToButtonEnabledConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            // Allows for the Taken button to be shaded off/disabled if user marks medication as taken
            return value is bool isTaken && !isTaken;
        }

        // Enables button again at the start of a new day (midnight)
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            var currentTime = DateTime.Now;

            // If it's 12:00...
            if (currentTime.Hour == 0 && currentTime.Minute == 0) {
               //Will reset the button to be enabled
                return false; 
            }

            return value;
        }
    }
}