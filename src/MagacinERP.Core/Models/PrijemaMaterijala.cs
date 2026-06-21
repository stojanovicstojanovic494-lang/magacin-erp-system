namespace MagacinERP.Core.Models;

public class PrijemaMaterijala
{
    public int PrijemaID { get; set; }
    public string BrojDokumenta { get; set; } = string.Empty;
    public DateTime DatumPrijeme { get; set; } = DateTime.Now;
    public int DobavljacID { get; set; }
    public string? Napomena { get; set; }
    public string Status { get; set; } = "Otvorena";
    public string? Korisnik { get; set; }
    public DateTime? DatumZavrsenja { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
    public List<PrijemaStavka> Stavke { get; set; } = new();
}

public class PrijemaStavka
{
    public int PrijemaStavkaID { get; set; }
    public int PrijemaID { get; set; }
    public int ArtikalID { get; set; }
    public int KolicinaNarudjena { get; set; }
    public int KolicinaPrimljena { get; set; }
    public decimal? CenaJedinice { get; set; }
    public string? Napomena { get; set; }
    public string? LotBroj { get; set; }
    public DateTime? DatumRoka { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
}
