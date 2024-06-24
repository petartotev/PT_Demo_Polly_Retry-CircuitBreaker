using DkWebService.EndPoint.Clients;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace DkWebService.UnitTests.Tests.Clients;

[TestFixture]
public class OdysseusClientTests : TestsBase
{
    private readonly Mock<ILogger<OdysseusClient>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _loggerMock.Reset();
    }

    [Test]
    // Retries: 2, StartInterval: 50 ms, MaxInterval: 100 ms
    public void GetSomethingFromTheOutsideWorld_WithTwoExponentialRetriesHavingStartInterval50MsAndMaxInterval100Ms_RetriesTwoTimes()
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

        var sut = new OdysseusClient(configuration, _loggerMock.Object);

        // Act
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetSomethingFromTheOutsideWorld(-1));

        // Assert
        _loggerMock.Invocations.Count.Should().Be(2 + 1);

        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 50 ms, Attempts: 1",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 100 ms, Attempts: 2",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "[OdysseusClient.GetSomethingFromTheOutsideWorld] - Failed to get strings for User -1 from Odysseus. Some AppSettings value: SomeValue123.",
            LogLevel.Error);
    }

    [Test]
    // Retries: 3, StartInterval: 50 ms, MaxInterval: 100 ms
    public void GetSomethingFromTheOutsideWorld_WithThreeExponentialRetriesHavingStartInterval50MsAndMaxInterval100Ms_RetriesThreeTimes()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "RestClientRetryAttempts", "3" },
            { "StartHttpRetryInterval", "00:00:00.050" },
            { "MaxHttpRetryInterval", "00:00:00.100" },
            { "SomeAppSettingValue", "SomeValue123" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var sut = new OdysseusClient(configuration, _loggerMock.Object);

        // Act
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetSomethingFromTheOutsideWorld(-1));

        // Assert
        _loggerMock.Invocations.Count.Should().Be(3 + 1);

        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 50 ms, Attempts: 1",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 100 ms, Attempts: 2",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 100 ms, Attempts: 3",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "[OdysseusClient.GetSomethingFromTheOutsideWorld] - Failed to get strings for User -1 from Odysseus. Some AppSettings value: SomeValue123.",
            LogLevel.Error);
    }

    [Test]
    // Retries: 5, StartInterval: 50 ms, MaxInterval: 1000 ms
    public void GetSomethingFromTheOutsideWorld_WithFiveExponentialRetriesHavingStartInterval50MsAndMaxInterval1000Ms_RetriesFiveTimes()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "RestClientRetryAttempts", "5" },
            { "StartHttpRetryInterval", "00:00:00.050" },
            { "MaxHttpRetryInterval", "00:00:01.000" },
            { "SomeAppSettingValue", "SomeValue123" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var sut = new OdysseusClient(configuration, _loggerMock.Object);

        // Act
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetSomethingFromTheOutsideWorld(-1));

        // Assert
        _loggerMock.Invocations.Count.Should().Be(5 + 1);

        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 50 ms, Attempts: 1",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 100 ms, Attempts: 2",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 200 ms, Attempts: 3",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 400 ms, Attempts: 4",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 800 ms, Attempts: 5",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "[OdysseusClient.GetSomethingFromTheOutsideWorld] - Failed to get strings for User -1 from Odysseus. Some AppSettings value: SomeValue123.",
            LogLevel.Error);
    }

    [Test]
    // Retries: 5, StartInterval: 50 ms, MaxInterval: 300 ms
    public void GetSomethingFromTheOutsideWorld_WithFiveExponentialRetriesHavingStartInterval50MsAndMaxInterval300Ms_RetriesFiveTimes()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "RestClientRetryAttempts", "5" },
            { "StartHttpRetryInterval", "00:00:00.050" },
            { "MaxHttpRetryInterval", "00:00:00.300" },
            { "SomeAppSettingValue", "SomeValue123" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var sut = new OdysseusClient(configuration, _loggerMock.Object);

        // Act
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetSomethingFromTheOutsideWorld(-1));

        // Assert
        _loggerMock.Invocations.Count.Should().Be(5 + 1);

        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 50 ms, Attempts: 1",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 100 ms, Attempts: 2",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 200 ms, Attempts: 3",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 300 ms, Attempts: 4",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 300 ms, Attempts: 5",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "RetryPolicy attempt. Error invoking OdysseusClient.GetSomethingFromTheOutsideWorld. SleepDuration: 300 ms, Attempts: 5",
            LogLevel.Warning);
        Asserter.Logging.ValidateLog(_loggerMock,
            "[OdysseusClient.GetSomethingFromTheOutsideWorld] - Failed to get strings for User -1 from Odysseus. Some AppSettings value: SomeValue123.",
            LogLevel.Error);
    }
}