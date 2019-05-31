using System.Drawing;

namespace Cake.Logger
{
    public enum Type
    {
        Debug,
        Info,
        Warn,
        Error,
        Success
    }

    public class TypeHelper
    {
        private const string Debug = "DEBUG";
        private const string Info = "INFO ";
        private const string Warn = "WARN ";
        private const string Error = "ERROR";
        private const string Success = "SUCC6";

        public static Color GetColor(Type type)
        {
            switch (type)
            {
                case Type.Debug:
                    return Color.DodgerBlue;
                case Type.Info:
                    return Color.DarkSeaGreen;
                case Type.Warn:
                    return Color.DarkOrange;
                case Type.Error:
                    return Color.DarkRed;
                case Type.Success:
                    return Color.ForestGreen;
                default:
                    return Color.ForestGreen;
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
                case Type.Warn:
                    return Warn;
                case Type.Error:
                    return Error;
                case Type.Success:
                    return Success;
                default:
                    return Info;
            }
        }
    }
}
