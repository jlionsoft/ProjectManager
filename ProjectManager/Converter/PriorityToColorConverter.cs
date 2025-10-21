using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectManager.Converter
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string priority = value as string;
            switch (priority)
            {
                case "Hoch":
                    return Brushes.Red;
                case "Mittel":
                    return Brushes.Orange;
                case "Niedrig":
                    return Brushes.Green;
                default:
                    return Brushes.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return MessageBox.Show("Convert back not possible.");
        }
    }
}
