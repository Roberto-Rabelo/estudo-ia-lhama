using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Regras;

public sealed class ValidadorNotaFiscal(IEnumerable<IRegraNotaFiscal> regras)
{
    private readonly IReadOnlyList<IRegraNotaFiscal> _regras = regras.ToList();

    public IReadOnlyList<IRegraNotaFiscal> Regras => _regras;

    public ResultadoValidacao Validar(NotaFiscal nota)
    {
        var violacoes = _regras
            .Select(regra => regra.Avaliar(nota))
            .OfType<ViolacaoRegra>()
            .ToList();

        return new ResultadoValidacao(nota.Numero, violacoes);
    }
}
