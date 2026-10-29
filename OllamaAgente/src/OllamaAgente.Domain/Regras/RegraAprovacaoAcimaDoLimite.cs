using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Regras;

public sealed class RegraAprovacaoAcimaDoLimite(Dinheiro limite) : IRegraNotaFiscal
{
    public static readonly Dinheiro LimitePadrao = new(50_000m);

    public string Codigo => "APROVACAO_PENDENTE";

    public string Descricao => $"Notas acima de {limite} exigem aprovação da diretoria.";

    public ViolacaoRegra? Avaliar(NotaFiscal nota) =>
        nota.ValorDeclarado.Valor > limite.Valor && !nota.AprovadaPelaDiretoria
            ? new ViolacaoRegra(Codigo, $"Nota de {nota.ValorDeclarado} está acima do limite de {limite} e não foi aprovada pela diretoria.")
            : null;
}
