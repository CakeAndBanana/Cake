using Discord;

namespace CakeBot.Helper
{
    public class CakeEmbedBuilder : EmbedBuilder
    {
        private const string DefaultThumbnailUrl = "https://Cake.s-ul.eu/lbofRWcs";

        private static readonly Color NoneColor = new Color(80, 0, 80);
        private static readonly Color ErrorColor = new Color(128, 0, 0);
        private static readonly Color SuccessColor = new Color(0, 153, 51);
        private static readonly Color DebugColor = new Color(0, 102, 255);
        private static readonly Color InfoColor = new Color(255, 255, 255);
        private static readonly Color WarningColor = new Color(255, 153, 0);

        public EmbedType EmbedType;

        public CakeEmbedBuilder()
        {
            Init();
        }

        public CakeEmbedBuilder(EmbedType type)
        {
            Init(type);
        }

        private void Init(EmbedType type = EmbedType.None)
        {
            EmbedType = type;
            WithThumbnailUrl(DefaultThumbnailUrl);
            Color color;
            switch(type)
            {
                case EmbedType.None:
                    color = NoneColor;
                    break;
                case EmbedType.Error:
                    color = ErrorColor;
                    break;
                case EmbedType.Success:
                    color = SuccessColor;
                    break;
                case EmbedType.Debug:
                    color = DebugColor;
                    break;
                case EmbedType.Info:
                    color = InfoColor;
                    break;
                case EmbedType.Warning:
                    color = WarningColor;
                    break;
                default:
                    color = NoneColor;
                    break;
            }
            WithColor(color);
        }
    }

    public enum EmbedType
    {
        None,
        Error,
        Success,
        Info,
        Debug,
        Warning
    }
}
