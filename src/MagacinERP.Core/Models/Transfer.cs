namespace MagacinERP.Core.Models;

public class Transfer
{
    public int TransferID { get; set; }
    public string BrojDokumenta { get; set; } = string.Empty;
    public DateTime DatumTransfera { get; set; } = DateTime.Now;
    public int IzLokacijeID { get; set; }
    public int ULokacijuID { get; set; }
    public string? Napomena { get; set; }
    public string Status { get; set; } = "Otvorena";
    public string? Korisnik { get; set; }
    public DateTime? DatumZavrsenja { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIzmene { get; set; } = DateTime.Now;
    public List<TransferStavka> Stavke { get; set; } = new();
}

public class TransferStavka
{
    public int TransferStavkaID { get; set; }
    public int TransferID { get; set; }
    public int ArtikalID { get; set; }
    public int Kolicina { get; set; }
    public int? LotID { get; set; }
    public string? Napomena { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
}
