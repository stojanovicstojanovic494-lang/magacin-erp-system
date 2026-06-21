using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface IInventuraRepository
{
    Inventura? GetById(int id);
    IEnumerable<Inventura> GetAll();
    void Add(Inventura inventura);
    void Update(Inventura inventura);
    void AddStavka(InventuraStavka stavka);
}
