using System.Drawing;

namespace CakeBot.Helper.Logging
{
    public enum LoggingType
    {
        Debug,
        Info,
        Warn,
        Error,
        Success
    }

    public class LoggingTypeHelper
    {
        public static string Debug = "DEBUG";
        public static string Info = "INFO ";
        public static string Warn = "WARN ";
        public static string Error = "ERROR";
        public static string Success = "SUCC6";

        public static Color GetColor(LoggingType type)
        {
            switch (type)
            {
                case LoggingType.Debug:
                    return Color.DodgerBlue;
                case LoggingType.Info:
                    return Color.DarkSeaGreen;
                case LoggingType.Warn:
                    return Color.DarkOrange;
                case LoggingType.Error:
                    return Color.DarkRed;
                case LoggingType.Success:
                    return Color.ForestGreen;
                default:
                    return Color.ForestGreen;
            }
        }

        public static string GetName(LoggingType type)
        {
            switch (type)
            {
                case LoggingType.Debug:
                    return Debug;
                case LoggingType.Info:
                    return Info;
                case LoggingType.Warn:
                    return Warn;
                case LoggingType.Error:
                    return Error;
                case LoggingType.Success:
                    return Success;
                default:
                    return Info;
            }
        }
    }
}
