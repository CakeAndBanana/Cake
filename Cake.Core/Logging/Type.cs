using System.Drawing;

namespace Cake.Core.Logging
{
    public enum Type
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public class TypeHelper
    {
        private const string Debug = "DEBUG";
        private const string Info = "INFO ";
        private const string Warn = "WARN ";
        private const string Error = "ERROR";
        private const string Fatal = "FATAL";

        public static Color GetColor(Type type)
        {
            switch (type)
            {
                case Type.Debug:
                    return Color.DodgerBlue;
                case Type.Info:
                    return Color.DarkSeaGreen;
                case Type.Warning:
                    return Color.DarkOrange;
                case Type.Error:
                    return Color.Red;
                case Type.Fatal:
                    return Color.DarkRed;
                default:
                    return Color.LightGray;
            }
        }

        public static string GetName(Type type)
        {
            switch (type)
            {
                case Type.Debug:
                    return Debug;
                case Type.Info:
                    return Info;
                case Type.Warning:
                    return Warn;
                case Type.Error:
                    return Error;
                case Type.Fatal:
                    return Fatal;
                default:
                    return Info;
            }
        }
    }
}
