using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Colorful;

namespace Cake.Core.Logging
{
    public class Message
    {
        public Message(string text, Type type, string memberName = "", string sourceFilePath = "", int sourceLine = 0)
        {
            Text = text;
            Type = type;
            MemberName = memberName;
            SourceFilePath = sourceFilePath;
            SourceLine = sourceLine;
            DefaultColor = TypeHelper.GetColor(Type);
        }

        public string Text { get; }
        public Type Type { get; }
        public string MemberName { get; }
        public string SourceFilePath { get; }
        public int SourceLine { get; }
        public Color DefaultColor { get; }

        public Formatter[] GetDefaultFormatter(DateTime time)
        {
            var timestampColor = Color.LightGray;
            var typeName = TypeHelper.GetName(Type);
            var sourcePath = "Cake." + SourceFilePath.Split(new[] { "Cake." }, StringSplitOptions.RemoveEmptyEntries).Last();

            Formatter[] formatter =
            {
                new Formatter(typeName, DefaultColor),
                new Formatter(time.ToString(CultureInfo.InvariantCulture), timestampColor),
                new Formatter(Text, DefaultColor),
                new Formatter(sourcePath, Color.DarkSlateBlue), 
                new Formatter(SourceLine, Color.DarkViolet)
            };

            return formatter;
        }
    }
}
