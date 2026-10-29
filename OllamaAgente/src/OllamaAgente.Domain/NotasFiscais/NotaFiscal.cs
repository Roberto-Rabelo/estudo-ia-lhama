using OllamaAgente.Domain.Comum;

namespace OllamaAgente.Domain.NotasFiscais;

public sealed class NotaFiscal : Entity<string>
{
    private readonly List<ItemNotaFiscal> _itens;

    private NotaFiscal(
        string numero,
        Fornecedor fornecedor,
        DateOnly dataEmissao,
        Dinheiro valorDeclarado,
        bool aprovadaPelaDiretoria,
        List<ItemNotaFiscal> itens)
        : base(numero)
    {
        Fornecedor = fornecedor;
        DataEmissao = dataEmissao;
        ValorDeclarado = valorDeclarado;
        AprovadaPelaDiretoria = aprovadaPelaDiretoria;
        _itens = itens;
    }

    public string Numero => Id;
    public Fornecedor Fornecedor { get; }
    public DateOnly DataEmissao { get; }
    public Dinheiro ValorDeclarado { get; }
    public bool AprovadaPelaDiretoria { get; }
    public IReadOnlyList<ItemNotaFiscal> Itens => _itens;

    public Dinheiro ValorCalculadoDosItens =>
        _itens.Aggregate(Dinheiro.Zero, (total, item) => total + item.ValorTotal);

    public static NotaFiscal Criar(
        string numero,
        Fornecedor fornecedor,
        DateOnly dataEmissao,
        Dinheiro valorDeclarado,
        bool aprovadaPelaDiretoria,
        IEnumerable<ItemNotaFiscal> itens)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new DomainException("O número da nota fiscal é obrigatório.");

        var listaItens = itens.ToList();
        if (listaItens.Count == 0)
            throw new DomainException($"A nota fiscal {numero} precisa ter pelo menos um item.");

        return new NotaFiscal(numero.Trim(), fornecedor, dataEmissao, valorDeclarado, aprovadaPelaDiretoria, listaItens);
    }
}
