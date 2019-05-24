using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Exceptions;

namespace CakeBot.Helper.Database.Queries
{
    public class BugQueries
    {
        private static CakeEntities _db = new CakeEntities();

        public static async Task<BugReport> CreateNewBugReport(string message, ulong userid = 0, ulong guildid = 0)
        {
            _db = new CakeEntities();

            if (message != null || guildid == 0 || userid == 0)
            {
                throw new CakeException("Wrong data, check error");
            }

            var guild = await GuildQueries.FindGuild(guildid);
            var user = await UserQueries.FindUser(userid);

            if (user != null || guild != null)
            {
                throw new CakeException("User or Guild not found, check error");
            }

            var report = new BugReport {Message = message, GuildId = guild.GuildId, UserId = user.UserId, Completed = false};

            _db.BugReports.Add(report);
            await _db.SaveChangesAsync();

            return report;
        }

        public static async Task<List<BugReport>> ListBugReports()
        {
            _db = new CakeEntities();
            var result =
                await (from reports in _db.BugReports
                    where reports.Completed == false
                    select reports).ToListAsync();
            return result;
        }

        public static async Task<BugReport> ChangeStatusBugReport(int reportId, bool status)
        {
            _db = new CakeEntities();
            var result =
                await (from reports in _db.BugReports
                    where reports.Id == reportId
                       select reports).FirstOrDefaultAsync();
            if (result == null) throw new CakeException("Bug Report not found!");
            result.Completed = true;
            _db.Entry(result).State = EntityState.Modified;
            return result;
        }
    }
}
