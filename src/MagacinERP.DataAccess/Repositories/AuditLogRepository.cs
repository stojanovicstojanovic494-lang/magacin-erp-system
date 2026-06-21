using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly DatabaseConnection _db;

    public AuditLogRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public void Add(AuditLog log)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, StaraVrednost, NovaVrednost, Korisnik)
              VALUES (@Tabela, @Akcija, @PK, @Stara, @Nova, @Korisnik)", conn);

        cmd.Parameters.AddWithValue("@Tabela", log.Tabela);
        cmd.Parameters.AddWithValue("@Akcija", (object?)log.Akcija ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PK", (object?)log.PrimarniKljuc ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Stara", (object?)log.StaraVrednost ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Nova", (object?)log.NovaVrednost ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Korisnik", (object?)log.Korisnik ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public IEnumerable<AuditLog> GetByDateRange(DateTime from, DateTime to)
    {
        var list = new List<AuditLog>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT AuditID, Tabela, Akcija, PrimarniKljuc, StaraVrednost, NovaVrednost, Korisnik, DatumAkcije
              FROM AuditLog WHERE DatumAkcije BETWEEN @Od AND @Do
              ORDER BY DatumAkcije DESC", conn);
        cmd.Parameters.AddWithValue("@Od", from);
        cmd.Parameters.AddWithValue("@Do", to);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapAuditLog(reader));

        return list;
    }

    public IEnumerable<AuditLog> GetRecent(int days)
    {
        var list = new List<AuditLog>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT TOP 500 AuditID, Tabela, Akcija, PrimarniKljuc, StaraVrednost, NovaVrednost, Korisnik, DatumAkcije
              FROM AuditLog WHERE DatumAkcije >= DATEADD(DAY, -@Dana, GETDATE())
              ORDER BY DatumAkcije DESC", conn);
        cmd.Parameters.AddWithValue("@Dana", days);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapAuditLog(reader));

        return list;
    }

    private static AuditLog MapAuditLog(SqlDataReader reader)
    {
        return new AuditLog
        {
            AuditID = reader.GetInt32(0),
            Tabela = reader.GetString(1),
            Akcija = reader.IsDBNull(2) ? null : reader.GetString(2),
            PrimarniKljuc = reader.IsDBNull(3) ? null : reader.GetString(3),
            StaraVrednost = reader.IsDBNull(4) ? null : reader.GetString(4),
            NovaVrednost = reader.IsDBNull(5) ? null : reader.GetString(5),
            Korisnik = reader.IsDBNull(6) ? null : reader.GetString(6),
            DatumAkcije = reader.GetDateTime(7)
        };
    }
}
