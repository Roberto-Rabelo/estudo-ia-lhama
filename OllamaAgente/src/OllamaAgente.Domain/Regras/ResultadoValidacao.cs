namespace OllamaAgente.Domain.Regras;

public sealed record ResultadoValidacao(string NumeroNota, IReadOnlyList<ViolacaoRegra> Violacoes)
{
    public bool EhValida => Violacoes.Count == 0;
}
