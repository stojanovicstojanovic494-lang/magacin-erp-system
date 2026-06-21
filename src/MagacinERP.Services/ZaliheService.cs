using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.Services;

public class ZaliheService
{
    private readonly IZaliheRepository _zaliheRepository;
    private readonly IArtikalRepository _artikalRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public ZaliheService(
        IZaliheRepository zaliheRepository,
        IArtikalRepository artikalRepository,
        IAuditLogRepository auditLogRepository)
    {
        _zaliheRepository = zaliheRepository;
        _artikalRepository = artikalRepository;
        _auditLogRepository = auditLogRepository;
    }

    public void AzurirajZaliheNakonPrijeme(int artikalId, int lokacijaId, int kolicina, string korisnik)
    {
        if (kolicina <= 0)
            throw new ArgumentException("Količina mora biti veća od nule.", nameof(kolicina));

        if (string.IsNullOrWhiteSpace(korisnik))
            throw new ArgumentException("Korisnik je obavezan.", nameof(korisnik));

        var existing = _zaliheRepository.GetByArtikalAndLokacija(artikalId, lokacijaId);

        if (existing != null)
        {
            existing.Kolicina += kolicina;
            existing.DisponibilnaKolicina = existing.Kolicina - existing.RezervovanoKolicina;
            existing.DatumZadnjeIzmene = DateTime.Now;
            _zaliheRepository.Update(existing);
        }
        else
        {
            var novaZaliha = new Zaliha
            {
                ArtikalID = artikalId,
                LokacijaID = lokacijaId,
                Kolicina = kolicina,
                DisponibilnaKolicina = kolicina,
                RezervovanoKolicina = 0
            };
            _zaliheRepository.Add(novaZaliha);
        }

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "Zalihe",
            Akcija = "UPDATE",
            PrimarniKljuc = artikalId.ToString(),
            NovaVrednost = $"Kolicina: {kolicina}",
            Korisnik = korisnik
        });
    }

    public void SmanjiZalihe(int artikalId, int lokacijaId, int kolicina, string korisnik)
    {
        if (kolicina <= 0)
            throw new ArgumentException("Količina mora biti veća od nule.", nameof(kolicina));

        if (string.IsNullOrWhiteSpace(korisnik))
            throw new ArgumentException("Korisnik je obavezan.", nameof(korisnik));

        var existing = _zaliheRepository.GetByArtikalAndLokacija(artikalId, lokacijaId);

        if (existing == null)
            throw new InvalidOperationException("Zaliha ne postoji za dati artikal i lokaciju.");

        if (existing.Kolicina < kolicina)
            throw new InvalidOperationException("Nedovoljna količina na zalihama.");

        existing.Kolicina -= kolicina;
        existing.DisponibilnaKolicina = existing.Kolicina - existing.RezervovanoKolicina;
        existing.DatumZadnjeIzmene = DateTime.Now;
        _zaliheRepository.Update(existing);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "Zalihe",
            Akcija = "UPDATE",
            PrimarniKljuc = artikalId.ToString(),
            NovaVrednost = $"Smanjeno za: {kolicina}",
            Korisnik = korisnik
        });
    }

    public IEnumerable<Artikal> GetArtikliSaNiskomZalijhom()
    {
        var artikli = _artikalRepository.GetActive();
        var result = new List<Artikal>();

        foreach (var artikal in artikli)
        {
            var zalihe = _zaliheRepository.GetByArtikalId(artikal.ArtikalID);
            var ukupnaKolicina = zalihe.Sum(z => z.Kolicina);

            if (ukupnaKolicina <= artikal.MinimalneStalje)
            {
                result.Add(artikal);
            }
        }

        return result;
    }

    public decimal GetVrednostZalihe(int artikalId)
    {
        var artikal = _artikalRepository.GetById(artikalId);
        if (artikal == null)
            throw new ArgumentException("Artikal ne postoji.", nameof(artikalId));

        var zalihe = _zaliheRepository.GetByArtikalId(artikalId);
        var ukupnaKolicina = zalihe.Sum(z => z.Kolicina);

        return ukupnaKolicina * (artikal.CenaKupovine ?? 0);
    }

    public IEnumerable<ABCAnalysisResult> ABCAnaliza()
    {
        var artikli = _artikalRepository.GetActive().ToList();
        var results = new List<ABCAnalysisResult>();

        foreach (var artikal in artikli)
        {
            var zalihe = _zaliheRepository.GetByArtikalId(artikal.ArtikalID);
            var kolicina = zalihe.Sum(z => z.Kolicina);
            var vrednost = kolicina * (artikal.CenaKupovine ?? 0);

            results.Add(new ABCAnalysisResult
            {
                ArtikalID = artikal.ArtikalID,
                SifraArtikla = artikal.SifraArtikla,
                NazivArtikla = artikal.NazivArtikla,
                Kolicina = kolicina,
                CenaKupovine = artikal.CenaKupovine ?? 0,
                Vrednost = vrednost
            });
        }

        var ukupnaVrednost = results.Sum(r => r.Vrednost);

        if (ukupnaVrednost == 0)
            return results;

        foreach (var result in results)
        {
            result.ProcenatVrednosti = Math.Round((result.Vrednost * 100) / ukupnaVrednost, 2);
            result.Kategorija = result.ProcenatVrednosti >= 80 ? "A - Kritična" :
                                result.ProcenatVrednosti >= 50 ? "B - Važna" :
                                "C - Ostalo";
        }

        return results.OrderByDescending(r => r.Vrednost);
    }

    public int GetDostupnaKolicina(int artikalId)
    {
        var zalihe = _zaliheRepository.GetByArtikalId(artikalId);
        return zalihe.Sum(z => z.DisponibilnaKolicina);
    }
}
