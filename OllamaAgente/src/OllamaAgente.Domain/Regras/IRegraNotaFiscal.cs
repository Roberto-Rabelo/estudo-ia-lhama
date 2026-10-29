using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Regras;

public interface IRegraNotaFiscal
{
    string Codigo { get; }

    string Descricao { get; }

    ViolacaoRegra? Avaliar(NotaFiscal nota);
}
