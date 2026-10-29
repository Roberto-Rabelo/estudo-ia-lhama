using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Regras;

public sealed class RegraDataEmissaoNaoFutura(TimeProvider relogio) : IRegraNotaFiscal
{
    public string Codigo => "DATA_FUTURA";

    public string Descricao => "A data de emissão não pode ser posterior à data atual.";

    public ViolacaoRegra? Avaliar(NotaFiscal nota)
    {
        var hoje = DateOnly.FromDateTime(relogio.GetLocalNow().DateTime);

        return nota.DataEmissao > hoje
            ? new ViolacaoRegra(Codigo, $"A data de emissão {nota.DataEmissao:dd/MM/yyyy} é posterior a hoje ({hoje:dd/MM/yyyy}).")
            : null;
    }
}
