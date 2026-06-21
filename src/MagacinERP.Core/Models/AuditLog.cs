namespace MagacinERP.Core.Models;

public class AuditLog
{
    public int AuditID { get; set; }
    public string Tabela { get; set; } = string.Empty;
    public string? Akcija { get; set; }
    public string? PrimarniKljuc { get; set; }
    public string? StaraVrednost { get; set; }
    public string? NovaVrednost { get; set; }
    public string? Korisnik { get; set; }
    public DateTime DatumAkcije { get; set; } = DateTime.Now;
}
