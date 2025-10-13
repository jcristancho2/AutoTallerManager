using Xunit;

namespace AutoTallerManager.Tests;

public class BasicTest
{
    [Fact]
    public void SimpleTest_ShouldPass()
    {
        // Arrange
        var expected = 2;
        var actual = 1 + 1;

        // Act & Assert
        Assert.Equal(expected, actual);
    }
}
