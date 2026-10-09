using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Auction.Avalonia.Converters;

public class CurrencyConverters : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal source)
        {
            return source.ToString("C2", culture);
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