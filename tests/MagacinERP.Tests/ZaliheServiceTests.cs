using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;
using MagacinERP.Services;
using Moq;

namespace MagacinERP.Tests;

public class ZaliheServiceTests
{
    private readonly Mock<IZaliheRepository> _zaliheRepoMock;
    private readonly Mock<IArtikalRepository> _artikalRepoMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly ZaliheService _service;

    public ZaliheServiceTests()
    {
        _zaliheRepoMock = new Mock<IZaliheRepository>();
        _artikalRepoMock = new Mock<IArtikalRepository>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _service = new ZaliheService(_zaliheRepoMock.Object, _artikalRepoMock.Object, _auditLogRepoMock.Object);
    }

    [Fact]
    public void AzurirajZaliheNakonPrijeme_NewEntry_AddsZaliha()
    {
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns((Zaliha?)null);

        _service.AzurirajZaliheNakonPrijeme(1, 1, 50, "admin");

        _zaliheRepoMock.Verify(r => r.Add(It.Is<Zaliha>(z =>
            z.ArtikalID == 1 && z.LokacijaID == 1 && z.Kolicina == 50 && z.DisponibilnaKolicina == 50)), Times.Once);
        _auditLogRepoMock.Verify(r => r.Add(It.IsAny<AuditLog>()), Times.Once);
    }

    [Fact]
    public void AzurirajZaliheNakonPrijeme_ExistingEntry_UpdatesQuantity()
    {
        var existing = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 100, RezervovanoKolicina = 10, DisponibilnaKolicina = 90 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(existing);

        _service.AzurirajZaliheNakonPrijeme(1, 1, 30, "admin");

        Assert.Equal(130, existing.Kolicina);
        Assert.Equal(120, existing.DisponibilnaKolicina);
        _zaliheRepoMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public void AzurirajZaliheNakonPrijeme_ZeroQuantity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.AzurirajZaliheNakonPrijeme(1, 1, 0, "admin"));
    }

    [Fact]
    public void AzurirajZaliheNakonPrijeme_NegativeQuantity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.AzurirajZaliheNakonPrijeme(1, 1, -5, "admin"));
    }

    [Fact]
    public void AzurirajZaliheNakonPrijeme_EmptyKorisnik_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.AzurirajZaliheNakonPrijeme(1, 1, 10, ""));
    }

    [Fact]
    public void SmanjiZalihe_SufficientStock_ReducesQuantity()
    {
        var existing = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 100, RezervovanoKolicina = 10, DisponibilnaKolicina = 90 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(existing);

        _service.SmanjiZalihe(1, 1, 30, "admin");

        Assert.Equal(70, existing.Kolicina);
        Assert.Equal(60, existing.DisponibilnaKolicina);
        _zaliheRepoMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public void SmanjiZalihe_InsufficientStock_ThrowsInvalidOperation()
    {
        var existing = new Zaliha { ArtikalID = 1, LokacijaID = 1, Kolicina = 10, RezervovanoKolicina = 0, DisponibilnaKolicina = 10 };
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns(existing);

        Assert.Throws<InvalidOperationException>(() =>
            _service.SmanjiZalihe(1, 1, 20, "admin"));
    }

    [Fact]
    public void SmanjiZalihe_NonExistentZaliha_ThrowsInvalidOperation()
    {
        _zaliheRepoMock.Setup(r => r.GetByArtikalAndLokacija(1, 1)).Returns((Zaliha?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.SmanjiZalihe(1, 1, 10, "admin"));
    }

    [Fact]
    public void SmanjiZalihe_ZeroQuantity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.SmanjiZalihe(1, 1, 0, "admin"));
    }

    [Fact]
    public void GetArtikliSaNiskomZalijhom_ReturnsArticlesBelowMinimum()
    {
        var artikli = new List<Artikal>
        {
            new() { ArtikalID = 1, MinimalneStalje = 50, Aktivan = true },
            new() { ArtikalID = 2, MinimalneStalje = 100, Aktivan = true }
        };
        _artikalRepoMock.Setup(r => r.GetActive()).Returns(artikli);
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha> { new() { Kolicina = 30 } });
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(2)).Returns(new List<Zaliha> { new() { Kolicina = 200 } });

        var result = _service.GetArtikliSaNiskomZalijhom().ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].ArtikalID);
    }

    [Fact]
    public void GetArtikliSaNiskomZalijhom_ExactMinimum_IncludesArticle()
    {
        var artikli = new List<Artikal> { new() { ArtikalID = 1, MinimalneStalje = 50, Aktivan = true } };
        _artikalRepoMock.Setup(r => r.GetActive()).Returns(artikli);
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha> { new() { Kolicina = 50 } });

        var result = _service.GetArtikliSaNiskomZalijhom().ToList();

        Assert.Single(result);
    }

    [Fact]
    public void GetVrednostZalihe_CalculatesCorrectly()
    {
        _artikalRepoMock.Setup(r => r.GetById(1)).Returns(new Artikal { ArtikalID = 1, CenaKupovine = 100m });
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha>
        {
            new() { Kolicina = 50 },
            new() { Kolicina = 30 }
        });

        var vrednost = _service.GetVrednostZalihe(1);

        Assert.Equal(8000m, vrednost);
    }

    [Fact]
    public void GetVrednostZalihe_NonExistentArtikal_ThrowsArgumentException()
    {
        _artikalRepoMock.Setup(r => r.GetById(999)).Returns((Artikal?)null);

        Assert.Throws<ArgumentException>(() => _service.GetVrednostZalihe(999));
    }

    [Fact]
    public void GetVrednostZalihe_NullPrice_ReturnsZero()
    {
        _artikalRepoMock.Setup(r => r.GetById(1)).Returns(new Artikal { ArtikalID = 1, CenaKupovine = null });
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha> { new() { Kolicina = 100 } });

        var vrednost = _service.GetVrednostZalihe(1);

        Assert.Equal(0m, vrednost);
    }

    [Fact]
    public void ABCAnaliza_CategorizesCorrectly()
    {
        var artikli = new List<Artikal>
        {
            new() { ArtikalID = 1, SifraArtikla = "A1", NazivArtikla = "High", CenaKupovine = 1000m, Aktivan = true },
            new() { ArtikalID = 2, SifraArtikla = "A2", NazivArtikla = "Low", CenaKupovine = 10m, Aktivan = true }
        };
        _artikalRepoMock.Setup(r => r.GetActive()).Returns(artikli);
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha> { new() { Kolicina = 100 } });
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(2)).Returns(new List<Zaliha> { new() { Kolicina = 100 } });

        var result = _service.ABCAnaliza().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("A - Kritična", result[0].Kategorija);
        Assert.Equal("C - Ostalo", result[1].Kategorija);
    }

    [Fact]
    public void ABCAnaliza_NoStock_ReturnsEmptyCategories()
    {
        _artikalRepoMock.Setup(r => r.GetActive()).Returns(new List<Artikal>());

        var result = _service.ABCAnaliza().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void ABCAnaliza_ZeroTotalValue_ReturnsResultsWithoutPercentage()
    {
        var artikli = new List<Artikal>
        {
            new() { ArtikalID = 1, SifraArtikla = "A1", NazivArtikla = "Free", CenaKupovine = 0m, Aktivan = true }
        };
        _artikalRepoMock.Setup(r => r.GetActive()).Returns(artikli);
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha> { new() { Kolicina = 100 } });

        var result = _service.ABCAnaliza().ToList();

        Assert.Single(result);
        Assert.Equal(0m, result[0].ProcenatVrednosti);
    }

    [Fact]
    public void GetDostupnaKolicina_SumsDisponibilna()
    {
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha>
        {
            new() { DisponibilnaKolicina = 50 },
            new() { DisponibilnaKolicina = 30 }
        });

        var result = _service.GetDostupnaKolicina(1);

        Assert.Equal(80, result);
    }

    [Fact]
    public void GetDostupnaKolicina_NoZalihe_ReturnsZero()
    {
        _zaliheRepoMock.Setup(r => r.GetByArtikalId(1)).Returns(new List<Zaliha>());

        var result = _service.GetDostupnaKolicina(1);

        Assert.Equal(0, result);
    }
}
