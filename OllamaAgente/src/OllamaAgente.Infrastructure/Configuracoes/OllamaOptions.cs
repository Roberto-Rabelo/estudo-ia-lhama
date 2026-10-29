namespace OllamaAgente.Infrastructure.Configuracoes;

public sealed class OllamaOptions
{
    public const string Secao = "Ollama";

    public string Url { get; init; } = "http://localhost:11434";

    public string Modelo { get; init; } = "qwen3:8b";

    public float Temperatura { get; init; }
}
