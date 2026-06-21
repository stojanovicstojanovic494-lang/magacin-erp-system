using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;
using MagacinERP.Services;
using Moq;

namespace MagacinERP.Tests;

public class InventuraServiceTests
{
    private readonly Mock<IInventuraRepository> _inventuraRepoMock;
    private readonly Mock<IZaliheRepository> _zaliheRepoMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly InventuraService _service;

    public InventuraServiceTests()
    {
        _inventuraRepoMock = new Mock<IInventuraRepository>();
        _zaliheRepoMock = new Mock<IZaliheRepository>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _service = new InventuraService(_inventuraRepoMock.Object, _zaliheRepoMock.Object, _auditLogRepoMock.Object);
    }

    [Fact]
    public void DodajInventuru_ValidInput_CreatesInventura()
    {
        _service.DodajInventuru("INV-001", "Potpuna", "admin", "Godišnja inventura");

        _inventuraRepoMock.Verify(r => r.Add(It.Is<Inventura>(i =>
            i.BrojDokumenta == "INV-001" && i.TipInventure == "Potpuna" && i.Status == "U_toku")), Times.Once);
        _auditLogRepoMock.Verify(r => r.Add(It.IsAny<AuditLog>()), Times.Once);
    }

    [Fact]
    public void DodajInventuru_EmptyBrojDokumenta_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajInventuru("", "Potpuna", "admin"));
    }

    [Fact]
    public void DodajInventuru_EmptyTipInventure_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajInventuru("INV-001", "", "admin"));
    }

    [Fact]
    public void DodajInventuru_InvalidTipInventure_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajInventuru("INV-001", "Nevalidan", "admin"));
    }

    [Theory]
    [InlineData("Parcijalna")]
    [InlineData("Potpuna")]
    public void DodajInventuru_ValidTypes_Succeeds(string tip)
    {
        _service.DodajInventuru("INV-001", tip, "admin");

        _inventuraRepoMock.Verify(r => r.Add(It.Is<Inventura>(i => i.TipInventure == tip)), Times.Once);
    }

    [Fact]
    public void DodajInventuru_EmptyKorisnik_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajInventuru("INV-001", "Potpuna", ""));
    }

    [Fact]
    public void DodajStavku_ValidInput_AddsStavkaWithDifference()
    {
        var inventura = new Inventura { InventuraID = 1, Status = "U_toku" };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);

        var zaliha = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 100 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(zaliha);

        _service.DodajStavku(1, 1, 1, 95, "admin");

        _inventuraRepoMock.Verify(r => r.AddStavka(It.Is<InventuraStavka>(s =>
            s.UtvrdjenaKolicina == 95 && s.SistemskaBaza == 100 && s.Razlika == -5)), Times.Once);
    }

    [Fact]
    public void DodajStavku_NoExistingZaliha_SystemBaseIsZero()
    {
        var inventura = new Inventura { InventuraID = 1, Status = "U_toku" };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns((Zaliha?)null);

        _service.DodajStavku(1, 1, 1, 50, "admin");

        _inventuraRepoMock.Verify(r => r.AddStavka(It.Is<InventuraStavka>(s =>
            s.SistemskaBaza == 0 && s.Razlika == 50)), Times.Once);
    }

    [Fact]
    public void DodajStavku_NonExistentInventura_ThrowsInvalidOperation()
    {
        _inventuraRepoMock.Setup(r => r.GetById(999)).Returns((Inventura?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajStavku(999, 1, 1, 50, "admin"));
    }

    [Fact]
    public void DodajStavku_ClosedInventura_ThrowsInvalidOperation()
    {
        var inventura = new Inventura { InventuraID = 1, Status = "Zavrsena" };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajStavku(1, 1, 1, 50, "admin"));
    }

    [Fact]
    public void DodajStavku_NegativeKolicina_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(1, 1, 1, -5, "admin"));
    }

    [Fact]
    public void DodajStavku_InvalidInventuraId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajStavku(0, 1, 1, 50, "admin"));
    }

    [Fact]
    public void ZavrsiInventuru_WithDifferences_AppliesCorrections()
    {
        var inventura = new Inventura
        {
            InventuraID = 1,
            Status = "U_toku",
            Stavke = new List<InventuraStavka>
            {
                new() { ArtikalID = 1, LokacijaID = 1, UtvrdjenaKolicina = 80, Razlika = -20 }
            }
        };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);

        var zaliha = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 100, RezervovanoKolicina = 10, DisponibilnaKolicina = 90 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(zaliha);

        _service.ZavrsiInventuru(1, "admin", primenjiRazlike: true);

        Assert.Equal(80, zaliha.Kolicina);
        Assert.Equal(70, zaliha.DisponibilnaKolicina);
        Assert.Equal("Zavrsena", inventura.Status);
    }

    [Fact]
    public void ZavrsiInventuru_WithoutApplyingDifferences_DoesNotChangeZalihe()
    {
        var inventura = new Inventura
        {
            InventuraID = 1,
            Status = "U_toku",
            Stavke = new List<InventuraStavka>
            {
                new() { ArtikalID = 1, LokacijaID = 1, UtvrdjenaKolicina = 80, Razlika = -20 }
            }
        };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);

        _service.ZavrsiInventuru(1, "admin", primenjiRazlike: false);

        Assert.Equal("Zavrsena", inventura.Status);
        _zaliheRepoMock.Verify(r => r.Update(It.IsAny<Zaliha>()), Times.Never);
    }

    [Fact]
    public void ZavrsiInventuru_NoStavke_ThrowsInvalidOperation()
    {
        var inventura = new Inventura
        {
            InventuraID = 1,
            Status = "U_toku",
            Stavke = new List<InventuraStavka>()
        };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);

        Assert.Throws<InvalidOperationException>(() =>
            _service.ZavrsiInventuru(1, "admin"));
    }

    [Fact]
    public void ZavrsiInventuru_AlreadyClosed_ThrowsInvalidOperation()
    {
        var inventura = new Inventura { InventuraID = 1, Status = "Zavrsena" };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);

        Assert.Throws<InvalidOperationException>(() =>
            _service.ZavrsiInventuru(1, "admin"));
    }

    [Fact]
    public void ZavrsiInventuru_NonExistent_ThrowsInvalidOperation()
    {
        _inventuraRepoMock.Setup(r => r.GetById(999)).Returns((Inventura?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.ZavrsiInventuru(999, "admin"));
    }

    [Fact]
    public void ZavrsiInventuru_ApplyDifferences_NewZalihaCreated()
    {
        var inventura = new Inventura
        {
            InventuraID = 1,
            Status = "U_toku",
            Stavke = new List<InventuraStavka>
            {
                new() { ArtikalID = 5, LokacijaID = 3, UtvrdjenaKolicina = 25, Razlika = 25 }
            }
        };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(5, 3)).Returns((Zaliha?)null);

        _service.ZavrsiInventuru(1, "admin", primenjiRazlike: true);

        _zaliheRepoMock.Verify(r => r.Add(It.Is<Zaliha>(z =>
            z.ArtikalID == 5 && z.LokacijaID == 3 && z.Kolicina == 25)), Times.Once);
    }

    [Fact]
    public void ZavrsiInventuru_ZeroDifference_DoesNotUpdateZaliha()
    {
        var inventura = new Inventura
        {
            InventuraID = 1,
            Status = "U_toku",
            Stavke = new List<InventuraStavka>
            {
                new() { ArtikalID = 1, LokacijaID = 1, UtvrdjenaKolicina = 100, Razlika = 0 }
            }
        };
        _inventuraRepoMock.Setup(r => r.GetById(1)).Returns(inventura);

        _service.ZavrsiInventuru(1, "admin", primenjiRazlike: true);

        _zaliheRepoMock.Verify(r => r.Update(It.IsAny<Zaliha>()), Times.Never);
        _zaliheRepoMock.Verify(r => r.Add(It.IsAny<Zaliha>()), Times.Never);
    }
}
