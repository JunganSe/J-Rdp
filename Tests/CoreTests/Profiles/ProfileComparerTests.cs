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
        var profile1 = new Profile()
        {
            Id = 1,
            Name = "Profile1",
            Enabled = true,
            WatchFolder = "C:\\Foo",
            Filter = "*.rdp",
            MoveToFolder = "C:\\Bar",
            Launch = true,
            Delete = false,
            Settings = ["Setting1", "Setting2"]
        };
        var profile2 = new Profile()
        {
            Id = 1,
            Name = "Profile1",
            Enabled = true,
            WatchFolder = "C:\\Foo",
            Filter = "*.rdp",
            MoveToFolder = "C:\\Bar",
            Launch = true,
            Delete = false,
            Settings = ["Setting1", "Setting2"]
        };

        // Act
        bool result = _comparer.Equals(profile1, profile2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CompareProfiles_EqualProfilesWithDifferentId_ReturnsTrue()
    {
        // Arrange
        var profile1 = new Profile()
        {
            Id = 1,
            Name = "Profile1",
            Enabled = true,
            WatchFolder = "C:\\Foo",
            Filter = "*.rdp",
            MoveToFolder = "C:\\Bar",
            Launch = true,
            Delete = false,
            Settings = ["Setting1", "Setting2"]
        };
        var profile2 = new Profile()
        {
            Id = 2,
            Name = "Profile1",
            Enabled = true,
            WatchFolder = "C:\\Foo",
            Filter = "*.rdp",
            MoveToFolder = "C:\\Bar",
            Launch = true,
            Delete = false,
            Settings = ["Setting1", "Setting2"]
        };

        // Act
        bool result = _comparer.Equals(profile1, profile2);

        // Assert
        Assert.IsTrue(result);
    }
}
