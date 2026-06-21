namespace MagacinERP.Core.Models;

public class IzdavanjeMaterijala
{
    public int IzdavanjeID { get; set; }
    public string BrojDokumenta { get; set; } = string.Empty;
    public DateTime DatumIzdavanja { get; set; } = DateTime.Now;
    public string? TipIzdavanja { get; set; }
    public string? Napomena { get; set; }
    public string Status { get; set; } = "Otvorena";
    public string? Korisnik { get; set; }
    public DateTime? DatumZavrsenja { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
    public List<IzdavanjeStavka> Stavke { get; set; } = new();
}

public class IzdavanjeStavka
{
    public int IzdavanjeStavkaID { get; set; }
    public int IzdavanjeID { get; set; }
    public int ArtikalID { get; set; }
    public int KolicinaTrazena { get; set; }
    public int KolicinaIzdata { get; set; }
    public decimal? CenaJedinice { get; set; }
    public string? Napomena { get; set; }
    public int? LotID { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
}
