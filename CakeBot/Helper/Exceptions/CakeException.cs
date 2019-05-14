using System;
using CakeBot.Helper.Logging;
using Discord.Commands;

namespace CakeBot.Helper.Exceptions
{
    public class CakeException : Exception
    {
        private const string DefaultErrorMessage = "Error!";

        private CakeEmbedBuilder _embedError;

        public CakeException()
        {
            Init();
        }

        public CakeException(string message) : base(message) => Init(message);

        public CakeException(string message, Exception innerException) : base(message, innerException) => Init(message);

        private void Init() => Init(DefaultErrorMessage);

        public CakeEmbedBuilder GetEmbededError() => _embedError;

        private void Init(string message)
        {
            Logger.LogMessage(new LoggingMessage($"Error occurred! ({message})", LoggingType.Error));

            _embedError = new CakeEmbedBuilder(EmbedType.Error);
            _embedError.AddField("Error", message);
        }
    }
}
