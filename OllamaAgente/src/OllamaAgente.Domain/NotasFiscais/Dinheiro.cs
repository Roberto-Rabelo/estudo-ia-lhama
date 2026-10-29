using System.Globalization;
using OllamaAgente.Domain.Comum;

namespace OllamaAgente.Domain.NotasFiscais;

public readonly record struct Dinheiro
{
    private static readonly CultureInfo CulturaBrasileira = CultureInfo.GetCultureInfo("pt-BR");

    public decimal Valor { get; }

    public Dinheiro(decimal valor)
    {
        if (valor < 0)
            throw new DomainException("Valores monetários não podem ser negativos.");

        Valor = decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
    }

    public static Dinheiro Zero => new(0);

    public static Dinheiro operator +(Dinheiro a, Dinheiro b) => new(a.Valor + b.Valor);

    public static Dinheiro operator *(Dinheiro a, decimal fator) => new(a.Valor * fator);

    public Dinheiro DiferencaAbsoluta(Dinheiro outro) => new(Math.Abs(Valor - outro.Valor));

    public override string ToString() => Valor.ToString("C", CulturaBrasileira);
}
