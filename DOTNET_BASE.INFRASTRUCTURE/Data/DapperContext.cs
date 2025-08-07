using Npgsql;
using System.Data;

namespace DOTNET_BASE.INFRASTRUCTURE.Data;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}