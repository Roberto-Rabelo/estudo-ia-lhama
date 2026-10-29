using OllamaAgente.Domain.Comum;

namespace OllamaAgente.Domain.NotasFiscais;

public sealed record ItemNotaFiscal
{
    public string Descricao { get; }
    public decimal Quantidade { get; }
    public Dinheiro ValorUnitario { get; }

    public ItemNotaFiscal(string descricao, decimal quantidade, Dinheiro valorUnitario)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("A descrição do item é obrigatória.");
        if (quantidade <= 0)
            throw new DomainException($"A quantidade do item '{descricao}' deve ser maior que zero.");

        Descricao = descricao.Trim();
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }

    public Dinheiro ValorTotal => ValorUnitario * Quantidade;
}
