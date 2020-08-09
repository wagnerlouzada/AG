using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomExtensions
{
    // Extension methods must be defined in a static class.
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }

    public static class ObjectExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static int ObjectToInt(this object Value)
        {
            if (Value == null) return 0;
            int Result = 0;
            try
            {
                Result = Convert.ToInt16(Value);
            }
            catch
            {
                return 0;
            }
            return Result;
        }

        public static decimal ObjectToDecimal(this object Value)
        {
            if (Value == null) return 0;
            decimal Result = 0;
            try
            {
                Result = Convert.ToDecimal(Value);
            }
            catch
            {
                return 0;
            }
            return Result;
        }

        public static long ObjectToLong(this object Value)
        {
            if (Value == null) return 0;
            long Result = 0;
            try
            {
                Result = Convert.ToInt64(Value);
            }
            catch
            {
                return 0;
            }
            return Result;
        }

        public static String ObjectToString(this object Value)
        {
            if (Value == null) return null;
            String Result = null;
            try
            {
                Result = Value.ToString();
                if (Result == "{}") Result = null;
            }
            catch
            {
                return null;
            }
            return Result;
        }

        public static Boolean ObjectToBoolean(this object Value)
        {
            String value = Convert.ToString(Value);
            Boolean Result = false;
            if (value.ToString().ToUpper() == "TRUE") { Result = true; }
            if (value.ToString() == "1") { Result = true; }
            return Result;
        }

        public static DateTime? ObjectToDateTime(this object Value)
        {
            DateTime? Result = null;
            try
            {
                Result = Convert.ToDateTime(Value);
            }
            catch
            {
                return null;
            }
            return Result;
        }


    }

}

