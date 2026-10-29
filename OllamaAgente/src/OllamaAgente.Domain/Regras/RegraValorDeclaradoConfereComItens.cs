using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Regras;

public sealed class RegraValorDeclaradoConfereComItens : IRegraNotaFiscal
{
    private static readonly Dinheiro Tolerancia = new(0.01m);

    public string Codigo => "VALOR_DIVERGENTE";

    public string Descricao => "O valor declarado da nota deve ser igual à soma dos itens.";

    public ViolacaoRegra? Avaliar(NotaFiscal nota)
    {
        var diferenca = nota.ValorDeclarado.DiferencaAbsoluta(nota.ValorCalculadoDosItens);

        return diferenca.Valor <= Tolerancia.Valor
            ? null
            : new ViolacaoRegra(
                Codigo,
                $"Valor declarado {nota.ValorDeclarado} difere da soma dos itens {nota.ValorCalculadoDosItens} em {diferenca}.");
    }
}
