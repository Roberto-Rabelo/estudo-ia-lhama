using OllamaAgente.Domain.Comum;

namespace OllamaAgente.Domain.NotasFiscais;

/// <summary>
/// CNPJ numérico ou alfanumérico (IN RFB 2.229/2024). Pode representar um valor inválido,
/// pois o domínio analisa documentos recebidos; a validade é verificada por regra de negócio.
/// </summary>
public sealed record Cnpj
{
    private const int Tamanho = 14;
    private static readonly int[] PesosPrimeiroDigito = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] PesosSegundoDigito = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    public string Valor { get; }

    private Cnpj(string valor) => Valor = valor;

    public static Cnpj Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("O CNPJ é obrigatório.");

        var normalizado = new string(valor.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());
        return new Cnpj(normalizado);
    }

    public bool EhValido =>
        Valor.Length == Tamanho
        && Valor[..12].All(char.IsAsciiLetterOrDigit)
        && Valor[12..].All(char.IsAsciiDigit)
        && Valor.Distinct().Count() > 1
        && DigitosVerificadoresConferem();

    private bool DigitosVerificadoresConferem()
    {
        var raiz = Valor[..12];
        var primeiro = CalcularDigito(raiz, PesosPrimeiroDigito);
        var segundo = CalcularDigito(raiz + primeiro, PesosSegundoDigito);
        return Valor[12] - '0' == primeiro && Valor[13] - '0' == segundo;
    }

    private static int CalcularDigito(string caracteres, int[] pesos)
    {
        var resto = caracteres.Select((c, i) => (c - '0') * pesos[i]).Sum() % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    public override string ToString() =>
        Valor.Length == Tamanho
            ? $"{Valor[..2]}.{Valor[2..5]}.{Valor[5..8]}/{Valor[8..12]}-{Valor[12..]}"
            : Valor;
}
