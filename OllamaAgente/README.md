# OllamaAgente

Agente em C# .NET 10 que analisa notas fiscais usando um LLM local via Ollama. As regras de negócio ficam no domínio (código determinístico e testado); o LLM só entende a pergunta, chama as ferramentas e explica o resultado.

## Estrutura (DDD / Clean Architecture)

```
src/
  OllamaAgente.Domain/          Regras de negócio puras, sem dependências externas
    Comum/                      Entity<TId>, DomainException
    NotasFiscais/               NotaFiscal (agregado), Fornecedor, ItemNotaFiscal, Cnpj, Dinheiro, INotaFiscalRepository
    Regras/                     IRegraNotaFiscal, regras concretas, ValidadorNotaFiscal
  OllamaAgente.Application/     Casos de uso e portas (interfaces)
    Abstracoes/                 IAgenteAnalista, IDiagnosticoLlm
    NotasFiscais/               FerramentasNotasFiscais (ferramentas do agente) e DTOs
    Agente/                     Instruções (prompt de sistema) do agente
  OllamaAgente.Infrastructure/  Adaptadores
    Agente/                     AgenteAnalistaNotasFiscais (Microsoft Agent Framework)
    Ollama/                     OllamaDiagnostico (servidor no ar? modelo instalado?)
    Persistencia/               NotaFiscalJsonRepository
  OllamaAgente.Cli/             Apresentação: console, appsettings.json, dados/notas.json
tests/
  OllamaAgente.Domain.Tests/    Testes das regras de negócio (não precisam do Ollama)
```

Dependências apontam para dentro: `Cli → Infrastructure → Application → Domain`.

## Como testar

### 1. Ollama

```bash
brew install ollama      # ou o instalador em https://ollama.com
ollama serve             # deixe rodando em outro terminal
ollama pull qwen3:8b     # ~5 GB, suporta tool calling
```

### 2. Testes de domínio (sem Ollama)

```bash
dotnet test
```

### 3. Teste rápido do agente (perguntas prontas, sem digitar nada)

```bash
dotnet run --project src/OllamaAgente.Cli -- --teste
```

### 4. Modo interativo

```bash
dotnet run --project src/OllamaAgente.Cli
```

Comandos: `/sair`, `/nova` (limpa o histórico da conversa), `/exemplos`.

Antes de conversar, o programa verifica se o Ollama está no ar e se o modelo está instalado, e diz exatamente o que fazer se algo faltar.

## Configuração

`src/OllamaAgente.Cli/appsettings.json`:

| Chave | Padrão | Descrição |
|---|---|---|
| `Ollama:Url` | `http://localhost:11434` | Endereço do servidor Ollama |
| `Ollama:Modelo` | `qwen3:8b` | Qualquer modelo com a tag `tools` em ollama.com/library |
| `Ollama:Temperatura` | `0` | 0 = respostas mais determinísticas |
| `Dados:CaminhoNotas` | `dados/notas.json` | Arquivo com as notas fiscais |

Também pode ser sobrescrito por variável de ambiente, por exemplo `Ollama__Modelo=llama3.1:8b`.

## Dados de exemplo

`dados/notas.json` tem 8 notas. Três são válidas; as outras violam pelo menos uma regra:

| Nota | Regra violada |
|---|---|
| NF-1002 | `CNPJ_INVALIDO` |
| NF-1003 | `APROVACAO_PENDENTE` (acima de R$ 50.000 sem aprovação) |
| NF-1004 | `VALOR_DIVERGENTE` (declarado ≠ soma dos itens) |
| NF-1005 | `DATA_FUTURA` |
| NF-1008 | `CNPJ_INVALIDO`, `VALOR_DIVERGENTE`, `APROVACAO_PENDENTE` |

A NF-1007 usa um CNPJ alfanumérico válido (formato da Receita Federal a partir de julho de 2026).

## Como estender

- **Nova regra:** crie uma classe que implemente `IRegraNotaFiscal` em `Domain/Regras`, registre em `Application/DependencyInjection.cs` e escreva o teste. O agente passa a usá-la automaticamente.
- **Nova ferramenta do agente:** adicione um método com `[Description]` em `FerramentasNotasFiscais` e inclua em `AgenteAnalistaNotasFiscais.CriarFerramentas`.
- **Outra fonte de dados (XML, Excel, PostgreSQL):** implemente `INotaFiscalRepository` na Infrastructure e troque o registro.
- **Outro provedor de LLM (OpenAI, Gemini):** troque apenas o `IChatClient` registrado em `Infrastructure/DependencyInjection.cs`; domínio, aplicação e agente não mudam.
