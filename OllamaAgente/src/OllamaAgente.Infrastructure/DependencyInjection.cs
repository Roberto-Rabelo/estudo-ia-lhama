using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OllamaAgente.Application.Abstracoes;
using OllamaAgente.Domain.NotasFiscais;
using OllamaAgente.Infrastructure.Agente;
using OllamaAgente.Infrastructure.Configuracoes;
using OllamaAgente.Infrastructure.Ollama;
using OllamaAgente.Infrastructure.Persistencia;
using OllamaSharp;

namespace OllamaAgente.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var secaoOllama = configuration.GetSection(OllamaOptions.Secao);
        var ollama = secaoOllama.Get<OllamaOptions>() ?? new OllamaOptions();

        services.Configure<OllamaOptions>(secaoOllama);
        services.Configure<DadosOptions>(configuration.GetSection(DadosOptions.Secao));

        services.AddSingleton(_ => new OllamaApiClient(new Uri(ollama.Url), ollama.Modelo));
        services.AddSingleton<IOllamaApiClient>(sp => sp.GetRequiredService<OllamaApiClient>());
        services.AddChatClient(sp => sp.GetRequiredService<OllamaApiClient>())
            .ConfigureOptions(opcoes => opcoes.Temperature ??= ollama.Temperatura);

        services.AddSingleton<INotaFiscalRepository, NotaFiscalJsonRepository>();
        services.AddSingleton<IDiagnosticoLlm, OllamaDiagnostico>();
        services.AddSingleton<IAgenteAnalista, AgenteAnalistaNotasFiscais>();

        return services;
    }
}
