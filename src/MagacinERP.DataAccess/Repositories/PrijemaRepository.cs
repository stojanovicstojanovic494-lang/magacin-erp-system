using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class PrijemaRepository : IPrijemaRepository
{
    private readonly DatabaseConnection _db;

    public PrijemaRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public PrijemaMaterijala? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT PrijemaID, BrojDokumenta, DatumPrijeme, DobavljacID, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM PrijemaMaterijala WHERE PrijemaID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var prijema = MapPrijema(reader);
            reader.Close();
            prijema.Stavke = GetStavke(conn, prijema.PrijemaID);
            return prijema;
        }

        return null;
    }

    public IEnumerable<PrijemaMaterijala> GetAll()
    {
        var list = new List<PrijemaMaterijala>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT PrijemaID, BrojDokumenta, DatumPrijeme, DobavljacID, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM PrijemaMaterijala ORDER BY DatumPrijeme DESC", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapPrijema(reader));

        return list;
    }

    public IEnumerable<PrijemaMaterijala> GetByStatus(string status)
    {
        var list = new List<PrijemaMaterijala>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT PrijemaID, BrojDokumenta, DatumPrijeme, DobavljacID, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM PrijemaMaterijala WHERE Status = @Status ORDER BY DatumPrijeme DESC", conn);
        cmd.Parameters.AddWithValue("@Status", status);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapPrijema(reader));

        return list;
    }

    public void Add(PrijemaMaterijala prijema)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO PrijemaMaterijala (BrojDokumenta, DobavljacID, Napomena, Status, Korisnik)
              VALUES (@Broj, @DobavljacId, @Napomena, @Status, @Korisnik);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@Broj", prijema.BrojDokumenta);
        cmd.Parameters.AddWithValue("@DobavljacId", prijema.DobavljacID);
        cmd.Parameters.AddWithValue("@Napomena", (object?)prijema.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", prijema.Status);
        cmd.Parameters.AddWithValue("@Korisnik", (object?)prijema.Korisnik ?? DBNull.Value);

        prijema.PrijemaID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(PrijemaMaterijala prijema)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE PrijemaMaterijala SET
                Status = @Status, Napomena = @Napomena,
                DatumZavrsenja = @DatumZavrsenja, DatumIzmene = GETDATE()
              WHERE PrijemaID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", prijema.PrijemaID);
        cmd.Parameters.AddWithValue("@Status", prijema.Status);
        cmd.Parameters.AddWithValue("@Napomena", (object?)prijema.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DatumZavrsenja", (object?)prijema.DatumZavrsenja ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public void AddStavka(PrijemaStavka stavka)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO PrijemaStavke (PrijemaID, ArtikalID, KolicinaNarudjena, KolicinaPrimljena, CenaJedinice, LotBroj, DatumRoka)
              VALUES (@PrijemaId, @ArtikalId, @KolNarudjena, @KolPrimljena, @Cena, @Lot, @DatumRoka);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@PrijemaId", stavka.PrijemaID);
        cmd.Parameters.AddWithValue("@ArtikalId", stavka.ArtikalID);
        cmd.Parameters.AddWithValue("@KolNarudjena", stavka.KolicinaNarudjena);
        cmd.Parameters.AddWithValue("@KolPrimljena", stavka.KolicinaPrimljena);
        cmd.Parameters.AddWithValue("@Cena", (object?)stavka.CenaJedinice ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Lot", (object?)stavka.LotBroj ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DatumRoka", (object?)stavka.DatumRoka ?? DBNull.Value);

        stavka.PrijemaStavkaID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    private List<PrijemaStavka> GetStavke(SqlConnection conn, int prijemaId)
    {
        var list = new List<PrijemaStavka>();
        using var cmd = new SqlCommand(
            @"SELECT PrijemaStavkaID, PrijemaID, ArtikalID, KolicinaNarudjena,
                     KolicinaPrimljena, CenaJedinice, Napomena, LotBroj, DatumRoka, DatumKreiranja
              FROM PrijemaStavke WHERE PrijemaID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", prijemaId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new PrijemaStavka
            {
                PrijemaStavkaID = reader.GetInt32(0),
                PrijemaID = reader.GetInt32(1),
                ArtikalID = reader.GetInt32(2),
                KolicinaNarudjena = reader.GetInt32(3),
                KolicinaPrimljena = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                CenaJedinice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                Napomena = reader.IsDBNull(6) ? null : reader.GetString(6),
                LotBroj = reader.IsDBNull(7) ? null : reader.GetString(7),
                DatumRoka = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                DatumKreiranja = reader.GetDateTime(9)
            });
        }

        return list;
    }

    private static PrijemaMaterijala MapPrijema(SqlDataReader reader)
    {
        return new PrijemaMaterijala
        {
            PrijemaID = reader.GetInt32(0),
            BrojDokumenta = reader.GetString(1),
            DatumPrijeme = reader.GetDateTime(2),
            DobavljacID = reader.GetInt32(3),
            Napomena = reader.IsDBNull(4) ? null : reader.GetString(4),
            Status = reader.GetString(5),
            Korisnik = reader.IsDBNull(6) ? null : reader.GetString(6),
            DatumZavrsenja = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
            DatumKreiranja = reader.GetDateTime(8),
            DatumIzmene = reader.GetDateTime(9)
        };
    }
}
