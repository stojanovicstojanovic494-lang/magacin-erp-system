using System.Data;
using System.Data.SqlClient;

namespace MagacinERP.Helpers;

public static class DatabaseHelper
{
    private static string _connectionString =
        "Server=localhost;Database=MagacinERP;Trusted_Connection=True;";

    public static string ConnectionString
    {
        get => _connectionString;
        set => _connectionString = value;
    }

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public static (int korisnikId, string? uloga) ValidacijaKorisnika(
        string korisnickoIme, string lozinka)
    {
        using var connection = GetConnection();
        using var command = new SqlCommand("sp_ValidacijaKorisnika", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Korisnicko_Ime", korisnickoIme);
        command.Parameters.AddWithValue("@Lozinka", lozinka);

        var korisnikIdParam = new SqlParameter("@KorisnikID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(korisnikIdParam);

        var ulogaParam = new SqlParameter("@Uloga", SqlDbType.NVarChar, 50)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(ulogaParam);

        connection.Open();
        command.ExecuteNonQuery();

        int korisnikId = (int)korisnikIdParam.Value;
        string? uloga = ulogaParam.Value == DBNull.Value
            ? null
            : (string)ulogaParam.Value;

        return (korisnikId, uloga);
    }
}
