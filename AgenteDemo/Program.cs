using System.ComponentModel;
using Google.GenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;

var provider = (Environment.GetEnvironmentVariable("LLM_PROVIDER") ?? "ollama").ToLowerInvariant();

IChatClient chatClient = provider switch
{
    "ollama" => new OllamaApiClient(
        new Uri(Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434"),
        Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "qwen3:8b"),

    "openai" => new OpenAIClient(RequireEnv("OPENAI_API_KEY"))
        .GetChatClient(Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4.1-mini")
        .AsIChatClient(),

    "gemini" => new Client(apiKey: RequireEnv("GEMINI_API_KEY"))
        .AsIChatClient(Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-2.5-flash"),

    _ => throw new InvalidOperationException($"Provedor desconhecido: {provider}. Use ollama, openai ou gemini."),
};

AIAgent agent = chatClient.AsAIAgent(
    name: "AssistenteOllama",
    instructions: "Você é um assistente prestativo que responde em português do Brasil. " +
                  "Use as ferramentas disponíveis sempre que a pergunta depender de dados delas.",
    tools:
    [
        AIFunctionFactory.Create(Ferramentas.ObterDataHoraAtual),
        AIFunctionFactory.Create(Ferramentas.ConsultarEstoque),
    ]);

AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine($"Agente pronto usando '{provider}'. Digite 'sair' para encerrar.");
while (true)
{
    Console.Write("\nVocê: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.Equals("sair", StringComparison.OrdinalIgnoreCase))
        break;

    Console.Write("Agente: ");
    await foreach (var update in agent.RunStreamingAsync(input, session))
        Console.Write(update.Text);
    Console.WriteLine();
}

static string RequireEnv(string name) =>
    Environment.GetEnvironmentVariable(name)
    ?? throw new InvalidOperationException($"Defina a variável de ambiente {name}.");

static class Ferramentas
{
    [Description("Retorna a data e hora atuais do servidor.")]
    public static string ObterDataHoraAtual() => DateTime.Now.ToString("dd/MM/yyyy HH:mm");

    [Description("Consulta a quantidade em estoque de um produto pelo nome.")]
    public static int ConsultarEstoque([Description("Nome do produto")] string produto) =>
        produto.ToLowerInvariant() switch
        {
            "notebook" => 12,
            "mouse" => 140,
            "monitor" => 0,
            _ => -1,
        };
}
