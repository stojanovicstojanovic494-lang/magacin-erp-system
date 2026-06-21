using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface IIzdavanjeRepository
{
    IzdavanjeMaterijala? GetById(int id);
    IEnumerable<IzdavanjeMaterijala> GetAll();
    IEnumerable<IzdavanjeMaterijala> GetByStatus(string status);
    void Add(IzdavanjeMaterijala izdavanje);
    void Update(IzdavanjeMaterijala izdavanje);
    void AddStavka(IzdavanjeStavka stavka);
}
