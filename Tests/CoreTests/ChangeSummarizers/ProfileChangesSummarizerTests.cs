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
}
