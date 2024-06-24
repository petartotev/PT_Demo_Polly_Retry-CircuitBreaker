using Microsoft.Extensions.Logging;
using Moq;

namespace DkWebService.UnitTests.Asserts;

public class LoggingAsserter
{
    public void ValidateLog<T>(Mock<ILogger<T>> loggerMock, string message, LogLevel logLevel = LogLevel.Error, int times = 1)
    {
        loggerMock.Verify(l => l.Log(
            logLevel,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(message)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Exactly(times));
    }
}
