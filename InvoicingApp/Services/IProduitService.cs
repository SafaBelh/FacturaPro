using InvoicingApp.Models;

namespace InvoicingApp.Services;

public interface IProduitService
{
    Task<List<Produit>> GetProduitsAsync();
    Task<Produit?> GetProduitByIdAsync(int id);
    Task AddProduitAsync(Produit produit);
    Task UpdateProduitAsync(Produit produit);
    Task DeleteProduitAsync(int id);
}