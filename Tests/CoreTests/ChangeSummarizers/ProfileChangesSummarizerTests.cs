using Core.ChangesSummarizers;
using Core.Profiles;

namespace CoreTests.ChangeSummarizers;

[TestClass]
public class ProfileChangesSummarizerTests
{
    [TestMethod]
    [DataRow(1, "Profile 1", true, "C:/Watch", "*.txt", "C:\\Processed", true, true)]
    [DataRow(0, "", false, "", "", "", false, false)]
    public void GetChangedProfilesSettings_IdenticalProfiles_ReturnsEmptyList(
        int id, string name, bool enabled, string watchFolder,
        string filter, string moveToFolder, bool launch, bool delete)
    {
        // Arrange
        var oldProfiles = new List<Profile>()
        {
            new()
            {
                Id = id,
                Name = name,
                Enabled = enabled,
                WatchFolder = watchFolder,
                Filter = filter,
                MoveToFolder = moveToFolder,
                Launch = launch,
                Delete = delete,
                Settings = [],
            },
            new() { Id = 2, Name = "Profile 2" },
            new(),
        };
        var newProfiles = new List<Profile>()
        {
            new()
            {
                Id = id,
                Name = name,
                Enabled = enabled,
                WatchFolder = watchFolder,
                Filter = filter,
                MoveToFolder = moveToFolder,
                Launch = launch,
                Delete = delete,
                Settings = [],
            },
            new() { Id = 2, Name = "Profile 2" },
            new(),
        };

        // Act
        List<string> changes = ProfileChangesSummarizer.GetChangedProfilesSettings(oldProfiles, newProfiles);

        // Assert
        Assert.AreEqual(0, changes.Count);
    }

    [TestMethod]
    public void GetChangedProfileSettings_IdenticalProfiles_ReturnsEmptyList()
    {
        // Arrange
        var oldProfile = new Profile()
        {
            Name = "Name A",
            Enabled = true,
            WatchFolder = "Watch A",
            Filter = "Filter A",
            MoveToFolder = "Move A",
            Launch = true,
            Delete = true,
        };
        var newProfile = new Profile()
        {
            Name = "Name A",
            Enabled = true,
            WatchFolder = "Watch A",
            Filter = "Filter A",
            MoveToFolder = "Move A",
            Launch = true,
            Delete = true,
        };

        // Act
        List<string> changes = ProfileChangesSummarizer.GetChangedProfileSettings(oldProfile, newProfile);

        // Assert
        Assert.AreEqual(0, changes.Count);
    }

    [TestMethod]
    public void GetChangedProfileSettings_AllValuesChanged_DetailedCheck_ReturnsCorrectChanges()
    {
        // Arrange
        var oldProfile = new Profile()
        {
            Name = "Name A",
            Enabled = false,
            WatchFolder = "Watch A",
            Filter = "Filter A",
            MoveToFolder = "Move A",
            Launch = false,
            Delete = false,
        };
        var newProfile = new Profile()
        {
            Name = "Name B",
            Enabled = true,
            WatchFolder = "Watch B",
            Filter = "Filter B",
            MoveToFolder = "Move B",
            Launch = true,
            Delete = true,
        };

        // Act
        List<string> changes = ProfileChangesSummarizer.GetChangedProfileSettings(oldProfile, newProfile);

        // Assert
        bool hasChangeMessage_Name = HasChangeMessageForProperty(changes, nameof(Profile.Name), oldProfile.Name, newProfile.Name);
        bool hasChangeMessage_Enabled = HasChangeMessageForProperty(changes, nameof(Profile.Enabled), oldProfile.Enabled, newProfile.Enabled);
        bool hasChangeMessage_WatchFolder = HasChangeMessageForProperty(changes, nameof(Profile.WatchFolder), oldProfile.WatchFolder, newProfile.WatchFolder);
        bool hasChangeMessage_Filter = HasChangeMessageForProperty(changes, nameof(Profile.Filter), oldProfile.Filter, newProfile.Filter);
        bool hasChangeMessage_MoveToFolder = HasChangeMessageForProperty(changes, nameof(Profile.MoveToFolder), oldProfile.MoveToFolder, newProfile.MoveToFolder);
        bool hasChangeMessage_Launch = HasChangeMessageForProperty(changes, nameof(Profile.Launch), oldProfile.Launch, newProfile.Launch);
        bool hasChangeMessage_Delete = HasChangeMessageForProperty(changes, nameof(Profile.Delete), oldProfile.Delete, newProfile.Delete);

        Assert.AreEqual(7, changes.Count);
        Assert.IsTrue(hasChangeMessage_Name);
        Assert.IsTrue(hasChangeMessage_Enabled);
        Assert.IsTrue(hasChangeMessage_WatchFolder);
        Assert.IsTrue(hasChangeMessage_Filter);
        Assert.IsTrue(hasChangeMessage_MoveToFolder);
        Assert.IsTrue(hasChangeMessage_Launch);
        Assert.IsTrue(hasChangeMessage_Delete);
    }

    private bool HasChangeMessageForProperty<T>(List<string> changes, string propertyName, T oldValue, T newValue)
    {
        return changes.Any(str =>
            str.Contains(propertyName)
            && str.Contains($"{oldValue}")
            && str.Contains($"{newValue}"));
    }
}
