namespace MagacinERP.Core.Models;

public class Korisnik
{
    public int KorisnikID { get; set; }
    public string KorisnickoIme { get; set; } = string.Empty;
    public string Lozinka { get; set; } = string.Empty;
    public string ImeKorisnika { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Uloga { get; set; }
    public bool Aktivan { get; set; } = true;
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
}
