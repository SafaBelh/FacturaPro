using InvoicingApp.Models;

namespace InvoicingApp.Services;

public interface IClientService
{
    Task<List<Client>> GetClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task AddClientAsync(Client client);
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(int id);
}