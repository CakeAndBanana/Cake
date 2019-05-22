using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.Background;
using CakeBot.Helper.Modules.Profile;
using Discord;
using Discord.WebSocket;

namespace CakeBot.Modules.Services
{
    public class ProfileService : CustomBaseService
    {

        public async Task GetProfile(SocketUser userTest = null)
        {
            var baseAddress = Directory.GetCurrentDirectory();
            try
            {
                var imageName = Module.Context.User.Id + ".png";
                var imageSaveLocation = baseAddress + @"\Images\" + imageName;
                Logger.LogDebug("Saving image at " + imageSaveLocation);
                var profile = new BaseProfile();
                Logger.LogDebug("Generating image " + imageName);
                if (userTest != null) { await profile.CreateForUser(userTest, imageSaveLocation); }
                else { await profile.CreateForUser(Module.Context.User, imageSaveLocation); }
                Logger.LogDebug("Sending image " + imageName);
                await Module.Context.Channel.SendFileAsync(imageSaveLocation);
                Logger.LogDebug("Deleting image " + imageName);
                File.Delete(imageSaveLocation);
            }
            catch (FileNotFoundException e) // TODO add proper error handling
            {
                Logger.LogError(e.Message);
            }
        }
        public async Task ChangeColor(string Hex)
        {
            try
            {
                if (!int.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, null, out int x))
                {
                    await SendMessageAsync($"{Module.Context.User.Mention}, incorrect format! Please use correct Hex. (Don't use #)");
                    return;
                }

                await SendMessageAsync(Module.Context.User.Mention + await UserQueries.SetProfileColor(Module.Context.User.Id, Hex));
            }
            catch (FileNotFoundException e) // TODO add proper error handling
            {
                Logger.LogError(e.Message);
            }
        }

        public async Task SetBackground(int bgId, ulong userId)
        {
            try
            {
                var result = await BackgroundQueries.SetUserBackground(bgId, userId);
                await SendMessageAsync(Module.Context.User.Mention + result);
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        public async Task BuyBackground(int bgId, ulong userId)
        {
            try
            {
                var result = await BackgroundQueries.BuyBackground(bgId, userId);
                await SendMessageAsync(Module.Context.User.Mention + result);
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        public async Task ListBackgrounds(ulong userId, string Category = "")
        {

            try
            {
                string categories = "**Categories:**\n";
                int index = 0;
                IEmote[] emojis = {new Emoji("◀"), new Emoji("✅"), new Emoji("🚫"),new Emoji("▶")  };

                var ownedBg = await BackgroundQueries.GetListOfOwnedBackgrounds(userId);
                var allBg = await BackgroundQueries.GetListOfAllBackgrounds();
                if (Category != "")
                {
                    allBg = allBg.Where(x => x.Category == Category.ToLower()).ToList();
                }
                else if (Category != "") allBg = allBg.Where(x => x.Category == Category).ToList();

                if (allBg.Count == 0)
                {
                    foreach(var category in await BackgroundQueries.GetListOfAllBackgrounds())
                    {
                        if(!categories.Contains(category.Category)) categories += $"`{category.Category}`  ";
                    }
                    await SendMessageAsync(Module.Context.User.Mention+ $", one or more categories you listed are not valid!" +
                        $"\n{categories}");
                    return;
                }
                string[] description = new string[allBg.Count];

                #region old
                foreach (var bg in allBg)
                {
                    string owned = "Not Owned";
                    if (ownedBg.Where(x => x.BackgroundId == bg.BackgroundId).FirstOrDefault() != null) owned = "Owned";
                    description[index] += $"{bg.BackgroundUrl}\n**[{bg.BackgroundId}]** **{bg.BackgroundDir}** - :euro: **{bg.BackgroundPrice}** (**{owned}**)";
                    index++;
                }
                #endregion
                var msg = await Module.Context.Channel.SendMessageAsync(description[0]);
                await msg.AddReactionsAsync(emojis);
                var trackUser = new PurchasingUser()
                {
                    Message = msg,
                    user = Module.Context.User,
                    CurrentBgIndex = 0,
                    BackgroundStrings = description,
                    Backgrounds = allBg,
                    currentBgId = 1
                };
                Global.UsersToTrack.Add(trackUser);
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}
