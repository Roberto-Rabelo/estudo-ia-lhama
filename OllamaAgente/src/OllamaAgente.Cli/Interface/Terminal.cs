namespace OllamaAgente.Cli.Interface;

internal static class Terminal
{
    public static void Titulo(string texto)
    {
        Escrever(ConsoleColor.Cyan, $"\n=== {texto} ===\n");
    }

    public static void Sucesso(string texto) => Escrever(ConsoleColor.Green, $"[ok] {texto}\n");

    public static void Erro(string texto) => Escrever(ConsoleColor.Red, $"[erro] {texto}\n");

    public static void Info(string texto) => Escrever(ConsoleColor.DarkGray, $"{texto}\n");

    public static void Pergunta(string texto) => Escrever(ConsoleColor.Yellow, $"\nVocê: {texto}\n");

    public static string? LerPergunta()
    {
        Escrever(ConsoleColor.Yellow, "\nVocê: ");
        return Console.ReadLine();
    }

    public static void InicioResposta() => Escrever(ConsoleColor.Cyan, "Agente: ");

    public static void Trecho(string texto) => Console.Write(texto);

    public static void FimResposta(TimeSpan duracao) =>
        Escrever(ConsoleColor.DarkGray, $"\n({duracao.TotalSeconds:N1}s)\n");

    private static void Escrever(ConsoleColor cor, string texto)
    {
        var corAnterior = Console.ForegroundColor;
        Console.ForegroundColor = cor;
        Console.Write(texto);
        Console.ForegroundColor = corAnterior;
    }
}
