using Core.Configuration;
using Core.Helpers;

namespace Core.Tests;

[TestClass]
public class ProfileHelperTests
{
    [TestMethod]
    [DataRow(0, true)]
    [DataRow(1, false)]
    [DataRow(2, false)]
    [DataRow(3, false)]
    public void IsProfileValid(int index, bool expected)
    {
        // Arrange
        var profiles = GetMockProfiles();

        // Act
        var actual = ProfileHelper.IsProfileValid(profiles[index], out _);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    private List<Profile> GetMockProfiles()
        => [
            new()
            {
                WatchFolder = @"C:\Foo", // Good
                Filter = "Bar", // Good
            },
            new()
            {
                WatchFolder = @"Foo", // Bad
                Filter = "Bar", // Good
            },
            new()
            {
                WatchFolder = "", // Bad
                Filter = "Bar", // Good
            },
            new()
            {
                WatchFolder = @"C:\Foo", // Good
                Filter = "", // Bad
            },
        ];
}
