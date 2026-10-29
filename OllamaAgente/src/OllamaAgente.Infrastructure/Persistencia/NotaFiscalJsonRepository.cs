using System.Text.Json;
using Microsoft.Extensions.Options;
using OllamaAgente.Domain.NotasFiscais;
using OllamaAgente.Infrastructure.Configuracoes;

namespace OllamaAgente.Infrastructure.Persistencia;

public sealed class NotaFiscalJsonRepository : INotaFiscalRepository
{
    private static readonly JsonSerializerOptions OpcoesJson = new(JsonSerializerDefaults.Web);

    private readonly Lazy<Task<IReadOnlyDictionary<string, NotaFiscal>>> _notas;

    public NotaFiscalJsonRepository(IOptions<DadosOptions> opcoes)
    {
        var caminho = ResolverCaminho(opcoes.Value.CaminhoNotas);
        _notas = new Lazy<Task<IReadOnlyDictionary<string, NotaFiscal>>>(() => CarregarAsync(caminho));
    }

    public async Task<NotaFiscal?> ObterPorNumeroAsync(string numero, CancellationToken cancellationToken = default)
    {
        var notas = await _notas.Value.WaitAsync(cancellationToken);
        return notas.GetValueOrDefault(numero.Trim());
    }

    public async Task<IReadOnlyList<NotaFiscal>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var notas = await _notas.Value.WaitAsync(cancellationToken);
        return notas.Values.ToList();
    }

    private static async Task<IReadOnlyDictionary<string, NotaFiscal>> CarregarAsync(string caminho)
    {
        await using var arquivo = File.OpenRead(caminho);
        var registros = await JsonSerializer.DeserializeAsync<List<NotaFiscalJson>>(arquivo, OpcoesJson)
                        ?? throw new InvalidDataException($"O arquivo {caminho} não contém notas fiscais.");

        return registros
            .Select(registro => registro.ParaDominio())
            .ToDictionary(nota => nota.Numero, StringComparer.OrdinalIgnoreCase);
    }

    private static string ResolverCaminho(string caminho) =>
        Path.IsPathRooted(caminho) ? caminho : Path.Combine(AppContext.BaseDirectory, caminho);
}
