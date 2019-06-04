using System.Data;
using System.Data.SqlClient;

namespace Cake.Storage.DbQueries
{
    public class GuildQueries
    {
        private static SqlConnection Conn { get; set; }

        public GuildQueries(string conString = null)
        {
            Conn = new DbConnection(conString).ReturnConnection();
        }   

        public string GetPrefixGuild(ulong guildId)
        {
            Conn.Open();
            string prefix = null;
            using (var command = new SqlCommand($"SELECT GuildPrefix FROM Dbo.DiscordGuild WHERE GuildId={guildId}", Conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prefix = reader.GetValue(0).ToString();
                    }
                }
            }
            Conn.Close();
            return prefix;
        }
    }
}
