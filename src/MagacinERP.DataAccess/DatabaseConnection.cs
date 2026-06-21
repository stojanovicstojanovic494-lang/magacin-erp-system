using Microsoft.Data.SqlClient;

namespace MagacinERP.DataAccess;

public class DatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public static string DefaultConnectionString =>
        "Server=localhost;Database=MagacinERP;Trusted_Connection=True;TrustServerCertificate=True;";
}
