using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class InventuraRepository : IInventuraRepository
{
    private readonly DatabaseConnection _db;

    public InventuraRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public Inventura? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT InventuraID, BrojDokumenta, DatumInventure, TipInventure, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM Inventure WHERE InventuraID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var inventura = MapInventura(reader);
            reader.Close();
            inventura.Stavke = GetStavke(conn, inventura.InventuraID);
            return inventura;
        }

        return null;
    }

    public IEnumerable<Inventura> GetAll()
    {
        var list = new List<Inventura>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT InventuraID, BrojDokumenta, DatumInventure, TipInventure, Napomena,
                     Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM Inventure ORDER BY DatumInventure DESC", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapInventura(reader));

        return list;
    }

    public void Add(Inventura inventura)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO Inventure (BrojDokumenta, TipInventure, Napomena, Status, Korisnik)
              VALUES (@Broj, @Tip, @Napomena, @Status, @Korisnik);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@Broj", inventura.BrojDokumenta);
        cmd.Parameters.AddWithValue("@Tip", (object?)inventura.TipInventure ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Napomena", (object?)inventura.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", inventura.Status);
        cmd.Parameters.AddWithValue("@Korisnik", (object?)inventura.Korisnik ?? DBNull.Value);

        inventura.InventuraID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Inventura inventura)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE Inventure SET
                Status = @Status, Napomena = @Napomena,
                DatumZavrsenja = @DatumZavrsenja, DatumIzmene = GETDATE()
              WHERE InventuraID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", inventura.InventuraID);
        cmd.Parameters.AddWithValue("@Status", inventura.Status);
        cmd.Parameters.AddWithValue("@Napomena", (object?)inventura.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DatumZavrsenja", (object?)inventura.DatumZavrsenja ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public void AddStavka(InventuraStavka stavka)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO InventureStavke (InventuraID, LokacijaID, ArtikalID, UtvrdjenaKolicina, SistemskaBaza, Razlika, Napomena)
              VALUES (@InvId, @LokId, @ArtId, @Utvrdjena, @Sistemska, @Razlika, @Napomena);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@InvId", stavka.InventuraID);
        cmd.Parameters.AddWithValue("@LokId", stavka.LokacijaID);
        cmd.Parameters.AddWithValue("@ArtId", stavka.ArtikalID);
        cmd.Parameters.AddWithValue("@Utvrdjena", stavka.UtvrdjenaKolicina);
        cmd.Parameters.AddWithValue("@Sistemska", (object?)stavka.SistemskaBaza ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Razlika", (object?)stavka.Razlika ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Napomena", (object?)stavka.Napomena ?? DBNull.Value);

        stavka.InventuraStavkaID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    private List<InventuraStavka> GetStavke(SqlConnection conn, int inventuraId)
    {
        var list = new List<InventuraStavka>();
        using var cmd = new SqlCommand(
            @"SELECT InventuraStavkaID, InventuraID, LokacijaID, ArtikalID,
                     UtvrdjenaKolicina, SistemskaBaza, Razlika, Napomena, DatumKreiranja
              FROM InventureStavke WHERE InventuraID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", inventuraId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new InventuraStavka
            {
                InventuraStavkaID = reader.GetInt32(0),
                InventuraID = reader.GetInt32(1),
                LokacijaID = reader.GetInt32(2),
                ArtikalID = reader.GetInt32(3),
                UtvrdjenaKolicina = reader.GetInt32(4),
                SistemskaBaza = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                Razlika = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                Napomena = reader.IsDBNull(7) ? null : reader.GetString(7),
                DatumKreiranja = reader.GetDateTime(8)
            });
        }

        return list;
    }

    private static Inventura MapInventura(SqlDataReader reader)
    {
        return new Inventura
        {
            InventuraID = reader.GetInt32(0),
            BrojDokumenta = reader.GetString(1),
            DatumInventure = reader.GetDateTime(2),
            TipInventure = reader.IsDBNull(3) ? null : reader.GetString(3),
            Napomena = reader.IsDBNull(4) ? null : reader.GetString(4),
            Status = reader.GetString(5),
            Korisnik = reader.IsDBNull(6) ? null : reader.GetString(6),
            DatumZavrsenja = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
            DatumKreiranja = reader.GetDateTime(8),
            DatumIzmene = reader.GetDateTime(9)
        };
    }
}
