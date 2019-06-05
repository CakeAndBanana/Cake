using System;
using System.Drawing;
using System.IO;
using Cake.Storage;
using Colorful;
using Console = Colorful.Console;

namespace Cake.Core.Logging
{
    public class Logger : ILogger
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
        private static Logger _instance;
        private static bool _isDebugging;
        private const string FinalMessageFormat = "{0} {1} {3} | {2}";
        private static readonly Color DefaultColor = Color.DarkGray;

        public static ILogger Get()
        {
            if (_instance == null)
            {
                _instance = new Logger();

                #if DEBUG
                _isDebugging = true;
                #endif

                _instance.CreateLog();
                _instance.SendWelcomeMessage();
            }

            return _instance;
        }

        ILogger ILogger.Get()
        {
            return Get();
        }
        
        public void CreateLog()
        {
            if (!File.Exists(Path))
            {
                File.Create(Path);
            }
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
            string[] messages,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            var preparedMessages = new Message[messages.Length];
            for (var i = 0; i < messages.Length; i++)
            {
                preparedMessages[i] = new Message(messages[i], type);
            }
            Log(preparedMessages);
        }

        private void Log(params Message[] messages)
        {
            var now = DateTime.UtcNow;
            try
            {
                if (!CakeJson.GetConfig().LogEnabled) return;
                using (var tw = new StreamWriter(Path, true))
                {
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
                            tw.WriteLine($"[{typeName}] {now} | {message.Text}");
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
                    finally
                    {
                        tw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void LogException(Exception exception)
        {
            Log(new ExceptionMessage(exception));
        }
    }
}
