using DkWebService.UnitTests.Asserts;

namespace DkWebService.UnitTests.Tests;

public class TestsBase
{
    public TestAsserter Asserter { get; set; } = new();
}
