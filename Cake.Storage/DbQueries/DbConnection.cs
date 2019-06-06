using System.Data.SqlClient;

namespace Cake.Storage.DbQueries
{
    public class DbConnection : IDbConnection
    {
        public string ConString { get; }

        public DbConnection(string conString = null)
        {
            if (conString == null)
            {
                conString = CakeJson.GetConfig().ConnectionString;
            }
            ConString = conString;
        }

        public SqlConnection ReturnConnection()
        {
            return new SqlConnection(ConString);
        }
    }
}
