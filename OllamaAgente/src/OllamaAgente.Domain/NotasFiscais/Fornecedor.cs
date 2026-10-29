using OllamaAgente.Domain.Comum;

namespace OllamaAgente.Domain.NotasFiscais;

public sealed record Fornecedor
{
    public string RazaoSocial { get; }
    public Cnpj Cnpj { get; }

    public Fornecedor(string razaoSocial, Cnpj cnpj)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new DomainException("A razão social do fornecedor é obrigatória.");

        RazaoSocial = razaoSocial.Trim();
        Cnpj = cnpj;
    }
}
