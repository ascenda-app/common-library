using System.Diagnostics.Metrics;

namespace Common.Infrastructure.Observability;

public static class AscendaMetrics
{
    public const string MeterName = "Ascenda";

    public static Meter Meter { get; } = new(MeterName);
}
