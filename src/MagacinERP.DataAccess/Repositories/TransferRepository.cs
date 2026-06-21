using Microsoft.Data.SqlClient;
using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.DataAccess.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly DatabaseConnection _db;

    public TransferRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public Transfer? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT TransferID, BrojDokumenta, DatumTransfera, IzLokacijeID, ULokacijuID,
                     Napomena, Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM Transferi WHERE TransferID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var transfer = MapTransfer(reader);
            reader.Close();
            transfer.Stavke = GetStavke(conn, transfer.TransferID);
            return transfer;
        }

        return null;
    }

    public IEnumerable<Transfer> GetAll()
    {
        var list = new List<Transfer>();
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"SELECT TransferID, BrojDokumenta, DatumTransfera, IzLokacijeID, ULokacijuID,
                     Napomena, Status, Korisnik, DatumZavrsenja, DatumKreiranja, DatumIzmene
              FROM Transferi ORDER BY DatumTransfera DESC", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapTransfer(reader));

        return list;
    }

    public void Add(Transfer transfer)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO Transferi (BrojDokumenta, IzLokacijeID, ULokacijuID, Napomena, Status, Korisnik)
              VALUES (@Broj, @IzLok, @ULok, @Napomena, @Status, @Korisnik);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@Broj", transfer.BrojDokumenta);
        cmd.Parameters.AddWithValue("@IzLok", transfer.IzLokacijeID);
        cmd.Parameters.AddWithValue("@ULok", transfer.ULokacijuID);
        cmd.Parameters.AddWithValue("@Napomena", (object?)transfer.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", transfer.Status);
        cmd.Parameters.AddWithValue("@Korisnik", (object?)transfer.Korisnik ?? DBNull.Value);

        transfer.TransferID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Transfer transfer)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"UPDATE Transferi SET
                Status = @Status, Napomena = @Napomena,
                DatumZavrsenja = @DatumZavrsenja, DatumIzmene = GETDATE()
              WHERE TransferID = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", transfer.TransferID);
        cmd.Parameters.AddWithValue("@Status", transfer.Status);
        cmd.Parameters.AddWithValue("@Napomena", (object?)transfer.Napomena ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DatumZavrsenja", (object?)transfer.DatumZavrsenja ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public void AddStavka(TransferStavka stavka)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new SqlCommand(
            @"INSERT INTO TransferiStavke (TransferID, ArtikalID, Kolicina, LotID, Napomena)
              VALUES (@TransferId, @ArtikalId, @Kolicina, @LotId, @Napomena);
              SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@TransferId", stavka.TransferID);
        cmd.Parameters.AddWithValue("@ArtikalId", stavka.ArtikalID);
        cmd.Parameters.AddWithValue("@Kolicina", stavka.Kolicina);
        cmd.Parameters.AddWithValue("@LotId", (object?)stavka.LotID ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Napomena", (object?)stavka.Napomena ?? DBNull.Value);

        stavka.TransferStavkaID = Convert.ToInt32(cmd.ExecuteScalar());
    }

    private List<TransferStavka> GetStavke(SqlConnection conn, int transferId)
    {
        var list = new List<TransferStavka>();
        using var cmd = new SqlCommand(
            @"SELECT TransferStavkaID, TransferID, ArtikalID, Kolicina, LotID, Napomena, DatumKreiranja
              FROM TransferiStavke WHERE TransferID = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", transferId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new TransferStavka
            {
                TransferStavkaID = reader.GetInt32(0),
                TransferID = reader.GetInt32(1),
                ArtikalID = reader.GetInt32(2),
                Kolicina = reader.GetInt32(3),
                LotID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                Napomena = reader.IsDBNull(5) ? null : reader.GetString(5),
                DatumKreiranja = reader.GetDateTime(6)
            });
        }

        return list;
    }

    private static Transfer MapTransfer(SqlDataReader reader)
    {
        return new Transfer
        {
            TransferID = reader.GetInt32(0),
            BrojDokumenta = reader.GetString(1),
            DatumTransfera = reader.GetDateTime(2),
            IzLokacijeID = reader.GetInt32(3),
            ULokacijuID = reader.GetInt32(4),
            Napomena = reader.IsDBNull(5) ? null : reader.GetString(5),
            Status = reader.GetString(6),
            Korisnik = reader.IsDBNull(7) ? null : reader.GetString(7),
            DatumZavrsenja = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
            DatumKreiranja = reader.GetDateTime(9),
            DatumIzmene = reader.GetDateTime(10)
        };
    }
}
