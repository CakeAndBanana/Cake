using System;
using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Logging;

namespace Cake.Core.Exceptions
{
    public class CakeException : Exception
    {
        private const string DefaultErrorMessage = "Error!";
        private readonly Logger _logger = Logger.Get() as Logger;

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
            _logger.Log(Logging.Type.Error, message);

            _embedError = new CakeEmbedBuilder(EmbedType.Error);
            _embedError.AddField("Error", message);
        }
    }
}