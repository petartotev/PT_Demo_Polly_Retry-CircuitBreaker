using Polly;

namespace DkWebService.EndPoint.Clients;

public abstract class BaseClient
{
    protected BaseClient(IConfiguration configuration, ILogger<BaseClient> logger)
    {
        Configuration = configuration;
        Logger = logger;
        RetryPolicy = SetupRetryPolicy();
    }

    protected IConfiguration Configuration { get; init; }
    protected ILogger<BaseClient> Logger { get; init; }
    protected IAsyncPolicy RetryPolicy { get; set; }

    protected virtual IAsyncPolicy SetupRetryPolicy()
    {
        var result = Policy.Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: Configuration.GetValue<int>("RestClientRetryAttempts"),
                sleepDurationProvider:
                retryAttempt =>
                {
                    var exponent = retryAttempt - 1;
                    var nextRetryDelay = TimeSpan.FromMilliseconds(Configuration.GetValue<TimeSpan>("StartHttpRetryInterval").TotalMilliseconds * Math.Pow(2, exponent));
                    var retryDelayInSeconds = Math.Min(nextRetryDelay.TotalSeconds, Configuration.GetValue<TimeSpan>("MaxHttpRetryInterval").TotalSeconds);

                    return TimeSpan.FromSeconds(retryDelayInSeconds);
                },
                onRetry: (ex, sleepDuration, attemptNumber, context) =>
                {
                    Logger.LogWarning(
                        ex,
                        "RetryPolicy attempt. Error invoking {methodName}. SleepDuration: {sleepDuration} ms, Attempts: {attemptNumber}",
                        context["MethodName"] ?? "N/A",
                        sleepDuration.Milliseconds,
                        attemptNumber);
                });

        return result;
    }

    protected virtual async Task<T> ExecuteWithPolicyAsync<T>(
        Func<Context, Task<T>> func,
        string operationName)
    {
        RetryPolicy ??= Policy.NoOpAsync();
        var context = new Context
        {
            ["MethodName"] = operationName
        };

        return await RetryPolicy.ExecuteAsync(ctx => func(ctx), context);
    }
}
