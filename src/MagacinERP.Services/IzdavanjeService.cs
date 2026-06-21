using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.Services;

public class IzdavanjeService
{
    private readonly IIzdavanjeRepository _izdavanjeRepository;
    private readonly IZaliheRepository _zaliheRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public IzdavanjeService(
        IIzdavanjeRepository izdavanjeRepository,
        IZaliheRepository zaliheRepository,
        IAuditLogRepository auditLogRepository)
    {
        _izdavanjeRepository = izdavanjeRepository;
        _zaliheRepository = zaliheRepository;
        _auditLogRepository = auditLogRepository;
    }

    public int DodajIzdavanje(string brojDokumenta, string tipIzdavanja, string korisnik, string? napomena = null)
    {
        if (string.IsNullOrWhiteSpace(brojDokumenta))
            throw new ArgumentException("Broj dokumenta je obavezan.", nameof(brojDokumenta));

        if (string.IsNullOrWhiteSpace(tipIzdavanja))
            throw new ArgumentException("Tip izdavanja je obavezan.", nameof(tipIzdavanja));

        var validniTipovi = new[] { "Proizvodnja", "Prodaja", "Povracaj", "Transfer" };
        if (!validniTipovi.Contains(tipIzdavanja))
            throw new ArgumentException("Nevalidan tip izdavanja.", nameof(tipIzdavanja));

        if (string.IsNullOrWhiteSpace(korisnik))
            throw new ArgumentException("Korisnik je obavezan.", nameof(korisnik));

        var izdavanje = new IzdavanjeMaterijala
        {
            BrojDokumenta = brojDokumenta,
            TipIzdavanja = tipIzdavanja,
            Napomena = napomena,
            Status = "Otvorena",
            Korisnik = korisnik
        };

        _izdavanjeRepository.Add(izdavanje);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "IzdavanjeMaterijala",
            Akcija = "INSERT",
            PrimarniKljuc = izdavanje.IzdavanjeID.ToString(),
            NovaVrednost = $"Broj: {brojDokumenta}",
            Korisnik = korisnik
        });

        return izdavanje.IzdavanjeID;
    }

    public void DodajStavku(int izdavanjeId, int artikalId, int kolicinaTrazena, string korisnik, decimal? cenaJedinice = null, int? lotId = null)
    {
        if (izdavanjeId <= 0)
            throw new ArgumentException("Izdavanje ID mora biti validan.", nameof(izdavanjeId));

        if (artikalId <= 0)
            throw new ArgumentException("Artikal ID mora biti validan.", nameof(artikalId));

        if (kolicinaTrazena <= 0)
            throw new ArgumentException("Količina mora biti veća od nule.", nameof(kolicinaTrazena));

        var izdavanje = _izdavanjeRepository.GetById(izdavanjeId);
        if (izdavanje == null)
            throw new InvalidOperationException("Izdavanje ne postoji.");

        if (izdavanje.Status != "Otvorena")
            throw new InvalidOperationException("Nije moguće dodati stavku u završeno ili otkazano izdavanje.");

        var stavka = new IzdavanjeStavka
        {
            IzdavanjeID = izdavanjeId,
            ArtikalID = artikalId,
            KolicinaTrazena = kolicinaTrazena,
            CenaJedinice = cenaJedinice,
            LotID = lotId
        };

        _izdavanjeRepository.AddStavka(stavka);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "IzdavanjeStavke",
            Akcija = "INSERT",
            PrimarniKljuc = izdavanjeId.ToString(),
            NovaVrednost = $"Artikal ID: {artikalId}",
            Korisnik = korisnik
        });
    }

    public void ZavrsiIzdavanje(int izdavanjeId, string korisnik)
    {
        var izdavanje = _izdavanjeRepository.GetById(izdavanjeId);
        if (izdavanje == null)
            throw new InvalidOperationException("Izdavanje ne postoji.");

        if (izdavanje.Status != "Otvorena")
            throw new InvalidOperationException("Samo otvoreno izdavanje se može završiti.");

        izdavanje.Status = "Zavrsena";
        izdavanje.DatumZavrsenja = DateTime.Now;
        izdavanje.DatumIzmene = DateTime.Now;
        _izdavanjeRepository.Update(izdavanje);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "IzdavanjeMaterijala",
            Akcija = "UPDATE",
            PrimarniKljuc = izdavanjeId.ToString(),
            NovaVrednost = "Status: Zavrsena",
            Korisnik = korisnik
        });
    }
}
