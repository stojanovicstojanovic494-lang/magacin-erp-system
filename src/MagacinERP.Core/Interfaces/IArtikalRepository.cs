using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface IArtikalRepository
{
    Artikal? GetById(int id);
    Artikal? GetBySifra(string sifra);
    IEnumerable<Artikal> GetAll();
    IEnumerable<Artikal> GetActive();
    void Add(Artikal artikal);
    void Update(Artikal artikal);
    void Delete(int id);
}
