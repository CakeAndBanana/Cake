using System;

namespace Cake.Core.Logging
{
    public interface ILogger
    {
        ILogger Get();
        void CreateLog();
        void SendWelcomeMessage();
        void Log(Type type, string[] messages, string memberName, int sourceLineNumber);
        void LogException(Exception exception);
    }
}
