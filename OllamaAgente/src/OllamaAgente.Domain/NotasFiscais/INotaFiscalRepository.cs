namespace OllamaAgente.Domain.NotasFiscais;

public interface INotaFiscalRepository
{
    Task<NotaFiscal?> ObterPorNumeroAsync(string numero, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotaFiscal>> ListarAsync(CancellationToken cancellationToken = default);
}
