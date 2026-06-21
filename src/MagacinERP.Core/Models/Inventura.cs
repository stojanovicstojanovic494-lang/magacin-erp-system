namespace MagacinERP.Core.Models;

public class Inventura
{
    public int InventuraID { get; set; }
    public string BrojDokumenta { get; set; } = string.Empty;
    public DateTime DatumInventure { get; set; } = DateTime.Now;
    public string? TipInventure { get; set; }
    public string? Napomena { get; set; }
    public string Status { get; set; } = "U_toku";
    public string? Korisnik { get; set; }
    public DateTime? DatumZavrsenja { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
    public List<InventuraStavka> Stavke { get; set; } = new();
}

public class InventuraStavka
{
    public int InventuraStavkaID { get; set; }
    public int InventuraID { get; set; }
    public int LokacijaID { get; set; }
    public int ArtikalID { get; set; }
    public int UtvrdjenaKolicina { get; set; }
    public int? SistemskaBaza { get; set; }
    public int? Razlika { get; set; }
    public string? Napomena { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
}
