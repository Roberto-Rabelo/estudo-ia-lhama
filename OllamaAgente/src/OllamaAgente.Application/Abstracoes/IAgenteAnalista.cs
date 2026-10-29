namespace OllamaAgente.Application.Abstracoes;

public interface IAgenteAnalista
{
    IAsyncEnumerable<string> PerguntarAsync(string pergunta, CancellationToken cancellationToken = default);

    void ReiniciarConversa();
}
