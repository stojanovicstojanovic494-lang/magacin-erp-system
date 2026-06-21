using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;
using MagacinERP.Services;
using Moq;

namespace MagacinERP.Tests;

public class PrijemaServiceTests
{
    private readonly Mock<IPrijemaRepository> _prijemaRepoMock;
    private readonly Mock<IZaliheRepository> _zaliheRepoMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly PrijemaService _service;

    public PrijemaServiceTests()
    {
        _prijemaRepoMock = new Mock<IPrijemaRepository>();
        _zaliheRepoMock = new Mock<IZaliheRepository>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _service = new PrijemaService(_prijemaRepoMock.Object, _zaliheRepoMock.Object, _auditLogRepoMock.Object);
    }

    [Fact]
    public void DodajPrijemu_ValidInput_CreatesPrijema()
    {
        _service.DodajPrijemu("GRN-001", 1, "admin", "Test napomena");

        _prijemaRepoMock.Verify(r => r.Add(It.Is<PrijemaMaterijala>(p =>
            p.BrojDokumenta == "GRN-001" && p.DobavljacID == 1 && p.Status == "Otvorena")), Times.Once);
        _auditLogRepoMock.Verify(r => r.Add(It.IsAny<AuditLog>()), Times.Once);
    }

    [Fact]
    public void DodajPrijemu_EmptyBrojDokumenta_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajPrijemu("", 1, "admin"));
    }

    [Fact]
    public void DodajPrijemu_InvalidDobavljacId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajPrijemu("GRN-001", 0, "admin"));
    }

    [Fact]
    public void DodajPrijemu_EmptyKorisnik_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajPrijemu("GRN-001", 1, ""));
    }

    [Fact]
    public void DodajStavku_ValidInput_AddsStavka()
    {
        var prijema = new PrijemaMaterijala { PrijemaID = 1, Status = "Otvorena" };
        _prijemaRepoMock.Setup(r => r.GetById(1)).Returns(prijema);

        _service.DodajStavku(1, 1, 100, 50.00m, "admin", "LOT001", DateTime.Now.AddMonths(6));

        _prijemaRepoMock.Verify(r => r.AddStavka(It.Is<PrijemaStavka>(s =>
            s.PrijemaID == 1 && s.ArtikalID == 1 && s.KolicinaNarudjena == 100)), Times.Once);
    }

    [Fact]
    public void DodajStavku_NonExistentPrijema_ThrowsInvalidOperation()
    {
        _prijemaRepoMock.Setup(r => r.GetById(999)).Returns((PrijemaMaterijala?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajStavku(999, 1, 100, 50.00m, "admin"));
    }

    [Fact]
    public void DodajStavku_ClosedPrijema_ThrowsInvalidOperation()
    {
        var prijema = new PrijemaMaterijala { PrijemaID = 1, Status = "Zavrsena" };
        _prijemaRepoMock.Setup(r => r.GetById(1)).Returns(prijema);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajStavku(1, 1, 100, 50.00m, "admin"));
    }

    [Fact]
    public void DodajStavku_ZeroKolicina_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(1, 1, 0, 50.00m, "admin"));
    }

    [Fact]
    public void DodajStavku_NegativeCena_ThrowsArgumentException()
    {
        var prijema = new PrijemaMaterijala { PrijemaID = 1, Status = "Otvorena" };
        _prijemaRepoMock.Setup(r => r.GetById(1)).Returns(prijema);

        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(1, 1, 100, -10.00m, "admin"));
    }

    [Fact]
    public void DodajStavku_InvalidPrijemaId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(0, 1, 100, 50.00m, "admin"));
    }

    [Fact]
    public void DodajStavku_InvalidArtikalId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(1, 0, 100, 50.00m, "admin"));
    }

    [Fact]
    public void ZavrsiPrijemu_OpenPrijema_ChangesStatusToZavrsena()
    {
        var prijema = new PrijemaMaterijala { PrijemaID = 1, Status = "Otvorena" };
        _prijemaRepoMock.Setup(r => r.GetById(1)).Returns(prijema);

        _service.ZavrsiPrijemu(1, "admin");

        Assert.Equal("Zavrsena", prijema.Status);
        Assert.NotNull(prijema.DatumZavrsenja);
        _prijemaRepoMock.Verify(r => r.Update(prijema), Times.Once);
    }

    [Fact]
    public void ZavrsiPrijemu_AlreadyClosed_ThrowsInvalidOperation()
    {
        var prijema = new PrijemaMaterijala { PrijemaID = 1, Status = "Zavrsena" };
        _prijemaRepoMock.Setup(r => r.GetById(1)).Returns(prijema);

        Assert.Throws<InvalidOperationException>(() =>
            _service.ZavrsiPrijemu(1, "admin"));
    }

    [Fact]
    public void ZavrsiPrijemu_NonExistent_ThrowsInvalidOperation()
    {
        _prijemaRepoMock.Setup(r => r.GetById(999)).Returns((PrijemaMaterijala?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.ZavrsiPrijemu(999, "admin"));
    }

    [Fact]
    public void OtkaziPrijemu_OpenPrijema_ChangesStatusToOtkazana()
    {
        var prijema = new PrijemaMaterijala { PrijemaID = 1, Status = "Otvorena" };
        _prijemaRepoMock.Setup(r => r.GetById(1)).Returns(prijema);

        _service.OtkaziPrijemu(1, "admin");

        Assert.Equal("Otkazana", prijema.Status);
        _prijemaRepoMock.Verify(r => r.Update(prijema), Times.Once);
    }

    [Fact]
    public void OtkaziPrijemu_AlreadyClosed_ThrowsInvalidOperation()
    {
        var prijema = new PrijemaMaterijala { PrijemaID = 1, Status = "Zavrsena" };
        _prijemaRepoMock.Setup(r => r.GetById(1)).Returns(prijema);

        Assert.Throws<InvalidOperationException>(() =>
            _service.OtkaziPrijemu(1, "admin"));
    }

    [Fact]
    public void OtkaziPrijemu_NonExistent_ThrowsInvalidOperation()
    {
        _prijemaRepoMock.Setup(r => r.GetById(999)).Returns((PrijemaMaterijala?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.OtkaziPrijemu(999, "admin"));
    }
}
