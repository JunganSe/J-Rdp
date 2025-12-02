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

    // TODO: returns correct lists, throws when wrong format

    #endregion

    #region ValidateSettingsFormat

    [TestMethod]
    public void ValidateSettingsFormat_ValidSettings_DoesNotThrow()
    {
        // Arrange
        var settings = new List<string> { "key1:value1", "key2:value2", "key3:value3" };

        // Act
        ProfileSettingsChangesSummarizer.ThrowIfAnySettingIsInvalid(settings);

        // Assert
        // No exception thrown means the test passes.
    }

    [TestMethod]
    public void ValidateSettingsFormat_InValidSettings_ThrowsArgumentException()
    {
        // Arrange
        var settings = new List<string> { "invalidSetting" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            ProfileSettingsChangesSummarizer.ThrowIfAnySettingIsInvalid(settings));
    }

    #endregion

    #region Helpers

    private bool HasAddMessage(List<string> changes, string setting)
    {
        return changes.Any(str => str.Contains("Added") && str.Contains(setting));
    }

    private bool HasRemoveMessage(List<string> changes, string setting)
    {
        return changes.Any(str =>
            str.Contains("Removed")
            && str.Contains(setting));
    }

    private bool HasChangeMessage(List<string> changes, string oldSetting, string newSetting)
    {
        return changes.Any(str =>
            str.Contains("Changed")
            && str.Contains(oldSetting)
            && str.Contains(newSetting));
    }

    #endregion
}