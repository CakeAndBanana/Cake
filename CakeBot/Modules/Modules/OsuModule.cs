using System.Threading.Tasks;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Modules.Osu;
using CakeBot.Modules.Services;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Alias("o")]
    [Group("osu")]
    [Name("osu!")]
    public class OsuModule : CustomBaseModule
    {
        private readonly OsuService _service;
        private static int? _mapId;

        public static int? GetMapId()
        {
            return _mapId;
        }

        public static void SetMapId(int? value)
        {
            _mapId = value;
        }

        public OsuModule(OsuService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("set")]
        [Remarks(">osu set (username)")]
        [Summary("Binds your osu username to your discord name.")]
        [Alias("s")]
        public async Task SetUser(string userId, [Remainder] string args = "")
        {
            if (args == "id")
            {
                await _service.SetUserWithId(userId);
            }
            else
            {
                await _service.SetUser(userId);
            }
        }

        [Alias("m")]
        [Command("mode")]
        [Summary(">osu mode (0-3)")]
        [Remarks("Changes the mode of your username.")]
        public async Task SetMode(string mode)
        {
            var modeNumber = (int) OsuMode.GetOsuMode(mode);
            await _service.SetMode(modeNumber);
        }

        [Alias("u")]
        [Command("user")]
        [Summary(">osu user (username)")]
        [Remarks("Returns userdata of a user.")]
        public async Task User([Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;
            try
            {
                osuDiscordArg = new OsuArg(arg);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(embed: e.GetEmbededError().Build());
            }
            await _service.GetUser(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());
        }

        [Alias("b")]
        [Command("best")]
        [Summary(">osu best (mode) (username)")]
        [Remarks("Returns someones best plays.")]
        public async Task UserBest([Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;
            try
            {
                osuDiscordArg = new OsuArg(arg,true);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }

            if (osuDiscordArg != null)
            {
                await _service.GetUserBest(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername(), osuDiscordArg.IsRecent(), osuDiscordArg.GetPlayNumber());
            }
        }

        [Alias("r")]
        [Command("recent")]
        public async Task RecentNoNumber([Remainder] string arg = "")
        {
            var osuDiscordArg = new OsuArg(arg);
            await _service.GetUserRecent(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());
            if (GetMapId() != null)
            {
                await Helper.Database.Queries.ChannelQueries.InsertMapId(Context.Channel.Id, Context.Guild.Id, (int)GetMapId());
            }
        }

        [Alias("r")]
        [Command("recent")]
        [Summary(">osu recent (amount) (username)")]
        [Remarks("Returns someones recently played maps")]
        public async Task Recent(int n = 1, [Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;
            try
            {
                osuDiscordArg = new OsuArg(arg);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(embed: e.GetEmbededError().Build());
            }
            await _service.GetUserRecent(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername(), n);
            if (GetMapId() != null)
            {
                await Helper.Database.Queries.ChannelQueries.InsertMapId(Context.Channel.Id, Context.Guild.Id, (int)GetMapId());
            }
        }

        [Alias("c")]
        [Command("compare")]
        [Summary(">osu compare")]
        [Remarks("Compare your score on a recently played map")]
        public async Task Compare([Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;
            try
            {
                osuDiscordArg = new OsuArg(arg);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(embed: e.GetEmbededError().Build());
            }
            await _service.GetCompare(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());
        }

        [Alias("t")]
        [Command("track")]
        [Summary(">osu track (add/remove) (user)")]
        [Remarks("Tracks a player with recent scores and top pp plays")]
        public async Task SetTrack([Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;
            try
            {
                osuDiscordArg = new OsuArg(arg);
            }
            catch (CakeException e)
            {
                await Context.Channel.SendMessageAsync(embed: e.GetEmbededError().Build());
            }
            await _service.SetTrack(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());
        }
    }
}