using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using Cake.Storage;
using Colorful;
using Console = Colorful.Console;

namespace Cake.Logger
{
    public class Logger : ILogger
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
        private static Logger _instance;
        private static bool _isDebugging;
        private Assembly _assembly = Assembly.GetExecutingAssembly();

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

        public void Log(Type type, params string[] messages)
        {
            var preparedMessages = new Message[messages.Length];
            for (var i = 0; i < messages.Length; i++)
            {
                preparedMessages[i] = new Message(messages[i], type);
            }
            Log(preparedMessages);
        }

        public void Log(params Message[] messages)
        {
            var now = DateTime.UtcNow;
            try
            {
                if (CakeJson.GetConfig().LogEnabled)
                {
                    using (var tw = new StreamWriter(Path, true))
                    {
                        foreach (var message in messages)
                        {
                            if ((!_isDebugging) && message.Type == Type.Debug)
                            {
                                return;
                            }

                            const string finalMessage = "[{0}]\t{1} | {2}";
                            var messageColor = TypeHelper.GetColor(message.Type);
                            var timestampColor = Color.LightGray;

                            Formatter[] formatter = {
                                new Formatter(TypeHelper.GetName(message.Type), messageColor),
                                new Formatter(now.ToString(CultureInfo.InvariantCulture), timestampColor),
                                new Formatter(message.Text, messageColor)
                            };
                            Console.WriteLineFormatted(finalMessage, Color.DarkGray, formatter);
                            tw.WriteLine($"[{message.Type.ToString()}] {now} | {message.Text} \n");
                        }
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
