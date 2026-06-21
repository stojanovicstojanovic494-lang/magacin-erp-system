using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class SkladisnaLokacijaRepository : ISkladisnaLokacijaRepository
{
    private readonly DatabaseConnection _db;

    public SkladisnaLokacijaRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public SkladisnaLokacija? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT LokacijaID, ZonaID, Red, Polica, Opis, JeLiSlobodna, Aktivna, DatumKreiranja, DatumIzmene
              FROM SkladisneLokacije WHERE LokacijaID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapLokacija(reader);

        return null;
    }

    public IEnumerable<SkladisnaLokacija> GetAll()
    {
        var list = new List<SkladisnaLokacija>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT LokacijaID, ZonaID, Red, Polica, Opis, JeLiSlobodna, Aktivna, DatumKreiranja, DatumIzmene
              FROM SkladisneLokacije WHERE Aktivna = 1 ORDER BY ZonaID, Red, Polica", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapLokacija(reader));

        return list;
    }

    public IEnumerable<SkladisnaLokacija> GetDostupne()
    {
        var list = new List<SkladisnaLokacija>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT LokacijaID, ZonaID, Red, Polica, Opis, JeLiSlobodna, Aktivna, DatumKreiranja, DatumIzmene
              FROM SkladisneLokacije WHERE Aktivna = 1 AND JeLiSlobodna = 1
              ORDER BY ZonaID, Red, Polica", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapLokacija(reader));

        return list;
    }

    public void Update(SkladisnaLokacija lokacija)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE SkladisneLokacije SET
                JeLiSlobodna = @Slobodna, Aktivna = @Aktivna, DatumIzmene = GETDATE()
              WHERE LokacijaID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", lokacija.LokacijaID);
        cmd.Parameters.AddWithValue("@Slobodna", lokacija.JeLiSlobodna);
        cmd.Parameters.AddWithValue("@Aktivna", lokacija.Aktivna);

        cmd.ExecuteNonQuery();
    }

    private static SkladisnaLokacija MapLokacija(SqlDataReader reader)
    {
        return new SkladisnaLokacija
        {
            LokacijaID = reader.GetInt32(0),
            ZonaID = reader.GetInt32(1),
            Red = reader.GetInt32(2),
            Polica = reader.GetInt32(3),
            Opis = reader.IsDBNull(4) ? null : reader.GetString(4),
            JeLiSlobodna = reader.GetBoolean(5),
            Aktivna = reader.GetBoolean(6),
            DatumKreiranja = reader.GetDateTime(7),
            DatumIzmene = reader.GetDateTime(8)
        };
    }
}
