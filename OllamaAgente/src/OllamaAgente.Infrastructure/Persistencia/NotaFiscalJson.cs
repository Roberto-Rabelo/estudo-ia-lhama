using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Infrastructure.Persistencia;

internal sealed record NotaFiscalJson(
    string Numero,
    FornecedorJson Fornecedor,
    DateOnly DataEmissao,
    decimal ValorDeclarado,
    bool AprovadaPelaDiretoria,
    IReadOnlyList<ItemNotaFiscalJson> Itens)
{
    public NotaFiscal ParaDominio() => NotaFiscal.Criar(
        Numero,
        new Fornecedor(Fornecedor.RazaoSocial, Cnpj.Criar(Fornecedor.Cnpj)),
        DataEmissao,
        new Dinheiro(ValorDeclarado),
        AprovadaPelaDiretoria,
        Itens.Select(i => new ItemNotaFiscal(i.Descricao, i.Quantidade, new Dinheiro(i.ValorUnitario))));
}

internal sealed record FornecedorJson(string RazaoSocial, string Cnpj);

internal sealed record ItemNotaFiscalJson(string Descricao, decimal Quantidade, decimal ValorUnitario);
