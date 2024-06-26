﻿using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace RequestService.Policies;

public class ClientPolicy
{
    public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }

    public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
    
    public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }

    public IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy { get; }

    public ClientPolicy()
    {
        ImmediateHttpRetry = Policy
            .HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            .RetryAsync(5);

        LinearHttpRetry = Policy
            .HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

        ExponentialHttpRetry = Policy
            .HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        CircuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
    }
}
