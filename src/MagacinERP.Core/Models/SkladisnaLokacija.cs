namespace MagacinERP.Core.Models;

public class SkladisnaLokacija
{
    public int LokacijaID { get; set; }
    public int ZonaID { get; set; }
    public int Red { get; set; }
    public int Polica { get; set; }
    public string? Opis { get; set; }
    public bool JeLiSlobodna { get; set; } = true;
    public bool Aktivna { get; set; } = true;
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
}
