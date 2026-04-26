using Microsoft.EntityFrameworkCore;
using InvoicingApp.Components;
using InvoicingApp.Data.EFCore;
using InvoicingApp.Services;
using Radzen;
using InvoicingApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register DbContext with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Register custom services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<IFactureService, FactureService>();
builder.Services.AddSingleton<ITimbreFiscalService, TimbreFiscalService>();
builder.Services.AddSingleton<AuthService>();

// Register Radzen components
builder.Services.AddRadzenComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Clients.Any())
    {
        // 1. Clients (5 clients)
        var clients = new[]
        {
            new Client { Nom = "Société Médina Tech", Adresse = "Rue de Carthage, Tunis", MatriculeFiscale = "1234567/X/N/001" },
            new Client { Nom = "Olivia Distribution", Adresse = "Sfax, Tunisie", MatriculeFiscale = "1112223/A/M/002" },
            new Client { Nom = "Dar El Founoun", Adresse = "Sidi Bou Said, Tunis", MatriculeFiscale = "5556667/B/N/004" },
            new Client { Nom = "Hammamet Resort Group", Adresse = "Hammamet Sud", MatriculeFiscale = "4443332/C/M/007" },
            new Client { Nom = "Nour Electronics", Adresse = "Ariana, Tunis", MatriculeFiscale = "9876543/D/N/009" }
        };
        context.Clients.AddRange(clients);
        context.SaveChanges();

        // 2. Products (6 products)
        var produits = new[]
        {
            new Produit { Nom = "Développement Blazor (jour)", PrixHT = 750m, TauxTVA = 0.19m },
            new Produit { Nom = "Design UI/UX (forfait)", PrixHT = 1200m, TauxTVA = 0.19m },
            new Produit { Nom = "Hébergement Cloud / mois", PrixHT = 89m, TauxTVA = 0.19m },
            new Produit { Nom = "Formation interne (heure)", PrixHT = 120m, TauxTVA = 0.13m },
            new Produit { Nom = "Audit sécurité", PrixHT = 2500m, TauxTVA = 0.19m },
            new Produit { Nom = "Support technique (forfait)", PrixHT = 300m, TauxTVA = 0.19m }
        };
        context.Produits.AddRange(produits);
        context.SaveChanges();

        // Helper to get product by name
        var p = produits.ToDictionary(p => p.Nom);

        // 3. Invoices (8 invoices with different dates and statuses)
        var invoices = new List<Facture>();
        var random = new Random();

        // Helper to add invoice with random lines
        void AddInvoice(Client client, int daysAgo, decimal timbreFiscal = 1.0m, string status = "sent")
        {
            var facture = new Facture
            {
                Date = DateTime.Now.AddDays(-daysAgo),
                ClientId = client.Id,
                TimbreFiscal = timbreFiscal,
                Lignes = new List<LigneFacture>()
            };
            invoices.Add(facture);
            context.Factures.Add(facture);
            // We'll add lines after the facture is saved (to get Id) but we can add directly
        }

        // Create invoices with varied dates and products
        var clientList = clients.ToList();
        
        // Invoice 1 (client1) – 40 days ago, 2 products
        var inv1 = new Facture { Date = DateTime.Now.AddDays(-40), ClientId = clientList[0].Id, TimbreFiscal = 1.0m };
        inv1.Lignes.Add(new LigneFacture { ProduitId = p["Développement Blazor (jour)"].Id, Quantite = 8, PrixUnitaireHT = p["Développement Blazor (jour)"].PrixHT, TauxTVA = p["Développement Blazor (jour)"].TauxTVA });
        inv1.Lignes.Add(new LigneFacture { ProduitId = p["Hébergement Cloud / mois"].Id, Quantite = 3, PrixUnitaireHT = p["Hébergement Cloud / mois"].PrixHT, TauxTVA = p["Hébergement Cloud / mois"].TauxTVA });
        context.Factures.Add(inv1);

        // Invoice 2 (client2) – 30 days ago, 2 products
        var inv2 = new Facture { Date = DateTime.Now.AddDays(-30), ClientId = clientList[1].Id, TimbreFiscal = 1.0m };
        inv2.Lignes.Add(new LigneFacture { ProduitId = p["Design UI/UX (forfait)"].Id, Quantite = 1, PrixUnitaireHT = p["Design UI/UX (forfait)"].PrixHT, TauxTVA = p["Design UI/UX (forfait)"].TauxTVA });
        inv2.Lignes.Add(new LigneFacture { ProduitId = p["Formation interne (heure)"].Id, Quantite = 12, PrixUnitaireHT = p["Formation interne (heure)"].PrixHT, TauxTVA = p["Formation interne (heure)"].TauxTVA });
        context.Factures.Add(inv2);

        // Invoice 3 (client3) – 25 days ago, 1 product (paid)
        var inv3 = new Facture { Date = DateTime.Now.AddDays(-25), ClientId = clientList[2].Id, TimbreFiscal = 1.0m };
        inv3.Lignes.Add(new LigneFacture { ProduitId = p["Audit sécurité"].Id, Quantite = 1, PrixUnitaireHT = p["Audit sécurité"].PrixHT, TauxTVA = p["Audit sécurité"].TauxTVA });
        context.Factures.Add(inv3);

        // Invoice 4 (client4) – 18 days ago, 2 products
        var inv4 = new Facture { Date = DateTime.Now.AddDays(-18), ClientId = clientList[3].Id, TimbreFiscal = 1.0m };
        inv4.Lignes.Add(new LigneFacture { ProduitId = p["Développement Blazor (jour)"].Id, Quantite = 5, PrixUnitaireHT = p["Développement Blazor (jour)"].PrixHT, TauxTVA = p["Développement Blazor (jour)"].TauxTVA });
        inv4.Lignes.Add(new LigneFacture { ProduitId = p["Support technique (forfait)"].Id, Quantite = 2, PrixUnitaireHT = p["Support technique (forfait)"].PrixHT, TauxTVA = p["Support technique (forfait)"].TauxTVA });
        context.Factures.Add(inv4);

        // Invoice 5 (client1) – 12 days ago, 1 product
        var inv5 = new Facture { Date = DateTime.Now.AddDays(-12), ClientId = clientList[0].Id, TimbreFiscal = 1.0m };
        inv5.Lignes.Add(new LigneFacture { ProduitId = p["Hébergement Cloud / mois"].Id, Quantite = 6, PrixUnitaireHT = p["Hébergement Cloud / mois"].PrixHT, TauxTVA = p["Hébergement Cloud / mois"].TauxTVA });
        context.Factures.Add(inv5);

        // Invoice 6 (client2) – 7 days ago, 1 product (draft)
        var inv6 = new Facture { Date = DateTime.Now.AddDays(-7), ClientId = clientList[1].Id, TimbreFiscal = 1.0m };
        inv6.Lignes.Add(new LigneFacture { ProduitId = p["Formation interne (heure)"].Id, Quantite = 8, PrixUnitaireHT = p["Formation interne (heure)"].PrixHT, TauxTVA = p["Formation interne (heure)"].TauxTVA });
        context.Factures.Add(inv6);

        // Invoice 7 (client3) – 5 days ago, 2 products (overdue)
        var inv7 = new Facture { Date = DateTime.Now.AddDays(-5), ClientId = clientList[2].Id, TimbreFiscal = 1.0m };
        inv7.Lignes.Add(new LigneFacture { ProduitId = p["Design UI/UX (forfait)"].Id, Quantite = 1, PrixUnitaireHT = p["Design UI/UX (forfait)"].PrixHT, TauxTVA = p["Design UI/UX (forfait)"].TauxTVA });
        inv7.Lignes.Add(new LigneFacture { ProduitId = p["Support technique (forfait)"].Id, Quantite = 1, PrixUnitaireHT = p["Support technique (forfait)"].PrixHT, TauxTVA = p["Support technique (forfait)"].TauxTVA });
        context.Factures.Add(inv7);

        // Invoice 8 (client4) – 2 days ago, 1 product (paid)
        var inv8 = new Facture { Date = DateTime.Now.AddDays(-2), ClientId = clientList[3].Id, TimbreFiscal = 1.0m };
        inv8.Lignes.Add(new LigneFacture { ProduitId = p["Audit sécurité"].Id, Quantite = 1, PrixUnitaireHT = p["Audit sécurité"].PrixHT, TauxTVA = p["Audit sécurité"].TauxTVA });
        context.Factures.Add(inv8);

        context.SaveChanges(); // saves all invoices and lines

        Console.WriteLine($"Seeding completed: {clients.Length} clients, {produits.Length} produits, {invoices.Count} factures.");
    }
}

app.Run();