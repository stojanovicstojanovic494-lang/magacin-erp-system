using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;
using MagacinERP.Services;
using Moq;

namespace MagacinERP.Tests;

public class KorisnikServiceTests
{
    private readonly Mock<IKorisnikRepository> _korisnikRepoMock;
    private readonly KorisnikService _service;

    public KorisnikServiceTests()
    {
        _korisnikRepoMock = new Mock<IKorisnikRepository>();
        _service = new KorisnikService(_korisnikRepoMock.Object);
    }

    [Fact]
    public void ValidacijaKorisnika_ValidCredentials_ReturnsIdAndRole()
    {
        var korisnik = new Korisnik
        {
            KorisnikID = 1,
            KorisnickoIme = "admin",
            Lozinka = "admin123",
            Uloga = "Admin",
            Aktivan = true
        };
        _korisnikRepoMock.Setup(r => r.GetByKorisnickoIme("admin")).Returns(korisnik);

        var result = _service.ValidacijaKorisnika("admin", "admin123");

        Assert.Equal(1, result.KorisnikID);
        Assert.Equal("Admin", result.Uloga);
    }

    [Fact]
    public void ValidacijaKorisnika_InvalidPassword_ReturnsNegativeId()
    {
        var korisnik = new Korisnik
        {
            KorisnikID = 1,
            KorisnickoIme = "admin",
            Lozinka = "admin123",
            Uloga = "Admin",
            Aktivan = true
        };
        _korisnikRepoMock.Setup(r => r.GetByKorisnickoIme("admin")).Returns(korisnik);

        var result = _service.ValidacijaKorisnika("admin", "wrong_password");

        Assert.Equal(-1, result.KorisnikID);
        Assert.Null(result.Uloga);
    }

    [Fact]
    public void ValidacijaKorisnika_NonExistentUser_ReturnsNegativeId()
    {
        _korisnikRepoMock.Setup(r => r.GetByKorisnickoIme("unknown")).Returns((Korisnik?)null);

        var result = _service.ValidacijaKorisnika("unknown", "password");

        Assert.Equal(-1, result.KorisnikID);
        Assert.Null(result.Uloga);
    }

    [Fact]
    public void ValidacijaKorisnika_InactiveUser_ReturnsNegativeId()
    {
        var korisnik = new Korisnik
        {
            KorisnikID = 1,
            KorisnickoIme = "admin",
            Lozinka = "admin123",
            Uloga = "Admin",
            Aktivan = false
        };
        _korisnikRepoMock.Setup(r => r.GetByKorisnickoIme("admin")).Returns(korisnik);

        var result = _service.ValidacijaKorisnika("admin", "admin123");

        Assert.Equal(-1, result.KorisnikID);
        Assert.Null(result.Uloga);
    }

    [Fact]
    public void ValidacijaKorisnika_EmptyUsername_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.ValidacijaKorisnika("", "password"));
    }

    [Fact]
    public void ValidacijaKorisnika_EmptyPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.ValidacijaKorisnika("admin", ""));
    }

    [Fact]
    public void DodajKorisnika_ValidInput_CreatesKorisnik()
    {
        _korisnikRepoMock.Setup(r => r.GetByKorisnickoIme("newuser")).Returns((Korisnik?)null);

        _service.DodajKorisnika("newuser", "pass123", "Ime", "Prezime", "Magaciner", "test@test.rs");

        _korisnikRepoMock.Verify(r => r.Add(It.Is<Korisnik>(k =>
            k.KorisnickoIme == "newuser" && k.Uloga == "Magaciner" && k.Aktivan == true)), Times.Once);
    }

    [Fact]
    public void DodajKorisnika_DuplicateUsername_ThrowsInvalidOperation()
    {
        _korisnikRepoMock.Setup(r => r.GetByKorisnickoIme("existing")).Returns(new Korisnik { KorisnickoIme = "existing" });

        Assert.Throws<InvalidOperationException>(() =>
            _service.DodajKorisnika("existing", "pass123", "Ime", "Prezime", "Admin"));
    }

    [Fact]
    public void DodajKorisnika_ShortPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajKorisnika("user", "12345", "Ime", "Prezime", "Admin"));
    }

    [Fact]
    public void DodajKorisnika_EmptyUsername_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajKorisnika("", "pass123", "Ime", "Prezime", "Admin"));
    }

    [Fact]
    public void DodajKorisnika_EmptyPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajKorisnika("user", "", "Ime", "Prezime", "Admin"));
    }

    [Fact]
    public void DodajKorisnika_EmptyIme_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajKorisnika("user", "pass123", "", "Prezime", "Admin"));
    }

    [Fact]
    public void DodajKorisnika_EmptyPrezime_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajKorisnika("user", "pass123", "Ime", "", "Admin"));
    }

    [Fact]
    public void DodajKorisnika_InvalidUloga_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.DodajKorisnika("user", "pass123", "Ime", "Prezime", "SuperAdmin"));
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Magaciner")]
    [InlineData("Pregled")]
    public void DodajKorisnika_ValidRoles_Succeeds(string uloga)
    {
        _korisnikRepoMock.Setup(r => r.GetByKorisnickoIme(It.IsAny<string>())).Returns((Korisnik?)null);

        _service.DodajKorisnika("user", "pass123", "Ime", "Prezime", uloga);

        _korisnikRepoMock.Verify(r => r.Add(It.Is<Korisnik>(k => k.Uloga == uloga)), Times.Once);
    }

    [Fact]
    public void DeaktivirajKorisnika_ExistingUser_SetsInactive()
    {
        var korisnik = new Korisnik { KorisnikID = 1, Aktivan = true };
        _korisnikRepoMock.Setup(r => r.GetById(1)).Returns(korisnik);

        _service.DeaktivirajKorisnika(1);

        Assert.False(korisnik.Aktivan);
        _korisnikRepoMock.Verify(r => r.Update(korisnik), Times.Once);
    }

    [Fact]
    public void DeaktivirajKorisnika_NonExistentUser_ThrowsInvalidOperation()
    {
        _korisnikRepoMock.Setup(r => r.GetById(999)).Returns((Korisnik?)null);

        Assert.Throws<InvalidOperationException>(() =>
            _service.DeaktivirajKorisnika(999));
    }
}
