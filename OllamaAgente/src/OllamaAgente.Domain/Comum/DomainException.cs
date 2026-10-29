namespace OllamaAgente.Domain.Comum;

public sealed class DomainException(string mensagem) : Exception(mensagem);
