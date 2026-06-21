using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class KorisnikRepository : IKorisnikRepository
{
    private readonly DatabaseConnection _db;

    public KorisnikRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public Korisnik? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT KorisnikID, Korisnicko_Ime, Lozinka, ImeKorisnika, Prezime,
                     Email, Uloga, Aktivan, DatumKreiranja, DatumIzmene
              FROM Korisnici WHERE KorisnikID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapKorisnik(reader);

        return null;
    }

    public Korisnik? GetByKorisnickoIme(string korisnickoIme)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT KorisnikID, Korisnicko_Ime, Lozinka, ImeKorisnika, Prezime,
                     Email, Uloga, Aktivan, DatumKreiranja, DatumIzmene
              FROM Korisnici WHERE Korisnicko_Ime = @Ime", conn);
        cmd.Parameters.AddWithValue("@Ime", korisnickoIme);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapKorisnik(reader);

        return null;
    }

    public IEnumerable<Korisnik> GetAll()
    {
        var list = new List<Korisnik>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT KorisnikID, Korisnicko_Ime, Lozinka, ImeKorisnika, Prezime,
                     Email, Uloga, Aktivan, DatumKreiranja, DatumIzmene
              FROM Korisnici ORDER BY KorisnikID", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapKorisnik(reader));

        return list;
    }

    public void Add(Korisnik korisnik)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO Korisnici (Korisnicko_Ime, Lozinka, ImeKorisnika, Prezime, Email, Uloga, Aktivan)
              VALUES (@Ime, @Lozinka, @ImeKorisnika, @Prezime, @Email, @Uloga, @Aktivan);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@Ime", korisnik.KorisnickoIme);
        cmd.Parameters.AddWithValue("@Lozinka", korisnik.Lozinka);
        cmd.Parameters.AddWithValue("@ImeKorisnika", korisnik.ImeKorisnika);
        cmd.Parameters.AddWithValue("@Prezime", korisnik.Prezime);
        cmd.Parameters.AddWithValue("@Email", (object?)korisnik.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Uloga", (object?)korisnik.Uloga ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Aktivan", korisnik.Aktivan);

        korisnik.KorisnikID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Korisnik korisnik)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE Korisnici SET
                Korisnicko_Ime = @Ime, Lozinka = @Lozinka,
                ImeKorisnika = @ImeKorisnika, Prezime = @Prezime,
                Email = @Email, Uloga = @Uloga, Aktivan = @Aktivan,
                DatumIzmene = GETDATE()
              WHERE KorisnikID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", korisnik.KorisnikID);
        cmd.Parameters.AddWithValue("@Ime", korisnik.KorisnickoIme);
        cmd.Parameters.AddWithValue("@Lozinka", korisnik.Lozinka);
        cmd.Parameters.AddWithValue("@ImeKorisnika", korisnik.ImeKorisnika);
        cmd.Parameters.AddWithValue("@Prezime", korisnik.Prezime);
        cmd.Parameters.AddWithValue("@Email", (object?)korisnik.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Uloga", (object?)korisnik.Uloga ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Aktivan", korisnik.Aktivan);

        cmd.ExecuteNonQuery();
    }

    private static Korisnik MapKorisnik(SqlDataReader reader)
    {
        return new Korisnik
        {
            KorisnikID = reader.GetInt32(0),
            KorisnickoIme = reader.GetString(1),
            Lozinka = reader.GetString(2),
            ImeKorisnika = reader.GetString(3),
            Prezime = reader.GetString(4),
            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
            Uloga = reader.IsDBNull(6) ? null : reader.GetString(6),
            Aktivan = reader.GetBoolean(7),
            DatumKreiranja = reader.GetDateTime(8),
            DatumIzmene = reader.GetDateTime(9)
        };
    }
}
