using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Cake.Storage.DbQueries
{
    interface IDbConnection
    {
        string ConString { get; }
        SqlConnection ReturnConnection();
    }
}
