using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RecurringDonations;

public sealed class ProcessRecurringDonations(ILogger<ProcessRecurringDonations> logger)
{
    [Function(nameof(ProcessRecurringDonations))]
    public Task Run([TimerTrigger("0 0 2 * * *")] TimerInfo timerInfo)
    {
        logger.LogInformation("Recurring donation processor started at {UtcNow}", DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}

