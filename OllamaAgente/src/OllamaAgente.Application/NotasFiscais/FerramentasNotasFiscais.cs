using System.ComponentModel;
using OllamaAgente.Domain.NotasFiscais;
using OllamaAgente.Domain.Regras;

namespace OllamaAgente.Application.NotasFiscais;

/// <summary>
/// Casos de uso expostos ao agente como ferramentas. As descrições são lidas pelo LLM
/// para decidir qual ferramenta chamar, por isso devem ser claras e objetivas.
/// </summary>
public sealed class FerramentasNotasFiscais(INotaFiscalRepository repositorio, ValidadorNotaFiscal validador)
{
    [Description("Lista todas as notas fiscais com número, fornecedor, CNPJ, data de emissão e valor declarado.")]
    public async Task<IReadOnlyList<NotaFiscalResumoDto>> ListarNotas(CancellationToken cancellationToken = default)
    {
        var notas = await repositorio.ListarAsync(cancellationToken);
        return notas.Select(n => n.ParaResumo()).ToList();
    }

    [Description("Retorna os detalhes de uma nota fiscal, incluindo os itens. Retorna null se a nota não existir.")]
    public async Task<NotaFiscalDetalheDto?> ConsultarNota(
        [Description("Número da nota fiscal, por exemplo NF-1001")] string numero,
        CancellationToken cancellationToken = default)
    {
        var nota = await repositorio.ObterPorNumeroAsync(numero, cancellationToken);
        return nota?.ParaDetalhe();
    }

    [Description("Valida uma nota fiscal contra as regras de negócio e retorna as violações. Retorna null se a nota não existir.")]
    public async Task<ResultadoValidacaoDto?> ValidarNota(
        [Description("Número da nota fiscal, por exemplo NF-1001")] string numero,
        CancellationToken cancellationToken = default)
    {
        var nota = await repositorio.ObterPorNumeroAsync(numero, cancellationToken);
        return nota is null ? null : validador.Validar(nota).ParaDto();
    }

    [Description("Valida todas as notas fiscais e retorna somente as que violam alguma regra de negócio.")]
    public async Task<IReadOnlyList<ResultadoValidacaoDto>> ListarNotasComProblemas(CancellationToken cancellationToken = default)
    {
        var notas = await repositorio.ListarAsync(cancellationToken);
        return notas
            .Select(validador.Validar)
            .Where(resultado => !resultado.EhValida)
            .Select(resultado => resultado.ParaDto())
            .ToList();
    }

    [Description("Calcula a quantidade de notas e o valor total declarado agrupados por fornecedor.")]
    public async Task<IReadOnlyList<TotalPorFornecedorDto>> CalcularTotalPorFornecedor(CancellationToken cancellationToken = default)
    {
        var notas = await repositorio.ListarAsync(cancellationToken);
        return notas
            .GroupBy(n => n.Fornecedor)
            .Select(grupo => new TotalPorFornecedorDto(
                grupo.Key.RazaoSocial,
                grupo.Key.Cnpj.ToString(),
                grupo.Count(),
                grupo.Sum(n => n.ValorDeclarado.Valor)))
            .OrderByDescending(total => total.ValorTotal)
            .ToList();
    }

    [Description("Lista as regras de negócio aplicadas às notas fiscais, com código e descrição.")]
    public IReadOnlyList<string> ListarRegras() =>
        validador.Regras.Select(regra => $"{regra.Codigo}: {regra.Descricao}").ToList();
}
