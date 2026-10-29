using Microsoft.Extensions.DependencyInjection;
using OllamaAgente.Application.NotasFiscais;
using OllamaAgente.Domain.Regras;

namespace OllamaAgente.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddSingleton<IRegraNotaFiscal, RegraCnpjFornecedorValido>();
        services.AddSingleton<IRegraNotaFiscal, RegraValorDeclaradoConfereComItens>();
        services.AddSingleton<IRegraNotaFiscal>(new RegraAprovacaoAcimaDoLimite(RegraAprovacaoAcimaDoLimite.LimitePadrao));
        services.AddSingleton<IRegraNotaFiscal, RegraDataEmissaoNaoFutura>();
        services.AddSingleton<ValidadorNotaFiscal>();

        services.AddSingleton<FerramentasNotasFiscais>();

        return services;
    }
}
