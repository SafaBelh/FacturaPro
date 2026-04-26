using System.ComponentModel.DataAnnotations;

namespace InvoicingApp.Models;

public class LigneFacture
{
    [Key]
    public int Id { get; set; }

    public int FactureId { get; set; }
    public Facture Facture { get; set; } = null!;

    public int ProduitId { get; set; }
    public Produit Produit { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int Quantite { get; set; } = 1;

    public decimal PrixUnitaireHT { get; set; }
    public decimal TauxTVA { get; set; }

    public decimal MontantHT => Quantite * PrixUnitaireHT;
    public decimal MontantTVA => MontantHT * TauxTVA;
}