using System;
using System.Text.RegularExpressions;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using static System.Int32;

namespace CakeBot.Helper.Modules.Osu
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
                    throw new CakeException("``The top play has to be between 1 and 100``");
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
}
