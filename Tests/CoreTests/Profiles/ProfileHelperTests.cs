using Core.Profiles;

namespace CoreTests.Profiles;

[TestClass]
public class ProfileHelperTests
{
    #region DeepCopies

    [TestMethod]
    public void GetDeepCopies_NoProfiles()
    {
        // Arrange
        var expectedProfiles = new List<Profile>();

        // Act
        var actualProfiles = ProfileHelper.GetDeepCopies(expectedProfiles);

        // Assert
        Assert.AreEqual(0, actualProfiles.Count);
    }

    [TestMethod]
    public void GetDeepCopies_Valid()
    {
        // Arrange
        var expectedProfiles = GetMockProfiles();

        // Act
        var actualProfiles1 = ProfileHelper.GetDeepCopies(expectedProfiles);
        var actualProfiles2 = ProfileHelper.GetDeepCopies(expectedProfiles, copyId: true);
        var actualProfiles3 = ProfileHelper.GetDeepCopies(expectedProfiles, copyId: false);

        // Assert
        for (int i = 0; i < expectedProfiles.Count; i++)
        {
            AssertProfilesAreEqual(expectedProfiles[i], actualProfiles1[i], compareId: true);
            AssertProfilesAreEqual(expectedProfiles[i], actualProfiles2[i], compareId: true);
            AssertProfilesAreEqual(expectedProfiles[i], actualProfiles3[i], compareId: false);
            Assert.AreNotEqual(expectedProfiles[i].Id, actualProfiles3[i].Id);
            Assert.IsTrue(actualProfiles3[i].Id <= 0);
        }
    }

    [TestMethod]
    public void GetDeepCopy_Valid()
    {
        // Arrange
        var expected = GetMockProfiles().First();

        // Act
        var actual1 = ProfileHelper.GetDeepCopy(expected);
        var actual2 = ProfileHelper.GetDeepCopy(expected, copyId: true);
        var actual3 = ProfileHelper.GetDeepCopy(expected, copyId: false);

        // Assert
        AssertProfilesAreEqual(expected, actual1, compareId: true);
        AssertProfilesAreEqual(expected, actual2, compareId: true);
        AssertProfilesAreEqual(expected, actual3, compareId: false);
        Assert.AreNotEqual(expected.Id, actual3.Id);
        Assert.IsTrue(actual3.Id <= 0);
    }

    private void AssertProfilesAreEqual(Profile expected, Profile actual, bool compareId)
    {
        if (compareId)
            Assert.AreEqual(expected.Id, actual.Id);

        Assert.AreEqual(expected.Enabled, actual.Enabled);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.WatchFolder, actual.WatchFolder);
        Assert.AreEqual(expected.Filter, actual.Filter);
        Assert.AreEqual(expected.MoveToFolder, actual.MoveToFolder);
        Assert.AreEqual(expected.Launch, actual.Launch);
        Assert.AreEqual(expected.Delete, actual.Delete);
        CollectionAssert.AreEqual(expected.Settings, actual.Settings);
    }

    #endregion

    #region EnabledState

    [TestMethod]
    public void SetEnabledStatesFromMatchingProfileInfos_Enabled()
    {
        // Arrange
        var profiles = GetMockProfiles();
        var profileInfos = new List<ProfileInfo>
        {
            new() { Id = 1, Enabled = true },
        };

        // Act
        ProfileHelper.SetEnabledStatesFromMatchingProfileInfos(profiles, profileInfos);

        // Assert
        Assert.IsTrue(profiles[0].Enabled);
    }

    [TestMethod]
    public void SetEnabledStatesFromMatchingProfileInfos_Disabled()
    {
        // Arrange
        var profiles = GetMockProfiles();
        var profileInfos = new List<ProfileInfo>
        {
            new() { Id = 1, Enabled = false },
            new() { Id = 2},
            new() { Id = 99, Enabled = true},
        };

        // Act
        ProfileHelper.SetEnabledStatesFromMatchingProfileInfos(profiles, profileInfos);

        // Assert
        Assert.IsFalse(profiles[0].Enabled);
        Assert.IsFalse(profiles[1].Enabled);
        Assert.IsFalse(profiles[2].Enabled);
    }

    #endregion

    #region Profile comparison

    [TestMethod]
    public void AreProfilesEquivalent_Equivalent()
    {
        // Arrange
        var profilesA = GetMockProfiles();
        var profilesB = GetMockProfiles();

        // Act
        bool areEquivalent = ProfileHelper.AreProfilesEquivalent(profilesA, profilesB);

        // Assert
        Assert.IsTrue(areEquivalent);
    }

    [TestMethod]
    public void AreProfilesEquivalent_Different_Added()
    {
        // Arrange
        var profilesA = GetMockProfiles();
        var profilesB = GetMockProfiles();
        profilesB.Add(new());

        // Act
        bool areEquivalent = ProfileHelper.AreProfilesEquivalent(profilesA, profilesB);

        // Assert
        Assert.IsFalse(areEquivalent);
    }

    [TestMethod]
    public void AreProfilesEquivalent_Different_Removed()
    {
        // Arrange
        var profilesA = GetMockProfiles();
        var profilesB = GetMockProfiles();
        profilesB.RemoveAt(0);

        // Act
        bool areEquivalent = ProfileHelper.AreProfilesEquivalent(profilesA, profilesB);

        // Assert
        Assert.IsFalse(areEquivalent);
    }

    [TestMethod]
    public void AreProfilesEquivalent_Different_AddedAndRemoved()
    {
        // Arrange
        var profilesA = GetMockProfiles();
        var profilesB = GetMockProfiles();
        profilesB.RemoveAt(0);
        profilesB.Add(new());

        // Act
        bool areEquivalent = ProfileHelper.AreProfilesEquivalent(profilesA, profilesB);

        // Assert
        Assert.IsFalse(areEquivalent);
    }

    [TestMethod]
    public void AreProfilesEquivalent_Different_Changed()
    {
        // Arrange
        var profilesA = new List<Profile>() {
            new()
            {
                Enabled = true,
                Name = "Profile 1",
                WatchFolder = "C:\\WatchFolder1",
                Filter = "Filter1",
                MoveToFolder = "C:\\MoveToFolder1",
                Launch = true,
                Delete = true,
                Settings = ["Setting1a", "Setting1b"],
            }
        };
        var profilesB = new List<Profile>() {
            new()
            {
                Enabled = false,
                Name = "Profile 1",
                WatchFolder = "C:\\WatchFolder1",
                Filter = "Filter1",
                MoveToFolder = "C:\\MoveToFolder1",
                Launch = true,
                Delete = true,
                Settings = ["Setting1a", "Setting1b"],
            }
        };

        // Act
        bool areEquivalent = ProfileHelper.AreProfilesEquivalent(profilesA, profilesB);

        // Assert
        Assert.IsFalse(areEquivalent);
    }

    #endregion

    #region Mocks

    private List<Profile> GetMockProfiles() =>
    [
        GetMockProfile1(),
        GetMockProfile2(),
        GetMockProfile3(),
        GetMockProfile4(),
    ];

    private Profile GetMockProfile1() => new(id: 1)
    {
        Enabled = true,
        Name = "Profile 1",
        WatchFolder = "C:\\WatchFolder1",
        Filter = "Filter1",
        MoveToFolder = "C:\\MoveToFolder1",
        Launch = true,
        Delete = true,
        Settings = ["Setting1a", "Setting1b"],
    };

    private Profile GetMockProfile2() => new(id: 2)
    {
        Enabled = false,
        Name = "Profile 2",
        WatchFolder = "C:\\WatchFolder2",
        Filter = "Filter2",
        MoveToFolder = "C:\\MoveToFolder2",
        Launch = false,
        Delete = false,
        Settings = ["Setting2a", "Setting2b"],
    };

    private Profile GetMockProfile3() => new(id: 3)
    {
        Enabled = false,
        Name = "Profile 3",
        WatchFolder = "C:\\WatchFolder3",
        Filter = "Filter3",
        MoveToFolder = "C:\\MoveToFolder3",
        Launch = false,
        Delete = false,
        Settings = ["Setting3a", "Setting3b"],
    };

    private Profile GetMockProfile4() => new(id: 4)
    {
        Enabled = false,
        Name = "Profile 4",
        WatchFolder = "C:\\WatchFolder4",
        Filter = "Filter4",
        MoveToFolder = "C:\\MoveToFolder4",
        Launch = false,
        Delete = false,
        Settings = ["Setting4a", "Setting4b"],
    };

    #endregion
}