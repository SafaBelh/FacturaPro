using System.ComponentModel.DataAnnotations;

namespace InvoicingApp.Models;

public class Client
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = "";

    [StringLength(200)]
    public string Adresse { get; set; } = "";

    [StringLength(50)]
    public string MatriculeFiscale { get; set; } = "";

    public ICollection<Facture> Factures { get; set; } = new List<Facture>();
}