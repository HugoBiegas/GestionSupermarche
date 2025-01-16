using System.Globalization;

namespace GestionSupermarche.Converters
{
    public class HeuresValidationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return strValue;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                string numbersOnly = new string(strValue.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(numbersOnly))
                {
                    return "0";
                }

                if (int.TryParse(numbersOnly, out int number))
                {
                    number = Math.Min(Math.Max(number, 0), 24);
                    return number.ToString();
                }
            }
            return "0";
        }
    }
}