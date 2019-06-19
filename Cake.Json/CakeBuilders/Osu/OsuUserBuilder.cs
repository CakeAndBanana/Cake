using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Cake.Json;
using CakeBot.Helper.Modules.Osu.Model;

namespace CakeBot.Helper.Modules.Osu.Builder
{
    public class OsuUserBuilder : OsuJsonBaseBuilder<OsuJsonUser>
    {
        private static readonly NumberFormatInfo Nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = ".", CurrencySymbol = "" };

        public string UserId; // u
        public string Mode; // m
        public string Type; // type
        public string EventDays; // event_days

        public OsuJsonUser Execute()
        {
            var userArray = ExecuteJson(OsuApiRequest.User);
            var user = ProcessJson(userArray);
            return user;
        }

        public OsuJsonUser ProcessJson(OsuJsonUser[] userArray)
        {
            foreach (var item in userArray)
            {
                item.url = OsuUtil.OsuUserUrl + item.user_id;
                item.image = OsuUtil.GetOsuUserPictureUrl(item.user_id);
                item.flag = OsuUtil.OsuFlagUrl + item.country + ".png";
                item.flag_old = OsuUtil.OsuOldFlagUrl + item.country + ".gif";
                item.osutrack = OsuUtil.OsuTrack + item.username;
                item.osustats = OsuUtil.OsuStats + item.username;
                item.osuskills = OsuUtil.OsuSkills + item.username;
                item.osuchan = OsuUtil.OsuChan + item.user_id + $"/?m={Mode}";
                item.spectateUser = OsuUtil.OsuSpectate + item.user_id;

                item.playcount = Convert.ToDecimal(item.playcount).ToString("C0", Nfi);
                item.ranked_score = Convert.ToDecimal(item.ranked_score).ToString("C0", Nfi);
                item.total_score = Convert.ToDecimal(item.total_score).ToString("C0", Nfi);
                item.accuracy = Math.Round(Convert.ToDouble(item.accuracy.Replace(",", ".")), 2).ToString(CultureInfo.InvariantCulture);
                item.level = Math.Round(Convert.ToDouble(item.level.Replace(",", ".")), 0).ToString(CultureInfo.InvariantCulture);
            }

            var userList = userArray.ToList();

            return userList.FirstOrDefault();
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (UserId != null || UserId != "")
            {
                urlBuilder.Append("&u=");
                urlBuilder.Append(UserId);
            }

            if (Mode != null || Mode != "")
            {
                urlBuilder.Append("&m=");
                urlBuilder.Append(Mode);
            }

            if (Type != null || Type != "")
            {
                urlBuilder.Append("&mods=");
                urlBuilder.Append(Type);
            }

            if (EventDays != null || EventDays != "")
            {
                urlBuilder.Append("event_days");
                urlBuilder.Append(EventDays);
            }

            return urlBuilder.ToString();
        }
    }
}
