using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class ArtikalRepository : IArtikalRepository
{
    private readonly DatabaseConnection _db;

    public ArtikalRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public Artikal? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT ArtikalID, SifraArtikla, NazivArtikla, KategorijaID, JedinicaMereID,
                     CenaKupovine, CenaProdaje, MinimalneStalje, MaksimalneStalje,
                     Tezina, Zapremina, Opis, Barkod, Aktivan, DatumKreiranja, DatumIzmene
              FROM Artikli WHERE ArtikalID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapArtikal(reader);

        return null;
    }

    public Artikal? GetBySifra(string sifra)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT ArtikalID, SifraArtikla, NazivArtikla, KategorijaID, JedinicaMereID,
                     CenaKupovine, CenaProdaje, MinimalneStalje, MaksimalneStalje,
                     Tezina, Zapremina, Opis, Barkod, Aktivan, DatumKreiranja, DatumIzmene
              FROM Artikli WHERE SifraArtikla = @Sifra", conn);
        cmd.Parameters.AddWithValue("@Sifra", sifra);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapArtikal(reader);

        return null;
    }

    public IEnumerable<Artikal> GetAll()
    {
        var list = new List<Artikal>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT ArtikalID, SifraArtikla, NazivArtikla, KategorijaID, JedinicaMereID,
                     CenaKupovine, CenaProdaje, MinimalneStalje, MaksimalneStalje,
                     Tezina, Zapremina, Opis, Barkod, Aktivan, DatumKreiranja, DatumIzmene
              FROM Artikli ORDER BY SifraArtikla", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapArtikal(reader));

        return list;
    }

    public IEnumerable<Artikal> GetActive()
    {
        var list = new List<Artikal>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT ArtikalID, SifraArtikla, NazivArtikla, KategorijaID, JedinicaMereID,
                     CenaKupovine, CenaProdaje, MinimalneStalje, MaksimalneStalje,
                     Tezina, Zapremina, Opis, Barkod, Aktivan, DatumKreiranja, DatumIzmene
              FROM Artikli WHERE Aktivan = 1 ORDER BY SifraArtikla", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapArtikal(reader));

        return list;
    }

    public void Add(Artikal artikal)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO Artikli (SifraArtikla, NazivArtikla, KategorijaID, JedinicaMereID,
                     CenaKupovine, CenaProdaje, MinimalneStalje, MaksimalneStalje,
                     Tezina, Zapremina, Opis, Barkod, Aktivan)
              VALUES (@Sifra, @Naziv, @KatId, @JmId, @CenaKup, @CenaProd, @MinSt, @MaxSt,
                      @Tezina, @Zapremina, @Opis, @Barkod, @Aktivan);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@Sifra", artikal.SifraArtikla);
        cmd.Parameters.AddWithValue("@Naziv", artikal.NazivArtikla);
        cmd.Parameters.AddWithValue("@KatId", artikal.KategorijaID);
        cmd.Parameters.AddWithValue("@JmId", artikal.JedinicaMereID);
        cmd.Parameters.AddWithValue("@CenaKup", (object?)artikal.CenaKupovine ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CenaProd", (object?)artikal.CenaProdaje ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MinSt", artikal.MinimalneStalje);
        cmd.Parameters.AddWithValue("@MaxSt", artikal.MaksimalneStalje);
        cmd.Parameters.AddWithValue("@Tezina", (object?)artikal.Tezina ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Zapremina", (object?)artikal.Zapremina ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Opis", (object?)artikal.Opis ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Barkod", (object?)artikal.Barkod ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Aktivan", artikal.Aktivan);

        artikal.ArtikalID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Artikal artikal)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE Artikli SET
                SifraArtikla = @Sifra, NazivArtikla = @Naziv,
                KategorijaID = @KatId, JedinicaMereID = @JmId,
                CenaKupovine = @CenaKup, CenaProdaje = @CenaProd,
                MinimalneStalje = @MinSt, MaksimalneStalje = @MaxSt,
                Tezina = @Tezina, Zapremina = @Zapremina,
                Opis = @Opis, Barkod = @Barkod, Aktivan = @Aktivan,
                DatumIzmene = GETDATE()
              WHERE ArtikalID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", artikal.ArtikalID);
        cmd.Parameters.AddWithValue("@Sifra", artikal.SifraArtikla);
        cmd.Parameters.AddWithValue("@Naziv", artikal.NazivArtikla);
        cmd.Parameters.AddWithValue("@KatId", artikal.KategorijaID);
        cmd.Parameters.AddWithValue("@JmId", artikal.JedinicaMereID);
        cmd.Parameters.AddWithValue("@CenaKup", (object?)artikal.CenaKupovine ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CenaProd", (object?)artikal.CenaProdaje ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MinSt", artikal.MinimalneStalje);
        cmd.Parameters.AddWithValue("@MaxSt", artikal.MaksimalneStalje);
        cmd.Parameters.AddWithValue("@Tezina", (object?)artikal.Tezina ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Zapremina", (object?)artikal.Zapremina ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Opis", (object?)artikal.Opis ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Barkod", (object?)artikal.Barkod ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Aktivan", artikal.Aktivan);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            "UPDATE Artikli SET Aktivan = 0, DatumIzmene = GETDATE() WHERE ArtikalID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    private static Artikal MapArtikal(SqlDataReader reader)
    {
        return new Artikal
        {
            ArtikalID = reader.GetInt32(0),
            SifraArtikla = reader.GetString(1),
            NazivArtikla = reader.GetString(2),
            KategorijaID = reader.GetInt32(3),
            JedinicaMereID = reader.GetInt32(4),
            CenaKupovine = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
            CenaProdaje = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
            MinimalneStalje = reader.IsDBNull(7) ? 10 : reader.GetInt32(7),
            MaksimalneStalje = reader.IsDBNull(8) ? 1000 : reader.GetInt32(8),
            Tezina = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
            Zapremina = reader.IsDBNull(10) ? null : reader.GetDecimal(10),
            Opis = reader.IsDBNull(11) ? null : reader.GetString(11),
            Barkod = reader.IsDBNull(12) ? null : reader.GetString(12),
            Aktivan = reader.GetBoolean(13),
            DatumKreiranja = reader.GetDateTime(14),
            DatumIzmene = reader.GetDateTime(15)
        };
    }
}
