using System;
using System.Net;

namespace Cake.Logger
{
    public interface ILogger
    {
        ILogger Get();
        void CreateLog();
        void SendWelcomeMessage();
        void Log(Type type, params string[] messages);
        void Log(params Message[] messages);
        void LogException(Exception exception);
    }
}
