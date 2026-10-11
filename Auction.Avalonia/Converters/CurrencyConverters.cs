using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Auction_Avalonia.Converters;

public class CurrencyConverters : IValueConverter
{

    private static readonly CultureInfo DanishCulture = new("da-DK");
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal source)
        {
            return source.ToString("C2", DanishCulture);
        }
        return new BindingNotification(new InvalidCastException(), 
                                                BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, 
                                object? parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
}