using Microsoft.Extensions.Configuration;

namespace InvoicingApp.Services;

public class TimbreFiscalService : ITimbreFiscalService
{
    private readonly IConfiguration _configuration;
    public TimbreFiscalService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public decimal GetTimbreFiscal() => _configuration.GetValue<decimal>("TimbreFiscal", 1.0m);
}