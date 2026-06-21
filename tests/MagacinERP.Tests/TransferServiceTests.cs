using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;
using MagacinERP.Services;
using Moq;

namespace MagacinERP.Tests;

public class TransferServiceTests
{
    private readonly Mock<ITransferRepository> _transferRepoMock;
    private readonly Mock<IZaliheRepository> _zaliheRepoMock;
    private readonly Mock<ISkladisnaLokacijaRepository> _lokacijaRepoMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly TransferService _service;

    public TransferServiceTests()
    {
        _transferRepoMock = new Mock<ITransferRepository>();
        _zaliheRepoMock = new Mock<IZaliheRepository>();
        _lokacijaRepoMock = new Mock<ISkladisnaLokacijaRepository>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _service = new TransferService(_transferRepoMock.Object, _zaliheRepoMock.Object, _lokacijaRepoMock.Object, _auditLogRepoMock.Object);
    }

    [Fact]
    public void DodajTransfer_ValidInput_CreatesTransfer()
    {
        _lokacijaRepoMock.Setup(r => r.GetById(1)).Returns(new SkladisnaLokacija { LokacijaID = 1 });
        _lokacijaRepoMock.Setup(r => r.GetById(2)).Returns(new SkladisnaLokacija { LokacijaID = 2 });

        _service.DodajTransfer("TR-001", 1, 2, "admin", "Test transfer");

        _transferRepoMock.Verify(r => r.Add(It.Is<Transfer>(t =>
            t.BrojDokumenta == "TR-001" && t.IzLokacijeID == 1 && t.ULokacijuID == 2)), Times.Once);
    }

    [Fact]
    public void DodajTransfer_SameLocation_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajTransfer("TR-001", 1, 1, "admin"));
    }

    [Fact]
    public void DodajTransfer_EmptyBrojDokumenta_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajTransfer("", 1, 2, "admin"));
    }

    [Fact]
    public void DodajTransfer_InvalidIzLokacija_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajTransfer("TR-001", 0, 2, "admin"));
    }

    [Fact]
    public void DodajTransfer_InvalidULokacija_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajTransfer("TR-001", 1, 0, "admin"));
    }

    [Fact]
    public void DodajTransfer_EmptyKorisnik_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajTransfer("TR-001", 1, 2, ""));
    }

    [Fact]
    public void DodajTransfer_NonExistentIzLokacija_ThrowsInvalidOperation()
    {
        _lokacijaRepoMock.Setup(r => r.GetById(1)).Returns((SkladisnaLokacija?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajTransfer("TR-001", 1, 2, "admin"));
    }

    [Fact]
    public void DodajTransfer_NonExistentULokacija_ThrowsInvalidOperation()
    {
        _lokacijaRepoMock.Setup(r => r.GetById(1)).Returns(new SkladisnaLokacija { LokacijaID = 1 });
        _lokacijaRepoMock.Setup(r => r.GetById(2)).Returns((SkladisnaLokacija?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajTransfer("TR-001", 1, 2, "admin"));
    }

    [Fact]
    public void IzvrsiTransfer_ValidTransfer_MovesStock()
    {
        var transfer = new Transfer
        {
            TransferID = 1,
            IzLokacijeID = 1,
            ULokacijuID = 2,
            Status = "Otvorena",
            Stavke = new List<TransferStavka>
            {
                new() { ArtikalID = 1, Kolicina = 30 }
            }
        };
        _transferRepoMock.Setup(r => r.GetById(1)).Returns(transfer);

        var izZaliha = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 100, RezervovanoKolicina = 0, DisponibilnaKolicina = 100 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(izZaliha);
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 2)).Returns((Zaliha?)null);

        _service.IzvrsiTransfer(1, "admin");

        Assert.Equal(70, izZaliha.Kolicina);
        Assert.Equal("Zavrsena", transfer.Status);
        _zaliheRepoMock.Verify(r => r.Add(It.Is<Zaliha>(z => z.ArtikalID == 1 && z.LokacijaID == 2 && z.Kolicina == 30)), Times.Once);
    }

    [Fact]
    public void IzvrsiTransfer_ExistingDestination_UpdatesStock()
    {
        var transfer = new Transfer
        {
            TransferID = 1,
            IzLokacijeID = 1,
            ULokacijuID = 2,
            Status = "Otvorena",
            Stavke = new List<TransferStavka>
            {
                new() { ArtikalID = 1, Kolicina = 20 }
            }
        };
        _transferRepoMock.Setup(r => r.GetById(1)).Returns(transfer);

        var izZaliha = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 50, RezervovanoKolicina = 0, DisponibilnaKolicina = 50 };
        var uZaliha = new Zaliha { ArtikalID = 1, LokacijaID = 2, Kolicina = 40, RezervovanoKolicina = 5, DisponibilnaKolicina = 35 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(izZaliha);
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 2)).Returns(uZaliha);

        _service.IzvrsiTransfer(1, "admin");

        Assert.Equal(30, izZaliha.Kolicina);
        Assert.Equal(60, uZaliha.Kolicina);
        Assert.Equal(55, uZaliha.DisponibilnaKolicina);
    }

    [Fact]
    public void IzvrsiTransfer_InsufficientStock_ThrowsInvalidOperation()
    {
        var transfer = new Transfer
        {
            TransferID = 1,
            IzLokacijeID = 1,
            ULokacijuID = 2,
            Status = "Otvorena",
            Stavke = new List<TransferStavka>
            {
                new() { ArtikalID = 1, Kolicina = 200 }
            }
        };
        _transferRepoMock.Setup(r => r.GetById(1)).Returns(transfer);

        var izZaliha = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 50 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(izZaliha);

        Assert.Throws<InvalidOperationException>(() =>
            _service.IzvrsiTransfer(1, "admin"));
    }

    [Fact]
    public void IzvrsiTransfer_NoStavke_ThrowsInvalidOperation()
    {
        var transfer = new Transfer
        {
            TransferID = 1,
            IzLokacijeID = 1,
            ULokacijuID = 2,
            Status = "Otvorena",
            Stavke = new List<TransferStavka>()
        };
        _transferRepoMock.Setup(r => r.GetById(1)).Returns(transfer);

        Assert.Throws<InvalidOperationException>(() =>
            _service.IzvrsiTransfer(1, "admin"));
    }

    [Fact]
    public void IzvrsiTransfer_ClosedTransfer_ThrowsInvalidOperation()
    {
        var transfer = new Transfer { TransferID = 1, Status = "Zavrsena" };
        _transferRepoMock.Setup(r => r.GetById(1)).Returns(transfer);

        Assert.Throws<InvalidOperationException>(() =>
            _service.IzvrsiTransfer(1, "admin"));
    }

    [Fact]
    public void IzvrsiTransfer_NonExistent_ThrowsInvalidOperation()
    {
        _transferRepoMock.Setup(r => r.GetById(999)).Returns((Transfer?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.IzvrsiTransfer(999, "admin"));
    }
}
