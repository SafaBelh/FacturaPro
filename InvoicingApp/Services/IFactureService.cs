using InvoicingApp.Models;

namespace InvoicingApp.Services;

public interface IFactureService
{
    Task<List<Facture>> GetFacturesAsync();
    Task<Facture?> GetFactureByIdAsync(int id);
    Task AddFactureAsync(Facture facture, List<LigneFacture> lignes);
    Task DeleteFactureAsync(int id);

    // Dashboard – HT
    Task<decimal> GetTotalTVAAsync();
    Task<Dictionary<decimal, decimal>> GetTVAParTauxAsync();
    Task<decimal> GetTotalTimbreFiscalAsync();
    Task<decimal> GetChiffreAffairesHTAsync();
    Task<Dictionary<string, decimal>> GetCAHTPerClientAsync();
    Task<Dictionary<int, decimal>> GetCAHTPerMonthAsync();
    Task<Dictionary<string, decimal>> GetSalesPerProductAsync();

    // Dashboard – TTC (with timbre)
    Task<decimal> GetChiffreAffairesTTCAsync();
    Task<Dictionary<string, decimal>> GetTTCCPerClientAsync();
    Task<Dictionary<int, decimal>> GetTTCCPerMonthAsync();
    Task<Dictionary<string, decimal>> GetTTCCPerProductAsync();
    Task UpdateFactureAsync(Facture facture, List<LigneFacture> lignes);
}