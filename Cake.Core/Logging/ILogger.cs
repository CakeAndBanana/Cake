using System;

namespace Cake.Core.Logging
{
    public interface ILogger
    {
        ILogger Get();
        void CreateLog();
        void SendWelcomeMessage();
        void Log(Type type, string message, string memberName = "", string sourceFilePath = "", int lineNumber = 0);
        void Log(Type type, string[] messages, string memberName = "", string sourceFilePath = "", int lineNumber = 0);
        void LogError(Exception exception, string memberName = "", string sourceFilePath = "", int lineNumber = 0);
        void LogException(Exception exception);
    }
}
