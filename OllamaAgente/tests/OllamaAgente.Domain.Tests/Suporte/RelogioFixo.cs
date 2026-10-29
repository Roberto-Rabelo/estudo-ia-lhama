namespace OllamaAgente.Domain.Tests.Suporte;

internal sealed class RelogioFixo(DateOnly hoje) : TimeProvider
{
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    public override DateTimeOffset GetUtcNow() => new(hoje.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);
}
