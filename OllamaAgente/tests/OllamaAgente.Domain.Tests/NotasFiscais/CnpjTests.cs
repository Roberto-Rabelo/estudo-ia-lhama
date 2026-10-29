using OllamaAgente.Domain.Comum;
using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Tests.NotasFiscais;

public class CnpjTests
{
    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11222333000181")]
    [InlineData("12.ABC.345/01DE-35")]
    [InlineData("12abc34501de35")]
    public void EhValido_DeveAceitarCnpjNumericoEAlfanumericoComDigitosCorretos(string valor)
    {
        Assert.True(Cnpj.Criar(valor).EhValido);
    }

    [Theory]
    [InlineData("12.345.678/0001-00")]
    [InlineData("11.111.111/1111-11")]
    [InlineData("123")]
    [InlineData("12.ABC.345/01DE-3A")]
    public void EhValido_DeveRejeitarCnpjComDigitosErradosTamanhoInvalidoOuRepetido(string valor)
    {
        Assert.False(Cnpj.Criar(valor).EhValido);
    }

    [Fact]
    public void ToString_DeveFormatarComPontuacao()
    {
        Assert.Equal("12.ABC.345/01DE-35", Cnpj.Criar("12abc34501de35").ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_DeveLancarExcecaoQuandoVazio(string valor)
    {
        Assert.Throws<DomainException>(() => Cnpj.Criar(valor));
    }
}
