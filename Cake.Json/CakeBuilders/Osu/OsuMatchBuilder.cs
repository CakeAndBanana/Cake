using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Json;
using CakeBot.Helper.Modules.Osu.Model;

namespace CakeBot.Helper.Modules.Osu.Builder
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
            if (MatchId != null || MatchId != "")
            {
                urlBuilder.Append("&mp=");
                urlBuilder.Append(MatchId);
            }

            return urlBuilder.ToString();
        }
    }
}
