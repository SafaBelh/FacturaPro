using Microsoft.EntityFrameworkCore;
using InvoicingApp.Data.EFCore;
using InvoicingApp.Models;

namespace InvoicingApp.Services;

public class FactureService : IFactureService
{
    private readonly AppDbContext _context;

    public FactureService(AppDbContext context)
    {
        _context = context;
    }

    // 🟣🟣🟣 BASIC CRUD LOGIC 🟣🟣🟣 //
    public async Task<List<Facture>> GetFacturesAsync()
    {
        return await _context.Factures
            .Include(f => f.Client)
            .Include(f => f.Lignes)
                .ThenInclude(l => l.Produit)
            .ToListAsync();
    }

    public async Task<Facture?> GetFactureByIdAsync(int id)
    {
        return await _context.Factures
            .Include(f => f.Client)
            .Include(f => f.Lignes)
                .ThenInclude(l => l.Produit)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task AddFactureAsync(Facture facture, List<LigneFacture> lignes)
    {
        foreach (var ligne in lignes)
        {
            ligne.Facture = facture;
            _context.LignesFacture.Add(ligne);
        }
        _context.Factures.Add(facture);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteFactureAsync(int id)
    {
        var facture = await _context.Factures.FindAsync(id);
        if (facture != null)
        {
            _context.Factures.Remove(facture);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateFactureAsync(Facture facture, List<LigneFacture> lignes)
    {
        
        var existingFacture = await _context.Factures.FindAsync(facture.Id);
        if (existingFacture == null)
            throw new InvalidOperationException("Facture non trouvée.");

        
        existingFacture.ClientId = facture.ClientId;
        existingFacture.Date = facture.Date;
        existingFacture.TimbreFiscal = facture.TimbreFiscal;

        
        var existingLines = _context.LignesFacture.Where(l => l.FactureId == facture.Id);
        _context.LignesFacture.RemoveRange(existingLines);

        
        foreach (var ligne in lignes)
        {
            ligne.FactureId = facture.Id;
            ligne.Id = 0;               
            _context.LignesFacture.Add(ligne);
        }

        await _context.SaveChangesAsync();
    }
    
    
    
    // 🟣🟣🟣 ANALYTICS - KPIs LOGIC 🟣🟣🟣 //
    public async Task<decimal> GetTotalTVAAsync()
    {
        return await _context.LignesFacture
            .SumAsync(l => l.Quantite * l.PrixUnitaireHT * l.TauxTVA);
    }

    public async Task<Dictionary<decimal, decimal>> GetTVAParTauxAsync()
    {
        return await _context.LignesFacture
            .GroupBy(l => l.TauxTVA)
            .Select(g => new { Taux = g.Key, Total = g.Sum(l => l.Quantite * l.PrixUnitaireHT * l.TauxTVA) })
            .ToDictionaryAsync(k => k.Taux, v => v.Total);
    }

    public async Task<decimal> GetTotalTimbreFiscalAsync()
    {
        return await _context.Factures.SumAsync(f => f.TimbreFiscal);
    }

    public async Task<decimal> GetChiffreAffairesHTAsync()
    {
        return await _context.LignesFacture
            .SumAsync(l => l.Quantite * l.PrixUnitaireHT);
    }

    public async Task<Dictionary<string, decimal>> GetCAHTPerClientAsync()
    {
        return await _context.LignesFacture
            .Include(l => l.Facture)
                .ThenInclude(f => f.Client)
            .Where(l => l.Facture.Client != null)
            .GroupBy(l => l.Facture.Client.Nom)
            .Select(g => new { Client = g.Key, Total = g.Sum(l => l.Quantite * l.PrixUnitaireHT) })
            .ToDictionaryAsync(k => k.Client, v => v.Total);
    }

    public async Task<Dictionary<int, decimal>> GetCAHTPerMonthAsync()
    {
        return await _context.LignesFacture
            .Include(l => l.Facture)
            .GroupBy(l => l.Facture.Date.Month)
            .Select(g => new { Month = g.Key, Total = g.Sum(l => l.Quantite * l.PrixUnitaireHT) })
            .ToDictionaryAsync(k => k.Month, v => v.Total);
    }

    public async Task<Dictionary<string, decimal>> GetSalesPerProductAsync()
    {
        return await _context.LignesFacture
            .Include(l => l.Produit)
            .GroupBy(l => l.Produit.Nom)
            .Select(g => new { Product = g.Key, Total = g.Sum(l => l.Quantite * l.PrixUnitaireHT) })
            .ToDictionaryAsync(k => k.Product, v => v.Total);
    }

    public async Task<decimal> GetChiffreAffairesTTCAsync()
    {
        var caTTC = await _context.LignesFacture
            .SumAsync(l => l.Quantite * l.PrixUnitaireHT * (1 + l.TauxTVA));
        var totalTimbre = await _context.Factures.SumAsync(f => f.TimbreFiscal);
        return caTTC + totalTimbre;
    }

    public async Task<Dictionary<string, decimal>> GetTTCCPerClientAsync()
    {
        return await _context.LignesFacture
            .Include(l => l.Facture)
                .ThenInclude(f => f.Client)
            .Where(l => l.Facture.Client != null)
            .GroupBy(l => l.Facture.Client.Nom)
            .Select(g => new { Client = g.Key, Total = g.Sum(l => l.Quantite * l.PrixUnitaireHT * (1 + l.TauxTVA)) })
            .ToDictionaryAsync(k => k.Client, v => v.Total);
    }

    public async Task<Dictionary<int, decimal>> GetTTCCPerMonthAsync()
    {
        return await _context.LignesFacture
            .Include(l => l.Facture)
            .GroupBy(l => l.Facture.Date.Month)
            .Select(g => new { Month = g.Key, Total = g.Sum(l => l.Quantite * l.PrixUnitaireHT * (1 + l.TauxTVA)) })
            .ToDictionaryAsync(k => k.Month, v => v.Total);
    }

    public async Task<Dictionary<string, decimal>> GetTTCCPerProductAsync()
    {
        return await _context.LignesFacture
            .Include(l => l.Produit)
            .GroupBy(l => l.Produit.Nom)
            .Select(g => new { Product = g.Key, Total = g.Sum(l => l.Quantite * l.PrixUnitaireHT * (1 + l.TauxTVA)) })
            .ToDictionaryAsync(k => k.Product, v => v.Total);
    }


}