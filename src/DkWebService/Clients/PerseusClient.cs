using DkWebService.EndPoint.Clients.Interfaces;
using Polly;

namespace DkWebService.EndPoint.Clients
{
    // IMMEDIATE RETRY
    public class PerseusClient : BaseClient, IPerseusClient
    {
        public PerseusClient(IConfiguration configuration, ILogger<BaseClient> logger)
            : base(configuration, logger)
        {
        }

        public async Task<bool> CheckIfDepositIsFraudAsync(int depositId)
        {
            try
            {
                return await ExecuteWithPolicyAsync(
                    async context =>
                    {
                        if (depositId == 12345)
                        {
                            throw new ArgumentException($"Invalid depositId! {depositId} is used for testing only!", nameof(depositId));
                        }

                        var response = await IsDepositIdEvent(depositId);

                        return response;
                    },
                    $"{nameof(PerseusClient)}.{nameof(CheckIfDepositIsFraudAsync)}");
            }
            catch (Exception e)
            {
                Logger.LogError(
                    e,
                    "[{Method}] - Failed to get isFraud for Deposit {DepositId} from Perseus. Some AppSettings value: {SomeAppSettingValue}.",
                    $"{nameof(PerseusClient)}.{nameof(CheckIfDepositIsFraudAsync)}",
                    depositId,
                    Configuration.GetValue<string>("SomeAppSettingValue"));

                throw;
            }
        }

        // IMMEDIATE RETRY
        protected override IAsyncPolicy SetupRetryPolicy()
        {
            var result = Policy.Handle<Exception>()
                .RetryAsync(
                Configuration.GetValue<int>("RestClientRetryAttempts"),
                onRetry: (ex, attemptNumber, context) =>
                {
                    Logger.LogWarning(
                        ex,
                        "RetryPolicy attempt. Error invoking {methodName}. SleepDuration: 0 ms (Immediate Retry!), Attempts: {attemptNumber}",
                        context["MethodName"] ?? "N/A",
                        attemptNumber);
                });

            return result;
        }

        private static async Task<bool> IsDepositIdEvent(int depositId)
        {
            return await Task.Run(() => depositId % 2 == 0);
        }
    }
}
