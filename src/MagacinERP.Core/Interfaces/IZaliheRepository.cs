using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface IZaliheRepository
{
    Zaliha? GetByArtikalAndLokacija(int artikalId, int lokacijaId);
    IEnumerable<Zaliha> GetByArtikalId(int artikalId);
    IEnumerable<Zaliha> GetAll();
    void Add(Zaliha zaliha);
    void Update(Zaliha zaliha);
}
