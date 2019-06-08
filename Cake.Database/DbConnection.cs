using System.Data.SqlClient;
using Cake.Storage;

namespace Cake.Database
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
