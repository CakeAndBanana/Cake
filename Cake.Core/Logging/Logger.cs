using System;
using System.Drawing;
using System.IO;
using Cake.Json;
using Colorful;
using Console = Colorful.Console;

namespace Cake.Core.Logging
{
    public class Logger : ILogger
    {
        private static Logger _instance;
        private static bool _isDebugging;
        private const string FinalMessageFormat = "{0} {1} {3}:{4} | {2}";
        private static readonly Color DefaultColor = Color.DarkGray;

        public static ILogger Get()
        {
            if (_instance == null)
            {
                _instance = new Logger();

                #if DEBUG
                _isDebugging = true;
                #endif

                _instance.SendWelcomeMessage();
            }

            return _instance;
        }

        ILogger ILogger.Get()
        {
            return Get();
        }

        public void SendWelcomeMessage()
        {
            var styleSheet = new StyleSheet(Color.LightGray);
            styleSheet.AddStyle("C", Color.Red);
            styleSheet.AddStyle("a", Color.Orange);
            styleSheet.AddStyle("k", Color.Yellow);
            styleSheet.AddStyle("e", Color.Lime);
            styleSheet.AddStyle("!", Color.DeepSkyBlue);
            var font = FigletFont.Load(Properties.Resources.colossal);
            Console.WriteAsciiStyled("~ Cake! ~", font, styleSheet);
        }

        public void Log(Type type, 
            string message, 
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber]
            int lineNumber = 0)
        {
            Message[] messageArray = { new Message(message, type, memberName, sourceFilePath, lineNumber) };
            Log(messageArray);
        }

        public void Log(Type type,
            string[] messages,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber]
            int lineNumber = 0)
        {
            var preparedMessages = new Message[messages.Length];
            for (var i = 0; i < messages.Length; i++)
            {
                preparedMessages[i] = new Message(messages[i], type, memberName, sourceFilePath, lineNumber);
            }
            Log(preparedMessages);
        }

        public void LogError(Exception exception,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber]
            int lineNumber = 0)
        {
            Message[] messages = { new ExceptionMessage(exception) };
            Log(messages);
        }

        private void Log(params Message[] messages)
        {
            var now = DateTime.UtcNow;
            try
            {
                foreach (var message in messages)
                {
                    if (!_isDebugging && message.Type == Type.Debug)
                    {
                        return;
                    }
                    var typeName = TypeHelper.GetName(message.Type);
                    Console.WriteLineFormatted(FinalMessageFormat,
                        DefaultColor,
                        message.GetDefaultFormatter(now));
                }
            }
            catch (Exception exception) when (
                exception is IOException
                || exception is ObjectDisposedException)
            {
                var exceptionMessage = new ExceptionMessage(exception);

                Console.WriteLineFormatted(FinalMessageFormat,
                    DefaultColor,
                    exceptionMessage.GetDefaultFormatter(now));
            }
        }

        public void LogException(Exception exception)
        {
            Log(new ExceptionMessage(exception));
        }
    }
}
