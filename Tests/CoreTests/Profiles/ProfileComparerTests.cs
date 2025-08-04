using Core.Profiles;

namespace CoreTests.Profiles;

[TestClass]
public class ProfileComparerTests
{
    private readonly EqualityComparer_Profile_AllExceptId _comparer = new();

    [TestMethod]
    public void CompareProfiles_EqualProfilesWithSameId_ReturnsTrue()
    {
        // Arrange
        var profile1 = GetMockProfile(id: 1);
        var profile2 = GetMockProfile(id: 1);

        // Act
        bool result = _comparer.Equals(profile1, profile2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CompareProfiles_EqualProfilesWithDifferentId_ReturnsTrue()
    {
        // Arrange
        var profile1 = GetMockProfile(id: 1);
        var profile2 = GetMockProfile(id: 2);

        // Act
        bool result = _comparer.Equals(profile1, profile2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/Bar", true, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/FOO", "*.rdp", "C:/BAR", true, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/Bar", true, false, "Setting2", "Setting1", "Setting3")]
    public void CompareProfiles_DifferentProfiles_ReturnsTrue(
        string name, bool enabled, string watchFolder, string filter,
        string moveToFolder, bool launch, bool delete,
        string setting1, string setting2, string setting3)
    {
        // Arrange
        var profile1 = GetMockProfile(id: 1);
        var profile2 = new Profile()
        {
            Name = name,
            Enabled = enabled,
            WatchFolder = watchFolder,
            Filter = filter,
            MoveToFolder = moveToFolder,
            Launch = launch,
            Delete = delete,
            Settings = [setting1, setting2, setting3]
        };

        // Act
        bool result = _comparer.Equals(profile1, profile2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow("Profile2", true, "C:/Foo", "*.rdp", "C:/Bar", true, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", false, "C:/Foo", "*.rdp", "C:/Bar", true, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/AAA", "*.rdp", "C:/Bar", true, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.AAA", "C:/Bar", true, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/AAA", true, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/Bar", false, false, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/Bar", true, true, "Setting1", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/Bar", true, false, "SettingA", "Setting2", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/Bar", true, false, "Setting1", "SettingB", "Setting3")]
    [DataRow("Profile1", true, "C:/Foo", "*.rdp", "C:/Bar", true, false, "Setting1", "Setting2", "SettingC")]
    public void CompareProfiles_DifferentProfiles_ReturnsFalse(
        string name, bool enabled, string watchFolder, string filter,
        string moveToFolder, bool launch, bool delete,
        string setting1, string setting2, string setting3)
    {
        // Arrange
        var profile1 = GetMockProfile(id: 1);
        var profile2 = new Profile()
        {
            Name = name,
            Enabled = enabled,
            WatchFolder = watchFolder,
            Filter = filter,
            MoveToFolder = moveToFolder,
            Launch = launch,
            Delete = delete,
            Settings = [setting1, setting2, setting3]
        };

        // Act
        bool result = _comparer.Equals(profile1, profile2);

        // Assert
        Assert.IsFalse(result);
    }

    #region Mocks

    private Profile GetMockProfile(int id) => new(id)
    {
        Name = "Profile1",
        Enabled = true,
        WatchFolder = "C:/Foo",
        Filter = "*.rdp",
        MoveToFolder = "C:/Bar",
        Launch = true,
        Delete = false,
        Settings = ["Setting1", "Setting2", "Setting3"]
    };

    #endregion
}
