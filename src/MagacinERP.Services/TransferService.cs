using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.Services;

public class TransferService
{
    private readonly ITransferRepository _transferRepository;
    private readonly IZaliheRepository _zaliheRepository;
    private readonly ISkladisnaLokacijaRepository _lokacijaRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public TransferService(
        ITransferRepository transferRepository,
        IZaliheRepository zaliheRepository,
        ISkladisnaLokacijaRepository lokacijaRepository,
        IAuditLogRepository auditLogRepository)
    {
        _transferRepository = transferRepository;
        _zaliheRepository = zaliheRepository;
        _lokacijaRepository = lokacijaRepository;
        _auditLogRepository = auditLogRepository;
    }

    public int DodajTransfer(string brojDokumenta, int izLokacijeId, int uLokacijuId, string korisnik, string? napomena = null)
    {
        if (string.IsNullOrWhiteSpace(brojDokumenta))
            throw new ArgumentException("Broj dokumenta je obavezan.", nameof(brojDokumenta));

        if (izLokacijeId <= 0)
            throw new ArgumentException("Izlazna lokacija mora biti validna.", nameof(izLokacijeId));

        if (uLokacijuId <= 0)
            throw new ArgumentException("Ulazna lokacija mora biti validna.", nameof(uLokacijuId));

        if (izLokacijeId == uLokacijuId)
            throw new ArgumentException("Izlazna i ulazna lokacija ne mogu biti iste.");

        if (string.IsNullOrWhiteSpace(korisnik))
            throw new ArgumentException("Korisnik je obavezan.", nameof(korisnik));

        var izLokacija = _lokacijaRepository.GetById(izLokacijeId);
        if (izLokacija == null)
            throw new InvalidOperationException("Izlazna lokacija ne postoji.");

        var uLokacija = _lokacijaRepository.GetById(uLokacijuId);
        if (uLokacija == null)
            throw new InvalidOperationException("Ulazna lokacija ne postoji.");

        var transfer = new Transfer
        {
            BrojDokumenta = brojDokumenta,
            IzLokacijeID = izLokacijeId,
            ULokacijuID = uLokacijuId,
            Napomena = napomena,
            Status = "Otvorena",
            Korisnik = korisnik
        };

        _transferRepository.Add(transfer);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "Transferi",
            Akcija = "INSERT",
            PrimarniKljuc = transfer.TransferID.ToString(),
            NovaVrednost = $"Broj: {brojDokumenta}",
            Korisnik = korisnik
        });

        return transfer.TransferID;
    }

    public void IzvrsiTransfer(int transferId, string korisnik)
    {
        var transfer = _transferRepository.GetById(transferId);
        if (transfer == null)
            throw new InvalidOperationException("Transfer ne postoji.");

        if (transfer.Status != "Otvorena")
            throw new InvalidOperationException("Samo otvoren transfer se može izvršiti.");

        if (!transfer.Stavke.Any())
            throw new InvalidOperationException("Transfer mora imati bar jednu stavku.");

        foreach (var stavka in transfer.Stavke)
        {
            var izZaliha = _zaliheRepository.GetByArtikalAndLokacija(stavka.ArtikalID, transfer.IzLokacijeID);
            if (izZaliha == null || izZaliha.Kolicina < stavka.Kolicina)
                throw new InvalidOperationException($"Nedovoljna količina artikla {stavka.ArtikalID} na izlaznoj lokaciji.");

            izZaliha.Kolicina -= stavka.Kolicina;
            izZaliha.DisponibilnaKolicina = izZaliha.Kolicina - izZaliha.RezervovanoKolicina;
            _zaliheRepository.Update(izZaliha);

            var uZaliha = _zaliheRepository.GetByArtikalAndLokacija(stavka.ArtikalID, transfer.ULokacijuID);
            if (uZaliha != null)
            {
                uZaliha.Kolicina += stavka.Kolicina;
                uZaliha.DisponibilnaKolicina = uZaliha.Kolicina - uZaliha.RezervovanoKolicina;
                _zaliheRepository.Update(uZaliha);
            }
            else
            {
                _zaliheRepository.Add(new Zaliha
                {
                    ArtikalID = stavka.ArtikalID,
                    LokacijaID = transfer.ULokacijuID,
                    Kolicina = stavka.Kolicina,
                    DisponibilnaKolicina = stavka.Kolicina,
                    RezervovanoKolicina = 0
                });
            }
        }

        transfer.Status = "Zavrsena";
        transfer.DatumZavrsenja = DateTime.Now;
        transfer.DatumIzmene = DateTime.Now;
        _transferRepository.Update(transfer);

        _auditLogRepository.Add(new AuditLog
        {
            Tabela = "Transferi",
            Akcija = "UPDATE",
            PrimarniKljuc = transferId.ToString(),
            NovaVrednost = "Status: Zavrsena",
            Korisnik = korisnik
        });
    }
}
