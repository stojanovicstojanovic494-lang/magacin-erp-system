using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;
using MagacinERP.Services;
using Moq;

namespace MagacinERP.Tests;

public class IzdavanjeServiceTests
{
    private readonly Mock<IIzdavanjeRepository> _izdavanjeRepoMock;
    private readonly Mock<IZaliheRepository> _zaliheRepoMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly IzdavanjeService _service;

    public IzdavanjeServiceTests()
    {
        _izdavanjeRepoMock = new Mock<IIzdavanjeRepository>();
        _zaliheRepoMock = new Mock<IZaliheRepository>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _service = new IzdavanjeService(_izdavanjeRepoMock.Object, _zaliheRepoMock.Object, _auditLogRepoMock.Object);
    }

    [Fact]
    public void DodajIzdavanje_ValidInput_CreatesIzdavanje()
    {
        _service.DodajIzdavanje("IZ-001", "Proizvodnja", "admin", "Test");

        _izdavanjeRepoMock.Verify(r => r.Add(It.Is<IzdavanjeMaterijala>(i =>
            i.BrojDokumenta == "IZ-001" && i.TipIzdavanja == "Proizvodnja" && i.Status == "Otvorena")), Times.Once);
        _auditLogRepoMock.Verify(r => r.Add(It.IsAny<AuditLog>()), Times.Once);
    }

    [Fact]
    public void DodajIzdavanje_EmptyBrojDokumenta_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajIzdavanje("", "Proizvodnja", "admin"));
    }

    [Fact]
    public void DodajIzdavanje_EmptyTipIzdavanja_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajIzdavanje("IZ-001", "", "admin"));
    }

    [Fact]
    public void DodajIzdavanje_InvalidTipIzdavanja_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajIzdavanje("IZ-001", "Nevalidan", "admin"));
    }

    [Theory]
    [InlineData("Proizvodnja")]
    [InlineData("Prodaja")]
    [InlineData("Povracaj")]
    [InlineData("Transfer")]
    public void DodajIzdavanje_AllValidTypes_Succeeds(string tip)
    {
        _service.DodajIzdavanje("IZ-001", tip, "admin");

        _izdavanjeRepoMock.Verify(r => r.Add(It.Is<IzdavanjeMaterijala>(i =>
            i.TipIzdavanja == tip)), Times.Once);
    }

    [Fact]
    public void DodajIzdavanje_EmptyKorisnik_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajIzdavanje("IZ-001", "Proizvodnja", ""));
    }

    [Fact]
    public void DodajStavku_ValidInput_AddsStavka()
    {
        var izdavanje = new IzdavanjeMaterijala { IzdavanjeID = 1, Status = "Otvorena" };
        _izdavanjeRepoMock.Setup(r => r.GetById(1)).Returns(izdavanje);

        _service.DodajStavku(1, 1, 50, "admin", 100m, 1);

        _izdavanjeRepoMock.Verify(r => r.AddStavka(It.Is<IzdavanjeStavka>(s =>
            s.IzdavanjeID == 1 && s.ArtikalID == 1 && s.KolicinaTrazena == 50)), Times.Once);
    }

    [Fact]
    public void DodajStavku_NonExistentIzdavanje_ThrowsInvalidOperation()
    {
        _izdavanjeRepoMock.Setup(r => r.GetById(999)).Returns((IzdavanjeMaterijala?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajStavku(999, 1, 50, "admin"));
    }

    [Fact]
    public void DodajStavku_ClosedIzdavanje_ThrowsInvalidOperation()
    {
        var izdavanje = new IzdavanjeMaterijala { IzdavanjeID = 1, Status = "Zavrsena" };
        _izdavanjeRepoMock.Setup(r => r.GetById(1)).Returns(izdavanje);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajStavku(1, 1, 50, "admin"));
    }

    [Fact]
    public void DodajStavku_ZeroKolicina_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(1, 1, 0, "admin"));
    }

    [Fact]
    public void DodajStavku_InvalidIzdavanjeId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(0, 1, 50, "admin"));
    }

    [Fact]
    public void DodajStavku_InvalidArtikalId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(1, 0, 50, "admin"));
    }

    [Fact]
    public void ZavrsiIzdavanje_OpenIzdavanje_ChangesStatusToZavrsena()
    {
        var izdavanje = new IzdavanjeMaterijala { IzdavanjeID = 1, Status = "Otvorena" };
        _izdavanjeRepoMock.Setup(r => r.GetById(1)).Returns(izdavanje);

        _service.ZavrsiIzdavanje(1, "admin");

        Assert.Equal("Zavrsena", izdavanje.Status);
        Assert.NotNull(izdavanje.DatumZavrsenja);
        _izdavanjeRepoMock.Verify(r => r.Update(izdavanje), Times.Once);
    }

    [Fact]
    public void ZavrsiIzdavanje_AlreadyClosed_ThrowsInvalidOperation()
    {
        var izdavanje = new IzdavanjeMaterijala { IzdavanjeID = 1, Status = "Zavrsena" };
        _izdavanjeRepoMock.Setup(r => r.GetById(1)).Returns(izdavanje);

        Assert.Throws<InvalidOperationException>(() =>
            _service.ZavrsiIzdavanje(1, "admin"));
    }

    [Fact]
    public void ZavrsiIzdavanje_NonExistent_ThrowsInvalidOperation()
    {
        _izdavanjeRepoMock.Setup(r => r.GetById(999)).Returns((IzdavanjeMaterijala?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.ZavrsiIzdavanje(999, "admin"));
    }
}
