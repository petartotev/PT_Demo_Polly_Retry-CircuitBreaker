using DkWebService.EndPoint.Clients;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

namespace DkWebService.UnitTests.Tests.Clients;

[TestFixture]
public class AgamemnonClientTests : TestsBase
{
    private readonly Mock<ILogger<AgamemnonClient>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _loggerMock.Reset();
    }

    [Test]
    // Retries: 2, StartInterval: 50 ms, MaxInterval: 100 ms
    public void GetCalculationsByAgamemnonApi_WithTwoLinearRetriesHavingStartInterval50Ms_RetriesTwoTimes()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "RestClientRetryAttempts", "2" },
            { "StartHttpRetryInterval", "00:00:00.050" },
            { "MaxHttpRetryInterval", "00:00:00.100" },
            { "SomeAppSettingValue", "SomeValue123" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var sut = new AgamemnonClient(configuration, _loggerMock.Object);

        // Act
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetCalculationsByAgamemnonApiAsync(123));

        // Assert
        _loggerMock.Invocations.Count.Should().Be(2 + 1);

        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking AgamemnonClient.GetCalculationsByAgamemnonApi. SleepDuration: 50 ms, Attempts: 1",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking AgamemnonClient.GetCalculationsByAgamemnonApi. SleepDuration: 50 ms, Attempts: 2",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "[AgamemnonClient.GetCalculationsByAgamemnonApi] - Failed to get int for Event 123 from Agamemnon. Some AppSettings value: SomeValue123.",
            LogLevel.Error);
    }

    [Test]
    // Retries: 5, StartInterval: 250 ms, MaxInterval: 100 ms
    public void GetCalculationsByAgamemnonApi_WithFiveLinearRetriesHavingStartInterval50Ms_RetriesFiveTimes()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "RestClientRetryAttempts", "5" },
            { "StartHttpRetryInterval", "00:00:00.250" },
            { "MaxHttpRetryInterval", "00:00:00.100" },
            { "SomeAppSettingValue", "SomeValue123" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var sut = new AgamemnonClient(configuration, _loggerMock.Object);

        var timer = new Stopwatch();

        // Act
        timer.Start();
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetCalculationsByAgamemnonApiAsync(123));
        timer.Stop();

        // Assert
        timer.Elapsed.Should().BeGreaterThan(TimeSpan.FromMilliseconds(5 * 250));

        _loggerMock.Invocations.Count.Should().Be(5 + 1);

        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking AgamemnonClient.GetCalculationsByAgamemnonApi. SleepDuration: 250 ms, Attempts: 1",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking AgamemnonClient.GetCalculationsByAgamemnonApi. SleepDuration: 250 ms, Attempts: 2",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking AgamemnonClient.GetCalculationsByAgamemnonApi. SleepDuration: 250 ms, Attempts: 3",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking AgamemnonClient.GetCalculationsByAgamemnonApi. SleepDuration: 250 ms, Attempts: 4",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking AgamemnonClient.GetCalculationsByAgamemnonApi. SleepDuration: 250 ms, Attempts: 5",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "[AgamemnonClient.GetCalculationsByAgamemnonApi] - Failed to get int for Event 123 from Agamemnon. Some AppSettings value: SomeValue123.",
            LogLevel.Error);
    }
}
