namespace MagacinERP.Core.Models;

public class Zaliha
{
    public int ZalihaID { get; set; }
    public int ArtikalID { get; set; }
    public int LokacijaID { get; set; }
    public int Kolicina { get; set; }
    public int RezervovanoKolicina { get; set; }
    public int DisponibilnaKolicina { get; set; }
    public DateTime DatumZadnjeIzmene { get; set; } = DateTime.Now;
}
