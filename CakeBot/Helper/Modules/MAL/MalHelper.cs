using CakeBot.Helper.Modules.MAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeBot.Helper.Modules.MAL
{
    public class MalHelper
    {
        public static List<string> GenresToArray(Model.Genres[] genres)
        {
            List<string> genresList = new List<string>();
            foreach(var genre in genres)
            {
                genresList.Add(genre.name);
            }
            return genresList;
        }

        public static CakeEmbedBuilder AnimeToEmbed(Model.AnimeData anime, bool isSearch)
        {
            var embedBuilder = new CakeEmbedBuilder()
                    .WithAuthor(author =>
                    {
                        author
                            .WithName(anime.title)
                            .WithUrl(anime.url);
                    })
                    .WithThumbnailUrl(anime.image_url)
                    .WithDescription(
                    $"**Synopsis: **{ anime.synopsis }\n\n" +
                    $"**Score: **{ anime.score }\n" +
                    $"**Episodes: **{ anime.episodes }\n" +
                    $"**Type: **{ anime.type }\n" +
                    $"**Members: **{ anime.members }"
                    )
                    .WithFooter(footer =>
                    {
                        if (anime.r18) footer.WithIconUrl("https://www.docsquiffy.com/wp-content/uploads/2019/02/18-icon.png");
                        if (isSearch)
                        {
                            if (anime.airing) footer.WithText("Start date: " + DateTime.Parse(anime.start_date).ToShortDateString());
                            else footer.WithText("Aired: " + DateTime.Parse(anime.start_date).ToShortDateString() + $" to {DateTime.Parse(anime.end_date).ToShortDateString()}");
                        }
                        else
                        {
                            if (anime.airing_start == null) footer.WithText("Not yet aired");
                            else footer.WithText("Aired: " + DateTime.Parse(anime.airing_start).ToShortDateString() + $" | Source: {anime.source}");
                        }
                    }) as CakeEmbedBuilder;
            if (anime.genres != null) embedBuilder.Description += $"\n**Genres: **{ string.Join(",", GenresToArray(anime.genres)) }";
            return embedBuilder;
        }
        public static bool IsGoodScore(AnimeData anime) => anime.score < (decimal)5.5 ? true : false;

    }
}
