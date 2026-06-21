using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class IzdavanjeRepository : IIzdavanjeRepository
{
    private readonly DatabaseConnection _db;

    public IzdavanjeRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public IzdavanjeMaterijala? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT IzdavanjeID, BrojDokumenta, DatumIzdavanja, TipIzdavanja, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM IzdavanjeMaterijala WHERE IzdavanjeID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var izdavanje = MapIzdavanje(reader);
            reader.Close();
            izdavanje.Stavke = GetStavke(conn, izdavanje.IzdavanjeID);
            return izdavanje;
        }

        return null;
    }

    public IEnumerable<IzdavanjeMaterijala> GetAll()
    {
        var list = new List<IzdavanjeMaterijala>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT IzdavanjeID, BrojDokumenta, DatumIzdavanja, TipIzdavanja, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM IzdavanjeMaterijala ORDER BY DatumIzdavanja DESC", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapIzdavanje(reader));

        return list;
    }

    public IEnumerable<IzdavanjeMaterijala> GetByStatus(string status)
    {
        var list = new List<IzdavanjeMaterijala>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT IzdavanjeID, BrojDokumenta, DatumIzdavanja, TipIzdavanja, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM IzdavanjeMaterijala WHERE Status = @Status ORDER BY DatumIzdavanja DESC", conn);
        cmd.Parameters.AddWithValue("@Status", status);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapIzdavanje(reader));

        return list;
    }

    public void Add(IzdavanjeMaterijala izdavanje)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO IzdavanjeMaterijala (BrojDokumenta, TipIzdavanja, Napomena, Status, Korisnik)
              VALUES (@Broj, @Tip, @Napomena, @Status, @Korisnik);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@Broj", izdavanje.BrojDokumenta);
        cmd.Parameters.AddWithValue("@Tip", (object?)izdavanje.TipIzdavanja ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Napomena", (object?)izdavanje.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", izdavanje.Status);
        cmd.Parameters.AddWithValue("@Korisnik", (object?)izdavanje.Korisnik ?? DBNull.Value);

        izdavanje.IzdavanjeID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(IzdavanjeMaterijala izdavanje)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE IzdavanjeMaterijala SET
                Status = @Status, Napomena = @Napomena,
                DatumZavrsenja = @DatumZavrsenja, DatumIzmene = GETDATE()
              WHERE IzdavanjeID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", izdavanje.IzdavanjeID);
        cmd.Parameters.AddWithValue("@Status", izdavanje.Status);
        cmd.Parameters.AddWithValue("@Napomena", (object?)izdavanje.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DatumZavrsenja", (object?)izdavanje.DatumZavrsenja ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public void AddStavka(IzdavanjeStavka stavka)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO IzdavanjeStavke (IzdavanjeID, ArtikalID, KolicinaTrazena, KolicinaIzdata, CenaJedinice, LotID)
              VALUES (@IzdavanjeId, @ArtikalId, @KolTrazena, @KolIzdata, @Cena, @LotId);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@IzdavanjeId", stavka.IzdavanjeID);
        cmd.Parameters.AddWithValue("@ArtikalId", stavka.ArtikalID);
        cmd.Parameters.AddWithValue("@KolTrazena", stavka.KolicinaTrazena);
        cmd.Parameters.AddWithValue("@KolIzdata", stavka.KolicinaIzdata);
        cmd.Parameters.AddWithValue("@Cena", (object?)stavka.CenaJedinice ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@LotId", (object?)stavka.LotID ?? DBNull.Value);

        stavka.IzdavanjeStavkaID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    private List<IzdavanjeStavka> GetStavke(SqlConnection conn, int izdavanjeId)
    {
        var list = new List<IzdavanjeStavka>();
        using var cmd = new SqlCommand(
            @"SELECT IzdavanjeStavkaID, IzdavanjeID, ArtikalID, KolicinaTrazena,
                     KolicinaIzdata, CenaJedinice, Napomena, LotID, DatumKreiranja
              FROM IzdavanjeStavke WHERE IzdavanjeID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", izdavanjeId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new IzdavanjeStavka
            {
                IzdavanjeStavkaID = reader.GetInt32(0),
                IzdavanjeID = reader.GetInt32(1),
                ArtikalID = reader.GetInt32(2),
                KolicinaTrazena = reader.GetInt32(3),
                KolicinaIzdata = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                CenaJedinice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                Napomena = reader.IsDBNull(6) ? null : reader.GetString(6),
                LotID = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                DatumKreiranja = reader.GetDateTime(8)
            });
        }

        return list;
    }

    private static IzdavanjeMaterijala MapIzdavanje(SqlDataReader reader)
    {
        return new IzdavanjeMaterijala
        {
            IzdavanjeID = reader.GetInt32(0),
            BrojDokumenta = reader.GetString(1),
            DatumIzdavanja = reader.GetDateTime(2),
            TipIzdavanja = reader.IsDBNull(3) ? null : reader.GetString(3),
            Napomena = reader.IsDBNull(4) ? null : reader.GetString(4),
            Status = reader.GetString(5),
            Korisnik = reader.IsDBNull(6) ? null : reader.GetString(6),
            DatumZavrsenja = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
            DatumKreiranja = reader.GetDateTime(8),
            DatumIzmene = reader.GetDateTime(9)
        };
    }
}
