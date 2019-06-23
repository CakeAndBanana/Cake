using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Json.CakeModels.Osu;

namespace Cake.Json.CakeBuilders.Osu
{
    public class OsuMatchBuilder : OsuJsonBaseBuilder<OsuJsonMatch>
    {
        public string MatchId; // mp

        public List<OsuJsonMatch> Execute()
        {
            var osuJsonMatchArray = ExecuteJson(OsuApiRequest.Multiplayer);
            //osuJsonMatchArray = ProcessJson(osuJsonMatchArray);
            return osuJsonMatchArray.ToList();
        }

        private OsuJsonMatch[] ProcessJson(OsuJsonMatch[] array)
        {
            foreach (var item in array)
            {
                //TODO add processing
            }

            return array;
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (!string.IsNullOrEmpty(MatchId))
            {
                urlBuilder.Append("&mp=");
                urlBuilder.Append(MatchId);
            }

            return urlBuilder.ToString();
        }
    }
}
