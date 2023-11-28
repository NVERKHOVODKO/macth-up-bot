using Data;
using EntityFrameworkLesson.Utils;
using Microsoft.Extensions.Hosting;


namespace TestProject1;

[TestFixture]
public class Tests
{
    [Test]
    public void CalculateMatch_ByInterests_ReturnsCorrectMatch()
    {
        // Arrange
        long firstUserId = 1;
        List<string> secPersonInterests = new List<string> { "music", "travel", "cooking" };

        // Act
        double match = MatchCalculator.CalculateMatchByInterests(firstUserId, secPersonInterests);

        // Assert
        Assert.That(match, Is.GreaterThanOrEqualTo(0.0));
        Assert.That(match, Is.LessThanOrEqualTo(100.0));
    }
}