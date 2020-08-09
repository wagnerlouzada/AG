using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Global
{
    public static class GlobalVariables
    {
        public const Int32 duracao = 10000;
#if DEBUG
        public static LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Debug);
#else
        public static LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Error);
#endif
        public const Int16 MenuItemMaxLength = 22;
        public const String CacheName = "cacheOper";
        public const String MainDbName = "AppV";

    }
}