namespace OllamaAgente.Application.Abstracoes;

public interface IDiagnosticoLlm
{
    Task<ResultadoDiagnostico> VerificarAsync(CancellationToken cancellationToken = default);
}

public sealed record ResultadoDiagnostico(bool Sucesso, string Mensagem)
{
    public static ResultadoDiagnostico Ok(string mensagem) => new(true, mensagem);

    public static ResultadoDiagnostico Falha(string mensagem) => new(false, mensagem);
}
