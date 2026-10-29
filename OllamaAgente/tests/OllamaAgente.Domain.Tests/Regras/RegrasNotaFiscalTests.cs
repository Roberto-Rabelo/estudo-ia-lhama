using OllamaAgente.Domain.Regras;
using OllamaAgente.Domain.Tests.Suporte;

namespace OllamaAgente.Domain.Tests.Regras;

public class RegrasNotaFiscalTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 28);

    [Fact]
    public void RegraCnpj_DeveApontarCnpjInvalido()
    {
        var nota = new NotaFiscalBuilder().ComCnpj("12.345.678/0001-00").Construir();

        var violacao = new RegraCnpjFornecedorValido().Avaliar(nota);

        Assert.Equal("CNPJ_INVALIDO", violacao?.Codigo);
    }

    [Fact]
    public void RegraValor_DeveApontarDivergenciaEntreDeclaradoEItens()
    {
        var nota = new NotaFiscalBuilder().ComItem("Mesa", 10, 1200).ComValorDeclarado(12_500).Construir();

        var violacao = new RegraValorDeclaradoConfereComItens().Avaliar(nota);

        Assert.Equal("VALOR_DIVERGENTE", violacao?.Codigo);
    }

    [Fact]
    public void RegraValor_DeveAceitarValorIgualASomaDosItens()
    {
        var nota = new NotaFiscalBuilder().ComItem("Mesa", 10, 1200).ComValorDeclarado(12_000).Construir();

        Assert.Null(new RegraValorDeclaradoConfereComItens().Avaliar(nota));
    }

    [Fact]
    public void RegraAprovacao_DeveExigirAprovacaoAcimaDoLimite()
    {
        var nota = new NotaFiscalBuilder().ComItem("Servidor", 3, 22_000).Construir();

        var violacao = new RegraAprovacaoAcimaDoLimite(RegraAprovacaoAcimaDoLimite.LimitePadrao).Avaliar(nota);

        Assert.Equal("APROVACAO_PENDENTE", violacao?.Codigo);
    }

    [Fact]
    public void RegraAprovacao_DeveAceitarNotaAprovada()
    {
        var nota = new NotaFiscalBuilder().ComItem("Storage", 1, 70_000).AprovadaPelaDiretoria().Construir();

        Assert.Null(new RegraAprovacaoAcimaDoLimite(RegraAprovacaoAcimaDoLimite.LimitePadrao).Avaliar(nota));
    }

    [Fact]
    public void RegraData_DeveApontarEmissaoFutura()
    {
        var nota = new NotaFiscalBuilder().EmitidaEm(Hoje.AddDays(1)).Construir();

        var violacao = new RegraDataEmissaoNaoFutura(new RelogioFixo(Hoje)).Avaliar(nota);

        Assert.Equal("DATA_FUTURA", violacao?.Codigo);
    }

    [Fact]
    public void RegraData_DeveAceitarEmissaoHoje()
    {
        var nota = new NotaFiscalBuilder().EmitidaEm(Hoje).Construir();

        Assert.Null(new RegraDataEmissaoNaoFutura(new RelogioFixo(Hoje)).Avaliar(nota));
    }

    [Fact]
    public void Validador_DeveAcumularTodasAsViolacoes()
    {
        var validador = new ValidadorNotaFiscal(
        [
            new RegraCnpjFornecedorValido(),
            new RegraValorDeclaradoConfereComItens(),
            new RegraAprovacaoAcimaDoLimite(RegraAprovacaoAcimaDoLimite.LimitePadrao),
            new RegraDataEmissaoNaoFutura(new RelogioFixo(Hoje)),
        ]);
        var nota = new NotaFiscalBuilder()
            .ComCnpj("33.445.566/0001-80")
            .ComItem("Consultoria", 100, 600)
            .ComValorDeclarado(61_000)
            .Construir();

        var resultado = validador.Validar(nota);

        Assert.False(resultado.EhValida);
        Assert.Equal(
            ["CNPJ_INVALIDO", "VALOR_DIVERGENTE", "APROVACAO_PENDENTE"],
            resultado.Violacoes.Select(v => v.Codigo));
    }
}
