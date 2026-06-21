using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class ZaliheRepository : IZaliheRepository
{
    private readonly DatabaseConnection _db;

    public ZaliheRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public Zaliha? GetByArtikalAndLokacija(int artikalId, int lokacijaId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT ZalihaID, ArtikalID, LokacijaID, Kolicina, RezervovanoKolicina,
                     DisponibilnaKolicina, DatumZadnjeIzmene
              FROM Zalihe WHERE ArtikalID = @ArtikalId AND LokacijaID = @LokacijaId", conn);
        cmd.Parameters.AddWithValue("@ArtikalId", artikalId);
        cmd.Parameters.AddWithValue("@LokacijaId", lokacijaId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapZaliha(reader);

        return null;
    }

    public IEnumerable<Zaliha> GetByArtikalId(int artikalId)
    {
        var list = new List<Zaliha>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT ZalihaID, ArtikalID, LokacijaID, Kolicina, RezervovanoKolicina,
                     DisponibilnaKolicina, DatumZadnjeIzmene
              FROM Zalihe WHERE ArtikalID = @ArtikalId", conn);
        cmd.Parameters.AddWithValue("@ArtikalId", artikalId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapZaliha(reader));

        return list;
    }

    public IEnumerable<Zaliha> GetAll()
    {
        var list = new List<Zaliha>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT ZalihaID, ArtikalID, LokacijaID, Kolicina, RezervovanoKolicina,
                     DisponibilnaKolicina, DatumZadnjeIzmene
              FROM Zalihe ORDER BY ArtikalID", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapZaliha(reader));

        return list;
    }

    public void Add(Zaliha zaliha)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO Zalihe (ArtikalID, LokacijaID, Kolicina, RezervovanoKolicina, DisponibilnaKolicina)
              VALUES (@ArtikalId, @LokacijaId, @Kolicina, @Rezervovano, @Disponibilna);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@ArtikalId", zaliha.ArtikalID);
        cmd.Parameters.AddWithValue("@LokacijaId", zaliha.LokacijaID);
        cmd.Parameters.AddWithValue("@Kolicina", zaliha.Kolicina);
        cmd.Parameters.AddWithValue("@Rezervovano", zaliha.RezervovanoKolicina);
        cmd.Parameters.AddWithValue("@Disponibilna", zaliha.DisponibilnaKolicina);

        zaliha.ZalihaID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Zaliha zaliha)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE Zalihe SET
                Kolicina = @Kolicina,
                RezervovanoKolicina = @Rezervovano,
                DisponibilnaKolicina = @Disponibilna,
                DatumZadnjeIzmene = GETDATE()
              WHERE ZalihaID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", zaliha.ZalihaID);
        cmd.Parameters.AddWithValue("@Kolicina", zaliha.Kolicina);
        cmd.Parameters.AddWithValue("@Rezervovano", zaliha.RezervovanoKolicina);
        cmd.Parameters.AddWithValue("@Disponibilna", zaliha.DisponibilnaKolicina);

        cmd.ExecuteNonQuery();
    }

    private static Zaliha MapZaliha(SqlDataReader reader)
    {
        return new Zaliha
        {
            ZalihaID = reader.GetInt32(0),
            ArtikalID = reader.GetInt32(1),
            LokacijaID = reader.GetInt32(2),
            Kolicina = reader.GetInt32(3),
            RezervovanoKolicina = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
            DisponibilnaKolicina = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
            DatumZadnjeIzmene = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6)
        };
    }
}
