using System.Diagnostics;
using OllamaAgente.Application.Abstracoes;

namespace OllamaAgente.Cli.Interface;

public sealed class ChatConsole(IAgenteAnalista agente, IDiagnosticoLlm diagnostico)
{
    private const string ComandoSair = "/sair";
    private const string ComandoNovaConversa = "/nova";
    private const string ComandoExemplos = "/exemplos";

    private static readonly string[] PerguntasDeExemplo =
    [
        "Quais regras de negócio são aplicadas às notas fiscais?",
        "Quais notas fiscais têm problemas? Explique cada um.",
        "Qual fornecedor tem o maior valor total em notas?",
        "Mostre os itens da nota NF-1004 e diga se ela está correta.",
    ];

    public async Task<int> ExecutarAsync(bool modoTeste, CancellationToken cancellationToken = default)
    {
        Terminal.Titulo("Agente Analista de Notas Fiscais (Ollama + .NET)");

        var resultado = await diagnostico.VerificarAsync(cancellationToken);
        if (!resultado.Sucesso)
        {
            Terminal.Erro(resultado.Mensagem);
            return 1;
        }

        Terminal.Sucesso(resultado.Mensagem);

        if (modoTeste)
            await ExecutarTesteRapidoAsync(cancellationToken);
        else
            await ExecutarModoInterativoAsync(cancellationToken);

        return 0;
    }

    private async Task ExecutarTesteRapidoAsync(CancellationToken cancellationToken)
    {
        foreach (var pergunta in PerguntasDeExemplo)
        {
            Terminal.Pergunta(pergunta);
            await ResponderAsync(pergunta, cancellationToken);
        }
    }

    private async Task ExecutarModoInterativoAsync(CancellationToken cancellationToken)
    {
        ExibirAjuda();

        while (true)
        {
            var entrada = Terminal.LerPergunta()?.Trim();
            if (entrada is null)
                return;

            switch (entrada.ToLowerInvariant())
            {
                case ComandoSair:
                    return;
                case "":
                    continue;
                case ComandoNovaConversa:
                    agente.ReiniciarConversa();
                    Terminal.Info("Conversa reiniciada.");
                    continue;
                case ComandoExemplos:
                    ExibirAjuda();
                    continue;
                default:
                    await ResponderAsync(entrada, cancellationToken);
                    continue;
            }
        }
    }

    private async Task ResponderAsync(string pergunta, CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();
        Terminal.InicioResposta();

        try
        {
            await foreach (var trecho in agente.PerguntarAsync(pergunta, cancellationToken))
                Terminal.Trecho(trecho);

            Terminal.FimResposta(cronometro.Elapsed);
        }
        catch (Exception erro) when (erro is not OperationCanceledException)
        {
            Terminal.Erro($"Falha ao consultar o agente: {erro.Message}");
        }
    }

    private static void ExibirAjuda()
    {
        Terminal.Info($"Comandos: {ComandoSair} para encerrar, {ComandoNovaConversa} para limpar o histórico, {ComandoExemplos} para ver esta ajuda.");
        Terminal.Info("Perguntas de exemplo:");
        foreach (var pergunta in PerguntasDeExemplo)
            Terminal.Info($"  - {pergunta}");
    }
}
