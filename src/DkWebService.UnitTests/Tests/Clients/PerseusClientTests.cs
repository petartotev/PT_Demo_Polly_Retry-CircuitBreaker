using DkWebService.EndPoint.Clients;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

namespace DkWebService.UnitTests.Tests.Clients
{
    [TestFixture]
    public class PerseusClientTests : TestsBase
    {
        private readonly Mock<ILogger<PerseusClient>> _loggerMock = new();

        [SetUp]
        public void Setup()
        {
            _loggerMock.Reset();
        }

        [Test]
        // Retries: 2, StartInterval: 50 ms, MaxInterval: 100 ms
        public void CheckIfFraud_WithTwoImmediateRetries_RetriesTwoTimes()
        {
            // Arrange
            var depositId = 12345;
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

            var sut = new PerseusClient(configuration, _loggerMock.Object);

            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            Assert.ThrowsAsync<ArgumentException>(() => sut.CheckIfDepositIsFraudAsync(depositId));
            stopwatch.Stop();

            // Assert
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromMilliseconds(125));

            _loggerMock.Invocations.Count.Should().Be(2 + 1);

            Asserter.Logging.ValidateLog(_loggerMock,
                "RetryPolicy attempt. Error invoking PerseusClient.CheckIfFraud. SleepDuration: 0 ms (Immediate Retry!), Attempts: 1",
                LogLevel.Warning);
            Asserter.Logging.ValidateLog(_loggerMock,
                "RetryPolicy attempt. Error invoking PerseusClient.CheckIfFraud. SleepDuration: 0 ms (Immediate Retry!), Attempts: 2",
                LogLevel.Warning);
            Asserter.Logging.ValidateLog(_loggerMock,
                $"[PerseusClient.CheckIfFraud] - Failed to get isFraud for Deposit {depositId} from Perseus. Some AppSettings value: SomeValue123.",
                LogLevel.Error);
        }
    }
}
