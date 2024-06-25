using DkWebService.EndPoint.Clients.Interfaces;
using Polly;

namespace DkWebService.EndPoint.Clients
{
    public class AgamemnonClient : BaseClient, IAgamemnonClient
    {
        private readonly Random _random = new();

        public AgamemnonClient(IConfiguration configuration, ILogger<AgamemnonClient> logger)
            : base(configuration, logger)
        {
        }

        public async Task<int> GetCalculationsByAgamemnonApi(int eventId)
        {
            try
            {
                return await ExecuteWithPolicyAsync(
                    async context =>
                    {
                        if (eventId == 123)
                        {
                            throw new ArgumentException($"Invalid eventId.", nameof(eventId));
                        }

                        var response = await DoSomeCalculationAsync();
                        return response;
                    },
                    $"{nameof(AgamemnonClient)}.{nameof(GetCalculationsByAgamemnonApi)}");
            }
            catch (Exception e)
            {
                Logger.LogError(
                    e,
                    "[{Method}] - Failed to get int for Event {EventId} from Agamemnon. Some AppSettings value: {SomeAppSettingValue}.",
                    $"{nameof(AgamemnonClient)}.{nameof(GetCalculationsByAgamemnonApi)}",
                    eventId,
                    Configuration.GetValue<string>("SomeAppSettingValue"));

                throw;
            }
        }

        protected override IAsyncPolicy SetupRetryPolicy()
        {
            var result = Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: Configuration.GetValue<int>("RestClientRetryAttempts"),
                    sleepDurationProvider:
                    retryAttempt => TimeSpan.FromMilliseconds(Configuration.GetValue<TimeSpan>("StartHttpRetryInterval").TotalMilliseconds),
                    onRetry: (ex, sleepDuration, attemptNumber, context) =>
                    {
                        Logger.LogWarning(
                            ex,
                            "RetryPolicy attempt. Error invoking {methodName}. SleepDuration: {sleepDuration} ms, Attempts: {attemptNumber}",
                            context["MethodName"] ?? "N/A",
                            sleepDuration.Milliseconds,
                            attemptNumber
                        );
                    }
                );

            return result;
        }

        private async Task<int> DoSomeCalculationAsync()
        {
            return await Task.Run(() => _random.Next(1, 1001));
        }
    }
}
