using System.Collections.Generic;
using CakeBot.Helper.Database.Model;
using Discord;

namespace CakeBot.Helper.Modules.Background
{
    public class PurchasingUser
    {
        public IUser user { get; set; }
        public IUserMessage Message { get; set; }
        public int CurrentBgIndex { get; set; }
        public List<ProfileBackground> Backgrounds { get; set; } = new List<ProfileBackground>();
        public string[] BackgroundStrings { get; set; }
        public int currentBgId { get; set; }
    }
}
