using Core.ProfileHandling;

namespace Core.Tests;

[TestClass]
public class ProfileValidatorTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void IsProfileValid_ReturnsTrue(int index)
    {
        // Arrange
        var profiles = GetValidMockProfiles();

        // Act
        var actual = ProfileValidator.IsProfileValid(profiles[index], out _);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void IsProfileValid_ReturnsFalse(int index)
    {
        // Arrange
        var profiles = GetInvalidMockProfiles();

        // Act
        var actual = ProfileValidator.IsProfileValid(profiles[index], out _);

        // Assert
        Assert.IsFalse(actual);
    }



    [TestMethod]
    [DataRow("a:b")]
    [DataRow("a:b:c")]
    [DataRow("desktopwidth:i:800")]
    public void IsProfileSettingValid_ReturnsTrue(string setting)
    {
        // Arrange

        // Act
        var actual = ProfileValidator.IsProfileSettingValid(setting, out _);

        // Assert
        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("a:")]
    [DataRow(":b")]
    [DataRow("abc")]
    public void IsProfileSettingValid_ReturnsFalse(string setting)
    {
        // Arrange

        // Act
        var actual = ProfileValidator.IsProfileSettingValid(setting, out _);

        // Assert
        Assert.IsFalse(actual);
    }



    #region Mocks

    private List<Profile> GetValidMockProfiles() =>
    [
        new()
        {
            WatchFolder = "C:/Foo",
            Filter = "Bar",
        },
        new()
        {
            WatchFolder = @"C:\Foo",
            Filter = "Bar",
            Settings = ["a:b"],
        },
        new()
        {
            WatchFolder = @"C:\Foo",
            Filter = "*",
            Settings = ["a:b:c"],
        },
    ];

    private List<Profile> GetInvalidMockProfiles() =>
    [
        new()
        {
            WatchFolder = @"Foo", // Bad
            Filter = "Bar",
        },
        new()
        {
            WatchFolder = "", // Bad
            Filter = "Bar",
        },
        new()
        {
            WatchFolder = @"C:\Foo",
            Filter = "", // Bad
        },
        new()
        {
            WatchFolder = @"C:\Foo",
            Filter = "Bar",
            Settings = ["hello"], // Bad
        },
    ];

    #endregion
}
