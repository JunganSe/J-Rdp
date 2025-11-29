using Core.ChangesSummarizers;
using Core.Configs;

namespace CoreTests.ChangeSummarizers;

[TestClass]
public class ConfigChangesSummarizerTests
{
    [TestMethod]
    [DataRow(0, 0, false, false)]
    [DataRow(1, 2, true, true)]
    [DataRow(3, 4, true, false)]
    [DataRow(5, 5, false, true)]
    public void GetChangedConfigSettings_NoValuesChanged_ReturnsEmptyList(
        int pollingInterval, int deleteDelay, bool showLog, bool logToFile)
    {
        // Arrange
        var oldConfig = new Config()
        {
            PollingInterval = pollingInterval,
            DeleteDelay = deleteDelay,
            ShowLog = showLog,
            LogToFile = logToFile,
        };
        var newConfig = new Config()
        {
            PollingInterval = pollingInterval,
            DeleteDelay = deleteDelay,
            ShowLog = showLog,
            LogToFile = logToFile,
        };

        // Act
        List<string> changes = ConfigChangesSummarizer.GetChangedConfigSettings(oldConfig, newConfig);

        // Assert
        Assert.AreEqual(0, changes.Count);
    }

    [TestMethod]
    [DataRow(0, 0, false, false, 1, 1, true, true)]
    [DataRow(1, 2, false, false, 2, 3, true, true)]
    [DataRow(-1, 0, true, false, 1, -2, false, true)]
    public void GetChangedConfigSettings_AllValuesChanged_ReturnsCorrectChanges(
        int pollingIntervalA, int deleteDelayA, bool showLogA, bool logToFileA,
        int pollingIntervalB, int deleteDelayB, bool showLogB, bool logToFileB)
    {
        // Arrange
        var oldConfig = new Config()
        {
            PollingInterval = pollingIntervalA,
            DeleteDelay = deleteDelayA,
            ShowLog = showLogA,
            LogToFile = logToFileA,
        };
        var newConfig = new Config()
        {
            PollingInterval = pollingIntervalB,
            DeleteDelay = deleteDelayB,
            ShowLog = showLogB,
            LogToFile = logToFileB,
        };

        // Act
        List<string> changes = ConfigChangesSummarizer.GetChangedConfigSettings(oldConfig, newConfig);

        bool isCorrectPollingIntervalMessage = changes.Any(str =>
            str.Contains(nameof(Config.PollingInterval))
            && str.Contains($"{oldConfig.PollingInterval}")
            && str.Contains($"{newConfig.PollingInterval}"));
        bool isCorrectDeleteDelayMessage = changes.Any(str =>
            str.Contains(nameof(Config.DeleteDelay))
            && str.Contains($"{oldConfig.DeleteDelay}")
            && str.Contains($"{newConfig.DeleteDelay}"));
        bool isCorrectShowLogMessage = changes.Any(str =>
            str.Contains(nameof(Config.ShowLog))
            && str.Contains($"{oldConfig.ShowLog}")
            && str.Contains($"{newConfig.ShowLog}"));
        bool isCorrectLogToFileMessage = changes.Any(str =>
            str.Contains(nameof(Config.LogToFile))
            && str.Contains($"{oldConfig.LogToFile}")
            && str.Contains($"{newConfig.LogToFile}"));

        // Assert
        Assert.AreEqual(4, changes.Count);
        Assert.IsTrue(isCorrectPollingIntervalMessage);
        Assert.IsTrue(isCorrectDeleteDelayMessage);
        Assert.IsTrue(isCorrectShowLogMessage);
        Assert.IsTrue(isCorrectLogToFileMessage);
    }
}
