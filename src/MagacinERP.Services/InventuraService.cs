using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.Services;

public class InventuraService
{
    private readonly IInventuraRepository _inventuraRepository;
    private readonly IZaliheRepository _zaliheRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public InventuraService(
        IInventuraRepository inventuraRepository,
        IZaliheRepository zaliheRepository,
        IAuditLogRepository auditLogRepository)
    {
        _inventuraRepository = inventuraRepository;
        _zaliheRepository = zaliheRepository;
        _auditLogRepository = auditLogRepository;
    }

    public int DodajInventuru(string brojDokumenta, string tipInventure, string korisnik, string? napomena = null)
    {
        if (string.IsNullOrWhiteSpace(brojDokumenta))
            throw new ArgumentException("Broj dokumenta je obavezan.", nameof(brojDokumenta));

        if (string.IsNullOrWhiteSpace(tipInventure))
            throw new ArgumentException("Tip inventure je obavezan.", nameof(tipInventure));

        var validniTipovi = new[] { "Parcijalna", "Potpuna" };
        if (!validniTipovi.Contains(tipInventure))
            throw new ArgumentException("Nevalidan tip inventure. Dozvoljeni: Parcijalna, Potpuna.", nameof(tipInventure));

        if (string.IsNullOrWhiteSpace(korisnik))
            throw new ArgumentException("Korisnik je obavezan.", nameof(korisnik));

        var inventura = new Inventura
        {
            BrojDokumenta = brojDokumenta,
            TipInventure = tipInventure,
            Napomena = napomena,
            Status = "U_toku",
            Korisnik = korisnik
        };

        _inventuraRepository.Add(inventura);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "Inventure",
            Akcija = "INSERT",
            PrimarniKljuc = inventura.InventuraID.ToString(),
            NovaVrednost = $"Broj: {brojDokumenta}",
            Korisnik = korisnik
        });

        return inventura.InventuraID;
    }

    public void DodajStavku(int inventuraId, int lokacijaId, int artikalId, int utvrdjenaKolicina, string korisnik)
    {
        if (inventuraId <= 0)
            throw new ArgumentException("Inventura ID mora biti validan.", nameof(inventuraId));

        if (utvrdjenaKolicina < 0)
            throw new ArgumentException("Utvrđena količina ne može biti negativna.", nameof(utvrdjenaKolicina));

        var inventura = _inventuraRepository.GetById(inventuraId);
        if (inventura == null)
            throw new InvalidOperationException("Inventura ne postoji.");

        if (inventura.Status != "U_toku")
            throw new InvalidOperationException("Stavke se mogu dodati samo u inventuru koja je u toku.");

        var zaliha = _zaliheRepository.GetByArtikalAndLokacija(artikalId, lokacijaId);
        var sistemskaKolicina = zaliha?.Kolicina ?? 0;

        var stavka = new InventuraStavka
        {
            InventuraID = inventuraId,
            LokacijaID = lokacijaId,
            ArtikalID = artikalId,
            UtvrdjenaKolicina = utvrdjenaKolicina,
            SistemskaBaza = sistemskaKolicina,
            Razlika = utvrdjenaKolicina - sistemskaKolicina
        };

        _inventuraRepository.AddStavka(stavka);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "InventureStavke",
            Akcija = "INSERT",
            PrimarniKljuc = inventuraId.ToString(),
            NovaVrednost = $"Artikal: {artikalId}, Razlika: {stavka.Razlika}",
            Korisnik = korisnik
        });
    }

    public void ZavrsiInventuru(int inventuraId, string korisnik, bool primenjiRazlike = false)
    {
        var inventura = _inventuraRepository.GetById(inventuraId);
        if (inventura == null)
            throw new InvalidOperationException("Inventura ne postoji.");

        if (inventura.Status != "U_toku")
            throw new InvalidOperationException("Samo inventura u toku se može završiti.");

        if (!inventura.Stavke.Any())
            throw new InvalidOperationException("Inventura mora imati bar jednu stavku.");

        if (primenjiRazlike)
        {
            foreach (var stavka in inventura.Stavke)
            {
                if (stavka.Razlika != 0)
                {
                    var zaliha = _zaliheRepository.GetByArtikalAndLokacija(stavka.ArtikalID, stavka.LokacijaID);
                    if (zaliha != null)
                    {
                        zaliha.Kolicina = stavka.UtvrdjenaKolicina;
                        zaliha.DisponibilnaKolicina = zaliha.Kolicina - zaliha.RezervovanoKolicina;
                        zaliha.DatumZadnjeIzmene = DateTime.Now;
                        _zaliheRepository.Update(zaliha);
                    }
                    else
                    {
                        _zaliheRepository.Add(new Zaliha
                        {
                            ArtikalID = stavka.ArtikalID,
                            LokacijaID = stavka.LokacijaID,
                            Kolicina = stavka.UtvrdjenaKolicina,
                            DisponibilnaKolicina = stavka.UtvrdjenaKolicina,
                            RezervovanoKolicina = 0
                        });
                    }
                }
            }
        }

        inventura.Status = "Zavrsena";
        inventura.DatumZavrsenja = DateTime.Now;
        inventura.DatumIzmene = DateTime.Now;
        _inventuraRepository.Update(inventura);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "Inventure",
            Akcija = "UPDATE",
            PrimarniKljuc = inventuraId.ToString(),
            NovaVrednost = $"Status: Zavrsena, PrimenjiRazlike: {primenjiRazlike}",
            Korisnik = korisnik
        });
    }
}
