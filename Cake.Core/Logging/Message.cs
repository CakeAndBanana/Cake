using System;
using System.Drawing;
using System.Globalization;
using Colorful;

namespace Cake.Core.Logging
{
    public class Message
    {
        public Message(string text, Type type)
        {
            Text = text;
            Type = type;
            DefaultColor = TypeHelper.GetColor(Type);
        }

        public string Text { get; }
        public Type Type { get; }
        public Color DefaultColor { get; }

        public Formatter[] GetDefaultFormatter(DateTime time)
        {
            var timestampColor = Color.LightGray;
            var typeName = TypeHelper.GetName(Type);

            Formatter[] formatter =
            {
                new Formatter(typeName, DefaultColor),
                new Formatter(time.ToString(CultureInfo.InvariantCulture), timestampColor),
                new Formatter(Text, DefaultColor)
            };

            return formatter;
        }
    }
}
