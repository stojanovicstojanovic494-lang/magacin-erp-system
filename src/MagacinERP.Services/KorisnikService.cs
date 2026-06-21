using MagacinERP.Core.Interfaces;
using MagacinERP.Core.Models;

namespace MagacinERP.Services;

public class KorisnikService
{
    private readonly IKorisnikRepository _korisnikRepository;

    public KorisnikService(IKorisnikRepository korisnikRepository)
    {
        _korisnikRepository = korisnikRepository;
    }

    public (int KorisnikID, string? Uloga) ValidacijaKorisnika(string korisnickoIme, string lozinka)
    {
        if (string.IsNullOrWhiteSpace(korisnickoIme))
            throw new ArgumentException("Korisničko ime je obavezno.", nameof(korisnickoIme));

        if (string.IsNullOrWhiteSpace(lozinka))
            throw new ArgumentException("Lozinka je obavezna.", nameof(lozinka));

        var korisnik = _korisnikRepository.GetByKorisnickoIme(korisnickoIme);

        if (korisnik == null || korisnik.Lozinka != lozinka || !korisnik.Aktivan)
            return (-1, null);

        return (korisnik.KorisnikID, korisnik.Uloga);
    }

    public void DodajKorisnika(string korisnickoIme, string lozinka, string ime, string prezime, string uloga, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(korisnickoIme))
            throw new ArgumentException("Korisničko ime je obavezno.", nameof(korisnickoIme));

        if (string.IsNullOrWhiteSpace(lozinka))
            throw new ArgumentException("Lozinka je obavezna.", nameof(lozinka));

        if (lozinka.Length < 6)
            throw new ArgumentException("Lozinka mora imati najmanje 6 karaktera.", nameof(lozinka));

        if (string.IsNullOrWhiteSpace(ime))
            throw new ArgumentException("Ime je obavezno.", nameof(ime));

        if (string.IsNullOrWhiteSpace(prezime))
            throw new ArgumentException("Prezime je obavezno.", nameof(prezime));

        var validneUloge = new[] { "Admin", "Magaciner", "Pregled" };
        if (!validneUloge.Contains(uloga))
            throw new ArgumentException("Nevalidna uloga. Dozvoljene: Admin, Magaciner, Pregled.", nameof(uloga));

        var existing = _korisnikRepository.GetByKorisnickoIme(korisnickoIme);
        if (existing != null)
            throw new InvalidOperationException("Korisničko ime već postoji.");

        var korisnik = new Korisnik
        {
            KorisnickoIme = korisnickoIme,
            Lozinka = lozinka,
            ImeKorisnika = ime,
            Prezime = prezime,
            Email = email,
            Uloga = uloga,
            Aktivan = true
        };

        _korisnikRepository.Add(korisnik);
    }

    public void DeaktivirajKorisnika(int korisnikId)
    {
        var korisnik = _korisnikRepository.GetById(korisnikId);
        if (korisnik == null)
            throw new InvalidOperationException("Korisnik ne postoji.");

        korisnik.Aktivan = false;
        korisnik.DatumIzmene = DateTime.Now;
        _korisnikRepository.Update(korisnik);
    }
}
