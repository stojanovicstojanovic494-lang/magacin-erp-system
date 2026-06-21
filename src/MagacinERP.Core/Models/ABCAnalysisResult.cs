namespace MagacinERP.Core.Models;

public class ABCAnalysisResult
{
    public int ArtikalID { get; set; }
    public string SifraArtikla { get; set; } = string.Empty;
    public string NazivArtikla { get; set; } = string.Empty;
    public int Kolicina { get; set; }
    public decimal CenaKupovine { get; set; }
    public decimal Vrednost { get; set; }
    public decimal ProcenatVrednosti { get; set; }
    public string Kategorija { get; set; } = string.Empty;
}
