namespace OllamaAgente.Infrastructure.Configuracoes;

public sealed class DadosOptions
{
    public const string Secao = "Dados";

    public string CaminhoNotas { get; init; } = "dados/notas.json";
}
