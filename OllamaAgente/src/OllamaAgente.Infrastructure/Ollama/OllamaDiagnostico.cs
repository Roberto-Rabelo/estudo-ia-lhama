using Microsoft.Extensions.Options;
using OllamaAgente.Application.Abstracoes;
using OllamaAgente.Infrastructure.Configuracoes;
using OllamaSharp;

namespace OllamaAgente.Infrastructure.Ollama;

public sealed class OllamaDiagnostico(IOllamaApiClient ollama, IOptions<OllamaOptions> opcoes) : IDiagnosticoLlm
{
    private const string TagPadrao = ":latest";

    public async Task<ResultadoDiagnostico> VerificarAsync(CancellationToken cancellationToken = default)
    {
        var config = opcoes.Value;

        if (!await EstaRodandoAsync(cancellationToken))
            return ResultadoDiagnostico.Falha($"O Ollama não respondeu em {config.Url}. Inicie o servidor com: ollama serve");

        var instalados = (await ollama.ListLocalModelsAsync(cancellationToken))
            .Select(modelo => modelo.Name)
            .ToList();

        if (!instalados.Any(nome => Normalizar(nome) == Normalizar(config.Modelo)))
            return ResultadoDiagnostico.Falha(
                $"O modelo '{config.Modelo}' não está instalado. Rode: ollama pull {config.Modelo}. " +
                $"Instalados: {(instalados.Count == 0 ? "nenhum" : string.Join(", ", instalados))}");

        return ResultadoDiagnostico.Ok($"Ollama em {config.Url} usando o modelo {config.Modelo}.");
    }

    private async Task<bool> EstaRodandoAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await ollama.IsRunningAsync(cancellationToken);
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    private static string Normalizar(string nomeModelo) =>
        nomeModelo.Contains(':') ? nomeModelo.ToLowerInvariant() : nomeModelo.ToLowerInvariant() + TagPadrao;
}
