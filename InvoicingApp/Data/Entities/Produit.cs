using System.ComponentModel.DataAnnotations;

namespace InvoicingApp.Models;

public class Produit
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = "";

    [Range(0, double.MaxValue)]
    public decimal PrixHT { get; set; }

    [Range(0, 1)]
    public decimal TauxTVA { get; set; }

    public ICollection<LigneFacture> LignesFacture { get; set; } = new List<LigneFacture>();
}