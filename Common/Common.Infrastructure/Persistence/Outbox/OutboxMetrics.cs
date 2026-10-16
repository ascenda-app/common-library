using System.Diagnostics.Metrics;
using Common.Infrastructure.Observability;

namespace Common.Infrastructure.Persistence.Outbox;

public sealed class OutboxMetrics
{
    private const string MessagesUnit = "{message}";

    private readonly Counter<long> published;
    private readonly Counter<long> failed;

    private OutboxBacklog backlog = OutboxBacklog.Empty;

    public OutboxMetrics()
    {
        published = AscendaMetrics.Meter.CreateCounter<long>(
            "ascenda.outbox.messages.published", MessagesUnit);
        failed = AscendaMetrics.Meter.CreateCounter<long>(
            "ascenda.outbox.messages.failed", MessagesUnit);

        AscendaMetrics.Meter.CreateObservableGauge(
            "ascenda.outbox.pending.count", () => backlog.PendingCount, MessagesUnit);
        AscendaMetrics.Meter.CreateObservableGauge(
            "ascenda.outbox.parked.count", () => backlog.ParkedCount, MessagesUnit);
        AscendaMetrics.Meter.CreateObservableGauge(
            "ascenda.outbox.pending.oldest_age", () => backlog.OldestPendingAge.TotalSeconds, "s");
    }

    public OutboxBacklog Backlog => backlog;

    public void RecordBacklog(OutboxBacklog value) => backlog = value;

    public void RecordPublished(string eventType) => published.Add(1, EventTypeTag(eventType));

    public void RecordFailed(string eventType) => failed.Add(1, EventTypeTag(eventType));

    private static KeyValuePair<string, object?> EventTypeTag(string eventType) =>
        new("event.type", eventType);
}
