namespace MagacinERP.Core.Models;

public class Dobavljac
{
    public int DobavljacID { get; set; }
    public string NazivFirme { get; set; } = string.Empty;
    public string? KontaktOsoba { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public string? Adresa { get; set; }
    public string? Grad { get; set; }
    public string? PostanskiBroj { get; set; }
    public string? Drzava { get; set; }
    public string? PIB { get; set; }
    public string? MaticniBroj { get; set; }
    public bool Aktivan { get; set; } = true;
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
}
