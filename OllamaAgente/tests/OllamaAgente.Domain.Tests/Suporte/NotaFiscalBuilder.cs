using OllamaAgente.Domain.NotasFiscais;

namespace OllamaAgente.Domain.Tests.Suporte;

internal sealed class NotaFiscalBuilder
{
    private string _numero = "NF-TESTE";
    private string _cnpj = "11.222.333/0001-81";
    private DateOnly _dataEmissao = new(2026, 10, 1);
    private decimal? _valorDeclarado;
    private bool _aprovada;
    private readonly List<ItemNotaFiscal> _itens = [];

    public NotaFiscalBuilder ComNumero(string numero) { _numero = numero; return this; }

    public NotaFiscalBuilder ComCnpj(string cnpj) { _cnpj = cnpj; return this; }

    public NotaFiscalBuilder EmitidaEm(DateOnly data) { _dataEmissao = data; return this; }

    public NotaFiscalBuilder ComValorDeclarado(decimal valor) { _valorDeclarado = valor; return this; }

    public NotaFiscalBuilder AprovadaPelaDiretoria() { _aprovada = true; return this; }

    public NotaFiscalBuilder ComItem(string descricao, decimal quantidade, decimal valorUnitario)
    {
        _itens.Add(new ItemNotaFiscal(descricao, quantidade, new Dinheiro(valorUnitario)));
        return this;
    }

    public NotaFiscal Construir()
    {
        var itens = _itens.Count > 0 ? _itens : [new ItemNotaFiscal("Item padrão", 1, new Dinheiro(100))];
        var valorDeclarado = _valorDeclarado ?? itens.Sum(i => i.ValorTotal.Valor);

        return NotaFiscal.Criar(
            _numero,
            new Fornecedor("Fornecedor Teste Ltda", Cnpj.Criar(_cnpj)),
            _dataEmissao,
            new Dinheiro(valorDeclarado),
            _aprovada,
            itens);
    }
}
