using System;
using System.Globalization;
using Xamarin.Forms;
using BrickController2.Helpers;


namespace BrickController2.UI.Converters
{
    public class ControllerModeStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (Nullable<bool>)value;
            if (boolValue == null)
            {
                return string.Empty;
            }
            else
            {
                return (bool)boolValue ? TranslationHelper.Translate("WhenOn") : TranslationHelper.Translate("WhenOff");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
