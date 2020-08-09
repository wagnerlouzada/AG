using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DB
{
    public static class DataReaderExtension
    {

        public static T isDBNULL<T>(this IDataRecord record, String ColName)
        {
            object value = record[ColName];

            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return (value == null || DBNull.Value.Equals(value)) ? default(T) : (T)Convert.ChangeType(value, t);
        }

    }
}
