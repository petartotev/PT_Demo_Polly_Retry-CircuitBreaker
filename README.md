# PT_Demo_Polly_Retry-CircuitBreaker

[Polly](https://www.nuget.org/packages/polly/) is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Rate-limiting, Hedging and Fallback in a fluent and thread-safe manner.

Salute for the demo on Retry policies, Les Jackson!<br>
https://www.youtube.com/watch?v=DSMdUvL8N30

## Contents
- [Original Demo](#original-demo)
    - [Initial Setup](#initial-setup)
    - [Retry Policy](#retry-policy)
    - [Circuit Breaker](#circuit-breaker)
- [DK Demo](#dk-demo)
    - [Initial Setup](#initial-setup-1)
    - [BaseClient](#baseclient)
    - [OdysseusClient - EXPONENTIAL RETRY](#odysseusclient-exponential-retry)
    - [AgamemnonClient - LINEAR RETRY](#agamemnonclient-linear-retry)
    - [PerseusClient - IMMEDIATE RETRY](#perseusclient-immediate-retry)
    - [Test](#test)
- [Known Issues](#known-issues)
- [Links](#links)

## Original Demo

### Initial Setup

Create a blank .NET Solution `PT_Demo_Polly` which contains 2 .NET 6 Web API projects:
- `RequestService`
    - Implement `RequestController.cs` with `[GET] Make Request` endpoint
    - Install `Polly` NuGet package (not sure if needed)
    - Install `Microsoft.Extensions.Http.Polly` NuGet package (required)

- `ResponseService`
    - Implement `ResponseController.cs` with `[GET] Get Response` endpoint

### Retry Policy

See implementation in `Program.cs`, `ClientPolicy.cs` and `RequestController.cs`.

There are 3 types of Retry mechanisms implemented in `ClientPolicy.cs`:
- ImmediateHttpRetry
- LinearHttpRetry
- ExponentialHttpRetry

### Circuit Breaker

1. In `ClientPolicy.cs`, add the following:

```
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

```

2. Update `Program.cs`:

```
using RequestService.Policies;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ClientPolicy>(new ClientPolicy());

builder.Services
    .AddHttpClient("TestClient")
    .AddPolicyHandler(request => request.Method == HttpMethod.Get ? new ClientPolicy().LinearHttpRetry : new ClientPolicy().ImmediateHttpRetry)
    .AddPolicyHandler(new ClientPolicy().CircuitBreakerPolicy);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

```

3. In `RequestController.cs`, add the following:

```
        [HttpGet]
        public async Task<ActionResult> MakeRequest()
        {
            var id = 25;
            var url = $"https://localhost:7000/api/v1/response/{id}";

            ...

            // 4 CIRCUIT BREAKER
            var client = _httpClientFactory.CreateClient("TestClient");

            try
            {
                var response = await client.GetAsync(url);

                // Process the response as needed

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle the exception or return an error response
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
```

## DK Demo

### Initial Setup

Either create or use existing .NET Solution `PT_Demo_Polly` and add the following Projects:
- `DKWebService.EndPoint`
    - Add NuGet `Microsoft.Extensions.Http.Polly` (8.0.6 or higher)
- `DKWebService.UnitTests`
    - Add NuGet `FluentAssertions` (6.12.0 or higher)
    - Add NuGet `Moq` (4.20.70 or higher)

### BaseClient

Implement BaseClient.

### OdysseusClient (EXPONENTIAL RETRY)

Implement `OdysseusClient` which inherits from `BaseClient` and uses its virtual `SetupRetryPolicy` method without overriding it in order to use EXPONENTIAL RETRY.

### AgamemnonClient (LINEAR RETRY)

Implement `AgamemnonClient` which inherits from `BaseClient`, but overrides its virtual `SetupRetryPolicy` in order to use LINEAR RETRY.

### PerseusClient (IMMEDIATE RETRY)

Implement `PerseusClient` which inherits from `BaseClient`, but overrides its virtual `SetupRetryPolicy` in order to use IMMEDIATE RETRY.

### Test

Test through the `StringyController` by calling its GET endpoints.

## Known Issues

1. `Make Request` endpoint (/api/v1/request) of `RequestService` project returns:

```
500 Internal Server Error

System.Text.Json.JsonException: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32. Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles. Path: ...
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   ...

HEADERS
=======
Accept: */*
Connection: keep-alive
Host: localhost:5000
User-Agent: PostmanRuntime/7.36.0
Accept-Encoding: gzip, deflate, br
Postman-Token: 0c062d8c-277a-4817-b949-4e35493e9a1d

```

✅ Add `.AddJsonOptions` to `builder.Services.AddControllers()` in `Program.cs`:

```
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
```

## Links
- https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern
- https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
