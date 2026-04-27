using System.ComponentModel.DataAnnotations;

namespace InvoicingApp.Models;

public class Facture
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public decimal TimbreFiscal { get; set; } = 1.0m; 

    public ICollection<LigneFacture> Lignes { get; set; } = new List<LigneFacture>();

    public decimal TotalHT => Lignes.Sum(l => l.MontantHT);
    public decimal TotalTVA => Lignes.Sum(l => l.MontantTVA);
    public decimal TotalTTC => TotalHT + TotalTVA + TimbreFiscal;
}