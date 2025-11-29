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
        var changes = ConfigChangesSummarizer.GetChangedConfigSettings(oldConfig, newConfig);

        // Assert
        Assert.AreEqual(0, changes.Count);
    }
}
