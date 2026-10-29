using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaAgente.Application;
using OllamaAgente.Cli.Interface;
using OllamaAgente.Infrastructure;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddSingleton<ChatConsole>();

using var host = builder.Build();

var modoTeste = args.Contains("--teste", StringComparer.OrdinalIgnoreCase);
return await host.Services.GetRequiredService<ChatConsole>().ExecutarAsync(modoTeste);
