using Microsoft.EntityFrameworkCore;
using InvoicingApp.Models;
namespace InvoicingApp.Data.EFCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Produit> Produits { get; set; }
    public DbSet<Facture> Factures { get; set; }
    public DbSet<LigneFacture> LignesFacture { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure decimal precision (optional but recommended)
        modelBuilder.Entity<Produit>()
            .Property(p => p.PrixHT)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<LigneFacture>()
            .Property(l => l.PrixUnitaireHT)
            .HasColumnType("decimal(18,2)");
    }
}