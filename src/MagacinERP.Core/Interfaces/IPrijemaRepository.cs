using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface IPrijemaRepository
{
    PrijemaMaterijala? GetById(int id);
    IEnumerable<PrijemaMaterijala> GetAll();
    IEnumerable<PrijemaMaterijala> GetByStatus(string status);
    void Add(PrijemaMaterijala prijema);
    void Update(PrijemaMaterijala prijema);
    void AddStavka(PrijemaStavka stavka);
}
