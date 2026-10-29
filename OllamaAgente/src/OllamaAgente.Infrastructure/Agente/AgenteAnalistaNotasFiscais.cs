using System.Runtime.CompilerServices;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaAgente.Application.Abstracoes;
using OllamaAgente.Application.Agente;
using OllamaAgente.Application.NotasFiscais;

namespace OllamaAgente.Infrastructure.Agente;

public sealed class AgenteAnalistaNotasFiscais(IChatClient chatClient, FerramentasNotasFiscais ferramentas) : IAgenteAnalista
{
    private readonly AIAgent _agente = chatClient.AsAIAgent(
        name: InstrucoesAgente.Nome,
        instructions: InstrucoesAgente.AnalistaNotasFiscais,
        tools: CriarFerramentas(ferramentas));

    private AgentSession? _sessao;

    public async IAsyncEnumerable<string> PerguntarAsync(
        string pergunta,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _sessao ??= await _agente.CreateSessionAsync(cancellationToken);

        await foreach (var atualizacao in _agente.RunStreamingAsync(pergunta, _sessao, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(atualizacao.Text))
                yield return atualizacao.Text;
        }
    }

    public void ReiniciarConversa() => _sessao = null;

    private static List<AITool> CriarFerramentas(FerramentasNotasFiscais ferramentas) =>
    [
        AIFunctionFactory.Create(ferramentas.ListarNotas),
        AIFunctionFactory.Create(ferramentas.ConsultarNota),
        AIFunctionFactory.Create(ferramentas.ValidarNota),
        AIFunctionFactory.Create(ferramentas.ListarNotasComProblemas),
        AIFunctionFactory.Create(ferramentas.CalcularTotalPorFornecedor),
        AIFunctionFactory.Create(ferramentas.ListarRegras),
    ];
}
