using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface ITransferRepository
{
    Transfer? GetById(int id);
    IEnumerable<Transfer> GetAll();
    void Add(Transfer transfer);
    void Update(Transfer transfer);
    void AddStavka(TransferStavka stavka);
}
