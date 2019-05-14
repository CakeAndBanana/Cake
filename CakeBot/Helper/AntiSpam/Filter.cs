using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Logging;
using Discord;
using Discord.Commands;

namespace CakeBot.Helper.AntiSpam
{
    public class Filter
    {
        public static async Task<bool> HandleMessage(CommandContext context)
        {
            var isLocked = IsAuthorLocked(context.Message.Author);

            if (isLocked)
            {
                return false; // User locked so we can delete the message
            }

            var isSpam = await IsMessageSpam(context.Message.Author, context.Message, context.Guild);

            if (isSpam)
            {
                // TODO check if raid
                // TODO handle raid
                return false; // This is spam we don't have to handle the message
            }

            return true; // Nothing wrong with the message
        }

        private static async Task<bool> IsMessageSpam(IUser author, IUserMessage message, IGuild guild)
        {
            if (author != null && message != null &&  guild != null)
            {
                /*//Check if user is admin 
                if (IsUserAdmin(author as IGuildUser))
                {
                    //Allow admins to spam
                    return false;
                }*/

                var timestamp = message.Timestamp;
                var track = await SpamTrackQueries.GetUserTrack(author.Id, guild.Id);
                var lastMessage = track.LastMessage;
                track.LastMessage = timestamp.ToUnixTimeMilliseconds();

                var interval = track.LastMessage - lastMessage;

                //Calculate the new presure
                track.Pressure -= SpamConfig.BasePressure * interval / (SpamConfig.PressureDecay * 1000.0f);
                track.Pressure += SpamConfig.BasePressure;

                var rawPressure = track.Pressure;

                //Reset the pressure to 0 if smaller then 0
                if (track.Pressure < 0)
                {
                    track.Pressure = 0;
                }

                await SpamTrackQueries.UpdateUserTrack(track);

                Logger.LogDebug("~~~ START TEST SPAM FILTER ~~~",
                    "User pressure  : " + track.Pressure,
                    "Raw pressure   : " + rawPressure,
                    "Pressure base  : " + SpamConfig.BasePressure,
                    "Pressure decay : " + SpamConfig.PressureDecay,
                    "Pressure max   : " + SpamConfig.MaxPressure,
                    "~~~ END TEST SPAM FILTER ~~~");

                if (track.Pressure > SpamConfig.MaxPressure)
                {
                    // TODO silence or kick the user
                    await LockUser(track, author);
                    return true;
                }

                return false;
            }

            return false;
        }

        private static bool IsAuthorLocked(IUser author)
        {
            //TODO check if the given user is locked

            return false;
        }

        private static async Task LockUser(SpamTrack track, IUser author)
        {
            //TODO lock user and save the lock inside a new table



            await ResetUserTrack(track);
        }

        private static async Task ResetUserTrack(SpamTrack track)
        {
            track.Pressure = 0;
            await SpamTrackQueries.UpdateUserTrack(track);
        }

        private static bool IsUserAdmin(IGuildUser user)
        {
            return user.GuildPermissions.Administrator;
        }
    }
}
