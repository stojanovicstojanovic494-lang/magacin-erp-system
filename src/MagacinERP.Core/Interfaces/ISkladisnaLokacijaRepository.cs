using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface ISkladisnaLokacijaRepository
{
    SkladisnaLokacija? GetById(int id);
    IEnumerable<SkladisnaLokacija> GetAll();
    IEnumerable<SkladisnaLokacija> GetDostupne();
    void Update(SkladisnaLokacija lokacija);
}
