namespace Cake.Logger
{
    public class Message
    {
        public Message(string text, Type type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; }
        public Type Type { get; }
    }
}
