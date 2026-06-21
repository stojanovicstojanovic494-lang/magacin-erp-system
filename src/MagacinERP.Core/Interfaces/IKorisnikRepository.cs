using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface IKorisnikRepository
{
    Korisnik? GetById(int id);
    Korisnik? GetByKorisnickoIme(string korisnickoIme);
    IEnumerable<Korisnik> GetAll();
    void Add(Korisnik korisnik);
    void Update(Korisnik korisnik);
}
