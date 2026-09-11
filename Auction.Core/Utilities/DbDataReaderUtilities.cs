using System.Data.Common;

namespace Auction_Core.Utilities;

public static class DbDataReaderUtilities
{
    public static double ReadDouble(this DbDataReader reader, string column)
    {
        return Convert.ToDouble(reader.GetValue(reader.GetOrdinal(column)));
    }

    public static bool HasValue(this DbDataReader reader, string column)
    {
        return !reader.IsDBNull(reader.GetOrdinal(column));
    }
}