using Core.ChangesSummarizers;

namespace CoreTests.ChangeSummarizers;

[TestClass]
public class ProfileSettingsChangesSummarizerTests
{
    #region GetSettingsChanges

    [TestMethod]
    public void GetSettingsChanges_IdenticalSettings_ReturnsEmptyList()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:value1", "key2:value2" };
        var newSettings = new List<string> { "key1:value1", "key2:value2" };

        // Act
        List<string> changes = ProfileSettingsChangesSummarizer.GetSettingsChanges(oldSettings, newSettings);

        // Assert
        Assert.AreEqual(0, changes.Count);
    }

    [TestMethod]
    public void GetSettingsChanges_AllSettingsChanged_DetailedCheck_ReturnsCorrectChanges()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:value1a", "key2:value2a", "key3:value3a" };
        var newSettings = new List<string> { "key1:value1b", "key2:value2b", "key3:value3b" };

        // Act
        List<string> changes = ProfileSettingsChangesSummarizer.GetSettingsChanges(oldSettings, newSettings);

        // Assert
        Assert.AreEqual(3, changes.Count);
        Assert.IsTrue(HasChangeMessage(changes, "key1:value1a", "key1:value1b"));
        Assert.IsTrue(HasChangeMessage(changes, "key2:value2a", "key2:value2b"));
        Assert.IsTrue(HasChangeMessage(changes, "key3:value3a", "key3:value3b"));
    }

    [TestMethod]
    public void GetSettingsChanges_OneAddedSetting_ReturnsCorrectChanges()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:value1", "key2:value2" };
        var newSettings = new List<string> { "key1:value1", "key2:value2", "key3:value3" };

        // Act
        List<string> changes = ProfileSettingsChangesSummarizer.GetSettingsChanges(oldSettings, newSettings);

        // Assert
        bool hasCorrectMessage = changes.Any(str => str.Contains("Added") && str.Contains("key3:value3"));

        Assert.AreEqual(1, changes.Count);
        Assert.IsTrue(hasCorrectMessage);
    }

    [TestMethod]
    public void GetSettingsChanges_TwoAddedSettings_ReturnsCorrectChanges()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:value1" };
        var newSettings = new List<string> { "key1:value1", "key2:value2", "key3:value3" };

        // Act
        List<string> changes = ProfileSettingsChangesSummarizer.GetSettingsChanges(oldSettings, newSettings);

        // Assert
        bool hasCorrectMessage = changes.Any(str => str.Contains("Added") && str.Contains("key2:value2") && str.Contains("key3:value3"));

        Assert.AreEqual(1, changes.Count);
        Assert.IsTrue(hasCorrectMessage);
    }

    [TestMethod]
    public void GetSettingsChanges_OneRemovedSetting_ReturnsCorrectChanges()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:value1", "key2:value2" };
        var newSettings = new List<string> { "key1:value1" };

        // Act
        List<string> changes = ProfileSettingsChangesSummarizer.GetSettingsChanges(oldSettings, newSettings);

        // Assert
        bool hasCorrectMessage = changes.Any(str => str.Contains("Removed") && str.Contains("key2:value2"));

        Assert.AreEqual(1, changes.Count);
        Assert.IsTrue(hasCorrectMessage);
    }

    [TestMethod]
    public void GetSettingsChanges_TwoRemovedSettings_ReturnsCorrectChanges()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:value1", "key2:value2", "key3:value3" };
        var newSettings = new List<string> { "key1:value1" };

        // Act
        List<string> changes = ProfileSettingsChangesSummarizer.GetSettingsChanges(oldSettings, newSettings);

        // Assert
        bool hasCorrectMessage = changes.Any(str => str.Contains("Removed") && str.Contains("key2:value2") && str.Contains("key3:value3"));

        Assert.AreEqual(1, changes.Count);
        Assert.IsTrue(hasCorrectMessage);
    }

    [TestMethod]
    public void GetSettingsChanges_AddedRemovedAndChangedSettings_ReturnsCorrectChanges()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:value1a", "key2:value2" };
        var newSettings = new List<string> { "key1:value1b", "key3:value3" };

        // Act
        List<string> changes = ProfileSettingsChangesSummarizer.GetSettingsChanges(oldSettings, newSettings);

        // Assert
        bool hasAddedMessage_Setting3 = changes.Any(str => str.Contains("Added") && str.Contains("key3:value3"));
        bool hasRemovedMessage_Setting2 = changes.Any(str => str.Contains("Removed") && str.Contains("key2:value2"));
        bool hasChangeMdessage_Setting1 = HasChangeMessage(changes, "key1:value1a", "key1:value1b");

        Assert.AreEqual(3, changes.Count);
        Assert.IsTrue(hasAddedMessage_Setting3);
        Assert.IsTrue(hasRemovedMessage_Setting2);
        Assert.IsTrue(hasChangeMdessage_Setting1);
    }

    #endregion

    #region GetGroupedSettings

    [TestMethod]
    public void GetGroupedSettings_Added_ReturnsCorrectGroups()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:KeptValue", "key2:KeptValue" };
        var newSettings = new List<string> { "key1:KeptValue", "key3:AddedValue", "key4:AddedValue", "key2:KeptValue" };

        // Act
        var (added, removed, changed) = ProfileSettingsChangesSummarizer.GetGroupedSettings(oldSettings, newSettings);

        // Assert
        Assert.AreEqual(2, added.Count);
        Assert.AreEqual(0, removed.Count);
        Assert.AreEqual(0, changed.Count);
        Assert.AreEqual("key3:AddedValue", added[0]);
        Assert.AreEqual("key4:AddedValue", added[1]);
    }

    [TestMethod]
    public void GetGroupedSettings_Removed_ReturnsCorrectGroups()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:KeptValue", "key2:RemovedValue", "key3:RemovedValue", "key4:KeptValue" };
        var newSettings = new List<string> { "key1:KeptValue", "key4:KeptValue" };

        // Act
        var (added, removed, changed) = ProfileSettingsChangesSummarizer.GetGroupedSettings(oldSettings, newSettings);

        // Assert
        Assert.AreEqual(0, added.Count);
        Assert.AreEqual(2, removed.Count);
        Assert.AreEqual(0, changed.Count);
        Assert.AreEqual("key2:RemovedValue", removed[0]);
        Assert.AreEqual("key3:RemovedValue", removed[1]);
    }

    [TestMethod]
    public void GetGroupedSettings_Changed_ReturnsCorrectGroups()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:KeptValue", "key2:RemovedValueA", "key3:RemovedValueA", "key4:KeptValue" };
        var newSettings = new List<string> { "key1:KeptValue", "key2:ChangedValueB", "key3:ChangedValueB", "key4:KeptValue" };

        // Act
        var (added, removed, changed) = ProfileSettingsChangesSummarizer.GetGroupedSettings(oldSettings, newSettings);

        // Assert
        Assert.AreEqual(0, added.Count);
        Assert.AreEqual(0, removed.Count);
        Assert.AreEqual(2, changed.Count);
        Assert.AreEqual(("key2:RemovedValueA", "key2:ChangedValueB"), changed[0]);
        Assert.AreEqual(("key3:RemovedValueA", "key3:ChangedValueB"), changed[1]);
    }

    [TestMethod]
    public void GetGroupedSettings_MixedChanges_ReturnsCorrectGroups()
    {
        // Arrange
        var oldSettings = new List<string> { "key1:ChangedValueA", "key2:KeptValue", "key3:RemovedValue" };
        var newSettings = new List<string> { "key1:ChangedValueB", "key2:KeptValue", "key4:AddedValue" };

        // Act
        var (added, removed, changed) = ProfileSettingsChangesSummarizer.GetGroupedSettings(oldSettings, newSettings);

        // Assert
        Assert.AreEqual(1, added.Count);
        Assert.AreEqual("key4:AddedValue", added[0]);

        Assert.AreEqual(1, removed.Count);
        Assert.AreEqual("key3:RemovedValue", removed[0]);

        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual(("key1:ChangedValueA", "key1:ChangedValueB"), changed[0]);
    }

    #endregion

    #region ThrowIfAnySettingIsInvalid

    [TestMethod]
    public void ThrowIfAnySettingIsInvalid_ValidSettings_DoesNotThrow()
    {
        // Arrange
        var settings = new List<string> { "key1:value1", "key2:value2", "key3:value3" };

        // Act
        ProfileSettingsChangesSummarizer.ThrowIfAnySettingIsInvalid(settings);

        // Assert
        // No exception thrown means the test passes.
    }

    [TestMethod]
    [DataRow("foo")]
    [DataRow(":foo")]
    [DataRow(" :foo")]
    [DataRow("foo:")]
    [DataRow("foo: ")]
    public void ThrowIfAnySettingIsInvalid_InvalidSettings_ThrowsArgumentException(string setting)
    {
        // Arrange
        void Action() => ProfileSettingsChangesSummarizer.ThrowIfAnySettingIsInvalid([setting]);

        // Act & Assert
        Assert.Throws<ArgumentException>(Action);
    }

    #endregion

    #region Helpers

    private bool HasChangeMessage(List<string> changes, string oldSetting, string newSetting)
    {
        return changes.Any(str =>
            str.Contains("Changed")
            && str.Contains(oldSetting)
            && str.Contains(newSetting));
    }

    #endregion
}