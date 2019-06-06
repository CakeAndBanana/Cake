using System.Data.SqlClient;


namespace Cake.Storage.DbQueries
{
    interface IDbConnection
    {
        string ConString { get; }
        SqlConnection ReturnConnection();
    }
}
