using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Regras;

public sealed class RegraCnpjFornecedorValido : IRegraNotaFiscal
{
    public string Codigo => "CNPJ_INVALIDO";

    public string Descricao => "O CNPJ do fornecedor deve ter dígitos verificadores válidos.";

    public ViolacaoRegra? Avaliar(NotaFiscal nota) =>
        nota.Fornecedor.Cnpj.EhValido
            ? null
            : new ViolacaoRegra(Codigo, $"O CNPJ {nota.Fornecedor.Cnpj} do fornecedor {nota.Fornecedor.RazaoSocial} é inválido.");
}
