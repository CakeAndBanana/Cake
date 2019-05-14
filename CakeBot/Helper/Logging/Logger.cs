using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using CakeBot.Core;
using Colorful;
using Console = Colorful.Console;

namespace CakeBot.Helper.Logging
{
    public class Logger
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
        private static bool _isDebugging;

        public static void LogInt()
        {
            #if DEBUG
            _isDebugging = true;
            #endif

            string message = "~ Cake! ~";

            if (File.Exists(Path))
            {
                LogFancy(message);
            }
            else
            {
                LogFile();
                LogFancy(message);
            }
        }

        public static void LogFile()
        {
            File.Create(Path);
        }

        private static LoggingMessage[] CreateArrayOfLoggingMessages(IReadOnlyCollection<string> messages, LoggingType type)
        {
            List<LoggingMessage> coloredMessages = new List<LoggingMessage>(messages.Count);
            foreach (var message in messages)
            {
                coloredMessages.Add(new LoggingMessage(message, type));
            }

            return coloredMessages.ToArray();
        }

        public static void LogFancy(string message)
        {
            var styleSheet = new StyleSheet(Color.White);
            styleSheet.AddStyle("C", Color.Red);
            styleSheet.AddStyle("a", Color.Orange);
            styleSheet.AddStyle("k", Color.Yellow);
            styleSheet.AddStyle("e", Color.Lime);
            styleSheet.AddStyle("!", Color.DeepSkyBlue);
            Console.WriteAsciiStyled(message, FigletFont.Load(System.IO.Path.Combine(Environment.CurrentDirectory, @"FigletFonts\", "colossal.flf")), styleSheet);
        }

        public static void LogDebug(params string[] messages)
        {
            var coloredMessages = CreateArrayOfLoggingMessages(messages, LoggingType.Debug);
            LogMessage(coloredMessages);
        }

        public static void LogInfo(params string[] messages)
        {
            var coloredMessages = CreateArrayOfLoggingMessages(messages, LoggingType.Info);
            LogMessage(coloredMessages);
        }

        public static void LogWarning(params string[] messages)
        {
            var coloredMessages = CreateArrayOfLoggingMessages(messages, LoggingType.Warn);
            LogMessage(coloredMessages);
        }

        public static void LogError(params string[] messages)
        {
            var coloredMessages = CreateArrayOfLoggingMessages(messages, LoggingType.Error);
            LogMessage(coloredMessages);
        }

        public static void LogSuccess(params string[] messages)
        {
            var coloredMessages = CreateArrayOfLoggingMessages(messages, LoggingType.Success);
            LogMessage(coloredMessages);
        }

        public static void LogMessage(params LoggingMessage[] messages)
        {
            var now = DateTime.UtcNow;
            try
            {
                if (Config.LogEnabled)
                {
                    using (var tw = new StreamWriter(Path, true))
                    {
                        foreach (var message in messages)
                        {
                            if ((!_isDebugging) && message.Type == LoggingType.Debug)
                            {
                                return;
                            }

                            const string finalMessage = "[{0}]\t{1} | {2}";
                            var messageColor = LoggingTypeHelper.GetColor(message.Type);
                            var timestampColor = Color.ForestGreen;

                            Formatter[] formatter = {
                                new Formatter(LoggingTypeHelper.GetName(message.Type), messageColor), 
                                new Formatter(now.ToString(CultureInfo.InvariantCulture), timestampColor),
                                new Formatter(message.Message, messageColor)
                            };
                            Console.WriteLineFormatted(finalMessage, Color.DarkGray, formatter);
                            tw.WriteLine($"[{message.Type.ToString()}] {now} | {message.Message} \n");
                        }
                        tw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
        }
    }
}
