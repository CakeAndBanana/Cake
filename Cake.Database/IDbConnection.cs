using System.Data.SqlClient;

namespace Cake.Database
{
    interface IDbConnection
    {
        string ConString { get; }
        SqlConnection ReturnConnection();
    }
}
