using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cake.Core.Discord.Services;
using Discord.Commands;
using static System.Int32;

namespace Cake.Core.Discord.Modules
{
    public class OsuArg
    {
        private readonly bool _userUsername;
        private readonly bool _recent;
        private readonly int? _play;
        private readonly string _userId;

        public OsuArg(string arg)
        {
            if (arg.Contains("-id"))
            {
                _userId = Regex.Match(arg, @"\d+").Value;
                _userUsername = false;
            }
            else
            {
                _userId = arg;
                _userUsername = true;
            }
        }

        public OsuArg(string arg, bool best)
        {
            if (arg.Contains("-r"))
            {
                _recent = true;
            }
            else if (arg.Contains("-p"))
            {
                _play = Parse(Regex.Match(arg, @"\d+").Value);
                if (_play > 100 || _play == 0)
                {
                    throw new Exception("``Play number has to be between 1 and 100``");
                }
            }
            else if (arg.Contains("-id"))
            {
                _userId = Regex.Match(arg, @"\d+").Value;
                _userUsername = false;
            }
            else
            {
                _userId = arg;
                _userUsername = true;
            }
        }

        public bool UseUsername()
        {
            return _userUsername;
        }

        public bool IsRecent()
        {
            return _recent;
        }

        public int? GetPlayNumber()
        {
            return _play;
        }

        public string GetUserId()
        {
            return _userId;
        }
    }

    [Name("osu!")]
    [Group("osu")]
    [Alias("o")]
    public class OsuModule : CustomBaseModule
    {
        private readonly OsuService _service;

        public OsuModule(OsuService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("set")]
        [Summary("osu set (?username)")]
        [Alias("s")]
        [Remarks("Binds your osu account to your discord user.")]
        public async Task SetAccount([Remainder]string userName)
        {
            await _service.SetAccount(userName);
        }

        [Command("profile")]
        [Summary("osu profile (?user)")]
        [Alias("u", "p")]
        [Remarks("Gets profile of current user or given user.")]
        public async Task GetProfile([Remainder]string args = "")
        {
            OsuArg osuDiscordArg = null;
            try
            {
                osuDiscordArg = new OsuArg(args);
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }
            await _service.GetUserProfile(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername());
        }

        [Alias("b")]
        [Command("best")]
        [Summary(">osu best (mode) (username)")]
        [Remarks("Returns someones best plays.")]
        public async Task GetUserBest([Remainder] string arg = "")
        {
            OsuArg osuDiscordArg = null;
            try
            {
                osuDiscordArg = new OsuArg(arg, true);
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
            }

            if (osuDiscordArg != null)
            {
                await _service.GetUserBest(osuDiscordArg.GetUserId(), osuDiscordArg.UseUsername(), osuDiscordArg.IsRecent(), osuDiscordArg.GetPlayNumber());
            }
        }
    }
}
