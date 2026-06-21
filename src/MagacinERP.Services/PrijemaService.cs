using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.Services;

public class PrijemaService
{
    private readonly IPrijemaRepository _prijemaRepository;
    private readonly IZaliheRepository _zaliheRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public PrijemaService(
        IPrijemaRepository prijemaRepository,
        IZaliheRepository zaliheRepository,
        IAuditLogRepository auditLogRepository)
    {
        _prijemaRepository = prijemaRepository;
        _zaliheRepository = zaliheRepository;
        _auditLogRepository = auditLogRepository;
    }

    public int DodajPrijemu(string brojDokumenta, int dobavljacId, string korisnik, string? napomena = null)
    {
        if (string.IsNullOrWhiteSpace(brojDokumenta))
            throw new ArgumentException("Broj dokumenta je obavezan.", nameof(brojDokumenta));

        if (dobavljacId <= 0)
            throw new ArgumentException("Dobavljač ID mora biti validan.", nameof(dobavljacId));

        if (string.IsNullOrWhiteSpace(korisnik))
            throw new ArgumentException("Korisnik je obavezan.", nameof(korisnik));

        var prijema = new PrijemaMaterijala
        {
            BrojDokumenta = brojDokumenta,
            DobavljacID = dobavljacId,
            Napomena = napomena,
            Status = "Otvorena",
            Korisnik = korisnik
        };

        _prijemaRepository.Add(prijema);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "PrijemaMaterijala",
            Akcija = "INSERT",
            PrimarniKljuc = prijema.PrijemaID.ToString(),
            NovaVrednost = $"Broj: {brojDokumenta}",
            Korisnik = korisnik
        });

        return prijema.PrijemaID;
    }

    public void DodajStavku(int prijemaId, int artikalId, int kolicinaNarudjena, decimal cenaJedinice, string korisnik, string? lotBroj = null, DateTime? datumRoka = null)
    {
        if (prijemaId <= 0)
            throw new ArgumentException("Prijema ID mora biti validan.", nameof(prijemaId));

        if (artikalId <= 0)
            throw new ArgumentException("Artikal ID mora biti validan.", nameof(artikalId));

        if (kolicinaNarudjena <= 0)
            throw new ArgumentException("Količina mora biti veća od nule.", nameof(kolicinaNarudjena));

        if (cenaJedinice < 0)
            throw new ArgumentException("Cena ne može biti negativna.", nameof(cenaJedinice));

        var prijema = _prijemaRepository.GetById(prijemaId);
        if (prijema == null)
            throw new InvalidOperationException("Prijema ne postoji.");

        if (prijema.Status != "Otvorena")
            throw new InvalidOperationException("Nije moguće dodati stavku u završenu ili otkazanu prijemu.");

        var stavka = new PrijemaStavka
        {
            PrijemaID = prijemaId,
            ArtikalID = artikalId,
            KolicinaNarudjena = kolicinaNarudjena,
            CenaJedinice = cenaJedinice,
            LotBroj = lotBroj,
            DatumRoka = datumRoka
        };

        _prijemaRepository.AddStavka(stavka);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "PrijemaStavke",
            Akcija = "INSERT",
            PrimarniKljuc = prijemaId.ToString(),
            NovaVrednost = $"Artikal ID: {artikalId}",
            Korisnik = korisnik
        });
    }

    public void ZavrsiPrijemu(int prijemaId, string korisnik)
    {
        var prijema = _prijemaRepository.GetById(prijemaId);
        if (prijema == null)
            throw new InvalidOperationException("Prijema ne postoji.");

        if (prijema.Status != "Otvorena")
            throw new InvalidOperationException("Samo otvorena prijema se može završiti.");

        prijema.Status = "Zavrsena";
        prijema.DatumZavrsenja = DateTime.Now;
        prijema.DatumIzmene = DateTime.Now;
        _prijemaRepository.Update(prijema);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "PrijemaMaterijala",
            Akcija = "UPDATE",
            PrimarniKljuc = prijemaId.ToString(),
            NovaVrednost = "Status: Zavrsena",
            Korisnik = korisnik
        });
    }

    public void OtkaziPrijemu(int prijemaId, string korisnik)
    {
        var prijema = _prijemaRepository.GetById(prijemaId);
        if (prijema == null)
            throw new InvalidOperationException("Prijema ne postoji.");

        if (prijema.Status != "Otvorena")
            throw new InvalidOperationException("Samo otvorena prijema se može otkazati.");

        prijema.Status = "Otkazana";
        prijema.DatumIzmene = DateTime.Now;
        _prijemaRepository.Update(prijema);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "PrijemaMaterijala",
            Akcija = "UPDATE",
            PrimarniKljuc = prijemaId.ToString(),
            NovaVrednost = "Status: Otkazana",
            Korisnik = korisnik
        });
    }
}
