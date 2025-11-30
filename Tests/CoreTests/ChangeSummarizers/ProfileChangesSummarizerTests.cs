using Core.ChangesSummarizers;
using Core.Profiles;

namespace CoreTests.ChangeSummarizers;

[TestClass]
public class ProfileChangesSummarizerTests
{
    #region GetChangedProfileSettings (singular)

    [TestMethod]
    public void GetChangedProfileSettings_IdenticalProfiles_ReturnsEmptyList()
    {
        // Arrange
        var oldProfile = GetMockProfile1();
        var newProfile = GetMockProfile1();

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

    #endregion

    #region GetChangedProfilesSettings (plural)

    [TestMethod]
    public void GetChangedProfilesSettings_EmptyInputs_ReturnsEmptyList()
    {
        // Arrange
        var oldProfiles = new List<Profile>();
        var newProfiles = new List<Profile>();

        // Act
        List<string> changes = ProfileChangesSummarizer.GetChangedProfilesSettings(oldProfiles, newProfiles);

        // Assert
        Assert.AreEqual(0, changes.Count);
    }

    [TestMethod]
    public void GetChangedProfilesSettings_AddedProfile_ReturnsCorrectChanges()
    {
        // Arrange
        var oldProfilesA = new List<Profile>() { };
        var newProfilesA = new List<Profile>() { new() { Id = 2, Name = "Profile2"}, };

        var oldProfilesB = new List<Profile>()
        {
            GetMockProfile1(),
        };
        var newProfilesB = new List<Profile>()
        {
            GetMockProfile1(),
            new() { Id = 2, Name = "Profile2"},
        };

        // Act
        List<string> changesA = ProfileChangesSummarizer.GetChangedProfilesSettings(oldProfilesA, newProfilesA);
        List<string> changesB = ProfileChangesSummarizer.GetChangedProfilesSettings(oldProfilesB, newProfilesB);

        // Assert
        bool hasAddedMessage_Profile2_A = changesA.Any(str => str.Contains("added") && str.Contains("Profile2"));
        bool hasAddedMessage_Profile2_B = changesB.Any(str => str.Contains("added") && str.Contains("Profile2"));

        Assert.AreEqual(1, changesA.Count);
        Assert.IsTrue(hasAddedMessage_Profile2_A);

        Assert.AreEqual(1, changesB.Count);
        Assert.IsTrue(hasAddedMessage_Profile2_B);
    }

    [TestMethod]
    public void GetChangedProfilesSettings_IdenticalProfiles_ReturnsEmptyList()
    {
        // Arrange
        var oldProfiles = new List<Profile>()
        {
            GetMockProfile1(),
            GetMockProfile2(),
            new() { Id = 3, Name = "Profile 3" },
            new(),
        };
        var newProfiles = new List<Profile>()
        {
            GetMockProfile1(),
            GetMockProfile2(),
            new() { Id = 3, Name = "Profile 3" },
            new(),
        };

        // Act
        List<string> changes = ProfileChangesSummarizer.GetChangedProfilesSettings(oldProfiles, newProfiles);

        // Assert
        Assert.AreEqual(0, changes.Count);
    }

    #endregion

    #region Helpers

    private bool HasChangeMessageForProperty<T>(List<string> changes, string propertyName, T oldValue, T newValue)
    {
        return changes.Any(str =>
            str.Contains(propertyName)
            && str.Contains($"{oldValue}")
            && str.Contains($"{newValue}"));
    }

    #endregion

    #region Mocking

    private Profile GetMockProfile1() => new()
    {
        Id = 1,
        Name = "Profile 1",
        Enabled = false,
        WatchFolder = "C:/Watch1",
        Filter = "*1.txt",
        MoveToFolder = "C:/Processed1",
        Launch = false,
        Delete = false,
        Settings = [],
    };

    private Profile GetMockProfile2() => new()
    {
        Id = 2,
        Name = "Profile 2",
        Enabled = true,
        WatchFolder = "C:/Watch2",
        Filter = "*2.txt",
        MoveToFolder = "C:/Processed2",
        Launch = true,
        Delete = true,
        Settings = [],
    };

    #endregion
}
