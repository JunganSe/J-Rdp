using Core.Configuration;
using Core.Helpers;

namespace Core.Tests;

[TestClass]
public class ProfileHelperTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    public void IsProfileValid_ReturnsTrue(int index)
    {
        // Arrange
        var profiles = GetValidMockProfiles();

        // Act
        var actual = ProfileHelper.IsProfileValid(profiles[index], out _);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void IsProfileValid_ReturnsFalse(int index)
    {
        // Arrange
        var profiles = GetInvalidMockProfiles();

        // Act
        var actual = ProfileHelper.IsProfileValid(profiles[index], out _);

        // Assert
        Assert.IsFalse(actual);
    }



    #region Mocks

    private List<Profile> GetValidMockProfiles()
        => [
            new()
            {
                WatchFolder = @"C:\Foo",
                Filter = "Bar",
            },
            new()
            {
                WatchFolder = @"C:\Foo",
                Filter = "*",
            },
        ];

    private List<Profile> GetInvalidMockProfiles()
        => [
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

    #endregion
}
