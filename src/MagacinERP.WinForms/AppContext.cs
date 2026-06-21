using MagacinERP.Core.Interfaces;
using MagacinERP.DataAccess;
using MagacinERP.DataAccess.Repositories;
using MagacinERP.Services;

namespace MagacinERP.WinForms;

public static class AppContext
{
    private static DatabaseConnection? _db;
    private static string _connectionString = DatabaseConnection.DefaultConnectionString;

    public static string? TrenutniKorisnik { get; set; }
    public static string? TrenutnaUloga { get; set; }

    public static void SetConnectionString(string connStr)
    {
        _connectionString = connStr;
        _db = null;
    }

    private static DatabaseConnection Db => _db ??= new DatabaseConnection(_connectionString);

    // Repositories
    public static IArtikalRepository ArtikalRepository => new ArtikalRepository(Db);
    public static IZaliheRepository ZaliheRepository => new ZaliheRepository(Db);
    public static IKorisnikRepository KorisnikRepository => new KorisnikRepository(Db);
    public static IPrijemaRepository PrijemaRepository => new PrijemaRepository(Db);
    public static IIzdavanjeRepository IzdavanjeRepository => new IzdavanjeRepository(Db);
    public static ITransferRepository TransferRepository => new TransferRepository(Db);
    public static IInventuraRepository InventuraRepository => new InventuraRepository(Db);
    public static ISkladisnaLokacijaRepository LokacijaRepository => new SkladisnaLokacijaRepository(Db);
    public static IAuditLogRepository AuditLogRepository => new AuditLogRepository(Db);

    // Services
    public static ZaliheService ZaliheService => new ZaliheService(ZaliheRepository, ArtikalRepository, AuditLogRepository);
    public static PrijemaService PrijemaService => new PrijemaService(PrijemaRepository, ZaliheRepository, AuditLogRepository);
    public static IzdavanjeService IzdavanjeService => new IzdavanjeService(IzdavanjeRepository, ZaliheRepository, AuditLogRepository);
    public static TransferService TransferService => new TransferService(TransferRepository, ZaliheRepository, LokacijaRepository, AuditLogRepository);
    public static InventuraService InventuraService => new InventuraService(InventuraRepository, ZaliheRepository, AuditLogRepository);
    public static KorisnikService KorisnikService => new KorisnikService(KorisnikRepository);
}
