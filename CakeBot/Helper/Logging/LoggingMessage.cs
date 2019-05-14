namespace CakeBot.Helper.Logging
{
    public class LoggingMessage
    {
        public LoggingMessage(string message, LoggingType type = LoggingType.Info)
        {
            Message = message;
            Type = type;
        }

        public string Message { get; }
        public LoggingType Type { get; }
    }
}
