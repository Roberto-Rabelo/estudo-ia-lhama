using OllamaAgente.Domain.Comum;
using OllamaAgente.Domain.NotasFiscais;
using OllamaAgente.Domain.Tests.Suporte;

namespace OllamaAgente.Domain.Tests.NotasFiscais;

public class NotaFiscalTests
{
    [Fact]
    public void ValorCalculadoDosItens_DeveSomarQuantidadeVezesValorUnitario()
    {
        var nota = new NotaFiscalBuilder()
            .ComItem("Cadeira", 20, 750)
            .ComItem("Mesa", 10, 1200)
            .Construir();

        Assert.Equal(27_000m, nota.ValorCalculadoDosItens.Valor);
    }

    [Fact]
    public void Criar_DeveLancarExcecaoQuandoNaoHaItens()
    {
        var fornecedor = new Fornecedor("Fornecedor", Cnpj.Criar("11.222.333/0001-81"));

        Assert.Throws<DomainException>(() =>
            NotaFiscal.Criar("NF-1", fornecedor, new DateOnly(2026, 1, 1), new Dinheiro(10), false, []));
    }

    [Fact]
    public void Dinheiro_DeveRejeitarValorNegativo()
    {
        Assert.Throws<DomainException>(() => new Dinheiro(-1));
    }
}
