namespace MagacinERP.Core.Models;

public class Artikal
{
    public int ArtikalID { get; set; }
    public string SifraArtikla { get; set; } = string.Empty;
    public string NazivArtikla { get; set; } = string.Empty;
    public int KategorijaID { get; set; }
    public int JedinicaMereID { get; set; }
    public decimal? CenaKupovine { get; set; }
    public decimal? CenaProdaje { get; set; }
    public int MinimalneStalje { get; set; } = 10;
    public int MaksimalneStalje { get; set; } = 1000;
    public decimal? Tezina { get; set; }
    public decimal? Zapremina { get; set; }
    public string? Opis { get; set; }
    public string? Barkod { get; set; }
    public bool Aktivan { get; set; } = true;
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
}
