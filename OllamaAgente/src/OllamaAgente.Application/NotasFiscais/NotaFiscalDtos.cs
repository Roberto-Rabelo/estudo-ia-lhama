using OllamaAgente.Domain.NotasFiscais;
using OllamaAgente.Domain.Regras;

namespace OllamaAgente.Application.NotasFiscais;

public sealed record NotaFiscalResumoDto(
    string Numero,
    string Fornecedor,
    string Cnpj,
    DateOnly DataEmissao,
    decimal ValorDeclarado);

public sealed record ItemNotaFiscalDto(string Descricao, decimal Quantidade, decimal ValorUnitario, decimal ValorTotal);

public sealed record NotaFiscalDetalheDto(
    NotaFiscalResumoDto Resumo,
    bool AprovadaPelaDiretoria,
    decimal ValorCalculadoDosItens,
    IReadOnlyList<ItemNotaFiscalDto> Itens);

public sealed record ViolacaoDto(string Codigo, string Mensagem);

public sealed record ResultadoValidacaoDto(string NumeroNota, bool EhValida, IReadOnlyList<ViolacaoDto> Violacoes);

public sealed record TotalPorFornecedorDto(string Fornecedor, string Cnpj, int QuantidadeNotas, decimal ValorTotal);

internal static class NotaFiscalMapeamento
{
    public static NotaFiscalResumoDto ParaResumo(this NotaFiscal nota) => new(
        nota.Numero,
        nota.Fornecedor.RazaoSocial,
        nota.Fornecedor.Cnpj.ToString(),
        nota.DataEmissao,
        nota.ValorDeclarado.Valor);

    public static NotaFiscalDetalheDto ParaDetalhe(this NotaFiscal nota) => new(
        nota.ParaResumo(),
        nota.AprovadaPelaDiretoria,
        nota.ValorCalculadoDosItens.Valor,
        nota.Itens
            .Select(i => new ItemNotaFiscalDto(i.Descricao, i.Quantidade, i.ValorUnitario.Valor, i.ValorTotal.Valor))
            .ToList());

    public static ResultadoValidacaoDto ParaDto(this ResultadoValidacao resultado) => new(
        resultado.NumeroNota,
        resultado.EhValida,
        resultado.Violacoes.Select(v => new ViolacaoDto(v.Codigo, v.Mensagem)).ToList());
}
